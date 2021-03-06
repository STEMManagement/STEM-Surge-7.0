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
using STEM.Sys.Messaging;
using STEM.Sys.IO.TCP;

namespace STEM.Surge.Messages
{
    /// <summary>
    /// A message sent in handshake informing on connection type
    /// </summary>
    public class ConnectionType : STEM.Sys.Messaging.Message
    {
        public delegate void HandshakeComplete(Connection connection);
        public event HandshakeComplete onHandshakeComplete;

        public enum Types { SurgeActor, SurgeBranchManager, SurgeSandbox, SurgeDeploymentManager }

        public Types Type { get; set; }
        
        public ConnectionType()
        {
        }

        public void PerformHandshake(MessageConnection connection)
        {
            STEM.Sys.Global.ThreadPool.RunOnce(new System.Threading.ParameterizedThreadStart(Handshake), connection);
        }

        void Handshake(object o)
        {
            MessageConnection connection = o as MessageConnection;

            STEM.Sys.EventLog.WriteEntry("ConnectionType.Handshake", "Performing handshake with " + connection.RemoteAddress + ".", Sys.EventLog.EventLogEntryType.Information);

            int retry = 0;
            Message response = connection.Send(this, TimeSpan.FromSeconds(10));
            while ((response is Timeout) && (retry < 5))
            {
                STEM.Sys.EventLog.WriteEntry("ConnectionType.Handshake", "Handshake timeout with " + connection.RemoteAddress + ".", Sys.EventLog.EventLogEntryType.Information);
                response = connection.Send(this, TimeSpan.FromSeconds(10));
                retry++;
            }

            if (response is Timeout || response is Undeliverable)
            {
                connection.Close();
                return;
            }

            if (onHandshakeComplete != null)
                try
                {
                    onHandshakeComplete(connection);
                }
                catch { }

            STEM.Sys.EventLog.WriteEntry("ConnectionType.Handshake", "Handshake completed with " + connection.RemoteAddress + ".", Sys.EventLog.EventLogEntryType.Information);
        }
    }
}
