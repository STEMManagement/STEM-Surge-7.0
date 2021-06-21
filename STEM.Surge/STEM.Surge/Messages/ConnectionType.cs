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
using STEM.Sys.Messaging;
using STEM.Sys.IO.TCP;

namespace STEM.Surge.Messages
{
    /// <summary>
    /// A message sent in handshake informing on connection type
    /// </summary>
    public class ConnectionType : STEM.Sys.Messaging.Message
    {
        public delegate void HandshakeComplete(ConnectionType sender, Connection connection);
        public event HandshakeComplete onHandshakeComplete;

        public enum Types { SurgeActor, SurgeBranchManager, SurgeSandbox, SurgeDeploymentManager }

        public Types Type { get; set; }
        
        public ConnectionType()
        {
        }

        int _ConnectionSessionID = 0;

        public void PerformHandshake(MessageConnection connection)
        {
            _ConnectionSessionID = connection.SessionID();

            STEM.Sys.Global.ThreadPool.RunOnce(new System.Threading.ParameterizedThreadStart(Handshake), connection);
        }

        void Handshake(object o)
        {
            MessageConnection connection = o as MessageConnection;

            try
            {
                if (connection.SessionID() != _ConnectionSessionID)
                {
                    STEM.Sys.EventLog.WriteEntry("ConnectionType.Handshake", "Handshake Undeliverable to " + connection.RemoteAddress + ".", Sys.EventLog.EventLogEntryType.Information);
                    return;
                }

                int port = connection.LocalPort;
                if (connection.ConnectionRole == Role.Server)
                    port = connection.RemotePort;

                STEM.Sys.EventLog.WriteEntry("ConnectionType.Handshake", "Performing handshake with " + connection.RemoteAddress + ":" + port + ".", Sys.EventLog.EventLogEntryType.Information);

                Message response = connection.Send(this, TimeSpan.FromSeconds(15));

                if (response is Timeout)
                {
                    if (connection.SessionID() != _ConnectionSessionID)
                    {
                        STEM.Sys.EventLog.WriteEntry("ConnectionType.Handshake", "Handshake Undeliverable to " + connection.RemoteAddress + ":" + port + ".", Sys.EventLog.EventLogEntryType.Information);
                        return;
                    }

                    STEM.Sys.EventLog.WriteEntry("ConnectionType.Handshake", "Handshake timeout with " + connection.RemoteAddress + ":" + port + ".", Sys.EventLog.EventLogEntryType.Information);

                    if (onHandshakeComplete != null)
                        connection.Close();

                    return;
                }

                if (response is Undeliverable)
                {
                    STEM.Sys.EventLog.WriteEntry("ConnectionType.Handshake", "Handshake Undeliverable to " + connection.RemoteAddress + ":" + port + ".", Sys.EventLog.EventLogEntryType.Information);
                    return;
                }

                if (onHandshakeComplete != null)
                    try
                    {
                        onHandshakeComplete(this, connection);
                    }
                    catch { }

                STEM.Sys.EventLog.WriteEntry("ConnectionType.Handshake", "Handshake completed with " + connection.RemoteAddress + ":" + port + ".", Sys.EventLog.EventLogEntryType.Information);
            }
            catch
            {
                if (connection.SessionID() == _ConnectionSessionID)
                {
                    connection.Close();
                }
            }
        }
    }
}
