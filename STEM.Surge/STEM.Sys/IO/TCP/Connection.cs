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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace STEM.Sys.IO.TCP
{
    public enum Role { Server, Client }

    public class Connection : IComparable, IDisposable
    {
        TcpClient _TcpClient = null;
        SslStream _SslStream = null;
        bool _SslConnection = false;

        Thread _Receiver = null;
        Thread _CalloutThread = null;
        object _AccessMutex = new object();

        bool _CloseBroadcast = true;
        void BroadcastClose()
        {
            lock (_AccessMutex)
            {
                if (!_CloseBroadcast)
                {
                    lock (_CallOutQueue)
                    {
                        EnqueueCallout(new ConnectionClosed(_Closed));

                        if (_ConnectionClosed != null)
                            foreach (ConnectionClosed c in _ConnectionClosed.GetInvocationList())
                                EnqueueCallout(c);
                    }

                    _CloseBroadcast = true;
                }
            }
        }

        public Role ConnectionRole { get; private set; }
        public int LocalPort { get; private set; }
        public string LocalAddress { get; private set; }
        public int RemotePort { get; private set; }
        public string RemoteAddress { get; private set; }

        public bool AutoReconnect { get; set; }

        public string Username { get; private set; }
        public string UserDescription { get; private set; }

        X509Certificate2 _ClientCertificate = null;

        public Connection(string ipAddress, int port, bool sslConnection, bool autoReconnect = false)
        {
            ConnectionRole = TCP.Role.Client;

            if (ipAddress == STEM.Sys.IO.Net.EmptyAddress)
                throw new Exception("Attempt to connect to the EmptyAddress");

            RemoteAddress = ipAddress;
            RemotePort = port;

            AutoReconnect = autoReconnect;

            _SslConnection = sslConnection;
        }

        public Connection(string ipAddress, int port, bool sslConnection, X509Certificate2 certificate, bool autoReconnect = false)
        {
            ConnectionRole = TCP.Role.Client;

            if (ipAddress == STEM.Sys.IO.Net.EmptyAddress)
                throw new Exception("Attempt to connect to the EmptyAddress");

            RemoteAddress = ipAddress;
            RemotePort = port;

            AutoReconnect = autoReconnect;

            _ClientCertificate = certificate;
            _SslConnection = sslConnection;
        }

        public Connection(TcpClient client, X509Certificate2 certificate)
        {
            if (_TcpClient == client || client == null)
                return;

            ConnectionRole = TCP.Role.Server;

            _TcpClient = client;

            _TcpClient.LingerState = new LingerOption(true, 0);
            _TcpClient.NoDelay = true;

            RemoteAddress = ((System.Net.IPEndPoint)_TcpClient.Client.RemoteEndPoint).Address.ToString();
            RemotePort = ((System.Net.IPEndPoint)_TcpClient.Client.RemoteEndPoint).Port;
            LocalAddress = ((System.Net.IPEndPoint)_TcpClient.Client.LocalEndPoint).Address.ToString();
            LocalPort = ((System.Net.IPEndPoint)_TcpClient.Client.LocalEndPoint).Port;

            if (certificate != null)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                _SslStream = new SslStream(_TcpClient.GetStream(), true, new RemoteCertificateValidationCallback(ValidateCertificate), null, EncryptionPolicy.RequireEncryption);
                _SslStream.AuthenticateAsServer(certificate, false, System.Security.Authentication.SslProtocols.Tls12, false);
            }
        }

        bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool valid = true;

            if (sslPolicyErrors != SslPolicyErrors.None)
                STEM.Sys.EventLog.WriteEntry("ValidateCertificate", sslPolicyErrors.ToString(), EventLog.EventLogEntryType.Warning);

            //if (certificate != null)
            //    lock (_AccessMutex)
            //    {
            //        try
            //        {
            //            valid = false;

            //            if (sslPolicyErrors == SslPolicyErrors.None ||
            //                sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
            //                valid = true;
            //        }
            //        finally
            //        {
            //            if (!valid)
            //            {
            //                try
            //                {
            //                    _TcpClient.Client.Shutdown(SocketShutdown.Both);
            //                }
            //                catch { }

            //                try
            //                {
            //                    _TcpClient.Close();
            //                }
            //                catch { }

            //                try
            //                {
            //                    _TcpClient.Dispose();
            //                }
            //                catch { }

            //                _TcpClient = null;
            //                _SslStream = null;
            //            }
            //        }
            //    }

            return valid;
        }

        void Bind(ConnectionOpened callback = null)
        {
            lock (_AccessMutex)
            {
                if (ConnectionRole == TCP.Role.Server)
                {
                    if (_Receiver != null && IsConnected())
                    {
                        if (callback != null)
                            EnqueueCallout(callback);

                        return;
                    }

                    if (!IsConnected())
                        throw new IOException("Client is no longer connected to this server.");
                }
                else
                {
                    if (!IsConnected())
                    {
                        try
                        {
                            _TcpClient = null;

                            string address = STEM.Sys.IO.Net.MachineAddress(RemoteAddress);

                            if (address == null || System.Net.IPAddress.None.ToString() == address)
                                throw new Exception("Address could not be reached.");

                            TcpClient client = new TcpClient(AddressFamily.InterNetwork);
                            client.Connect(RemoteAddress, RemotePort);

                            _TcpClient = client;
                            
                            if (_SslConnection)
                            {
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                _SslStream = new SslStream(_TcpClient.GetStream(), true, new RemoteCertificateValidationCallback(ValidateCertificate), null, EncryptionPolicy.RequireEncryption);
                                if (_ClientCertificate != null)
                                {
                                    X509CertificateCollection certCollection = new X509CertificateCollection();
                                    certCollection.Add(_ClientCertificate);
                                    _SslStream.AuthenticateAsClient("STEM.Surge", certCollection, System.Security.Authentication.SslProtocols.Tls12, false);
                                }
                                else
                                {
                                    _SslStream.AuthenticateAsClient("STEM.Surge");
                                }
                            }

                            if (!IsConnected())
                                throw new IOException();
                        }
                        catch 
                        {
                            if (_TcpClient != null)
                            {
                                try
                                {
                                    _TcpClient.Client.Shutdown(SocketShutdown.Both);
                                }
                                catch { }

                                try
                                {
                                    _TcpClient.Close();
                                }
                                catch { }

                                try
                                {
                                    _TcpClient.Dispose();
                                }
                                catch { }
                            }

                            _TcpClient = null;
                            _SslStream = null;
                            return;
                        }
                    }
                    else
                    {
                        if (callback != null)
                            EnqueueCallout(callback);

                        return;
                    }
                }

                if (_TcpClient != null)
                {
                    _TcpClient.ReceiveBufferSize = 1024 * 1024 * 256;
                    _TcpClient.SendBufferSize = 1024 * 256;
                    _TcpClient.Client.DontFragment = true;
                    _TcpClient.Client.Ttl = 42;

                    _TcpClient.LingerState = new LingerOption(true, 0);
                    _TcpClient.NoDelay = true;

                    RemoteAddress = ((System.Net.IPEndPoint)_TcpClient.Client.RemoteEndPoint).Address.ToString();


                    if (RemoteAddress == STEM.Sys.IO.Net.EmptyAddress)
                        throw new Exception("Attempt to connect to the EmptyAddress");

                    RemotePort = ((System.Net.IPEndPoint)_TcpClient.Client.RemoteEndPoint).Port;
                    LocalAddress = ((System.Net.IPEndPoint)_TcpClient.Client.LocalEndPoint).Address.ToString();
                    LocalPort = ((System.Net.IPEndPoint)_TcpClient.Client.LocalEndPoint).Port;

                    if (_Receiver == null)
                    {
                        _Receiver = new Thread(new ThreadStart(Receive));
                        _Receiver.IsBackground = true;
                        _Receiver.Priority = ThreadPriority.Highest;
                        _Receiver.Start();
                    }

                    lock (_CallOutQueue)
                    {
                        if (_ConnectionOpened != null)
                            foreach (ConnectionOpened c in _ConnectionOpened.GetInvocationList())
                                EnqueueCallout(c);

                        EnqueueCallout(new ConnectionOpened(_Opened));
                    }

                    _CloseBroadcast = false;
                }
            }
        }

        protected void Recycle(byte[] buf)
        {
            lock (_Recycler)
            {
                if (_Recycler.Count < 50)
                    _Recycler.Add(buf);
            }
        }

        static List<byte[]> _Recycler = new List<byte[]>();

        byte[] GetBuffer(int size)
        {
            byte[] ret = null;

            lock (_Recycler)
            {
                ret = _Recycler.OrderBy(i => i.Length).FirstOrDefault(i => i.Length >= size);

                if (ret != null)
                    _Recycler.Remove(ret);
                else if (_Recycler.Count == 50)
                    _Recycler.RemoveAt(0);
            }

            if (ret == null)
                ret = new byte[size];

            return ret;
        }

        void Receive()
        {
            DateTime lastConnectionTest = DateTime.UtcNow;

            try
            {
                while (_Receiver == System.Threading.Thread.CurrentThread)
                {
                    TcpClient client = _TcpClient;

                    if (client != null && client.Connected)
                    {
                        if (!_Open)
                        {
                            System.Threading.Thread.Sleep(100);
                            continue;
                        }

                        try
                        {
                            int rcvd = 0;

                            byte[] buf = GetBuffer(2097152);

                            int pos = 0;
                            lock (client)
                                while (client.Available > 0 && pos < buf.Length)
                                {
                                    lastConnectionTest = DateTime.UtcNow;

                                    if (_SslStream != null)
                                    {
                                        rcvd = _SslStream.Read(buf, pos, buf.Length - pos);
                                    }
                                    else
                                    {
                                        rcvd = client.GetStream().Read(buf, pos, buf.Length - pos);
                                    }

                                    pos += rcvd;

                                    if (rcvd == 0)
                                        break;
                                }

                            rcvd = pos;

                            if (rcvd > 0)
                            {
                                Receive(buf, rcvd, DateTime.UtcNow); 
                            }
                            else
                            {
                                if (buf != null)
                                {
                                    Recycle(buf);
                                    buf = null;
                                }

                                if ((DateTime.UtcNow - lastConnectionTest).TotalSeconds > 5)
                                {
                                    IsConnected();
                                    lastConnectionTest = DateTime.UtcNow;
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(10);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (client.Connected)
                            {
                                STEM.Sys.EventLog.WriteEntry("Connection.Receive", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                            }
                            else
                            {
                                IsConnected();
                            }
                        }
                    }
                    else
                    {
                        if (!IsConnected() && ConnectionRole == TCP.Role.Server)
                            return;

                        System.Threading.Thread.Sleep(10);
                    }
                }
            }
            finally
            {
                lock (_AccessMutex)
                    if (_Receiver == System.Threading.Thread.CurrentThread)
                        _Receiver = null;
            }
        }

        static byte[] ConnectionTestMessage = STEM.Sys.IO.StringCompression.CompressString(new STEM.Sys.Messaging.ConnectionTest().Serialize());

        public bool IsConnected()
        {
            if (_CloseBroadcast && _TcpClient == null)
                return false;

            TcpClient client = _TcpClient;

            if (client != null)
                lock (client)
                {
                    try
                    {
                        if (_SslStream != null)
                        {
                            _SslStream.Write(ConnectionTestMessage, 0, ConnectionTestMessage.Length);
                            _SslStream.Flush();
                        }
                        else
                        {
                            client.GetStream().Write(ConnectionTestMessage, 0, ConnectionTestMessage.Length);
                        }

                        if (client.Connected)
                        {
                            return true;
                        }
                    }
                    catch
                    {
                    }
                }

            lock (_AccessMutex)
            {
                if (client == _TcpClient)
                {
                    try
                    {
                        _TcpClient.Client.Shutdown(SocketShutdown.Both);
                    }
                    catch { }

                    try
                    {
                        _TcpClient.Close();
                    }
                    catch { }

                    try
                    {
                        _TcpClient.Dispose();
                    }
                    catch { }

                    _TcpClient = null;
                    _SslStream = null;

                    BroadcastClose();
                }

                return false;
            }
        }

        public virtual void Close()
        {
            lock (_AccessMutex)
            {
                if (_TcpClient != null)
                {
                    try
                    {
                        if (_TcpClient.Connected)
                        {
                            try
                            {
                                _TcpClient.Client.Shutdown(SocketShutdown.Both);
                            }
                            catch { }

                            try
                            {
                                _TcpClient.Close();
                            }
                            catch { }

                            try
                            {
                                _TcpClient.Dispose();
                            }
                            catch { }
                        }
                    }
                    catch (Exception ex)
                    {
                        STEM.Sys.EventLog.WriteEntry("Connection.Close:_TcpClient.Close", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                    }

                    try
                    {
                        _TcpClient.Close();
                    }
                    catch { }
                    
                    _TcpClient = null;
                    _SslStream = null;

                    try
                    {
                        if (_Receiver != null)
                        {
                            Thread t = _Receiver;
                            _Receiver = null;

                            try
                            {
                                t.Interrupt();
                            }
                            catch { }

                            try
                            {
                                t.Abort();
                            }
                            catch { }
                        }
                    }
                    catch { }

                    _Receiver = null;
                }
                else
                {
                    return;
                }

                BroadcastClose();
            }
        }

        /// <summary>
        /// Messages are queued to insure the socket remains clear.
        /// If your message handling is time consuming and your inbound message
        /// traffic backing up, you can see how bad things have gotten by looking
        /// at MessageBacklog.
        /// </summary>
        public virtual int MessageBacklog
        {
            get
            {
                return 0;
            }
        }

        void EnqueueCallout(object c)
        {
            lock (_CallOutQueue)
            {
                _CallOutQueue.Enqueue(c);

                System.Threading.Monitor.Pulse(_CallOutQueue);

                if (_CalloutThread == null)
                {
                    _CalloutThread = new Thread(new ThreadStart(_CallOut));
                    _CalloutThread.IsBackground = true;
                    _CalloutThread.Start();
                }
            }
        }

        bool _Open = false;
        void _Opened(Connection connection)
        {
            _Open = true;
        }

        void _Closed(Connection connection)
        {
            _Open = false;
        }

        /// <summary>
        /// If this is a client connection, Reconnect attempts a reconnect to the server.
        /// </summary>
        /// <param name="timeout">Maximum period of time to attempt to reconnect</param>
        /// <returns>True if the connection has been re-established</returns>
        public bool Reconnect(TimeSpan timeout)
        {
            if (ConnectionRole == TCP.Role.Server)
                throw new Exception("Cannot reconnect from server.");

            while (!IsConnected() && timeout.TotalSeconds > 0)
            {
                Bind();

                if (!IsConnected())
                {
                    System.Threading.Thread.Sleep(1000);
                    timeout = timeout.Subtract(TimeSpan.FromSeconds(1));
                }
            }

            return IsConnected();
        }

        /// <summary>
        /// If this is a client connection, Reconnect attempts a reconnect to the server.
        /// </summary>
        /// <returns>True if the connection has been re-established</returns>
        public bool Reconnect()
        {
            return Reconnect(TimeSpan.FromSeconds(1));
        }

        void _Reconnect()
        {
            if (AutoReconnect)
                Reconnect();
        }

        public virtual void Receive(byte[] message, int length, DateTime received) { }
                
        public delegate void ConnectionOpened(Connection connection);
        ConnectionOpened _ConnectionOpened;
        public event ConnectionOpened onOpened
        {
            add
            {
                lock (_AccessMutex)
                    if (_ConnectionOpened == null || !_ConnectionOpened.GetInvocationList().ToList().Contains(value))
                    {
                        _ConnectionOpened += value;
                        Bind(value);

                        if (ConnectionRole == TCP.Role.Client && AutoReconnect)
                            STEM.Sys.Global.ThreadPool.BeginAsync(new System.Threading.ThreadStart(_Reconnect), TimeSpan.FromSeconds(3));
                    }
            }

            remove
            {
                lock (_AccessMutex)
                {
                    if (_ConnectionOpened != null && _ConnectionOpened.GetInvocationList().ToList().Contains(value))
                        _ConnectionOpened -= value;

                    if (_ConnectionOpened == null || _ConnectionOpened.GetInvocationList().Length == 0)
                        if (ConnectionRole == TCP.Role.Client && AutoReconnect)
                            STEM.Sys.Global.ThreadPool.EndAsync(new System.Threading.ThreadStart(_Reconnect));
                }
            }
        }

        public delegate void ConnectionClosed(Connection connection);
        ConnectionClosed _ConnectionClosed = null;
        public event ConnectionClosed onClosed
        {
            add
            {
                lock (_AccessMutex)
                    if (_ConnectionClosed == null || !_ConnectionClosed.GetInvocationList().ToList().Contains(value))
                    {
                        _ConnectionClosed += value;

                        if (!IsConnected() && _Open)
                            EnqueueCallout(value);
                    }
            }

            remove
            {
                lock (_AccessMutex)
                    if (_ConnectionClosed != null && _ConnectionClosed.GetInvocationList().ToList().Contains(value))
                        _ConnectionClosed -= value;
            }
        }

        Queue<object> _CallOutQueue = new Queue<object>();
        void _CallOut()
        {
            try
            {
                while (true)
                {
                    if (_CalloutThread != System.Threading.Thread.CurrentThread)
                        return;

                    object cb = null;
                    if (_CallOutQueue.Count > 0)
                        lock (_CallOutQueue)
                            if (_CallOutQueue.Count > 0)
                                if (_CalloutThread == System.Threading.Thread.CurrentThread)
                                    cb = _CallOutQueue.Dequeue();

                    if (cb == null)
                        lock (_CallOutQueue)
                            if (_CallOutQueue.Count == 0)
                                if (_CalloutThread == System.Threading.Thread.CurrentThread)
                                {
                                    System.Threading.Monitor.Wait(_CallOutQueue, 250);

                                    if (_CallOutQueue.Count == 0)
                                        if (_CalloutThread == System.Threading.Thread.CurrentThread)
                                        {
                                            _CalloutThread = null;
                                            return;
                                        }

                                    continue;
                                }
                                else
                                {
                                    return;
                                }

                    if (cb != null)
                    {
                        if (cb is ConnectionClosed)
                        {
                            ConnectionClosed c = cb as ConnectionClosed;

                            try
                            {
                                c(this);
                            }
                            catch (Exception ex)
                            {
                                STEM.Sys.EventLog.WriteEntry("Connection.ConnectionClosed", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                            }
                        }
                        else if (cb is ConnectionOpened)
                        {
                            ConnectionOpened c = cb as ConnectionOpened;
                            try
                            {
                                c(this);
                            }
                            catch (Exception ex)
                            {
                                STEM.Sys.EventLog.WriteEntry("Connection.ConnectionOpened", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                            }
                        }

                        cb = null;
                    }
                }
            }
            finally
            {
                lock (_CallOutQueue)
                    if (_CalloutThread == System.Threading.Thread.CurrentThread)
                        _CalloutThread = null;
            }
        }

        public bool Send(byte[] message)
        {
            if (message == null || message.Length == 0)
                throw new ArgumentNullException(nameof(message));

            return Send(message, 0, message.Length);
        }

        public bool Send(byte[] message, int offset, int length)
        {
            if (message == null || message.Length == 0 || message.Length < offset)
                throw new ArgumentNullException(nameof(message));

            TcpClient client = _TcpClient;

            if (client != null && client.Connected)
                try
                {
                    length = length + offset;

                    while (client != null && client.Connected && offset < length)
                        try
                        {
                            lock (client)
                                if (_SslStream != null)
                                {
                                    _SslStream.Write(message, offset, length - offset);
                                    _SslStream.Flush();
                                }
                                else
                                {
                                    client.GetStream().Write(message, offset, length - offset);
                                }

                            offset += length - offset;
                        }
                        catch (SocketException ex)
                        {
                            if (ex.SocketErrorCode == SocketError.WouldBlock ||
                                ex.SocketErrorCode == SocketError.IOPending ||
                                ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                            {
                                // socket buffer is probably full, wait and try again
                                Thread.Sleep(10);
                            }
                            else
                            {
                                STEM.Sys.EventLog.WriteEntry("Connection.Send", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                            }
                        }

                    return offset == length;
                }
                catch (Exception ex)
                {
                    if (client.Connected)
                    {
                        STEM.Sys.EventLog.WriteEntry("Connection.Send", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                    }
                    else
                    {
                        IsConnected();
                    }
                }

            return false;
        }

        public int CompareTo(object obj)
        {
            Connection c = obj as Connection;
            if (c != null)
            {
                if (c.LocalPort == LocalPort &&
                    c.LocalAddress.Equals(LocalAddress, StringComparison.InvariantCultureIgnoreCase) &&
                    c.RemotePort == RemotePort &&
                    c.RemoteAddress.Equals(RemoteAddress, StringComparison.InvariantCultureIgnoreCase))
                    return 0;
            }

            return -1;
        }

        public static bool operator ==(Connection a, Connection b)
        {
            try
            {
                bool aNull = object.ReferenceEquals(a, null);
                bool bNull = object.ReferenceEquals(b, null);

                if (aNull && bNull)
                    return true;

                if ((aNull && !bNull) || (!aNull && bNull))
                    return false;

                return a.CompareTo(b) == 0;
            }
            catch { }

            return false;
        }

        public static bool operator !=(Connection a, Connection b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is Connection)
            {
                Connection a = obj as Connection;
                return a == this;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (LocalAddress.ToUpper(System.Globalization.CultureInfo.CurrentCulture) + ":" + LocalPort + "," + RemoteAddress.ToUpper(System.Globalization.CultureInfo.CurrentCulture) + ":" + RemotePort).GetHashCode();
        }

        public override string ToString()
        {
            return (LocalAddress.ToUpper(System.Globalization.CultureInfo.CurrentCulture) + ":" + LocalPort + "," + RemoteAddress.ToUpper(System.Globalization.CultureInfo.CurrentCulture) + ":" + RemotePort);
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
                _TcpClient.Dispose();
            }
            catch { }
        }
    }
}