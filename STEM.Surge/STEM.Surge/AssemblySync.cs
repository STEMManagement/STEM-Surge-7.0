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
using STEM.Sys.IO.TCP;
using STEM.Surge.Messages;
using STEM.Sys.Threading;

namespace STEM.Surge
{
    public class AssemblySync : STEM.Sys.Threading.IThreadable
    {
        static STEM.Sys.Threading.ThreadPool _Pool = new Sys.Threading.ThreadPool(Environment.ProcessorCount, true);

        AssemblyList AssemblyList = new Messages.AssemblyList();

        string _ExtensionDirectory = null;
        bool _Recurse = false;

        public AssemblySync(string extensionDirectory, bool recurse)
        {
            _ExtensionDirectory = extensionDirectory;
            _Recurse = recurse;
            _Pool.BeginAsync(this, TimeSpan.FromSeconds(1));
        }

        public void RegisterList(AssemblyList list)
        {
            lock (_RegisteredConnections)
            {
                DeliverDelta d = null;
                if (_RegisteredConnections.ContainsKey(list.MessageConnection))
                {
                    d = _RegisteredConnections[list.MessageConnection];
                    d.ReceiveList(list);
                    return;
                }

                if (d == null)
                {
                    d = new DeliverDelta(AssemblyList, list);
                    _RegisteredConnections[list.MessageConnection] = d;
                    _Pool.BeginAsync(d, TimeSpan.FromSeconds(3));

                    list.MessageConnection.onClosed += MessageConnection_onClosed;

                    return;
                }
            }
        }

        private void MessageConnection_onClosed(Connection connection)
        {
            MessageConnection c = connection as MessageConnection;

            lock (_RegisteredConnections)
                if (_RegisteredConnections.ContainsKey(c))
                {
                    _RegisteredConnections[c].Dispose();
                    _RegisteredConnections.Remove(c);
                }
        }

        Dictionary<MessageConnection, DeliverDelta> _RegisteredConnections = new Dictionary<MessageConnection, DeliverDelta>();

        class DeliverDelta : STEM.Sys.Threading.IThreadable
        {
            AssemblyList _MasterList;
            AssemblyList _ClientList;

            object _ListLock = new object();

            public DeliverDelta(AssemblyList masterList, AssemblyList clientList)
            {
                _MasterList = masterList;
                _ClientList = clientList;
            }

            public void ReceiveList(AssemblyList list)
            {
                if (System.Threading.Monitor.TryEnter(_ListLock))
                {
                    try
                    {
                        _ClientList = list;
                    }
                    finally
                    {
                        System.Threading.Monitor.Exit(_ListLock);
                    }
                }
            }

            protected override void Execute(ThreadPool owner)
            {
                bool deliveredAsms = false;
                bool connectionClosed = false;

                try
                {
                    lock (_ListLock)
                    {
                        bool initComplete = true;

                        List<string> listContent = _ClientList.Descriptions.Select(j => STEM.Sys.IO.Path.AdjustPath(j.Filename).ToUpper()).ToList();
                        List<string> localContent;

                        while (true)
                            try
                            {
                                localContent = _MasterList.Descriptions.Select(j => STEM.Sys.IO.Path.AdjustPath(j.Filename).ToUpper()).ToList();
                                break;
                            }
                            catch { }

                        string platform = "";

                        if (_ClientList.IsWindows)
                        {
                            platform = "win-x86";
                            if (_ClientList.IsX64)
                                platform = "win-x64";
                        }
                        else
                        {
                            platform = "linux-x86";
                            if (_ClientList.IsX64)
                                platform = "linux-x64";
                        }

                        foreach (string name in localContent.ToList())
                        {
                            STEM.Sys.IO.FileDescription f = null;

                            while (true)
                                try
                                {
                                    f = _MasterList.Descriptions.FirstOrDefault(i => i.Filename.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                                    break;
                                }
                                catch { }

                            string fullPath = System.IO.Path.Combine(f.Filepath, f.Filename);

                            if (fullPath.IndexOf("win-x86", StringComparison.InvariantCultureIgnoreCase) >= 0 && !platform.Equals("win-x86", StringComparison.InvariantCultureIgnoreCase))
                                localContent.Remove(name);
                            if (fullPath.IndexOf("win-x64", StringComparison.InvariantCultureIgnoreCase) >= 0 && !platform.Equals("win-x64", StringComparison.InvariantCultureIgnoreCase))
                                localContent.Remove(name);
                            if (fullPath.IndexOf("linux-x86", StringComparison.InvariantCultureIgnoreCase) >= 0 && !platform.Equals("linux-x86", StringComparison.InvariantCultureIgnoreCase))
                                localContent.Remove(name);
                            if (fullPath.IndexOf("linux-x64", StringComparison.InvariantCultureIgnoreCase) >= 0 && !platform.Equals("linux-x64", StringComparison.InvariantCultureIgnoreCase))
                                localContent.Remove(name);
                        }

                        initComplete = localContent.Except(listContent).Count() == 0;

                        if (initComplete)
                        {
                            if (!_ClientList.MessageConnection.Send(new AssemblyInitializationComplete()))
                            {
                                STEM.Sys.EventLog.WriteEntry("AssemblySync.DeliverDelta", "SendAssemblyList: Forced disconnect, " + _ClientList.MessageConnection.RemoteAddress, STEM.Sys.EventLog.EventLogEntryType.Information);

                                _ClientList.MessageConnection.Close();
                                connectionClosed = true;
                            }

                            ExecutionInterval = TimeSpan.FromSeconds(30);

                            return;
                        }

                        AssemblyList send = new AssemblyList();
                        send.Path = _ClientList.Path;

                        foreach (string name in localContent.Except(listContent).ToList())
                        {
                            STEM.Sys.IO.FileDescription f = null;

                            while (true)
                                try
                                {
                                    f = _MasterList.Descriptions.FirstOrDefault(i => i.Filename.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                                    break;
                                }
                                catch { }

                            send.Descriptions.Add(f);
                            _ClientList.Descriptions.Add(new STEM.Sys.IO.FileDescription(f.Filepath, f.Filename, false));

                            if (!_ClientList.MessageConnection.Send(send))
                            {
                                try
                                {
                                    STEM.Sys.EventLog.WriteEntry("AssemblySync.DeliverDelta", "SendAssemblyList: Forced disconnect, " + _ClientList.MessageConnection.RemoteAddress, STEM.Sys.EventLog.EventLogEntryType.Information);

                                    _ClientList.MessageConnection.Close();
                                    connectionClosed = true;
                                }
                                catch { }

                                return;
                            }

                            send.Descriptions.Clear();
                            deliveredAsms = true;
                        }

                        if (send.Descriptions.Count > 0)
                        {
                            if (!_ClientList.MessageConnection.Send(send))
                            {
                                try
                                {
                                    STEM.Sys.EventLog.WriteEntry("AssemblySync.DeliverDelta", "SendAssemblyList: Forced disconnect, " + _ClientList.MessageConnection.RemoteAddress, STEM.Sys.EventLog.EventLogEntryType.Information);

                                    _ClientList.MessageConnection.Close();
                                    connectionClosed = true;
                                }
                                catch { }

                                return;
                            }

                            send.Descriptions.Clear();
                            deliveredAsms = true;
                        }

                        if (deliveredAsms)
                            _ClientList.MessageConnection.Send(send);
                    }
                }
                catch { }
                finally
                {
                    if (connectionClosed)
                        owner.EndAsync(this);
                }
            }
        }

        protected override void Execute(Sys.Threading.ThreadPool owner)
        {
            bool added = false;
            foreach (string s in STEM.Sys.IO.Directory.STEM_GetFiles(_ExtensionDirectory, "*.dll|*.so|*.a|*.lib", "!.Archive|!TEMP", _Recurse ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly, false))
                if (!AssemblyList.Descriptions.Exists(i => i.Filename == s.Substring(_ExtensionDirectory.Length).Trim(System.IO.Path.DirectorySeparatorChar)))
                    try
                    {
                        Sys.IO.FileDescription d = new Sys.IO.FileDescription(_ExtensionDirectory, s.Substring(_ExtensionDirectory.Length).Trim(System.IO.Path.DirectorySeparatorChar), true);

                        if (d.Content.Length > 0)
                            lock (AssemblyList)
                            {
                                AssemblyList.Descriptions.Add(d);
                                added = true;
                            }
                    }
                    catch { }

            if (added)
            {
                lock (_RegisteredConnections)
                    foreach (DeliverDelta d in _RegisteredConnections.Values)
                    {
                        d.ExecutionInterval = TimeSpan.FromSeconds(1);
                    }
            }
        }
    }
}