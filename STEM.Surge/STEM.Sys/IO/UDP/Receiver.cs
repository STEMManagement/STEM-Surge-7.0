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
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace STEM.Sys.IO.UDP
{
    public class Receiver : STEM.Sys.IO.UDP.SocketHelper
    {
        static List<string> _ActiveListeners = new List<string>();

        List<System.Threading.Thread> _ListenThreads = new List<System.Threading.Thread>();

        string _ActiveListenerKey = null;

        bool _Enabled = false;
        public virtual bool Enable
        {
            get
            {
                return _Enabled;
            }
            set
            {
                lock (_ActiveListeners)
                    if (value == _Enabled)
                        return;

                if (value)
                {
                    lock (_ActiveListeners)
                        if (!_Enabled)
                            lock (_ListenThreads)
                            {
                                if (_ActiveListeners.Contains(LocalNetworkAdapter.ToUpper(System.Globalization.CultureInfo.CurrentCulture) + ":" + MulticastIP + ":" + MulticastPort.ToString(System.Globalization.CultureInfo.CurrentCulture)))
                                    return;

                                _Enabled = true;
                                _ActiveListenerKey = LocalNetworkAdapter.ToUpper(System.Globalization.CultureInfo.CurrentCulture) + ":" + MulticastIP + ":" + MulticastPort.ToString(System.Globalization.CultureInfo.CurrentCulture);
                                _ActiveListeners.Add(_ActiveListenerKey);

                                while (_ListenThreads.Count < 3)
                                {
                                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(Listen));
                                    _ListenThreads.Add(t);
                                    t.IsBackground = true;
                                    t.Start();
                                }
                            }
                }
                else
                {
                    lock (_ActiveListeners)
                        if (_Enabled)
                        {
                            _Enabled = false;

                            if (_Listener != null)
                            {
                                Close(_Listener);

                                while (_Listener.Connected)
                                    System.Threading.Thread.Sleep(10);

                                _Listener = null;
                            }

                            lock (FullBuffers)
                                FullBuffers.Clear();

                            lock (_ListenThreads)
                            {
                                _ListenThreads.Clear();

                                if (_ActiveListenerKey != null)
                                {
                                    if (_ActiveListeners.Contains(_ActiveListenerKey))
                                        _ActiveListeners.Remove(_ActiveListenerKey);

                                    _ActiveListenerKey = null;
                                    _Enabled = false;
                                }
                            }
                        }
                }
            }
        }

        public Receiver(int multicastPort, string localNetworkAdapter, string multicastIP)
            : base(multicastPort, localNetworkAdapter, multicastIP)
        {
            if (_ActiveListeners.Contains(LocalNetworkAdapter.ToUpper(System.Globalization.CultureInfo.CurrentCulture) + ":" + MulticastIP + ":" + MulticastPort.ToString(System.Globalization.CultureInfo.CurrentCulture)))
                return;
        }

        Queue<ByteBuffer> FullBuffers = new Queue<ByteBuffer>();
        Queue<ByteBuffer> RecycledBuffers = new Queue<ByteBuffer>();

        public ByteBuffer Receive()
        {
            ByteBuffer bb = null;
            try
            {
                lock (FullBuffers)
                    if (FullBuffers.Count > 0)
                        bb = FullBuffers.Dequeue();
            }
            catch { }

            return bb;
        }

        public void Receive(ByteBuffer message, out string ret)
        {
            if (message == null)
            {
                ret = null;
                return;
            }

            try
            {
                ret = STEM.Sys.IO.StringCompression.DecompressString(message.Bytes, message.Bytes.Length);
                return;
            }
            catch { }
            finally
            {
                Recycle(message);
            }

            ret = null;
        }

        public void Receive(ByteBuffer message, out byte[] ret)
        {
            if (message == null)
            {
                ret = null;
                return;
            }

            try
            {
                ret = message.Bytes.Take(message.Length).ToArray();
                return;
            }
            catch { }
            finally
            {
                Recycle(message);
            }

            ret = null;
        }

        public void Recycle(ByteBuffer buf)
        {
            try
            {
                if (buf != null)
                    if (RecycledBuffers.Count < 100)
                        if (!RecycledBuffers.Contains(buf))
                        {
                            buf.Length = 0;
                            RecycledBuffers.Enqueue(buf);
                        }
            }
            catch { }
        }

        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        Socket _Listener = null;
        void Listen()
        {
            try
            {
                while (_Enabled)
                {
                    try
                    {
                        lock (_ListenThreads)
                            if (!_ListenThreads.Contains(System.Threading.Thread.CurrentThread))
                                return;

                        if (_Listener == null)
                            lock (_ListenThreads)
                            {
                                if (_Enabled && _Listener == null && _ListenThreads.Contains(System.Threading.Thread.CurrentThread))
                                {
                                    stopWatch = new System.Diagnostics.Stopwatch();
                                    stopWatch.Start();
                                    _Listener = BuildReceiveSocket();
                                }
                            }

                        while (_Enabled)
                        {
                            ByteBuffer buf = null;

                            lock (_ListenThreads)
                            {
                                if (!_ListenThreads.Contains(System.Threading.Thread.CurrentThread))
                                    return;

                                if (_Enabled && _Listener != null && _ListenThreads.Contains(System.Threading.Thread.CurrentThread))
                                {
                                    while (true)
                                    {
                                        try
                                        {
                                            if (RecycledBuffers.Count > 0)
                                                buf = RecycledBuffers.Dequeue();

                                            break;
                                        }
                                        catch { }
                                    }

                                    if (buf == null)
                                        buf = new ByteBuffer(LargestBlockSize);

                                    buf.Length = _Listener.Receive(buf.Bytes, 0, buf.Bytes.Length, SocketFlags.None);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (buf.Length > 0 && _Enabled)
                            {
                                lock (FullBuffers)
                                    FullBuffers.Enqueue(buf);
                            }
                            else
                            {
                                Recycle(buf);
                            }
                        }
                    }
                    catch (System.Net.Sockets.SocketException ex)
                    {
                        if (ex.Message.Contains("period of time"))
                        {
                            if (stopWatch.ElapsedMilliseconds > 30000)
                            {
                                lock (_ListenThreads)
                                    if (_Enabled && _Listener != null && _ListenThreads.Contains(System.Threading.Thread.CurrentThread))
                                    {
                                        Close(_Listener);

                                        while (_Listener.Connected)
                                            System.Threading.Thread.Sleep(10);

                                        _Listener = null;
                                    }

                                stopWatch.Stop();
                            }

                            continue;
                        }

                        throw;
                    }
                    catch (Exception ex)
                    {
                        if (_Enabled)
                            STEM.Sys.EventLog.WriteEntry("Receiver.Listen", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);

                        lock (_ListenThreads)
                            if (_Enabled && _Listener != null && _ListenThreads.Contains(System.Threading.Thread.CurrentThread))
                            {
                                Close(_Listener);

                                while (_Listener.Connected)
                                    System.Threading.Thread.Sleep(10);

                                _Listener = null;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_Enabled)
                    STEM.Sys.EventLog.WriteEntry("Receiver.Listen", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }
            finally
            {
                lock (_ActiveListeners)
                    lock (_ListenThreads)
                        if (_ListenThreads.Contains(System.Threading.Thread.CurrentThread))
                        {
                            _ListenThreads.Remove(System.Threading.Thread.CurrentThread);

                            if (_ListenThreads.Count == 0)
                                if (_ActiveListenerKey != null)
                                {
                                    if (_ActiveListeners.Contains(_ActiveListenerKey))
                                        _ActiveListeners.Remove(_ActiveListenerKey);

                                    _ActiveListenerKey = null;
                                    _Enabled = false;

                                    if (_Listener != null)
                                    {
                                        Close(_Listener);

                                        while (_Listener.Connected)
                                            System.Threading.Thread.Sleep(10);

                                        _Listener = null;
                                    }
                                }
                        }
            }
        }
    }
}
