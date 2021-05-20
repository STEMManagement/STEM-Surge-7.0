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
using System.Text;
using STEM.Sys.Messaging;
using STEM.Sys.IO.TCP;
using STEM.Surge.Messages;
using System.Security.Cryptography.X509Certificates;

namespace STEM.Surge
{
    /// <summary>
    /// This is the base class for all actors connecting to one or many DeploymentManagers
    /// </summary>
    public abstract class SurgeActor
    {
        /// <summary>
        /// The port upon which to connect to the DeploymentManager(s)
        /// </summary>
        public int CommunicationPort { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SurgeActor(int communicationPort)
        {
            CommunicationPort = communicationPort;
        }

        protected readonly object ConnectionLock = new object();

        List<MessageConnection> _MessageConnections = new List<MessageConnection>();

        /// <summary>
        /// Add a MessageConnection to the list of managed connections 
        /// </summary>
        /// <param name="connection">The MessageConnection to add</param>
        protected void ManageConnection(MessageConnection connection)
        {
            lock (ConnectionLock)
            {
                if (!_MessageConnections.Contains(connection))
                {
                    _MessageConnections.Add(connection);
                }
            }
        }

        /// <summary>
        /// Get a copy of the list of active message connections
        /// </summary>
        /// <returns>The list of active MessageConnections</returns>
        protected List<MessageConnection> MessageConnections()
        {
            while (true)
                try
                {
                    return _MessageConnections.ToList();
                }
                catch { }
        }

        /// <summary>
        /// Get a specific active message connection if it exists
        /// </summary>
        /// <returns>The target MessageConnection if it exists, else null</returns>
        protected MessageConnection MessageConnection(string remoteAddress)
        {
            while (true)
                try
                {
                    return _MessageConnections.FirstOrDefault(i => i.RemoteAddress == remoteAddress);
                }
                catch { }
        }

        /// <summary>
        /// Returns a list of addresses with which we have an active connection
        /// </summary>
        /// <returns>List<string> of addresses</returns>
        public List<string> Connections()
        {
            while (true)
                try
                {
                    return _MessageConnections.Where(i => i.IsConnected()).Select(i => i.RemoteAddress).ToList();
                }
                catch { }
        }

        /// <summary>
        /// Query connection
        /// </summary>
        /// <param name="address">The address of interest</param>
        /// <returns>True if an active connection exists, else False</returns>
        public bool IsConnected(string address)
        {
            while (true)
                try
                {
                    return _MessageConnections.Exists(i => i.RemoteAddress == address && i.IsConnected());
                }
                catch { }
        }

        /// <summary>
        /// Query for any active connection
        /// </summary>
        /// <returns>True if an active connection exists, else False</returns>
        public bool IsConnected()
        {
            while (true)
                try
                {
                    return _MessageConnections.Any(i => i.IsConnected() == true);
                }
                catch { }
        }

        /// <summary>
        /// Called to establish a connection to a DeploymentManager
        /// Redundant calls are harmless
        /// </summary>
        /// <param name="address">The DeploymentManager ip</param>
        /// <param name="port">The communication port</param>
        /// <param name="autoReconnect">True if reconnects should be attempted when the connection is lost</param>
        /// <returns>True if the connection is active, else False</returns>
        public bool ConnectToDeploymentManager(string address, int port, bool sslConnection, bool autoReconnect)
        {
            string ipAddress = STEM.Sys.IO.Net.MachineAddress(address);

            lock (ConnectionLock)
            {
                MessageConnection c = _MessageConnections.FirstOrDefault(i => i.RemoteAddress == ipAddress);

                if (c == null)
                {
                    c = new MessageConnection(ipAddress, port, sslConnection, true, autoReconnect);
                    c.onClosed += onClosed;
                    c.onReceived += onReceived;
                    c.onOpened += onOpened;
                    _MessageConnections.Add(c);
                }

                return c.IsConnected();
            }
        }

        /// <summary>
        /// Called to establish a connection to a DeploymentManager
        /// Redundant calls are harmless
        /// </summary>
        /// <param name="address">The DeploymentManager ip</param>
        /// <param name="port">The communication port</param>
        /// <param name="certificate">Client certificate</param>
        /// <param name="autoReconnect">True if reconnects should be attempted when the connection is lost</param>
        /// <returns>True if the connection is active, else False</returns>
        public bool ConnectToDeploymentManager(string address, int port, bool sslConnection, X509Certificate2 certificate, bool autoReconnect)
        {
            string ipAddress = STEM.Sys.IO.Net.MachineAddress(address);

            lock (ConnectionLock)
            {
                MessageConnection c = _MessageConnections.FirstOrDefault(i => i.RemoteAddress == ipAddress);

                if (c == null)
                {
                    c = new MessageConnection(ipAddress, port, sslConnection, true, certificate, autoReconnect);
                    c.onClosed += onClosed;
                    c.onReceived += onReceived;
                    c.onOpened += onOpened;
                    _MessageConnections.Add(c);
                }

                return c.IsConnected();
            }
        }

        /// <summary>
        /// Close the connection to a DeploymentManager and stop any attempts to reconnect 
        /// </summary>
        /// <param name="address">The Address of the DeploymentManager</param>
        public void CloseDeploymentManagerConnection(string address)
        {
            string ipAddress = STEM.Sys.IO.Net.MachineAddress(address);

            lock (ConnectionLock)
            {
                MessageConnection c = _MessageConnections.FirstOrDefault(i => i.RemoteAddress == ipAddress);

                if (c != null)
                {
                    c.AutoReconnect = false;
                    c.Close();
                }
            }
        }

        /// <summary>
        /// Called when a message is received, in the order in which it was received on this connection
        /// Called from the message thread for this connection, beware of blocking this thread
        /// </summary>
        /// <param name="connection">The connection that received this message</param>
        /// <param name="message">The message received</param>
        protected virtual void onReceived(MessageConnection connection, Message message)
        {
        }

        /// <summary>
        /// Called when the connection opens
        /// </summary>
        /// <param name="connection">The connection that opened</param>
        protected virtual void onOpened(Connection connection)
        {
            lock (ConnectionLock)
            {
                MessageConnection c = connection as MessageConnection;
                if (c != null)
                    if (!_MessageConnections.Contains(c))
                    {
                        _MessageConnections.Add(c);
                    }

                ConnectionType m = new ConnectionType { Type = ActorType() };
                m.onHandshakeComplete += onHandshakeComplete;
                m.PerformHandshake(c);
            }
        }

        protected virtual ConnectionType.Types ActorType()
        {
            return ConnectionType.Types.SurgeActor;
        }

        protected virtual void onHandshakeComplete(Connection connection)
        {
        }

        /// <summary>
        /// Called when the connection closes
        /// </summary>
        /// <param name="connection">The connection that closed</param>
        protected virtual void onClosed(Connection connection)
        {
            lock (ConnectionLock)
            {
                MessageConnection c = connection as MessageConnection;
                if (c != null && !c.AutoReconnect)
                    if (_MessageConnections.Contains(c))
                    {
                        _MessageConnections.Remove(c);
                    }
            }
        }
    }
}
