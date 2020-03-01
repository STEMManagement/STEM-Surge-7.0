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
using STEM.Sys.Messaging;
using STEM.Surge.Messages;

namespace STEM.Surge
{
    /// <summary>
    /// This is an implementation of a SurgeActor whose intent it is to feed a UI with configuration, state, and activity 
    /// </summary>
    public class UIActor : SurgeActor
    {
        public UIActor(int communicationPort)
            : base(communicationPort)
        {
            System.Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            STEM.Sys.Serialization.VersionManager.Initialize(new List<string>(), true, false);

            AssemblyInitializationComplete = false;

            PrimaryDeploymentManagerIP = STEM.Sys.IO.Net.EmptyAddress;

            STEM.Sys.Global.ThreadPool.BeginAsync(new System.Threading.ThreadStart(ProcessBranchesMessages), TimeSpan.FromMilliseconds(250));
            STEM.Sys.Global.ThreadPool.BeginAsync(new System.Threading.ThreadStart(ProcessBacklogsMessages), TimeSpan.FromMilliseconds(250));
            STEM.Sys.Global.ThreadPool.BeginAsync(new System.Threading.ThreadStart(ProcessActiveDeploymentsMessages), TimeSpan.FromMilliseconds(250));
        }

        /// <summary>
        /// The address passed in to InitializeConnection
        /// </summary>
        public string PrimaryDeploymentManagerIP { get; set;}

        public delegate void ConnectionClosed(UIActor connection);
        public delegate void ConnectionOpened(UIActor connection);
        public delegate void StatusMessage(UIActor connection, string message);

        /// <summary>
        /// Called when the connection to the manager (passed in InitializeConnection) opens
        /// </summary>
        public event ConnectionOpened onPrimaryConnectionOpened;

        /// <summary>
        /// Called when the connection to the manager (passed in InitializeConnection) closes
        /// </summary>
        public event ConnectionClosed onPrimaryConnectionClosed;

        /// <summary>
        /// Intended to feed a status bar
        /// </summary>
        public event StatusMessage onUpdateStatusMessage;

        bool _SslConnection = false;

        bool _Connected = false;
        public bool AssemblyInitializationComplete { get; private set; }
        
        public bool InitializeConnection(string deploymentManagerIP, int port, bool sslConnection)
        {
            if (deploymentManagerIP != PrimaryDeploymentManagerIP)
            {
                lock (_DeploymentManagerConfigurationMutex)
                {
                    _DeploymentManagerConfiguration = new DeploymentManagerConfiguration();
                    _Connected = false;
                    AssemblyInitializationComplete = false;
                    PrimaryDeploymentManagerIP = deploymentManagerIP;
                }

                lock (_ActiveDeploymentsMessages)
                    _ActiveDeploymentsMessages.Clear();

                lock (_BacklogsLock)
                    _Backlogs.Clear();

                lock (_BacklogsMessages)
                    _BacklogsMessages.Clear();

                lock (_BranchEntries)
                    _BranchEntries.Clear();

                lock (_BranchesMessages)
                    _BranchesMessages.Clear();

                lock (_DeploymentsLock)
                    _Deployments.Clear();

                lock (_ActiveDeploymentsMessages)
                    _ActiveDeploymentsMessages.Clear();

                lock (_ManagerReportTimes)
                    _ManagerReportTimes.Clear();

                _SslConnection = sslConnection;

                return ConnectToDeploymentManager(deploymentManagerIP, _SslConnection ? port+1 : port, sslConnection, true);           
            }
            else
            {
                _SslConnection = sslConnection;

                return ConnectToDeploymentManager(deploymentManagerIP, _SslConnection ? port + 1 : port, sslConnection, true);
            }
        }

        /// <summary>
        /// Called when the connection closes
        /// </summary>
        /// <param name="connection">The connection that closed</param>
        protected override void onClosed(Sys.IO.TCP.Connection connection)
        {
            // Call to base
            base.onClosed(connection);

            // Lock _MessageConnections in case the connection re-opens while we are here
            lock (ConnectionLock)
            {
                // Stop sending assembly sync messages
                STEM.Sys.Global.ThreadPool.EndAsync(new System.Threading.ParameterizedThreadStart(SendAssemblyList), connection);

                // Clear queued messages

                lock (_BacklogsMessages)
                    if (_BacklogsMessages.ContainsKey(connection.RemoteAddress))
                    {
                        _BacklogsMessages.Remove(connection.RemoteAddress);
                    }

                lock (_BranchesMessages)
                    if (_BranchesMessages.ContainsKey(connection.RemoteAddress))
                    {
                        _BranchesMessages.Remove(connection.RemoteAddress);
                    }

                lock (_ActiveDeploymentsMessages)
                    if (_ActiveDeploymentsMessages.ContainsKey(connection.RemoteAddress))
                    {
                        _ActiveDeploymentsMessages.Remove(connection.RemoteAddress);

                        lock (_TmpDeployments)
                            _TmpDeployments = _TmpDeployments.Where(i => connection.RemoteAddress != i.DeploymentManagerIP).ToList();
                    }

                // Report disconnect
                try
                {
                    if (onUpdateStatusMessage != null)
                        onUpdateStatusMessage(this, "Deployment Manager Connection Lost (" + connection.RemoteAddress + ")");
                }
                catch { }

                // Call onPrimaryConnectionClosed
                
                bool callConnectionClosed = false;
                lock (_DeploymentManagerConfigurationMutex)
                    if (_Connected && connection.RemoteAddress == PrimaryDeploymentManagerIP)
                    {
                        callConnectionClosed = true;
                        _Connected = false;
                    }

                if (callConnectionClosed)
                    if (onPrimaryConnectionClosed != null && onPrimaryConnectionClosed != null)
                        try
                        {
                            onPrimaryConnectionClosed(this);
                        }
                        catch { }
            }
        }

        /// <summary>
        /// Called when the connection opens
        /// </summary>
        /// <param name="connection">The connection that opened</param>
        protected override void onOpened(Sys.IO.TCP.Connection connection)
        {
            // Call base
            base.onOpened(connection);

            MessageConnection c = connection as MessageConnection;

            // Lock _MessageConnections in case the connection closes while we are here
            lock (ConnectionLock)
            {
                // Report connection
                try
                {
                    if (onUpdateStatusMessage != null)
                        onUpdateStatusMessage(this, "Deployment Manager Connection Established (" + connection.RemoteAddress + ")");
                }
                catch { }

                // Call onPrimaryConnectionOpened
                bool connectionOpenedCall = false;
                lock (_DeploymentManagerConfigurationMutex)
                    if (!_Connected && connection.RemoteAddress == PrimaryDeploymentManagerIP)
                    {
                        c.Send(new AssemblyList(STEM.Sys.Serialization.VersionManager.VersionCache.Replace(Environment.CurrentDirectory, "."), true));

                        _LastAssemblyList = DateTime.UtcNow;

                        connectionOpenedCall = true;
                        _Connected = true;
                    }

                // Start sending assembly sync messages
                STEM.Sys.Global.ThreadPool.BeginAsync(new System.Threading.ParameterizedThreadStart(SendAssemblyList), connection, TimeSpan.FromSeconds(30));

                if (connectionOpenedCall && onPrimaryConnectionOpened != null)
                    try
                    {
                        onPrimaryConnectionOpened(this);
                    }
                    catch { }
            }
        }

        DateTime _LastAssemblyList = DateTime.UtcNow;
        ActiveDeploymentManagers _ActiveDeploymentManagers = null;

        /// <summary>
        /// Sending an AssemblyList message triggers the Deployment manager to sync us to its VersionCache
        /// Once it has completed the initialsync it will send us an AssemblyInitializationComplete message
        /// We will keep sending the AssemblyList message every 30 seconds in case Assemblies are added at the manager
        /// </summary>
        /// <param name="c">The connection to a DeploymentManager</param>
        void SendAssemblyList(object c)
        {
            MessageConnection connection = c as MessageConnection;

            if (connection.RemoteAddress == PrimaryDeploymentManagerIP)
            {
                while (!AssemblyInitializationComplete)
                {
                    if (_AsmPool.LoadLevel == 0 && (DateTime.UtcNow - _LastAssemblyList).TotalSeconds > 3)
                    {
                        _LastAssemblyList = DateTime.UtcNow;
                        connection.Send(new AssemblyList(STEM.Sys.Serialization.VersionManager.VersionCache.Replace(Environment.CurrentDirectory, "."), true));
                    }

                    System.Threading.Thread.Sleep(1000);
                }

                if (_ActiveDeploymentManagers != null)
                    foreach (string i in _ActiveDeploymentManagers.DeploymentManagers)
                        ConnectToDeploymentManager(i, _SslConnection ? CommunicationPort + 1 : CommunicationPort, _SslConnection, true);
            }
            else if (!AssemblyInitializationComplete)
            {
                return;
            }

            connection.Send(new AssemblyList(STEM.Sys.Serialization.VersionManager.VersionCache.Replace(Environment.CurrentDirectory, "."), true));
        }
        
        /// <summary>
        /// Attempt to send a message to all connected DeploymentManagers
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns>True if the message was sent to any manager, else False</returns>
        public bool SendToAll(Message message)
        {
            bool ret = false;
            try
            {
                foreach (MessageConnection c in MessageConnections())
                    if (c.Send(message))
                        ret = true;
            }
            catch { }

            if (!ret)
            {
                try
                {
                    if (onUpdateStatusMessage != null)
                        onUpdateStatusMessage(this, "Failed to send message (" + message.GetType().Name + ")");
                }
                catch { }
            }

            return ret;
        }

        /// <summary>
        /// Attempt to send a message to any connected DeploymentManager
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns>True if the message was sent to any manager, else False</returns>
        public bool Send(Message message)
        {
            try
            {
                foreach (MessageConnection c in MessageConnections())
                    if (c.Send(message))
                        return true;
            }
            catch { }

            try
            {
                if (onUpdateStatusMessage != null)
                    onUpdateStatusMessage(this, "Failed to send message (" + message.GetType().Name + ")");
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Attempt to send a message to any connected DeploymentManager and wait for a response
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="waitForResponse">The time to wait for a response</param>
        /// <returns>The message response or Undeliverable</returns>
        public Message Send(Message message, TimeSpan waitForResponse)
        {
            try
            {
                foreach (MessageConnection c in MessageConnections())
                {
                    Message m = c.Send(message, waitForResponse);
                    if (!(m is Undeliverable))
                        return m;
                }
            }
            catch { }

            try
            {
                if (onUpdateStatusMessage != null)
                    onUpdateStatusMessage(this, "Failed to send message (" + message.GetType().Name + ")");
            }
            catch { }

            return new Undeliverable();
        }

        /// <summary>
        /// Attempt to send a message to a specific DeploymentManager
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="deploymentManagerIP">The DeploymentManager to send the message to</param>
        /// <returns>True if the message was sent, else False</returns>
        public bool Send(Message message, string deploymentManagerIP)
        {
            try
            {
                MessageConnection c = MessageConnection(deploymentManagerIP);
                if (c != null)
                    if (c.Send(message))
                        return true;
            }
            catch { }

            try
            {
                if (onUpdateStatusMessage != null)
                    onUpdateStatusMessage(this, "Failed to send message (" + message.GetType().Name + ")");
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Attempt to send a message to a specific DeploymentManager and wait for a response
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="deploymentManagerIP">The DeploymentManager to send the message to</param>
        /// <param name="waitForResponse">The time to wait for a response</param>
        /// <returns>The message response or Undeliverable</returns>
        public Message Send(Message message, string deploymentManagerIP, TimeSpan waitForResponse)
        {
            try
            {
                MessageConnection c = MessageConnection(deploymentManagerIP);
                if (c != null)
                {
                    Message m = c.Send(message, waitForResponse);
                    if (!(m is Undeliverable))
                        return m;
                }
            }
            catch { }

            try
            {
                if (onUpdateStatusMessage != null)
                    onUpdateStatusMessage(this, "Failed to send message (" + message.GetType().Name + ")");
            }
            catch { }

            return new Undeliverable();
        }

        object _BranchesLock = new object();
        List<Branches.Entry> _BranchEntries = new List<Branches.Entry>();

        /// <summary>
        /// The list of current Branches.Entry objects
        /// </summary>
        public List<Branches.Entry> BranchEntries
        {
            get
            {
                lock (_BranchesLock)
                {
                    return _BranchEntries.ToList();
                }
            }
        }

        /// <summary>
        /// The list of BranchManager report times to each DeploymentManager
        /// </summary>
        public List<ManagerReportTimes> LatestManagerReportTimes
        {
            get
            {
                lock (_BranchEntries)
                {
                    return _ManagerReportTimes.ToList();
                }
            }
        }

        object _BacklogsLock = new object();
        List<Backlogs.Entry> _Backlogs = new List<Backlogs.Entry>();

        /// <summary>
        /// The list of current Backlogs.Entry objects
        /// </summary>
        public List<Backlogs.Entry> Backlogs
        {
            get
            {
                lock (_BacklogsLock)
                {
                    return _Backlogs.ToList();
                }
            }
        }

        /// <summary>
        /// Gets the list of current Backlogs.Entry objects for all pollers actively assigning
        /// </summary>
        /// <returns>List<Backlogs.Entry></returns>
        public List<Backlogs.Entry> BacklogsFromActiveAssigners()
        {
            lock (_BacklogsLock)
            {
                return _Backlogs.Where(i => i.Assigning == true).ToList();
            }
        }

        /// <summary>
        /// Gets the list of current Backlogs.Entry objects for a given switchboard row
        /// </summary>
        /// <param name="switchboardRowID">The switchboard row of interest</param>
        /// <returns>List<Backlogs.Entry></returns>
        public List<Backlogs.Entry> BacklogsBySwitchboardRowID(string switchboardRowID)
        {
            lock (_BacklogsLock)
            {
                return _Backlogs.Where(i => i.SwitchboardRowID == switchboardRowID).ToList();
            }
        }

        object _DeploymentsLock = new object();
        List<ActiveDeployments.Entry> _Deployments = new List<ActiveDeployments.Entry>();

        /// <summary>
        /// The list of current ActiveDeployments.Entry objects
        /// </summary>
        public List<ActiveDeployments.Entry> Deployments
        {
            get
            {
                lock (_DeploymentsLock)
                {
                    return _Deployments.ToList();
                }
            }
        }

        /// <summary>
        /// Gets the list of current ActiveDeployments.Entry objects for all pollers in deploymentControllerIDs
        /// </summary>
        /// <param name="deploymentControllerIDs">The deploymentControllerIDs of interest</param>
        /// <returns>List<ActiveDeployments.Entry></returns>
        public List<ActiveDeployments.Entry> DeploymentsByDeploymentController(List<string> deploymentControllerIDs)
        {
            lock (_DeploymentsLock)
            {
                return _Deployments.Where(i => deploymentControllerIDs.Contains(i.DeploymentControllerID)).ToList();
            }
        }

        /// <summary>
        /// Gets the list of current ActiveDeployments.Entry objects for a given switchboard row
        /// </summary>
        /// <param name="switchboardRowID">The switchboard row of interest</param>
        /// <returns>List<ActiveDeployments.Entry></returns>
        public List<ActiveDeployments.Entry> DeploymentsBySwitchboardRowID(string switchboardRowID)
        {
            lock (_DeploymentsLock)
            {
                return _Deployments.Where(i => i.SwitchboardRowID == switchboardRowID).ToList();
            }
        }

        object _DeploymentManagerConfigurationMutex = new object();
        DeploymentManagerConfiguration _DeploymentManagerConfiguration = new DeploymentManagerConfiguration();

        /// <summary>
        /// Gets the current DeploymentManagerConfiguration message
        /// </summary>
        public DeploymentManagerConfiguration DeploymentManagerConfiguration
        {
            get
            {
                lock (_DeploymentManagerConfigurationMutex)
                    return _DeploymentManagerConfiguration;
            }
        }

        /// <summary>
        /// Submit an update to the SwitchboardConfig
        /// </summary>
        /// <param name="switchboardConfiguration">The updated configuration</param>
        public void SubmitSwitchboardConfigurationUpdate(SwitchboardConfig switchboardConfiguration)
        {
            lock (_DeploymentManagerConfigurationMutex)
            {
                if (_DeploymentManagerConfiguration.SwitchboardConfigurationDescription.StringContent != switchboardConfiguration.GetXml())
                {
                    _DeploymentManagerConfiguration.SwitchboardConfigurationDescription.StringContent = switchboardConfiguration.GetXml();
                    _DeploymentManagerConfiguration.SwitchboardConfigurationDescription.LastWriteTimeUtc = DateTime.UtcNow;
                    SendToAll(_DeploymentManagerConfiguration);

                    try
                    {
                        try
                        {
                            if (onUpdateStatusMessage != null)
                                onUpdateStatusMessage(this, "Configuration Update Received: " + _DeploymentManagerConfiguration.SwitchboardConfigurationDescription.LastWriteTimeUtc.ToString("G"));
                        }
                        catch { }

                        foreach (EventHandler h in onSwitchboardConfigUpdated.GetInvocationList())
                            h(this, ResolveEventArgs.Empty);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Submit the entire DeploymentManagerConfiguration message
        /// </summary>
        public void SubmitConfigurationUpdate()
        {
            lock (_DeploymentManagerConfigurationMutex)
            {
                SendToAll(_DeploymentManagerConfiguration);
            }
        }

        /// <summary>
        /// Called when an update to the Switchboard is received
        /// </summary>
        public event EventHandler onSwitchboardConfigUpdated;

        Dictionary<string, Branches> _BranchesMessages = new Dictionary<string, Branches>();
        List<ManagerReportTimes> _ManagerReportTimes = new List<ManagerReportTimes>();

        public class ManagerReportTimes
        {
            public string ManagerIP { get; set; }
            public DateTime LastManagerReport { get; set; }
            public Dictionary<string, DateTime> LastBranchReport { get; set; }
        }

        /// <summary>
        /// Thread entry point to process Branches messages 
        /// </summary>
        void ProcessBranchesMessages()
        {
            try
            {
                List<string> keys = null;
                List<string> deadConnections = new List<string>();

                while (true)
                    try
                    {
                        deadConnections.AddRange(_BranchesMessages.Keys.Except(Connections()));
                        break;
                    }
                    catch { }

                lock (_BranchesMessages)
                {
                    foreach (string k in deadConnections)
                        _BranchesMessages.Remove(k);

                    keys = _BranchesMessages.Keys.ToList();
                }

                List<Branches.Entry> branchEntries = new List<Branches.Entry>();
                foreach (string k in keys)
                {
                    Branches m;
                    lock (_BranchesMessages)
                        m = _BranchesMessages[k];

                    ManagerReportTimes mrt = _ManagerReportTimes.FirstOrDefault(i => i.ManagerIP == m.MessageConnection.RemoteAddress);

                    if (mrt == null)
                    {
                        mrt = new ManagerReportTimes { ManagerIP = m.MessageConnection.RemoteAddress, LastBranchReport = new Dictionary<string, DateTime>() };
                        _ManagerReportTimes.Add(mrt);
                    }

                    foreach (Branches.Entry e in m.Entries)
                    {
                        if (e.BranchIP == null)
                            continue;
                        Branches.Entry o = branchEntries.FirstOrDefault(i => i.BranchIP == e.BranchIP);

                        mrt.LastManagerReport = m.TimeReceived;
                        mrt.LastBranchReport[e.BranchIP] = e.LastStateReport;

                        if (o == null)
                        {
                            branchEntries.Add(e);
                        }
                        else
                        {
                            o.Assigned += e.Assigned;
                            o.Processing += e.Processing;

                            o.CopyFrom(e);
                        }
                    }
                }

                lock (_BranchesLock)
                    _BranchEntries = branchEntries;
            }
            catch { }
        }

        Dictionary<string, Backlogs> _BacklogsMessages = new Dictionary<string, Backlogs>();

        /// <summary>
        /// Thread entry point to process Backlogs messages
        /// </summary>
        void ProcessBacklogsMessages()
        {
            while (true)
            {
                try
                {
                    List<Backlogs> backlogList = null;
                    List<string> deadConnections = new List<string>();
                        
                    while (true)
                        try
                        {
                            deadConnections.AddRange(_BacklogsMessages.Keys.Except(Connections()));
                            break;
                        }
                        catch { }
                    
                    lock (_BacklogsMessages)
                        if (_BacklogsMessages.Count > 0)
                        {
                            foreach (string k in deadConnections)
                                _BacklogsMessages.Remove(k);

                            backlogList = _BacklogsMessages.Select(i => i.Value).ToList();
                        }

                    if (backlogList == null || backlogList.Count == 0)
                        return;

                    Dictionary<string, Backlogs.Entry> backlogs = null;
                    
                    foreach (Backlogs m in backlogList)
                    {
                        if (backlogs == null)
                        {
                            backlogs = m.Entries.ToDictionary(i => i.DeploymentControllerID, i => i);

                            foreach (Backlogs.Entry x in backlogs.Values)
                                if (x.PollError != null && x.PollError.Trim() != "" && !x.PollError.StartsWith(x.LastPoll.ToString("G")))
                                    x.PollError = x.LastPoll.ToString("G") + ": " + x.PollError;

                            continue;
                        }

                        foreach (Backlogs.Entry x in m.Entries)
                        {
                            if (x.PollError != null && x.PollError.Trim() != "" && !x.PollError.StartsWith(x.LastPoll.ToString("G")))
                                x.PollError = x.LastPoll.ToString("G") + ": " + x.PollError;

                            if (!backlogs.ContainsKey(x.DeploymentControllerID))
                            {
                                backlogs[x.DeploymentControllerID] = x;

                                if (!x.Assigning)
                                    x.PerceivedBacklogCount = 0;

                                continue;
                            }

                            Backlogs.Entry y = backlogs[x.DeploymentControllerID];

                            y.Assigned += x.Assigned;
                            y.Processing += x.Processing;
                            y.Assigning = y.Assigning || x.Assigning;
                            y.PingFailure = y.PingFailure || x.PingFailure;
                            y.PollFailure = y.PollFailure || x.PollFailure;
                            y.ControllerLoadError = y.ControllerLoadError || x.ControllerLoadError;

                            if (x.PollError != null && y.PollError != null && x.PollError.Trim() != "" && !y.PollError.Trim().Contains(x.PollError.Trim()))
                            {
                                if (y.PollError.Trim() != "")
                                    y.PollError = y.PollError.Trim() + "\r\n" + x.PollError.Trim();
                                else
                                    y.PollError = x.PollError.Trim();
                            }

                            if (!String.IsNullOrEmpty(x.ListWalkSummary))
                            {
                                if (String.IsNullOrEmpty(y.ListWalkSummary))
                                {
                                    y.ListWalkSummary = x.ListWalkSummary.Trim();
                                }
                                else if (!y.ListWalkSummary.Contains(x.ListWalkSummary.Trim()))
                                {
                                    y.ListWalkSummary = y.ListWalkSummary.Trim() + "\r\n\r\n" + x.ListWalkSummary.Trim();
                                }
                            }

                            if (x.LastPoll > y.LastPoll && x.PollFailure == false && x.PingFailure == false)
                            {
                                y.LastPoll = x.LastPoll;
                                y.BacklogCount = x.BacklogCount;
                            }

                            if (x.Assigning && x.LastAssignment > y.LastAssignment)
                                y.PerceivedBacklogCount = x.PerceivedBacklogCount;

                            if (x.LastAssignment > y.LastAssignment)
                                y.LastAssignment = x.LastAssignment;

                            if (y.LastWalkStart == DateTime.MinValue)
                                y.LastWalkStart = x.LastWalkStart;

                            if (x.LastWalkStart != DateTime.MinValue && x.LastWalkStart < y.LastWalkStart)
                                y.LastWalkStart = x.LastWalkStart;                            

                            if (x.LastAssignmentEnd > y.LastAssignmentEnd)
                                y.LastAssignmentEnd = x.LastAssignmentEnd;

                            if (x.LastDataActivity > y.LastDataActivity)
                                y.LastDataActivity = x.LastDataActivity;
                        }
                    }

                    if (backlogs != null)
                        lock (_BacklogsLock)
                            _Backlogs = backlogs.Values.ToList();
                }
                catch (Exception ex)
                {
                    STEM.Sys.EventLog.WriteEntry("UIActor.ProcessBacklogsMessages", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                }

                System.Threading.Thread.Sleep(25);
            }
        }

        Dictionary<string, List<ActiveDeployments>> _ActiveDeploymentsMessages = new Dictionary<string, List<ActiveDeployments>>();
        List<ActiveDeployments.Entry> _TmpDeployments = new List<ActiveDeployments.Entry>();

        /// <summary>
        /// Thread entry point to process ActiveDeployments messages
        /// </summary>
        void ProcessActiveDeploymentsMessages()
        {
            List<string> deadConnections = new List<string>(100);
            List<string> keys = new List<string>(100);

            while (true)
            {
                try
                {
                    deadConnections.Clear();
                    keys.Clear();

                    while (true)
                        try
                        {
                            deadConnections.AddRange(_ActiveDeploymentsMessages.Keys.Except(Connections()));
                            break;
                        }
                        catch { }

                    lock (_ActiveDeploymentsMessages)
                    {
                        foreach (string k in deadConnections)
                            _ActiveDeploymentsMessages.Remove(k);

                        keys.AddRange(_ActiveDeploymentsMessages.Keys);
                    }

                    if (deadConnections.Count > 0)
                        lock (_TmpDeployments)
                            _TmpDeployments = _TmpDeployments.Where(i => !deadConnections.Contains(i.DeploymentManagerIP)).ToList();

                    if (keys.Count == 0 && deadConnections.Count > 0)
                    {
                        lock (_TmpDeployments)
                            lock (_DeploymentsLock)
                                _Deployments = _TmpDeployments.ToList();
                    }
                    else
                    {
                        foreach (string key in keys)
                        {
                            List<Guid> allActive = new List<Guid>();

                            List<ActiveDeployments> messages = null;

                            lock (_ActiveDeploymentsMessages[key])
                            {
                                messages = _ActiveDeploymentsMessages[key].ToList();
                                _ActiveDeploymentsMessages[key].Clear();
                            }

                            if (messages.Count == 0)
                                continue;

                            List<ActiveDeployments.Entry> working = new List<ActiveDeployments.Entry>(messages.Sum(i => i.Entries.Count));
                            HashSet <Guid> ids = new HashSet<Guid>();

                            foreach (ActiveDeployments m in messages.OrderByDescending(i => i.TimeReceived))
                            {
                                allActive.AddRange(m.Active);

                                if (m.Entries.Count > 0)
                                {
                                    working.AddRange(m.Entries.Where(i => !ids.Contains(i.InstructionSetID) && !(i.Completed != DateTime.MinValue && (DateTime.UtcNow - i.Completed).TotalMinutes > 2)));

                                    ids = new HashSet<Guid>(working.Select(i => i.InstructionSetID));
                                }
                            }

                            ids = new HashSet<Guid>(working.Select(i => i.InstructionSetID));

                            lock (_TmpDeployments)
                                _TmpDeployments.RemoveAll(i => ids.Contains(i.InstructionSetID) || deadConnections.Contains(i.DeploymentManagerIP) || (i.Completed != DateTime.MinValue && (DateTime.UtcNow - i.Completed).TotalMinutes > 2));
                            
                            lock (_TmpDeployments)
                                _TmpDeployments.RemoveAll(i => i.DeploymentManagerIP == key && !allActive.Contains(i.InstructionSetID));

                            lock (_TmpDeployments)
                                _TmpDeployments.AddRange(working);

                            lock (_TmpDeployments)
                                lock (_DeploymentsLock)
                                    _Deployments = _TmpDeployments.ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    STEM.Sys.EventLog.WriteEntry("UIActor.ProcessActiveDeploymentsMessages", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                }

                System.Threading.Thread.Sleep(25);
            }
        }

        bool _MessageBacklog = false;
        
        /// <summary>
        /// Called when a message is received, in the order in which it was received on this connection
        /// Called from the message thread for this connection, beware of blocking this thread
        /// </summary>
        /// <param name="connection">The connection that received this message</param>
        /// <param name="message">The message received</param>
        protected override void onReceived(MessageConnection connection, Message message)
        {
            if (connection.MessageBacklog > 25)
            {
                //STEM.Sys.EventLog.WriteEntry("UI.Receive", message.GetType().ToString(System.Globalization.CultureInfo.CurrentCulture), STEM.Sys.EventLog.EventLogEntryType.Information);

                _MessageBacklog = true;
                try
                {
                    if (onUpdateStatusMessage != null)
                        onUpdateStatusMessage(this, "Connection (" + connection.RemoteAddress + ") Message Backlog: " + connection.MessageBacklog);
                }
                catch { }
            }
            else if (_MessageBacklog)
            {
                _MessageBacklog = false;
                onUpdateStatusMessage(this, "Connection (" + connection.RemoteAddress + ") Message Backlog: 0");
            }

            if (message is ActiveDeploymentManagers)
            {
                _ActiveDeploymentManagers = message as ActiveDeploymentManagers;
            }
            else if (message is Branches)
            {
                Branches m = message as Branches;

                if (connection.IsConnected())
                    lock (_BranchesMessages)
                        _BranchesMessages[connection.RemoteAddress] = m;
            }
            else if (message is Backlogs)
            {
                Backlogs m = message as Backlogs;

                if (connection.IsConnected())
                    lock (_BacklogsMessages)
                        _BacklogsMessages[connection.RemoteAddress] = m;
            }
            else if (message is ActiveDeployments)
            {
                ActiveDeployments m = message as ActiveDeployments;

                if (!_ActiveDeploymentsMessages.ContainsKey(connection.RemoteAddress))
                    lock (_ActiveDeploymentsMessages)
                        if (!_ActiveDeploymentsMessages.ContainsKey(connection.RemoteAddress))
                            _ActiveDeploymentsMessages[connection.RemoteAddress] = new List<ActiveDeployments>();

                if (connection.IsConnected())
                    lock (_ActiveDeploymentsMessages[connection.RemoteAddress])
                        _ActiveDeploymentsMessages[connection.RemoteAddress].Add(m);
            }
            else if (message is AssemblyInitializationComplete)
            {
                if (AssemblyInitializationComplete)
                    return;

                AssemblyInitializationComplete = true;

                try
                {
                    if (onUpdateStatusMessage != null)
                        onUpdateStatusMessage(this, "Assembly Initialization Complete.");
                }
                catch { }
            }
            else if (message is DeploymentManagerConfiguration)
            {
                if (connection.RemoteAddress == PrimaryDeploymentManagerIP)
                {
                    DeploymentManagerConfiguration m = message as DeploymentManagerConfiguration;

                    bool updated = true;

                    bool connectionOpenedCall = false;

                    lock (_DeploymentManagerConfigurationMutex)
                    {
                        if (_DeploymentManagerConfiguration.MessageConnection == null)
                            _DeploymentManagerConfiguration = m;
                        else
                            updated = _DeploymentManagerConfiguration.Update(m);

                        if (_Connected == false)
                        {
                            connectionOpenedCall = true;
                            _Connected = true;
                        }
                    }

                    if (connectionOpenedCall && onPrimaryConnectionOpened != null)
                        try
                        {
                            onPrimaryConnectionOpened(this);
                        }
                        catch { }

                    if (updated)
                        try
                        {
                            try
                            {
                                if (onUpdateStatusMessage != null)
                                    onUpdateStatusMessage(this, "Configuration Update Received: " + m.TimeSent.ToString("G"));
                            }
                            catch { }

                            foreach (EventHandler h in onSwitchboardConfigUpdated.GetInvocationList())
                                h(this, ResolveEventArgs.Empty);
                        }
                        catch { }
                }
            }
            else if (message is AssemblyList)
            {
                AssemblyList aList = message as AssemblyList;

                _LastAssemblyList = DateTime.UtcNow;

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
            }
            else if (message is FileTransfer)
            {
                FileTransfer m = message as FileTransfer;

                if (m.DestinationPath == ".")
                {
                    m.Save();
                }
                else if (m.DestinationFilename.EndsWith(".dll.config", StringComparison.InvariantCultureIgnoreCase))
                {
                    m.DestinationPath = STEM.Sys.Serialization.VersionManager.VersionCache;
                    m.Save();
                }
                else if (m.DestinationFilename.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                {
                    _AsmPool.RunOnce(new System.Threading.ParameterizedThreadStart(LoadAsm), m);
                }
            }
        }
        
        STEM.Sys.Threading.ThreadPool _AsmPool = new Sys.Threading.ThreadPool(Int32.MaxValue);
        void LoadAsm(object o)
        {
            if (o is FileTransfer)
            {
                FileTransfer m = o as FileTransfer;

                m.DestinationPath = System.IO.Path.Combine(STEM.Sys.Serialization.VersionManager.VersionCache, m.DestinationPath);
                if (!System.IO.File.Exists(System.IO.Path.Combine(m.DestinationPath, m.DestinationFilename)))
                    STEM.Sys.Serialization.VersionManager.Cache(m.Save(), true);

                try
                {
                    if (onUpdateStatusMessage != null)
                        onUpdateStatusMessage(this, "Loading " + m.DestinationFilename);
                }
                catch { }
            }
            else if (o is STEM.Sys.IO.FileDescription)
            {
                STEM.Sys.IO.FileDescription m = o as STEM.Sys.IO.FileDescription;

                m.Filepath = STEM.Sys.Serialization.VersionManager.VersionCache;

                if (!System.IO.File.Exists(System.IO.Path.Combine(m.Filepath, m.Filename)))
                    STEM.Sys.Serialization.VersionManager.Cache(m.Save(STEM.Sys.IO.FileExistsAction.Skip), true);

                try
                {
                    if (onUpdateStatusMessage != null)
                        onUpdateStatusMessage(this, "Loading " + STEM.Sys.IO.Path.GetFileName(m.Filename));
                }
                catch { }
            }
        }
    }
}
