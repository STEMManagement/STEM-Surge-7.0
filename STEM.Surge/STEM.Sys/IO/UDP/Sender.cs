/*
 * Copyright 2019 STEM Management
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Net.NetworkInformation;

namespace STEM.Sys.IO.UDP
{
    public class Sender : STEM.Sys.IO.UDP.SocketHelper, IDisposable
    {
        Thread _FlushThread = null;
        static Dictionary<string, Sender> _ActiveSenders = new Dictionary<string, Sender>();
        static Dictionary<string, object> _SerialLocks = new Dictionary<string, object>();
        Socket _Sender = null;
        bool _Active = false;

        string _SenderKey = "";
        object _SendMutex = new object();
        
        int _PacketsPerDelay = 0;
        public int PacketsPerDelay
        {
            get
            {
                if (_ActiveSenders.ContainsKey(_SenderKey))
                    return _ActiveSenders[_SenderKey]._PacketsPerDelay;

                return 0;
            }

            set
            {
                if (_ActiveSenders.ContainsKey(_SenderKey))
                    _ActiveSenders[_SenderKey]._PacketsPerDelay = value;
            }
        }

        int _MSDelay = 0;
        public int MSDelay
        {
            get
            {
                if (_ActiveSenders.ContainsKey(_SenderKey))
                    return _ActiveSenders[_SenderKey]._MSDelay;

                return 0;
            }

            set
            {
                if (_ActiveSenders.ContainsKey(_SenderKey))
                    _ActiveSenders[_SenderKey]._MSDelay = value;
            }
        }

        int _PacketsInDelayWindow = 0;
        MemoryStream _SendBuffer = new MemoryStream(LargestBlockSize);
        int _References = 0;

        public Sender(int multicastPort, string localNetworkAdapter, string multicastIP, int msDelayBetweenPackets, int packetsPerDelayWindow)
            : base(multicastPort, localNetworkAdapter, multicastIP)
        {
            lock (_ActiveSenders)
            {
                _SenderKey = LocalNetworkAdapter.Trim().ToUpper(System.Globalization.CultureInfo.CurrentCulture) + ":" + MulticastIP + ":" + MulticastPort;

                if (!_ActiveSenders.ContainsKey(_SenderKey))
                {
                    _Active = true;

                    if (msDelayBetweenPackets < 0)
                        msDelayBetweenPackets = 0;

                    _MSDelay = msDelayBetweenPackets;

                    if (packetsPerDelayWindow < 0)
                        packetsPerDelayWindow = 0;

                    _PacketsPerDelay = packetsPerDelayWindow;
                    
                    _ActiveSenders[_SenderKey] = this;

                    if (!_SerialLocks.ContainsKey(LocalNetworkAdapter.Trim().ToUpper(System.Globalization.CultureInfo.CurrentCulture)))
                        _SerialLocks[LocalNetworkAdapter.Trim().ToUpper(System.Globalization.CultureInfo.CurrentCulture)] = new object();

                    _SendMutex = _SerialLocks[LocalNetworkAdapter.Trim().ToUpper(System.Globalization.CultureInfo.CurrentCulture)];

                    _References++;
                }
            }            
        }

        bool _Stop = false;
        public virtual void Close()
        {
            try
            {
                lock (_ActiveSenders)
                {
                    if (_ActiveSenders.ContainsKey(_SenderKey))
                    {
                        lock (_ActiveSenders[_SenderKey]._SendMutex)
                        {
                            _ActiveSenders[_SenderKey]._References--;

                            if (_ActiveSenders[_SenderKey]._References == 0)
                            {
                                _ActiveSenders[_SenderKey].Flush();

                                _ActiveSenders[_SenderKey]._Stop = true;

                                if (_ActiveSenders[_SenderKey]._Sender != null)
                                {
                                    try
                                    {
                                        _ActiveSenders[_SenderKey]._Sender.Close();
                                    }
                                    catch { }

                                    try
                                    {
                                        _ActiveSenders[_SenderKey]._Sender.Dispose();
                                    }
                                    catch { }
                                }

                                _ActiveSenders[_SenderKey]._Sender = null;

                                _ActiveSenders.Remove(_SenderKey);

                                _Active = false;
                            }
                        }
                    }
                }
            }
            catch { }
        }

        void FlushThread()
        {
            while (!_Stop)
                try
                {
                    Flush();
                }
                finally
                {
                    System.Threading.Thread.Sleep(500);
                }
        }

        void Flush()
        {
            try
            {
                lock (_SendMutex)
                    if (_Active && _SendBuffer.Position > 0)
                    {
                        if (_Sender == null || !_Sender.IsBound)
                        {
                            try
                            {
                                _Sender = BuildSendSocket();
                            }
                            catch
                            {
                                return;
                            }
                        }

                        while (_Sender.Send(_SendBuffer.GetBuffer(), 0, (int)_SendBuffer.Position, SocketFlags.None) != _SendBuffer.Position) { }

                        _PacketsSent++;

                        if (_MSDelay > 0)
                        {
                            _PacketsInDelayWindow++;

                            if (_PacketsInDelayWindow >= _PacketsPerDelay)
                            {
                                SpinWaitSleep(_MSDelay);
                                _PacketsInDelayWindow = 0;
                            }
                        }

                        _SendBuffer.Position = 0;
                    }
            }
            catch { }
        }

        long _PacketsSent = 0;
        public long PacketsSent
        {
            get
            {
                if (_ActiveSenders.ContainsKey(_SenderKey))
                    return _ActiveSenders[_SenderKey]._PacketsSent;

                return 0;
            }
        }

        public void ResetPacketsSent()
        {
            if (_ActiveSenders.ContainsKey(_SenderKey))
                lock (_ActiveSenders[_SenderKey])
                    _ActiveSenders[_SenderKey]._PacketsSent = 0;
        }

        public int Send(string message)
        {
            byte[] b = STEM.Sys.IO.StringCompression.CompressString(message);

            return Send(b, 0, b.Length, false);
        }

        public int Send(byte[] buffer, bool allowBuffering = false)
        {
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentNullException(nameof(buffer));

            return Send(buffer, 0, buffer.Length, allowBuffering);
        }

        public int Send(byte[] buffer, int offset, int size, bool allowBuffering = false)
        {
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentNullException(nameof(buffer));

            return _ActiveSenders[_SenderKey]._Send(buffer, offset, size, allowBuffering);
        }
        
        public int SendTo(string message, string address)
        {
            if (String.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            if (String.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            byte[] b = STEM.Sys.IO.StringCompression.CompressString(message);

            return SendTo(b, 0, b.Length, address);
        }

        public int SendTo(byte[] buffer, string address)
        {
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentNullException(nameof(buffer));

            if (String.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            return SendTo(buffer, 0, buffer.Length, address);
        }

        public int SendTo(byte[] buffer, int offset, int size, string address)
        {
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentNullException(nameof(buffer));

            return _ActiveSenders[_SenderKey]._SendTo(buffer, offset, size, address);
        }

        static void SpinWaitSleep(double milliseconds)
        {
            SpinWaitSleep(TimeSpan.FromTicks((long)(TimeSpan.TicksPerMillisecond * milliseconds)));
        }

        static void SpinWaitSleep(TimeSpan sleep)
        {
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

            try
            {
                stopWatch.Start();

                while (stopWatch.Elapsed < sleep)
                    System.Threading.Thread.SpinWait(1000);
            }
            catch { }
            finally
            {
                stopWatch.Stop();
            }
        }

        int _Send(byte[] buffer, int offset, int size, bool allowBuffering)
        {
            if (size < 1)
                return 0;

            if (size > (buffer.Length - offset))
            {
                STEM.Sys.EventLog.WriteEntry("Sender.Send", "Length of bytes to Send exceed buffer length.", STEM.Sys.EventLog.EventLogEntryType.Error);
                return 0;
            }

            try
            {
                int remaining = size;

                while (remaining > 0)
                {
                    int len = remaining;

                    lock (_SendMutex)
                    {
                        if (_Sender == null || !_Sender.IsBound)
                        {
                            if (_Active)
                            {
                                try
                                {
                                    _Sender = BuildSendSocket();
                                }
                                catch
                                {
                                    return 0;
                                }
                            }
                            else
                            {
                                return 0;
                            }
                        }

                        if (!allowBuffering)
                        {
                            Flush();
                        }
                        else
                        {
                            len = _SendBuffer.Capacity;
                            if (remaining < len)
                                len = remaining;

                            if ((_SendBuffer.Position + len) >= _SendBuffer.Capacity)
                            {
                                if (_SendBuffer.Position > 0)
                                {
                                    while (_Sender.Send(_SendBuffer.GetBuffer(), 0, (int)_SendBuffer.Position, SocketFlags.None) != _SendBuffer.Position) { }

                                    _PacketsSent++;

                                    if (_MSDelay > 0)
                                    {
                                        _PacketsInDelayWindow++;

                                        if (_PacketsInDelayWindow >= _PacketsPerDelay)
                                        {
                                            SpinWaitSleep(_MSDelay);
                                            _PacketsInDelayWindow = 0;
                                        }
                                    }

                                    _SendBuffer.Position = 0;

                                    continue;
                                }
                            }
                            else if (len < (_SendBuffer.Capacity * .75))
                            {
                                if (_FlushThread == null)
                                {
                                    _FlushThread = new Thread(new ThreadStart(FlushThread));
                                    _FlushThread.IsBackground = true;
                                    _FlushThread.Start();
                                }

                                _SendBuffer.Write(buffer, offset, len);
                                break;
                            }
                        }
                    }

                    while (_Sender.Send(buffer, offset, len, SocketFlags.None) != len) { }

                    _PacketsSent++;

                    if (_MSDelay > 0)
                    {
                        _PacketsInDelayWindow++;

                        if (_PacketsInDelayWindow >= _PacketsPerDelay)
                        {
                            SpinWaitSleep(_MSDelay);
                            _PacketsInDelayWindow = 0;
                        }
                    }

                    offset += len;
                    remaining -= len;
                }

                return size;
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Sender.Send", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                return 0;
            }
        }

        int _SendTo(byte[] buffer, int offset, int size, string address)
        {
            if (size < 1)
                return 0;

            if (size > (buffer.Length - offset))
            {
                STEM.Sys.EventLog.WriteEntry("Sender.Send", "Length of bytes to Send exceed buffer length.", STEM.Sys.EventLog.EventLogEntryType.Error);
                return 0;
            }

            try
            {
                int remaining = size;

                while (remaining > 0)
                {
                    int len = remaining;

                    lock (_SendMutex)
                    {
                        if (_Sender == null || !_Sender.IsBound)
                        {
                            if (_Active)
                            {
                                try
                                {
                                    _Sender = BuildSendSocket();
                                }
                                catch
                                {
                                    return 0;
                                }
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }

                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(address), MulticastPort);

                    while (_Sender.SendTo(buffer, offset, len, SocketFlags.None, endPoint) != len) { }

                    _PacketsSent++;

                    if (_MSDelay > 0)
                    {
                        _PacketsInDelayWindow++;

                        if (_PacketsInDelayWindow >= _PacketsPerDelay)
                        {
                            SpinWaitSleep(_MSDelay);
                            _PacketsInDelayWindow = 0;
                        }
                    }

                    offset += len;
                    remaining -= len;
                }

                return size;
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Sender.Send", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                return 0;
            }
        }

        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch { }

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            try
            {
                _SendBuffer.Dispose();
            }
            catch { }
        }
    }
}
