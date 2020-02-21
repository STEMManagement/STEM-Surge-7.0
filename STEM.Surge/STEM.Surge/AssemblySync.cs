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

namespace STEM.Surge
{
    public class AssemblySync : STEM.Sys.Threading.IThreadable
    {
        static STEM.Sys.Threading.ThreadPool _Pool = new Sys.Threading.ThreadPool(Environment.ProcessorCount);

        AssemblyList AssemblyList = new Messages.AssemblyList();

        string _ExtensionDirectory = null;
        bool _Recurse = false;

        public AssemblySync(string extensionDirectory, bool recurse)
        {
            _ExtensionDirectory = extensionDirectory;
            _Recurse = recurse;
            _Pool.BeginAsync(this, TimeSpan.FromSeconds(1));
        }

        List<AssemblyList> _AssemblyLists = new List<AssemblyList>();

        public void RegisterList(AssemblyList list)
        {
            lock (_AssemblyLists)
            {
                AssemblyList o = _AssemblyLists.FirstOrDefault(i => i.MessageConnection == list.MessageConnection);
                if (o != null)
                {
                    if (System.Threading.Monitor.TryEnter(o))
                    {
                        try
                        {
                            _AssemblyLists.Remove(o);
                        }
                        finally
                        {
                            System.Threading.Monitor.Exit(o);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

            bool initComplete = true;

            lock (AssemblyList)
                initComplete = AssemblyList.Descriptions.Select(i => STEM.Sys.IO.Path.AdjustPath(i.Filename)).Where(i => !list.Descriptions.Exists(j => STEM.Sys.IO.Path.AdjustPath(j.Filename).Equals(i, StringComparison.InvariantCultureIgnoreCase))).Count() == 0;
            
            if (initComplete)
                list.MessageConnection.Send(new AssemblyInitializationComplete());

            lock (_AssemblyLists)
            {
                _AssemblyLists.Add(list);
                _Pool.BeginAsync(new System.Threading.ParameterizedThreadStart(DeliverDelta), list.MessageConnection, TimeSpan.FromSeconds(3));
            }
        }

        void DeliverDelta(object o)
        {
            MessageConnection connection = o as MessageConnection;

            AssemblyList list = null;

            lock (_AssemblyLists)
            {
                list = _AssemblyLists.FirstOrDefault(i => i.MessageConnection == connection);
                if (list == null)
                {
                    _Pool.EndAsync(new System.Threading.ParameterizedThreadStart(DeliverDelta), o);
                    return;
                }
            }
            
            bool deliveredAsms = false;
            bool connectionClosed = false;

            try
            {
                lock (list)
                {
                    AssemblyList send = new AssemblyList();
                    send.Path = list.Path;

                    foreach (STEM.Sys.IO.FileDescription f in AssemblyList.Descriptions)
                    {
                        if (!list.Descriptions.Exists(i => STEM.Sys.IO.Path.AdjustPath(i.Filename).Equals(STEM.Sys.IO.Path.AdjustPath(f.Filename), StringComparison.InvariantCultureIgnoreCase)))
                        {
                            send.Descriptions.Add(f);
                            list.Descriptions.Add(new STEM.Sys.IO.FileDescription(f.Filepath, f.Filename, false));

                            if (send.Descriptions.Count == 4)
                            {
                                if (!list.MessageConnection.Send(send))
                                {
                                    try
                                    {
                                        STEM.Sys.EventLog.WriteEntry("AssemblySync.DeliverDelta", "SendAssemblyList: Forced disconnect, " + list.MessageConnection.RemoteAddress, STEM.Sys.EventLog.EventLogEntryType.Information);

                                        list.MessageConnection.Close();
                                        connectionClosed = true;
                                    }
                                    catch { }

                                    return;
                                }

                                send.Descriptions.Clear();
                                deliveredAsms = true;
                            }
                        }
                    }

                    if (send.Descriptions.Count > 0)
                    {
                        if (!list.MessageConnection.Send(send))
                        {
                            try
                            {
                                list.MessageConnection.Close();
                                connectionClosed = true;
                            }
                            catch { }

                            return;
                        }

                        send.Descriptions.Clear();
                        deliveredAsms = true;
                    }

                    if (deliveredAsms)
                        list.MessageConnection.Send(send);
                }
            }
            finally
            {
                if (connectionClosed)
                    Dispose(connection);
            }
        }

        public void Dispose(MessageConnection connection)
        {
            try
            {
                lock (_AssemblyLists)
                {
                    AssemblyList o = _AssemblyLists.FirstOrDefault(i => i.MessageConnection == connection);
                    if (o != null)
                    {
                        lock (o)
                            _AssemblyLists.Remove(o);
                    }

                    _Pool.EndAsync(new System.Threading.ParameterizedThreadStart(DeliverDelta), connection);
                }
            }
            catch { }
        }

        protected override void Execute(Sys.Threading.ThreadPool owner)
        {
            foreach (string s in STEM.Sys.IO.Directory.STEM_GetFiles(_ExtensionDirectory, "*.dll", "!.Archive|!TEMP", _Recurse ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly, false))
                if (!AssemblyList.Descriptions.Exists(i => i.Filename == s.Substring(_ExtensionDirectory.Length).Trim(System.IO.Path.DirectorySeparatorChar)))
                    try
                    {
                        Sys.IO.FileDescription d = new Sys.IO.FileDescription(_ExtensionDirectory, s.Substring(_ExtensionDirectory.Length).Trim(System.IO.Path.DirectorySeparatorChar), true);

                        if (d.Content.Length > 0)
                            lock (AssemblyList)
                                AssemblyList.Descriptions.Add(d);
                    }
                    catch { }
        }
    }
}