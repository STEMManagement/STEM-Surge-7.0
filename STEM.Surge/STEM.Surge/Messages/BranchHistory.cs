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
using STEM.Sys.Messaging;

namespace STEM.Surge.Messages
{
    /// <summary>
    /// This message is shared between DeploymentManagers coordinating together
    /// </summary>
    public class BranchHistory : STEM.Sys.Messaging.Message
    {
        public class Entry
        {
            public string BranchP { get; set; }
            public DateTime LastHeard { get; set; }
            public double ProcessorOverload { get; set; }

            public Entry()
            {
                LastHeard = DateTime.MinValue;
                ProcessorOverload = 1;
            }

            public void CopyFrom(Entry e)
            {
                BranchP = e.BranchP;

                if (LastHeard < e.LastHeard)
                    LastHeard = e.LastHeard;
            }
        }

        public List<Entry> Entries { get; set; }

        public BranchHistory()
        {
            Entries = new List<Entry>();
        }

        public void CopyFrom(BranchHistory e)
        {
            foreach (Entry n in e.Entries)
            {
                Entry o = Entries.FirstOrDefault(i => i.BranchP == n.BranchP);
                if (o != null)
                    o.CopyFrom(n);
                else
                    Entries.Add(n);
            }
        }
    }
}