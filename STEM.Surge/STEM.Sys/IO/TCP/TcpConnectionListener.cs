﻿/*
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
using System.IO;
using System.Threading;
using System.Net;
using System.Linq;
using System.Net.Sockets;

namespace STEM.Sys.IO.TCP
{
    public class TcpConnectionListener : IDisposable
    {
        Socket _Socket = null;

        Thread _ListenThread = null;
        Thread _ConnectCaller = null;

        public delegate void ConnectionOpened(TcpConnectionListener caller, Socket soc);
        ConnectionOpened _ConnectionOpened = null;

        public delegate void ServerClosed(TcpConnectionListener caller);
        ServerClosed _ServerClosed = null;
        
        object _ObjectLock = new object();

        bool _SuspendDosLikeBehavior = false;

        public int Port { get; private set; }

        bool _RandomPort = false;

        public TcpConnectionListener(int port)
        {
            Port = port;

            if (Port == 0)
                _RandomPort = true;
        }

        public TcpConnectionListener(int port, bool suspendDosLikeBehavior)
        {
            Port = port;
            _SuspendDosLikeBehavior = suspendDosLikeBehavior;

            if (Port == 0)
                _RandomPort = true;
        }

        Guid _Session = Guid.Empty;
        public event ConnectionOpened onConnect
        {
            add
            {
                if (_Session == Guid.Empty)
                {
                    lock (_ObjectLock)
                    {
                        if (_ConnectionOpened == null)
                        {
                            _Session = Guid.NewGuid();

                            _ConnectionOpened = value;

                            _Socket = null;

                            if (_RandomPort)
                            {
                                _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                _Socket.Bind(new IPEndPoint(System.Net.IPAddress.Any, Port));
                                _Socket.Listen(100);

                                Port = ((System.Net.IPEndPoint)_Socket.LocalEndPoint).Port;
                            }

                            _ListenThread = new Thread(new ParameterizedThreadStart(_Accept));
                            _ListenThread.Priority = ThreadPriority.AboveNormal;
                            _ListenThread.IsBackground = true;
                            _ListenThread.Start(_Session.ToString());

                            _ConnectCaller = new Thread(new ThreadStart(CallConnectionOpened));
                            _ConnectCaller.IsBackground = true;
                            _ConnectCaller.Start();
                        }
                        else
                        {
                            throw new Exception("onConnect has already been bound for this listner.");
                        }
                    }
                }
                else
                {
                    throw new Exception("onConnect has already been bound for this listner.");
                }
            }
            remove
            {
                lock (_ObjectLock)
                {
                    if (_ConnectionOpened == value)
                        Close();
                }
            }
        }

        public event ServerClosed onServerClosed
        {
            add
            {
                lock (_ObjectLock)
                {
                    if (_ServerClosed == null)
                    {
                        _ServerClosed = value;
                    }
                    else
                        throw new Exception("onServerClosed has already been bound for this listner.");
                }
            }
            remove
            {
                lock (_ObjectLock)
                {
                    if (_ServerClosed == value)
                        _ServerClosed = null;
                }
            }
        }

        public void Close()
        {
            if (_Session != Guid.Empty)
                lock (_ObjectLock)
                    try
                    {
                        if (_Session != Guid.Empty)
                        {
                            _ConnectCaller = null;

                            _Session = Guid.Empty;

                            try
                            {
                                if (_Socket != null)
                                    _Socket.Close();
                            }
                            catch { }

                            try
                            {
                                if (_Socket != null)
                                    _Socket.Dispose();
                            }
                            catch { }

                            _ConnectionOpened = null;

                            _Socket = null;

                            _SocketQueue.Clear();

                            if (_ServerClosed != null)
                                _ServerClosed(this);
                        }
                    }
                    catch { }
        }
                
        class Accepted
        {
            public string Address { get;set;}
            public DateTime Time { get; set;}

            public Accepted(Socket s)
            {
                Address = ((System.Net.IPEndPoint)s.RemoteEndPoint).Address.ToString();
                Time = DateTime.UtcNow;
            }
        }

        List<Accepted> _Accepted = new List<Accepted>();
        List<Accepted> _Suspended = new List<Accepted>();

        void _Accept(object o)
        {
            string sessionGuid = o as string;

            Socket listener = null;

            if (_RandomPort)
                listener = _Socket;

            try
            {
                while (sessionGuid == _Session.ToString())
                    try
                    {
                        if (listener == null)
                        {
                            lock (_ObjectLock)
                            {
                                if (sessionGuid == _Session.ToString())
                                {
                                    listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                    listener.Bind(new IPEndPoint(System.Net.IPAddress.Any, Port));
                                    listener.Listen(100);

                                    _Socket = listener;
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }

                        Socket s = listener.Accept();

                        if (s == null)
                            continue;

                        if (sessionGuid == _Session.ToString())
                            lock (_SocketQueue)
                            {
                                _SocketQueue.Enqueue(s);
                                System.Threading.Monitor.Pulse(_SocketQueue);
                            }
                    }
                    catch
                    {
                        if (!_RandomPort)
                        {
                            try
                            {
                                if (listener != null)
                                    listener.Close();
                            }
                            catch { }

                            try
                            {
                                if (listener != null)
                                    listener.Dispose();
                            }
                            catch { }

                            listener = null;
                        }

                        System.Threading.Thread.Sleep(100);
                    }
            }
            finally
            {
                try
                {
                    if (listener != null)
                        listener.Close();
                }
                catch { }

                try
                {
                    if (listener != null)
                        listener.Dispose();
                }
                catch { }

                listener = null;
            }
        }

        Queue<Socket> _SocketQueue = new Queue<Socket>();

        void CallConnectionOpened()
        {
            while (true)
            {
                if (_ConnectCaller != System.Threading.Thread.CurrentThread)
                    return;

                try
                {
                    Socket s = null;

                    if (_SocketQueue.Count > 0)
                        lock (_SocketQueue)
                            if (_SocketQueue.Count > 0)
                                s = _SocketQueue.Dequeue();

                    if (s != null)
                    {
                        if (_SuspendDosLikeBehavior)
                        {
                            lock (_Accepted)
                            {
                                Accepted cur = new Accepted(s);

                                foreach (Accepted a in _Accepted.Where(i => (DateTime.UtcNow - i.Time).TotalMinutes > 1).ToList())
                                    _Accepted.Remove(a);

                                _Accepted.Add(cur);
                                Accepted suspended = _Suspended.FirstOrDefault(i => i.Address == cur.Address);

                                if (suspended != null)
                                {
                                    if ((DateTime.UtcNow - suspended.Time).TotalMinutes > 1)
                                    {
                                        _Suspended.Remove(suspended);

                                        if (_Accepted.Count(i => i.Address == cur.Address) > 30)
                                        {
                                            _Suspended.Add(cur);

                                            try
                                            {
                                                s.Close();
                                            }
                                            catch { }

                                            continue;
                                        }

                                        try
                                        {
                                            STEM.Sys.EventLog.WriteEntry("TcpConnectionListener.CallConnectionOpened", "Lifting suspension for address " + cur.Address + " for DOS attack behavior.", STEM.Sys.EventLog.EventLogEntryType.Error);
                                        }
                                        catch { }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            s.Close();
                                        }
                                        catch { }

                                        continue;
                                    }
                                }

                                if (_Accepted.Count(i => i.Address == cur.Address) > 30)
                                {
                                    _Suspended.Add(cur);

                                    try
                                    {
                                        s.Close();
                                    }
                                    catch { }

                                    try
                                    {
                                        STEM.Sys.EventLog.WriteEntry("TcpConnectionListener.CallConnectionOpened", "Suspending address " + cur.Address + " for DOS attack behavior.", STEM.Sys.EventLog.EventLogEntryType.Error);
                                    }
                                    catch { }

                                    continue;
                                }
                            }
                        }

                        try
                        {
                            if (_ConnectionOpened != null)
                                _ConnectionOpened(this, s);
                        }
                        catch (Exception ex)
                        {
                            STEM.Sys.EventLog.WriteEntry("TcpConnectionListener.onConnect", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                        }
                    }
                    else
                    {
                        if (_SocketQueue.Count == 0)
                            lock (_SocketQueue)
                                if (_SocketQueue.Count == 0)
                                    System.Threading.Monitor.Wait(_SocketQueue, 30000);
                    }
                }
                catch { }
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
            Close();
        }
    }
}















