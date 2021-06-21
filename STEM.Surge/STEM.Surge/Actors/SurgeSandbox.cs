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
using System.Linq;
using STEM.Sys.Messaging;
using STEM.Surge.Messages;
using STEM.Sys.IO.TCP;
using System.Security.Cryptography.X509Certificates;

namespace STEM.Surge
{
    public class SurgeSandbox : SurgeBranchManager
    {
        public SurgeSandbox(int communicationPort, string postMortemCache)
            : base(null, communicationPort, postMortemCache, false, null, true)
        {
        }

        protected override void onOpened(Connection connection)
        {
            lock (ConnectionLock)
            {
                MessageConnection c = connection as MessageConnection;
                if (c != null)
                    ManageConnection(c);
                
                SandboxConnectionType m = new SandboxConnectionType { Type = ConnectionType.Types.SurgeSandbox, SandboxID = STEM.Sys.IO.Path.GetFileName(System.Environment.CurrentDirectory) };
                m.onHandshakeComplete += SandboxConnectionType_onHandshakeComplete;
                m.PerformHandshake(c);
            }
        }

        private void SandboxConnectionType_onHandshakeComplete(ConnectionType sender, Connection connection)
        {
            STEM.Sys.Global.ThreadPool.BeginAsync(new System.Threading.ThreadStart(Timeout), TimeSpan.FromSeconds(30));
        }

        protected override void onClosed(Connection connection)
        {
            System.Diagnostics.Process self = System.Diagnostics.Process.GetCurrentProcess();

            self.Kill();
        }

        void Timeout()
        {
            if ((DateTime.UtcNow - _LastAssignment).TotalMinutes > 5.0)
                if (_Assigned.Count(i => !i.IsStatic) == 0)
                {
                    System.Diagnostics.Process self = System.Diagnostics.Process.GetCurrentProcess();
                    self.Kill();
                }
        }

        DateTime _LastAssignment = DateTime.UtcNow;

        protected override void onReceived(MessageConnection connection, Message message)
        {
            if (message is AssignInstructionSet)
                _LastAssignment = DateTime.UtcNow;

            base.onReceived(connection, message);
        }
    }
}
