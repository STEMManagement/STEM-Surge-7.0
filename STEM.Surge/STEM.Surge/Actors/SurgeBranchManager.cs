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
using System.Linq;
using System.Diagnostics;
using STEM.Sys.Messaging;
using STEM.Surge.Messages;
using STEM.Sys.IO.TCP;
using STEM.Sys.Threading;

namespace STEM.Surge
{
    public class SurgeBranchManager : SurgeActor
    {
        static bool _IsWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

        string _PostMortemCache = null;
        public string PostMortemCache
        {
            get
            {
                if (_PostMortemCache != null)
                {
                    if (!System.IO.Directory.Exists(_PostMortemCache))
                        System.IO.Directory.CreateDirectory(_PostMortemCache);
                }

                return _PostMortemCache;
            }

            set
            {
                _PostMortemCache = STEM.Sys.IO.Path.AdjustPath(value);
            }
        }

        List<string> _Stale = new List<string>();

        ConfigurationDS _ConfigurationDS = null;

        STEM.Surge.BranchState _LastState = STEM.Surge.BranchState.Online;

        Dictionary<string, MessageConnection> _BranchHealthThreads = new Dictionary<string, MessageConnection>();

        TcpConnectionListener _TcpConnectionListener = null;

        bool _SES = false;
        bool _UseSSL = false;

        public SurgeBranchManager(int communicationPort, string postMortemCache, bool useSSL, bool ses)
            : base(communicationPort)
        {
            PostMortemCache = postMortemCache;

            _SES = ses;
            _UseSSL = useSSL;

            if (!(this is SurgeSandbox))
                foreach (string f in Directory.GetFiles(InstructionCache, "*.is"))
                    try
                    {
                        _Stale.Add(f);
                    }
                    catch
                    {
                        try
                        {
                            File.Delete(f);
                        }
                        catch { }
                    }

            if (File.Exists(Path.Combine(System.Environment.CurrentDirectory, "SurgeService.cfg")))
                try
                {
                    _ConfigurationDS = new ConfigurationDS();
                    _ConfigurationDS.ReadXml(Path.Combine(System.Environment.CurrentDirectory, "SurgeService.cfg"));
                }
                catch { }

            _TcpConnectionListener = new TcpConnectionListener(0);
            _TcpConnectionListener.onConnect += _TcpConnectionListener_onConnect;

            if (_SES)
            {
                ConnectToDeploymentManager(STEM.Sys.IO.Net.MachineIP(), _UseSSL ? CommunicationPort+1 : CommunicationPort, _UseSSL, true);
            }
        }

        void _TcpConnectionListener_onConnect(TcpConnectionListener caller, System.Net.Sockets.Socket soc)
        {
            lock (ConnectionLock)
            {
                MessageConnection c = new MessageConnection(new System.Net.Sockets.TcpClient { Client = soc }, null, true);

                c.onClosed += _SandboxConnections_onClosed;
                c.onReceived += _SandboxConnections_onReceived;
                c.onOpened += _SandboxConnections_onOpened;
            }
        }

        void _SandboxConnections_onOpened(Connection connection)
        {
            lock (ConnectionLock)
            {
                MessageConnection c = connection as MessageConnection;
                if (c != null)
                {
                    ConnectionType m = new ConnectionType { Type = ConnectionType.Types.SurgeBranchManager };
                    m.PerformHandshake(c);
                }
            }
        }

        void _SandboxConnections_onClosed(Connection connection)
        {
            lock (ConnectionLock)
            {
                base.onClosed(connection);

                lock (_RunningSandboxes)
                {
                    RunningSandbox sandbox = _RunningSandboxes.Where(i => i.Value.MessageConnection == connection).Select(i => i.Value).FirstOrDefault();

                    if (sandbox != null)
                        sandbox.Clear();
                }
            }
        }

        void _SandboxConnections_onReceived(MessageConnection connection, Message message)
        {
            if (message is ConnectionType)
            {
                lock (ConnectionLock)
                {
                    ConnectionType m = message as ConnectionType;

                    m.Respond(new MessageReceived(m.MessageID));

                    switch (m.Type)
                    {
                        case ConnectionType.Types.SurgeSandbox:
                            SandboxConnectionType sm = m as SandboxConnectionType;
                            if (sm == null)
                            {
                                connection.Close();
                                return;
                            }

                            RunningSandbox sandbox = null;

                            lock (_RunningSandboxes)
                            {
                                sandbox = _RunningSandboxes.Where(i => i.Value.SandboxID == sm.SandboxID).Select(i => i.Value).FirstOrDefault();

                                if (sandbox == null)
                                {
                                    try
                                    {
                                        connection.Close();
                                    }
                                    catch { }

                                    return;
                                }
                            }

                            connection.Send(new AssemblyInitializationComplete());

                            lock (sandbox)
                            {
                                sandbox.MessageConnection = connection;

                                foreach (Runner i in _Statics.Where(i => i.AssignInstructionSet.ExecuteStaticInSandboxes))
                                    connection.Send(i.AssignInstructionSet);

                                foreach (AssignInstructionSet i in sandbox.AssignedInstructionSets.ToList())
                                {
                                    i.SandboxID = "";
                                    i.SandboxAppConfigXml = "";
                                    if (!connection.Send(i))
                                    {
                                        sandbox.AssignedInstructionSets.Remove(i);

                                        ExecutionCompleted ec = new ExecutionCompleted();

                                        ec.InstructionSetID = i.InstructionSetID;
                                        ec.DeploymentControllerID = i.DeploymentControllerID;

                                        ec.InitiationSource = i.InitiationSource;
                                        ec.TimeCompleted = DateTime.UtcNow;

                                        ec.Exceptions = new List<Exception>();

                                        SurgeBranchManager.SendMessage(ec, i.MessageConnection, this);
                                    }
                                }
                            }

                            break;

                        default:
                            connection.Close();
                            break;
                    }

                    return;
                }
            }
            else if (message is InstructionMessage)
            {
                RunningSandbox sandbox = null;
                lock (_RunningSandboxes)
                    sandbox = _RunningSandboxes.Where(i => i.Value.MessageConnection == connection).Select(i => i.Value).FirstOrDefault();

                if (sandbox != null)
                    lock (sandbox)
                    {
                        InstructionMessage m = message as InstructionMessage;

                        AssignInstructionSet a = sandbox.AssignedInstructionSets.FirstOrDefault(i => i.InstructionSetID == m.InstructionSetID);

                        if (a != null && m is ExecutionCompleted)
                            sandbox.AssignedInstructionSets.Remove(a);

                        if (a != null)
                        {
                            SurgeBranchManager.SendMessage(m, a.MessageConnection, this);
                        }
                    }
            }
            else if (message is BranchHealthDetails)
            {
                lock (_RunningSandboxes)
                {
                    RunningSandbox s = _RunningSandboxes.Where(i => i.Value.MessageConnection == connection).Select(i => i.Value).FirstOrDefault();

                    if (s == null)
                    {
                        try
                        {
                            connection.Close();
                        }
                        catch { }

                        return;
                    }
                    else
                    {
                        s.LastHealthDetails = DateTime.UtcNow;
                    }
                }
            }
        }

        static STEM.Sys.Threading.ThreadPool _SandboxPool = new ThreadPool(TimeSpan.FromSeconds(3), 4, true);

        static Dictionary<string, RunningSandbox> _RunningSandboxes = new Dictionary<string, RunningSandbox>();
        class RunningSandbox : STEM.Sys.Threading.IThreadable
        {
            SurgeBranchManager _Owner = null;

            public Process Process { get; set; }
            public string SandboxID { get; set; }
            public string ApplicationDirectory { get; set; }

            public bool VersionCacheSync { get; set; }

            public MessageConnection MessageConnection { get; set; }

            public List<AssignInstructionSet> AssignedInstructionSets { get; set; }

            public DateTime LastHealthDetails { get; set; }

            public RunningSandbox(SurgeBranchManager owner)
            {
                _Owner = owner;

                LastHealthDetails = DateTime.UtcNow;
                AssignedInstructionSets = new List<AssignInstructionSet>();
            }

            protected override void Execute(ThreadPool owner)
            {
                if ((DateTime.UtcNow - LastHealthDetails).TotalMinutes > 2)
                {
                    try
                    {
                        MessageConnection.Close();
                    }
                    catch { }

                    try
                    {
                        Process.Kill();
                    }
                    catch { }

                    STEM.Sys.EventLog.WriteEntry("SurgeBranchManager.RunningSandbox.Silent", "Sandbox torn down due to socket silence.", STEM.Sys.EventLog.EventLogEntryType.Information);
                }
            }


            public void Clear()
            {
                lock (_RunningSandboxes)
                {
                    _RunningSandboxes.Remove(SandboxID);

                    lock (this)
                    {
                        foreach (AssignInstructionSet i in AssignedInstructionSets.ToList())
                        {
                            try
                            {
                                AssignedInstructionSets.Remove(i);

                                ExecutionCompleted ec = new ExecutionCompleted();

                                ec.InstructionSetID = i.InstructionSetID;
                                ec.DeploymentControllerID = i.DeploymentControllerID;

                                ec.InitiationSource = i.InitiationSource;
                                ec.TimeCompleted = DateTime.UtcNow;

                                ec.Exceptions = new List<Exception>();

                                SurgeBranchManager.SendMessage(ec, i.MessageConnection, _Owner);
                            }
                            catch { }
                            finally
                            {
                                while (File.Exists(Path.Combine(_Owner.InstructionCache, i.InstructionSetID.ToString() + ".is")))
                                    try
                                    {
                                        File.Delete(Path.Combine(_Owner.InstructionCache, i.InstructionSetID.ToString() + ".is"));
                                    }
                                    catch { System.Threading.Thread.Sleep(10); }
                            }
                        }
                    }

                    int retry = 10;
                    while (Directory.Exists(ApplicationDirectory) && retry > 0)
                        try
                        {
                            STEM.Sys.IO.Directory.STEM_Delete(ApplicationDirectory, false);
                            break;
                        }
                        catch { System.Threading.Thread.Sleep(1000); retry--; }

                    retry = 10;
                    ApplicationDirectory = STEM.Sys.IO.Path.GetDirectoryName(ApplicationDirectory);
                    if (Directory.GetDirectories(ApplicationDirectory).Count() == 0 && Directory.GetFiles(ApplicationDirectory).Count() == 0)
                        while (Directory.Exists(ApplicationDirectory) && retry > 0)
                            try
                            {
                                STEM.Sys.IO.Directory.STEM_Delete(ApplicationDirectory, false);
                                break;
                            }
                            catch { System.Threading.Thread.Sleep(1000); retry--; }

                    _SandboxPool.EndAsync(this);
                }
            }
        }

        RunningSandbox LaunchSandbox(string sandboxID, string appConfig, string altAssmStore)
        {
            try
            {
                string appPath = Path.Combine(System.Environment.CurrentDirectory, Path.Combine("Sandboxes", sandboxID));

                if (Directory.Exists(appPath))
                    try
                    {
                        STEM.Sys.IO.Directory.STEM_Delete(appPath, false);
                    }
                    catch { }

                if (!Directory.Exists(appPath))
                    Directory.CreateDirectory(appPath);

                if (sandboxID.StartsWith("1"))
                    foreach (string dll in STEM.Sys.IO.Directory.STEM_GetFiles(STEM.Sys.Serialization.VersionManager.VersionCache, "*.dll", "!.Archive|!TEMP", SearchOption.AllDirectories, false))
                    {
                        string file = dll.Replace(STEM.Sys.IO.Path.FirstTokenOfPath(dll), STEM.Sys.IO.Path.FirstTokenOfPath(STEM.Sys.Serialization.VersionManager.VersionCache)).Substring(STEM.Sys.Serialization.VersionManager.VersionCache.Length).Trim(Path.DirectorySeparatorChar);
                        file = Path.Combine(Path.Combine(appPath, STEM.Sys.IO.Path.GetFileName(STEM.Sys.Serialization.VersionManager.VersionCache)), file);
                        string dir = STEM.Sys.IO.Path.GetDirectoryName(file);

                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                        File.Copy(dll, file);
                    }

                ConfigurationDS configurationDS = new ConfigurationDS();
                configurationDS.ReadXml(Path.Combine(System.Environment.CurrentDirectory, "SurgeService.cfg"));

                configurationDS.Settings[0].SurgeCommunicationPort = _TcpConnectionListener.Port;

                if (altAssmStore != null)
                {
                    configurationDS.Settings[0].AlternateAssemblyStore = altAssmStore;
                }

                configurationDS.WriteXml(Path.Combine(appPath, "SurgeService.cfg"));

                if (STEM.Sys.Control.SystemRuntime == STEM.Sys.Control.Runtime.Core)
                {
                    lock (_RunningSandboxes)
                    {
                        try
                        {
                            File.Copy("STEM.SurgeService.dll", Path.Combine(appPath, "STEM.SurgeService.dll"));
                            try
                            {
                                File.Copy(Path.Combine(System.Environment.CurrentDirectory, "STEM.SurgeService.deps.json"), Path.Combine(appPath, "STEM.SurgeService.deps.json"));
                            }
                            catch { }
                            try
                            {
                                File.Copy(Path.Combine(System.Environment.CurrentDirectory, "STEM.SurgeService.runtimeconfig.json"), Path.Combine(appPath, "STEM.SurgeService.runtimeconfig.json"));
                            }
                            catch { }
                        }
                        catch (Exception ex)
                        {
                            STEM.Sys.EventLog.WriteEntry("SurgeBranchManager.LaunchSandbox", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                        }

                        foreach (string dll in Directory.GetFiles(System.Environment.CurrentDirectory, "*.dll"))
                            if (!dll.EndsWith("STEM.SurgeService.dll") && !dll.EndsWith("STEM.Auth.dll"))
                                File.Copy(dll, Path.Combine(appPath, STEM.Sys.IO.Path.GetFileName(dll)));
                        
                        appPath = Path.Combine(appPath, "STEM.SurgeService.dll");

                        try
                        {
                            ProcessStartInfo si = new ProcessStartInfo();
                            si.CreateNoWindow = true;
                            si.UseShellExecute = false;

                            si.FileName = "dotnet";
                            si.Arguments = "\"" + appPath + "\" -sandbox";

                            Process p = new Process();
                            p.StartInfo = si;
                            p.EnableRaisingEvents = true;
                            p.Exited += ClearSandbox;

                            p.Start();

                            RunningSandbox s = new RunningSandbox(this);
                            s.Process = p;
                            s.ApplicationDirectory = STEM.Sys.IO.Path.GetDirectoryName(appPath);
                            s.SandboxID = sandboxID;
                            s.VersionCacheSync = sandboxID.StartsWith("1", StringComparison.InvariantCultureIgnoreCase);
                            s.LastHealthDetails = DateTime.UtcNow;

                            _SandboxPool.BeginAsync(s);

                            _RunningSandboxes[s.SandboxID] = s;

                            return s;
                        }
                        catch
                        {
                            int retry = 10;
                            appPath = STEM.Sys.IO.Path.GetDirectoryName(appPath);
                            if (Directory.GetDirectories(appPath).Count() == 0 && Directory.GetFiles(appPath).Count() == 0)
                                while (Directory.Exists(appPath) && retry > 0)
                                    try
                                    {
                                        STEM.Sys.IO.Directory.STEM_Delete(appPath, false);
                                        break;
                                    }
                                    catch { System.Threading.Thread.Sleep(1000); retry--; }

                            retry = 10;
                            appPath = STEM.Sys.IO.Path.GetDirectoryName(appPath);
                            if (Directory.GetDirectories(appPath).Count() == 0 && Directory.GetFiles(appPath).Count() == 0)
                                while (Directory.Exists(appPath) && retry > 0)
                                    try
                                    {
                                        STEM.Sys.IO.Directory.STEM_Delete(appPath, false);
                                        break;
                                    }
                                    catch { System.Threading.Thread.Sleep(1000); retry--; }
                        }
                    }
                }
                else
                {
                    lock (_RunningSandboxes)
                    {
                        try
                        {
                            File.Copy(Process.GetCurrentProcess().MainModule.FileName, Path.Combine(appPath, sandboxID + ".exe"));
                            try
                            {
                                string appConfigXml = appConfig;

                                if (appConfigXml.Trim().Length < 1)
                                    appConfigXml = File.ReadAllText(Path.Combine(System.Environment.CurrentDirectory, Process.GetCurrentProcess().MainModule.FileName + ".config"));

                                File.WriteAllText(Path.Combine(appPath, sandboxID + ".exe.config"), appConfigXml);
                            }
                            catch { }
                        }
                        catch (Exception ex)
                        {
                            STEM.Sys.EventLog.WriteEntry("SurgeBranchManager.LaunchSandbox", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                        }

                        foreach (string dll in Directory.GetFiles(System.Environment.CurrentDirectory, "*.dll"))
                            File.Copy(dll, Path.Combine(appPath, Path.GetFileName(dll)));

                        appPath = Path.Combine(appPath, sandboxID + ".exe");

                        try
                        {
                            ProcessStartInfo si = new ProcessStartInfo();
                            si.CreateNoWindow = true;
                            si.UseShellExecute = false;

                            si.FileName = appPath;
                            si.Arguments = " -sandbox";

                            Process p = new Process();
                            p.StartInfo = si;
                            p.EnableRaisingEvents = true;
                            p.Exited += ClearSandbox;

                            p.Start();

                            RunningSandbox s = new RunningSandbox(this);
                            s.Process = p;
                            s.ApplicationDirectory = STEM.Sys.IO.Path.GetDirectoryName(appPath);
                            s.SandboxID = sandboxID;
                            s.VersionCacheSync = sandboxID.StartsWith("1", StringComparison.InvariantCultureIgnoreCase);
                            s.LastHealthDetails = DateTime.UtcNow;

                            _SandboxPool.BeginAsync(s);

                            _RunningSandboxes[s.SandboxID] = s;

                            return s;
                        }
                        catch
                        {
                            int retry = 10;
                            appPath = Path.GetDirectoryName(appPath);
                            if (Directory.GetDirectories(appPath).Count() == 0 && Directory.GetFiles(appPath).Count() == 0)
                                while (Directory.Exists(appPath) && retry > 0)
                                    try
                                    {
                                        STEM.Sys.IO.Directory.STEM_Delete(appPath, false);
                                        break;
                                    }
                                    catch { System.Threading.Thread.Sleep(1000); retry--; }

                            retry = 10;
                            appPath = Path.GetDirectoryName(appPath);
                            if (Directory.GetDirectories(appPath).Count() == 0 && Directory.GetFiles(appPath).Count() == 0)
                                while (Directory.Exists(appPath) && retry > 0)
                                    try
                                    {
                                        STEM.Sys.IO.Directory.STEM_Delete(appPath, false);
                                        break;
                                    }
                                    catch { System.Threading.Thread.Sleep(1000); retry--; }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("SurgeBranchManager.LaunchSandbox", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return null;
        }


        void ClearSandbox(object sender, EventArgs e)
        {
            Process p = (Process)sender;

            lock (_RunningSandboxes)
            {
                try
                {
                    RunningSandbox n = _RunningSandboxes.Where(i => i.Value.Process.Id == p.Id).Select(i => i.Value).FirstOrDefault();

                    if (n != null)
                        n.Clear();
                }
                catch (Exception ex)
                {
                    STEM.Sys.EventLog.WriteEntry("SurgeBranchManager.ClearSandbox", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                }
            }
        }

        protected override void onOpened(Connection connection)
        {
            lock (ConnectionLock)
            {
                MessageConnection c = connection as MessageConnection;
                if (c != null)
                    ManageConnection(c);
                
                ConnectionType m = new ConnectionType { Type = ConnectionType.Types.SurgeBranchManager };
                m.PerformHandshake(c);

                STEM.Sys.Global.ThreadPool.BeginAsync(new SendAssemblyList(c, this));
            }
        }

        class SendAssemblyList : STEM.Sys.Threading.IThreadable
        {
            static object _Lock = new object();
            static MessageConnection _AssemblyListInitializer = null;

            MessageConnection _Connection;
            SurgeBranchManager _Owner;

            public SendAssemblyList(MessageConnection connection, SurgeBranchManager owner)
            {
                _Connection = connection;
                _Owner = owner;
            }

            protected override void Execute(ThreadPool owner)
            {
                bool endAsync = false;

                try
                {
                    lock (_Owner._BranchHealthThreads)
                    {
                        if (_Owner._BranchHealthThreads.ContainsKey(_Connection.ToString()))
                        {
                            endAsync = true;
                            return;
                        }
                    }

                    lock (_Lock)
                    {
                        if (_AssemblyListInitializer == null)
                            _AssemblyListInitializer = _Connection;

                        if (!_Owner._AssemblyInitializationComplete)
                            if (_AssemblyListInitializer != _Connection)
                            {
                                ExecutionInterval = TimeSpan.FromSeconds(1);
                                return;
                            }
                    }

                    while (_Owner._AsmPool.LoadLevel > 0)
                        System.Threading.Thread.Sleep(10);

                    AssemblyList a = new AssemblyList(STEM.Sys.Serialization.VersionManager.VersionCache.Replace(Environment.CurrentDirectory, "."), true);

                    if (!_Connection.Send(a))
                    {
                        endAsync = true;

                        STEM.Sys.EventLog.WriteEntry("BranchManager.SendAssemblyList", "SendAssemblyList: Forced disconnect, " + _Connection.RemoteAddress, STEM.Sys.EventLog.EventLogEntryType.Information);

                        return;
                    }
                }
                finally
                {
                    if (endAsync)
                        lock (_Lock)
                        {
                            if (_AssemblyListInitializer == _Connection)
                                _AssemblyListInitializer = null;

                            owner.EndAsync(this);
                        }
                }
            }
        }

        protected override void onClosed(Connection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            base.onClosed(connection);

            MessageConnection c = connection as MessageConnection;
            
            try
            {
                lock (_BranchHealthThreads)
                    if (_BranchHealthThreads.ContainsKey(connection.ToString()))
                    {
                        _BranchHealthThreads.Remove(connection.ToString());
                    }
            }
            catch { }

            lock (_PollerPool.PollerThreads)
            {
                foreach (Poller p in _PollerPool.PollerThreads.ToList())
                {
                    try
                    {
                        p.PollerMessage.RemoveDeploymentManager(c);

                        if (p.PollerMessage.DeploymentManagers.Count == 0)
                        {
                            _PollerPool.PollerThreads.Remove(p);
                            _PollerPool.EndAsync(p);
                        }
                    }
                    catch { }
                }
            }
        }

        protected List<AssignInstructionSet> _Assigned = new List<AssignInstructionSet>();
        List<Runner> _Statics = new List<Runner>();

        public string InstructionCache
        {
            get
            {
                string d = System.IO.Path.Combine(System.Environment.CurrentDirectory, "InstructionCache");
                                                
                if (_SES && System.Environment.CurrentDirectory.ToUpper(System.Globalization.CultureInfo.CurrentCulture).Contains(Path.DirectorySeparatorChar + "SANDBOXES" + Path.DirectorySeparatorChar))
                {
                    d = System.Environment.CurrentDirectory.Replace(STEM.Sys.IO.Path.GetFileName(System.Environment.CurrentDirectory), "");
                    d = d.TrimEnd(Path.DirectorySeparatorChar);
                    d = d.Replace(STEM.Sys.IO.Path.GetFileName(d), "");
                    d = d.TrimEnd(Path.DirectorySeparatorChar);
                    d = Path.Combine(d, "InstructionCache");
                }

                if (!System.IO.Directory.Exists(d))
                    System.IO.Directory.CreateDirectory(d);

                return d;
            }
        }

        Dictionary<string, Queue<STEM.Sys.Messaging.Message>> _MessageQueue = new Dictionary<string, Queue<STEM.Sys.Messaging.Message>>();
        Dictionary<string, object> _MessageQueueLock = new Dictionary<string, object>();

        public bool Send(STEM.Sys.Messaging.Message message, MessageConnection destination, bool acceptResponsesUntilDisposed = false, bool queue = false)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            Queue<STEM.Sys.Messaging.Message> q1 = null;

            message.AcceptResponsesUntilDisposed = acceptResponsesUntilDisposed;

            if (destination.Send(message, acceptResponsesUntilDisposed))
            {
                if (_MessageQueue.ContainsKey(destination.RemoteAddress + destination.RemotePort))
                    lock (_MessageQueueLock[destination.RemoteAddress + destination.RemotePort])
                    {
                        q1 = _MessageQueue[destination.RemoteAddress + destination.RemotePort];

                        if (q1.Count > 0)
                        {
                            _MessageQueue[destination.RemoteAddress + destination.RemotePort] = new Queue<STEM.Sys.Messaging.Message>();

                            bool fail = false;
                            foreach (STEM.Sys.Messaging.Message m in q1)
                            {
                                if (destination.Send(m, m.AcceptResponsesUntilDisposed))
                                {
                                    try
                                    {
                                        m.Serialize();
                                        fail = true;
                                        break;
                                    }
                                    catch { }
                                }
                            }

                            if (fail)
                                _MessageQueue[destination.RemoteAddress + destination.RemotePort] = q1;
                        }
                    }

                return true;
            }

            if (!queue)
                return false;

            try
            {
                message.Serialize();

                if (!_MessageQueue.ContainsKey(destination.RemoteAddress + destination.RemotePort))
                {
                    lock (_MessageQueue)
                    {
                        if (!_MessageQueue.ContainsKey(destination.RemoteAddress + destination.RemotePort))
                        {
                            _MessageQueue[destination.RemoteAddress + destination.RemotePort] = new Queue<Message>();
                            _MessageQueueLock[destination.RemoteAddress + destination.RemotePort] = new object();
                        }
                    }
                }

                lock (_MessageQueueLock[destination.RemoteAddress + destination.RemotePort])
                {
                    _MessageQueue[destination.RemoteAddress + destination.RemotePort].Enqueue(message);
                }
            }
            catch { return false; }

            return true;
        }

        class Runner : STEM.Sys.Threading.IThreadable
        {
            static STEM.Sys.Threading.ThreadPool _RunnerPool = new Sys.Threading.ThreadPool(Int32.MaxValue, true);
            static STEM.Sys.Threading.ThreadPool _StaticPool = new Sys.Threading.ThreadPool(TimeSpan.FromSeconds(3), Environment.ProcessorCount / 2, true);

            public AssignInstructionSet AssignInstructionSet { get; set; }
            MessageConnection _MessageConnection { get; set; }
            SurgeBranchManager _BranchManager { get; set; }

            public Runner(SurgeBranchManager branchManager, MessageConnection connection, AssignInstructionSet assignInstructionSet)
            {
                _BranchManager = branchManager;
                _MessageConnection = connection;
                AssignInstructionSet = assignInstructionSet;

                lock (AssignInstructionSet)
                    AssignInstructionSet.ExecutionCompleted = DateTime.MinValue;

                try
                {
                    lock (_BranchManager._Assigned)
                    {
                        if (AssignInstructionSet.IsStatic)
                        {
                            Runner existing = _BranchManager._Statics.FirstOrDefault(i => i.AssignInstructionSet.InitiationSource.Equals(AssignInstructionSet.InitiationSource, StringComparison.InvariantCultureIgnoreCase));

                            if (existing != null && existing.AssignInstructionSet.LastWriteTime >= AssignInstructionSet.LastWriteTime)
                                return;

                            if (existing != null)
                            {
                                _BranchManager._Statics.Remove(existing);

                                try
                                {
                                    existing.Dispose();
                                }
                                catch { }
                            }

                            if (!_BranchManager._Statics.Contains(this))
                                _BranchManager._Statics.Add(this);

                            if (AssignInstructionSet.ContinuousExecution)
                            {
                                _StaticPool.BeginAsync(this);
                            }
                            else
                            {
                                _StaticPool.RunOnce(this);
                            }
                        }
                        else
                        {
                            if (!_BranchManager._Assigned.Exists(i => i.InstructionSetID == AssignInstructionSet.InstructionSetID))
                                _BranchManager._Assigned.Add(AssignInstructionSet);
                            else
                                return;

                            _RunnerPool.RunOnce(this);
                        }
                    }
                }
                catch (Exception ex)
                {
                    lock (AssignInstructionSet)
                    {
                        AssignInstructionSet.ExecutionCompleted = DateTime.UtcNow;

                        try
                        {
                            lock (_BranchManager._Assigned)
                                if (_BranchManager._Assigned.Exists(i => i.InstructionSetID == assignInstructionSet.InstructionSetID))
                                    _BranchManager._Assigned.Remove(assignInstructionSet);
                        }
                        catch { }

                        try
                        {
                            ExecutionCompleted ec = new ExecutionCompleted { DeploymentControllerID = AssignInstructionSet.DeploymentControllerID, InstructionSetID = AssignInstructionSet.InstructionSetID, InitiationSource = AssignInstructionSet.InitiationSource, Exceptions = new List<Exception>() };

                            ec.Exceptions.Add(ex);

                            if (_MessageConnection != null)
                                _BranchManager.Send(ec, _MessageConnection, false, true);

                            STEM.Sys.EventLog.WriteEntry("Runner.Runner", AssignInstructionSet.InitiationSource + ": " + ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                        }
                        catch (Exception ex2)
                        {
                            STEM.Sys.EventLog.WriteEntry("Runner.Runner", ex2.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                        }
                    }
                }
            }

            protected override void Execute(Sys.Threading.ThreadPool owner)
            {
                try
                {
                    while (!_BranchManager._AssemblyInitializationComplete)
                        System.Threading.Thread.Sleep(100);

                    MessageConnection a = _BranchManager.MessageConnection(_MessageConnection.RemoteAddress);

                    if (a != null)
                        _MessageConnection = a;

                    _InstructionSet runnable = null;

                    try
                    {
                        runnable = _InstructionSet.Deserialize(AssignInstructionSet.InstructionSet.Serialize()) as _InstructionSet;
                    }
                    catch (Exception ex)
                    {
                        if (AssignInstructionSet.IsStatic)
                        {
                            if (AssignInstructionSet.ContinuousExecution)
                                owner.EndAsync(this);

                            lock (_BranchManager._Assigned)
                                if (_BranchManager._Statics.Contains(this))
                                    _BranchManager._Statics.Remove(this);

                            STEM.Sys.EventLog.WriteEntry("Runner.Execute", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);

                            return;
                        }

                        throw ex;
                    }

                    runnable.DeploymentManagerIP = AssignInstructionSet.DeploymentManagerIP;
                    runnable.Assigned = AssignInstructionSet.TimeSent;
                    runnable.Received = AssignInstructionSet.TimeReceived;

                    if (!AssignInstructionSet.IsStatic)
                    {
                        File.WriteAllText(Path.Combine(_BranchManager.InstructionCache, runnable.ID.ToString() + ".is"), runnable.Serialize());
                        runnable.Run(_BranchManager, _MessageConnection, _BranchManager.InstructionCache, _BranchManager._LocalKeyManager, AssignInstructionSet.BranchIP);
                    }
                    else
                    {
                        if (AssignInstructionSet.ContinuousExecution)
                            ExecutionInterval = TimeSpan.FromSeconds(runnable.ContinuousExecutionInterval);

                        runnable.Run(_BranchManager, _MessageConnection, null, _BranchManager._LocalKeyManager, AssignInstructionSet.BranchIP);
                    }
                }
                catch (Exception ex)
                {
                    if (!AssignInstructionSet.IsStatic)
                        lock (AssignInstructionSet)
                            try
                            {
                                AssignInstructionSet.ExecutionCompleted = DateTime.UtcNow;

                                ExecutionCompleted ec = new ExecutionCompleted { DeploymentControllerID = AssignInstructionSet.DeploymentControllerID, InstructionSetID = AssignInstructionSet.InstructionSetID, InitiationSource = AssignInstructionSet.InitiationSource, Exceptions = new List<Exception>(), TimeCompleted = DateTime.UtcNow };

                                ec.Exceptions.Add(ex);

                                MessageConnection a = _BranchManager.MessageConnection(_MessageConnection.RemoteAddress);

                                if (a != null)
                                    _MessageConnection = a;

                                if (_MessageConnection != null)
                                    SurgeBranchManager.SendMessage(ec, _MessageConnection, _BranchManager);

                                STEM.Sys.EventLog.WriteEntry("Runner.Execute", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                            }
                            catch (Exception ex2)
                            {
                                STEM.Sys.EventLog.WriteEntry("Runner.Execute", ex2.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                            }
                }
                finally
                {
                    if (!AssignInstructionSet.IsStatic)
                        lock (_BranchManager._Assigned)
                        {
                            if (_BranchManager._Assigned.Contains(AssignInstructionSet))
                                _BranchManager._Assigned.Remove(AssignInstructionSet);
                        }

                    while (File.Exists(Path.Combine(_BranchManager.InstructionCache, AssignInstructionSet.InstructionSetID.ToString() + ".is")))
                        try
                        {
                            File.Delete(Path.Combine(_BranchManager.InstructionCache, AssignInstructionSet.InstructionSetID.ToString() + ".is"));
                        }
                        catch { System.Threading.Thread.Sleep(10); }
                }
            }
        }

        class Poller : STEM.Sys.Threading.IThreadable
        {
            public SurgeBranchManager Owner { get; set; }
            public PollDirectory PollerMessage { get; set; }

            protected override void Execute(Sys.Threading.ThreadPool owner)
            {
                try
                {
                    if (Directory.Exists(PollerMessage.Directory))
                        PollerMessage.Execute(Owner);
                }
                catch { }

                ExecutionInterval = PollerMessage.PollInterval.Value;
            }
        }

        class PollerPool : STEM.Sys.Threading.ThreadPool
        {
            public List<Poller> PollerThreads { get; set; }

            public PollerPool() : base(TimeSpan.FromSeconds(10), Environment.ProcessorCount)
            {
                PollerThreads = new List<Poller>();
            }

            protected override List<Sys.Threading.IThreadable> Sort(List<Sys.Threading.IThreadable> collection)
            {
                return collection.Where(i => !i.QueuedForExecution && (DateTime.UtcNow - i.LastExecutionEnd) > i.ExecutionInterval && Directory.Exists(((Poller)i).PollerMessage.Directory))
                                           .OrderBy(i => ((Poller)i).PollerMessage.PollInterval.Value.TotalMilliseconds - (DateTime.UtcNow - i.LastExecutionEnd).TotalMilliseconds).Take(MaximumConcurrent).ToList();

            }
        }

        PollerPool _PollerPool = new PollerPool();
        bool _AssemblyInitializationComplete = false;


        private void RequestResume_onResponse(Message delivered, Message response)
        {
            RequestResume m = delivered as RequestResume;
            RequestResume r = response as RequestResume;

            try
            {
                if (r != null && r.RequestApproved)
                {
                    AssignInstructionSet a = new AssignInstructionSet(m.InstructionSet, r.MessageConnection.RemoteAddress, m.InstructionSet.DeploymentControllerID, STEM.Sys.IO.Net.MachineIP(), "", "", "");
                    new Runner(this, r.MessageConnection, a);
                }
                else
                {
                    ExecutionCompleted ec = new ExecutionCompleted { DeploymentControllerID = m.InstructionSet.DeploymentControllerID, InstructionSetID = m.InstructionSet.ID, InitiationSource = m.InstructionSet.InitiationSource, Exceptions = new List<Exception>() };
                    r.MessageConnection.Send(ec);

                    while (File.Exists(Path.Combine(InstructionCache, m.InstructionSet.ID.ToString() + ".is")))
                        try
                        {
                            File.Delete(Path.Combine(InstructionCache, m.InstructionSet.ID.ToString() + ".is"));
                        }
                        catch { System.Threading.Thread.Sleep(10); }
                }
            }
            catch { }

            try
            {
                _Stale.Remove(Path.Combine(InstructionCache, m.InstructionSet.ID.ToString() + ".is"));
            }
            catch { }
        }

        protected override void onReceived(MessageConnection connection, Message message)
        {
            try
            {
                if (message is AssemblyInitializationComplete)
                {
                    if (!_AssemblyInitializationComplete)
                        while (_AsmPool.LoadLevel > 0)
                            System.Threading.Thread.Sleep(100);

                    _AssemblyInitializationComplete = true;

                    lock (_BranchHealthThreads)
                    {
                        if (_BranchHealthThreads.ContainsKey(connection.ToString()))
                            return;
                    }

                    SendBranchHealthDetails(connection);

                    lock (_ManagerMessageReturns)
                    {
                        if (_ManagerMessageReturns.ContainsKey(connection.RemoteAddress))
                        {
                            foreach (Message m in _ManagerMessageReturns[connection.RemoteAddress].ToList())
                                if (connection.Send(m))
                                    _ManagerMessageReturns[connection.RemoteAddress].Remove(m);

                            if (_ManagerMessageReturns[connection.RemoteAddress].Count() == 0)
                                _ManagerMessageReturns.Remove(connection.RemoteAddress);
                        }
                    }

                    lock (_Assigned)
                    {
                        foreach (AssignInstructionSet a in _Assigned.Where(i => i.MessageConnection.RemoteAddress == connection.RemoteAddress))
                        {
                            try
                            {
                                if (!a.IsStatic)
                                    lock (a)
                                    {
                                        if (a.ExecutionCompleted == DateTime.MinValue)
                                            connection.Send(new RequestResume(a.InstructionSet));
                                    }
                            }
                            catch { }
                        }

                        foreach (string s in _Stale.ToList())
                        {
                            try
                            {
                                _InstructionSet iSet = _InstructionSet.Deserialize(System.IO.File.ReadAllText(s)) as _InstructionSet;

                                if (iSet != null)
                                {
                                    if (iSet.DeploymentManagerIP == connection.RemoteAddress)
                                    {
                                        RequestResume m = new RequestResume(iSet);

                                        m.onResponse += RequestResume_onResponse;

                                        if (!connection.Send(m))
                                        {
                                            ExecutionCompleted ec = new ExecutionCompleted { DeploymentControllerID = iSet.DeploymentControllerID, InstructionSetID = iSet.ID, InitiationSource = iSet.InitiationSource, Exceptions = new List<Exception>() };
                                            connection.Send(ec);

                                            while (File.Exists(s))
                                                try
                                                {
                                                    File.Delete(s);
                                                }
                                                catch { System.Threading.Thread.Sleep(10); }

                                            _Stale.Remove(s);
                                        }
                                    }
                                }
                                else
                                {
                                    _Stale.Remove(s);
                                }
                            }
                            catch
                            {
                                while (File.Exists(s))
                                    try
                                    {
                                        File.Delete(s);
                                    }
                                    catch { System.Threading.Thread.Sleep(10); }

                                try
                                {
                                    _Stale.Remove(s);
                                }
                                catch { }
                            }
                        }
                    }

                    lock (_BranchHealthThreads)
                    {
                        if (_BranchHealthThreads.ContainsKey(connection.ToString()))
                            return;

                        System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ReportState));
                        t.IsBackground = true;
                        _BranchHealthThreads[connection.ToString()] = connection;
                        t.Start(connection.ToString());
                    }
                }
                else if (message is BringOnline)
                {
                    _LastState = STEM.Surge.BranchState.Online;
                    SendBranchHealthDetails(connection);
                }
                else if (message is TakeOffline)
                {
                    _LastState = STEM.Surge.BranchState.Offline;
                    SendBranchHealthDetails(connection);
                }
                else if (message is SetServiceConfiguration)
                {
                    SetServiceConfiguration m = message as SetServiceConfiguration;
                                       
                    try
                    {
                        if (File.Exists(Path.Combine(Environment.CurrentDirectory, "SurgeService.cfg")))
                        {
                            ConfigurationDS newConfig = new ConfigurationDS();
                            newConfig.ReadXml(Path.Combine(Environment.CurrentDirectory, "SurgeService.cfg"));
                            newConfig.Settings[0].SurgeCommunicationPort = m.SurgeCommunicationPort;
                            newConfig.Settings[0].SurgeDeploymentManagerAddress = m.SurgeDeploymentManagerAddress;
                            newConfig.Settings[0].ProcessorOverload = m.ProcessorOverload;
                            newConfig.Settings[0].PostMortemDirectory = m.PostMortemDirectory;
                            newConfig.Settings[0].RemoteConfigurationDirectory = m.RemoteConfigurationDirectory;
                            newConfig.Settings[0].UseSSL = m.UseSSL;
                            newConfig.WriteXml(Path.Combine(Environment.CurrentDirectory, "SurgeService.cfg"));

                            _ConfigurationDS = newConfig;
                        }
                    }
                    catch { }

                    lock (_RunningSandboxes)
                    {
                        foreach (RunningSandbox s in _RunningSandboxes.Values.ToList())
                            try
                            {
                                if (s.MessageConnection != null)
                                    if (!s.MessageConnection.Send(message))
                                    {
                                        try
                                        {
                                            s.MessageConnection.Close();
                                        }
                                        catch { }
                                    }
                            }
                            catch { }
                    }
                }
                else if (message is GetServiceConfiguration)
                {
                    GetServiceConfiguration m = message as GetServiceConfiguration;

                    ConfigurationDS newConfig = new ConfigurationDS();
                    newConfig.ReadXml(Path.Combine(Environment.CurrentDirectory, "SurgeService.cfg"));
                    m.SurgeCommunicationPort = newConfig.Settings[0].SurgeCommunicationPort;
                    m.SurgeDeploymentManagerAddress = newConfig.Settings[0].SurgeDeploymentManagerAddress;
                    m.ProcessorOverload = newConfig.Settings[0].ProcessorOverload;
                    m.PostMortemDirectory = newConfig.Settings[0].PostMortemDirectory;
                    m.RemoteConfigurationDirectory = newConfig.Settings[0].RemoteConfigurationDirectory;
                    m.UseSSL = newConfig.Settings[0].UseSSL;

                    message.Respond(m);
                }
                else if (message is InstructionSetRequested)
                {
                    InstructionSetRequested m = message as InstructionSetRequested;

                    if (File.Exists(Path.Combine(InstructionCache, m.InstructionSetID.ToString() + ".is")))
                    {
                        _InstructionSet r = _InstructionSet.Deserialize(File.ReadAllText(Path.Combine(InstructionCache, m.InstructionSetID.ToString() + ".is"))) as _InstructionSet;
                        r.BranchIP = m.MessageConnection.LocalAddress;
                        m.Respond(new InstructionSetRequested(r, r.BranchIP));
                    }

                    if (File.Exists(Path.Combine(ErrorDirectory, m.InstructionSetID.ToString() + ".is")))
                    {
                        _InstructionSet r = _InstructionSet.Deserialize(File.ReadAllText(Path.Combine(ErrorDirectory, m.InstructionSetID.ToString() + ".is"))) as _InstructionSet;
                        r.BranchIP = m.MessageConnection.LocalAddress;
                        m.Respond(new InstructionSetRequested(r, r.BranchIP));
                    }
                }
                else if (message is CancelExecution)
                {
                    CancelExecution m = message as CancelExecution;
                    lock (_Assigned)
                    {
                        AssignInstructionSet o = _Assigned.FirstOrDefault(i => i.InstructionSetID == m.InstructionSetID);
                        if (o != null)
                        {
                            _Assigned.Remove(o);
                            o.InstructionSet.Stop();
                        }
                        else
                        {
                            lock (_RunningSandboxes)
                            {
                                RunningSandbox sandbox = _RunningSandboxes.Where(i => i.Value.AssignedInstructionSets.Exists(j => j.InstructionSetID == m.InstructionSetID)).Select(i => i.Value).FirstOrDefault();

                                if (sandbox != null)
                                {
                                    sandbox.MessageConnection.Send(m);
                                }
                            }
                        }
                    }
                }
                else if (message is AssignInstructionSet)
                {
                    AssignInstructionSet m = message as AssignInstructionSet;

                    try
                    {
                        if (m.IsStatic && m.ExecuteStaticInSandboxes)
                        {
                            lock (_RunningSandboxes)
                            {
                                foreach (RunningSandbox s in _RunningSandboxes.Values)
                                {
                                    try
                                    {
                                        s.MessageConnection.Send(m);
                                    }
                                    catch { }
                                }
                            }
                        }

                        if (String.IsNullOrEmpty(m.SandboxID))
                        {
                            new Runner(this, connection, m);
                        }
                        else
                        {
                            string sandboxID = m.SandboxID;
                            m.SandboxID = "";

                            RunningSandbox sandbox = null;

                            lock (_RunningSandboxes)
                            {
                                if (!_RunningSandboxes.ContainsKey(sandboxID))
                                {
                                    if (_AssemblyInitializationComplete)
                                    {
                                        string altAssmStore = null;
                                        if (!String.IsNullOrEmpty(m.AlternateAssemblyStore))
                                            altAssmStore = m.AlternateAssemblyStore;

                                        sandbox = LaunchSandbox(sandboxID, m.SandboxAppConfigXml, altAssmStore);
                                    }
                                }
                                else
                                {
                                    sandbox = _RunningSandboxes[sandboxID];
                                }
                            }

                            if (sandbox == null || sandbox.MessageConnection == null)
                            {
                                ExecutionCompleted ec = new ExecutionCompleted();

                                ec.InstructionSetID = m.InstructionSetID;
                                ec.DeploymentControllerID = m.DeploymentControllerID;

                                ec.InitiationSource = m.InitiationSource;
                                ec.TimeCompleted = DateTime.UtcNow;

                                ec.Exceptions = new List<Exception>();

                                SurgeBranchManager.SendMessage(ec, connection, this);

                                return;
                            }

                            lock (sandbox)
                            {
                                sandbox.AssignedInstructionSets.Add(m);

                                if (sandbox.MessageConnection != null)
                                    if (!sandbox.MessageConnection.Send(m))
                                    {
                                        sandbox.AssignedInstructionSets.Remove(m);

                                        ExecutionCompleted ec = new ExecutionCompleted();

                                        ec.InstructionSetID = m.InstructionSetID;
                                        ec.DeploymentControllerID = m.DeploymentControllerID;

                                        ec.InitiationSource = m.InitiationSource;
                                        ec.TimeCompleted = DateTime.UtcNow;

                                        ec.Exceptions = new List<Exception>();

                                        SurgeBranchManager.SendMessage(ec, connection, this);
                                    }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ExecutionCompleted ec = new ExecutionCompleted { DeploymentControllerID = m.DeploymentControllerID, InstructionSetID = m.InstructionSetID, InitiationSource = m.InitiationSource, Exceptions = new List<Exception>() };
                        ec.Exceptions.Add(ex);

                        SurgeBranchManager.SendMessage(ec, connection, this);
                    }
                }
                else if (message is PollDirectory)
                {
                    PollDirectory m = message as PollDirectory;

                    lock (_PollerPool.PollerThreads)
                    {
                        Poller p = _PollerPool.PollerThreads.FirstOrDefault(i => i.PollerMessage.DeploymentControllerID == m.DeploymentControllerID);

                        if (p == null)
                        {
                            p = new Poller() { PollerMessage = m, Owner = this };
                            p.PollerMessage.AddDeploymentManager(connection);
                            p.ExecutionInterval = m.PollInterval.Value;
                            _PollerPool.PollerThreads.Add(p);
                            _PollerPool.BeginAsync(p);
                        }
                        else
                        {
                            p.PollerMessage.PollInterval = m.PollInterval;
                            p.PollerMessage.AddDeploymentManager(connection);
                        }
                    }
                }
                else if (message is StopPoller)
                {
                    StopPoller m = message as StopPoller;

                    lock (_PollerPool.PollerThreads)
                    {
                        Poller p = _PollerPool.PollerThreads.FirstOrDefault(i => i.PollerMessage.DeploymentControllerID == m.DeploymentControllerID);

                        if (p != null)
                        {
                            _PollerPool.PollerThreads.Remove(p);
                            _PollerPool.EndAsync(p);
                        }
                    }
                }
                else if (message is ClearErrors)
                {
                    ClearErrors m = message as ClearErrors;

                    if (Directory.Exists(ErrorDirectory))
                        try
                        {
                            if (m.SpecificErrors.Count == 0)
                            {
                                foreach (string s in Directory.GetFiles(ErrorDirectory, "*"))
                                    try
                                    {
                                        File.Delete(s);
                                    }
                                    catch { }
                            }
                            else
                            {
                                foreach (string e in m.SpecificErrors)
                                {
                                    if (File.Exists(Path.Combine(ErrorDirectory, e + ".is")))
                                    {
                                        try
                                        {
                                            File.Delete(Path.Combine(ErrorDirectory, e + ".is"));
                                            continue;
                                        }
                                        catch { }
                                    }

                                    if (File.Exists(Path.Combine(ErrorDirectory, e)))
                                    {
                                        try
                                        {
                                            File.Delete(Path.Combine(ErrorDirectory, e));
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                        catch { }

                    SendBranchHealthDetails(connection);
                }
                else if (message is AssemblyList)
                {
                    AssemblyList aList = message as AssemblyList;

                    foreach (STEM.Sys.IO.FileDescription m in aList.Descriptions)
                    {
                        if (m.Filename.EndsWith(".dll.config", StringComparison.InvariantCultureIgnoreCase))
                        {
                            m.Filepath = STEM.Sys.Serialization.VersionManager.VersionCache;
                            m.Save(STEM.Sys.IO.FileExistsAction.Overwrite);
                        }
                    }

                    foreach (STEM.Sys.IO.FileDescription m in aList.Descriptions)
                    {
                        if (m.Filepath == ".")
                        {
                            m.Save(STEM.Sys.IO.FileExistsAction.Overwrite);
                        }
                        else if (m.Filename.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                        {
                            _AsmPool.RunOnce(new System.Threading.ParameterizedThreadStart(LoadAsm), m);
                        }
                    }

                    lock (_RunningSandboxes)
                    {
                        foreach (RunningSandbox s in _RunningSandboxes.Values.Where(i => i.VersionCacheSync == true).ToList())
                            try
                            {
                                if (s.MessageConnection != null)
                                    if (!s.MessageConnection.Send(message))
                                    {
                                        try
                                        {
                                            s.MessageConnection.Close();
                                        }
                                        catch { }
                                    }
                            }
                            catch { }
                    }

                    //while (_AsmPool.LoadLevel > 0)
                    //    System.Threading.Thread.Sleep(10);

                    //connection.Send(new AssemblyList(STEM.Sys.Serialization.VersionManager.VersionCache.Replace(Environment.CurrentDirectory, "."), true));
                }
                else if (message is FileTransfer)
                {
                    FileTransfer m = message as FileTransfer;

                    if (m.DestinationPath == ".")
                    {
                        m.Save();

                        lock (_RunningSandboxes)
                        {
                            foreach (RunningSandbox s in _RunningSandboxes.Values.ToList())
                                try
                                {
                                    if (s.MessageConnection != null)
                                        if (!s.MessageConnection.Send(message))
                                        {
                                            try
                                            {
                                                s.MessageConnection.Close();
                                            }
                                            catch { }
                                        }
                                }
                                catch { }
                        }
                    }
                    else if (m.DestinationFilename.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _AsmPool.RunOnce(new System.Threading.ParameterizedThreadStart(LoadAsm), m);

                        lock (_RunningSandboxes)
                        {
                            foreach (RunningSandbox s in _RunningSandboxes.Values.Where(i => i.VersionCacheSync == true).ToList())
                                try
                                {
                                    if (s.MessageConnection != null)
                                        if (!s.MessageConnection.Send(message))
                                        {
                                            try
                                            {
                                                s.MessageConnection.Close();
                                            }
                                            catch { }
                                        }
                                }
                                catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("BranchManager.Receive", new Exception(message.Serialize(), ex).ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }
        }

        STEM.Sys.Threading.ThreadPool _AsmPool = new Sys.Threading.ThreadPool(Environment.ProcessorCount / 2, true);
        void LoadAsm(object o)
        {
            if (o is FileTransfer)
            {
                FileTransfer m = o as FileTransfer;

                m.DestinationPath = Path.Combine(STEM.Sys.Serialization.VersionManager.VersionCache, m.DestinationPath);
                if (!System.IO.File.Exists(System.IO.Path.Combine(m.DestinationPath, m.DestinationFilename)))
                    STEM.Sys.Serialization.VersionManager.Cache(m.Save(), false);
            }
            else if (o is STEM.Sys.IO.FileDescription)
            {
                STEM.Sys.IO.FileDescription m = o as STEM.Sys.IO.FileDescription;

                m.Filepath = STEM.Sys.Serialization.VersionManager.VersionCache;
                if (!System.IO.File.Exists(System.IO.Path.Combine(m.Filepath, m.Filename)))
                    STEM.Sys.Serialization.VersionManager.Cache(m.Save(Sys.IO.FileExistsAction.MakeUnique), false);
            }
        }

        string ErrorDirectory
        {
            get
            {
                string s = Path.Combine(System.Environment.CurrentDirectory, "Errors");
                if (_SES && System.Environment.CurrentDirectory.ToUpper(System.Globalization.CultureInfo.CurrentCulture).Contains(Path.DirectorySeparatorChar + "SANDBOXES" + Path.DirectorySeparatorChar))
                {
                    s = System.Environment.CurrentDirectory.Replace(STEM.Sys.IO.Path.GetFileName(System.Environment.CurrentDirectory), "");
                    s = s.TrimEnd(Path.DirectorySeparatorChar);
                    s = s.Replace(STEM.Sys.IO.Path.GetFileName(s), "");
                    s = s.TrimEnd(Path.DirectorySeparatorChar);
                    s = Path.Combine(s, "Errors");
                }

                if (!System.IO.Directory.Exists(s))
                    System.IO.Directory.CreateDirectory(s);

                return s;
            }
        }

        bool SendBranchHealthDetails(MessageConnection connection)
        {
            if (_AssemblyInitializationComplete)
            {
                BranchHealthDetails branchHealthDetails = new BranchHealthDetails();

                DateTime generationTime = DateTime.UtcNow;

                List<string> assignments = null;
                List<string> statics = null;

                lock (_Assigned)
                    lock (_RunningSandboxes)
                        try
                        {
                            assignments = _Assigned.Where(i => !i.IsStatic && i.MessageConnection.RemoteAddress == connection.RemoteAddress).Select(i => i.InstructionSetID.ToString()).ToList();

                            foreach (RunningSandbox s in _RunningSandboxes.Values)
                                lock (s)
                                    assignments.AddRange(s.AssignedInstructionSets.Where(i => i.DeploymentManagerIP == connection.RemoteAddress).Select(i => i.InstructionSetID.ToString()));

                            statics = _Statics.Select(i => i.AssignInstructionSet.InitiationSource).ToList();
                        }
                        catch { return false; }
                
                branchHealthDetails.Refresh(assignments, statics, ErrorDirectory);
                branchHealthDetails.BranchState = _LastState;
                branchHealthDetails.GenerationTime = generationTime;
                branchHealthDetails.ProcessorOverload = _ConfigurationDS.Settings[0].ProcessorOverload;

                return connection.Send(branchHealthDetails, false);
            }

            return false;
        }

        internal void ReportState(object o)
        {
            string connectionKey = o as string;

            if (String.IsNullOrEmpty(connectionKey))
                throw new ArgumentNullException(nameof(o));

            MessageConnection connection = null;

            lock (_BranchHealthThreads)
            {
                if (_BranchHealthThreads.ContainsKey(connectionKey))
                    connection = _BranchHealthThreads[connectionKey];
            }

            if (connection == null)
                return;

            DateTime lastAsmListing = DateTime.UtcNow;
            DateTime lastBranchHealthDetails = DateTime.MinValue;

            while (true)
            {
                if ((DateTime.UtcNow - lastBranchHealthDetails).TotalSeconds > 5)
                    try
                    {
                        if (SendBranchHealthDetails(connection))
                            lastBranchHealthDetails = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        STEM.Sys.EventLog.WriteEntry("BranchManager.ReportState", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                    }

                if ((DateTime.UtcNow - lastAsmListing).TotalMinutes > 2)
                    try
                    {
                        if (_AsmPool.LoadLevel == 0)
                            if (connection.Send(new AssemblyList(STEM.Sys.Serialization.VersionManager.VersionCache.Replace(Environment.CurrentDirectory, "."), true)))
                                lastAsmListing = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        STEM.Sys.EventLog.WriteEntry("BranchManager.ReportState", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                    }

                try
                {
                    if (_MessageQueue.ContainsKey(connection.RemoteAddress + connection.RemotePort))
                        lock (_MessageQueueLock[connection.RemoteAddress + connection.RemotePort])
                        {
                            if (_MessageQueue[connection.RemoteAddress + connection.RemoteAddress].Count > 0)
                            {
                                try
                                {
                                    if (SendBranchHealthDetails(connection))
                                        lastBranchHealthDetails = DateTime.UtcNow;
                                }
                                catch (Exception ex)
                                {
                                    STEM.Sys.EventLog.WriteEntry("BranchManager.ReportState", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                                }
                            }
                        }
                }
                catch { }

                System.Threading.Thread.Sleep(1000);

                try
                {
                    lock (_BranchHealthThreads)
                        if (!_BranchHealthThreads.ContainsKey(connectionKey))
                        {
                            return;
                        }
                }
                catch { }
            }
        }

        internal void EndRun(_InstructionSet iSet, bool error)
        {
            if (iSet == null)
                throw new ArgumentNullException(nameof(iSet));

            if (error)
                try
                {
                    System.IO.File.WriteAllText(System.IO.Path.Combine(ErrorDirectory, iSet.ID.ToString() + ".is"), iSet.Serialize());
                }
                catch (Exception ex)
                {
                    STEM.Sys.EventLog.WriteEntry("BranchManager.EndRun", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                }

            try
            {
                if (PostMortemCache != null && iSet.CachePostMortem)
                    System.IO.File.WriteAllText(System.IO.Path.Combine(PostMortemCache, iSet.ID.ToString() + ".is"), iSet.Serialize());
            }
            catch { }
        }
                
        static Dictionary<string, List<Message>> _ManagerMessageReturns = new Dictionary<string, List<Message>>();
        public static bool SendMessage(Message message, MessageConnection connection, SurgeBranchManager actor)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (actor == null)
                throw new ArgumentNullException(nameof(actor));

            if (!connection.Send(message))
            {
                if (actor != null)
                {
                    MessageConnection a = actor.MessageConnection(connection.RemoteAddress);

                    if (a != null)
                        if (a.Send(message))
                            return true;
                }

                lock (_ManagerMessageReturns)
                {
                    if (!_ManagerMessageReturns.ContainsKey(connection.RemoteAddress))
                        _ManagerMessageReturns[connection.RemoteAddress] = new List<Message>();

                    _ManagerMessageReturns[connection.RemoteAddress].Add(message);
                }

                return false;
            }

            return true;
        }

        STEM.Sys.State.KeyManager _LocalKeyManager = new STEM.Sys.State.KeyManager();        
    }
}