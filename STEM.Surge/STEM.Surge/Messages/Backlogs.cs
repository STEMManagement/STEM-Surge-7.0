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

namespace STEM.Surge.Messages
{    
    /// <summary>
    /// A list of current backlogs
    /// </summary>
    public class Backlogs : STEM.Sys.Messaging.Message
    {
        public class Entry
        {
            public string SwitchboardRowID { get; set; }
            public string DeploymentControllerID { get; set; }
            public string DeploymentManagerIP { get; set; }
            public string SwitchboardRowDescription { get; set; }
            public string DeploymentControllerDescription { get; set; }
            public string DeploymentController { get; set; }
            public bool Enabled { get; set; }
            public int MaxBranchLoad { get; set; }
            public DateTime LastDataActivity { get; set; }
            public DateTime LastPoll { get; set; }
            public DateTime LastAssignmentEnd { get; set; }
            public DateTime LastAssignment { get; set; }
            public DateTime LastWalkStart { get; set; }
            public bool Assigning { get; set; }
            public int BacklogCount { get; set; }
            public int PerceivedBacklogCount { get; set; }
            public int Assigned { get; set; }
            public int Processing { get; set; }
            public bool PollFailure { get; set; }
            public bool PingFailure { get; set; }
            public string PollError { get; set; }
            public bool ControllerLoadError { get; set; }
            public string ListWalkSummary { get; set; }

            public Entry()
            {
                LastDataActivity = DateTime.UtcNow;
                LastWalkStart = DateTime.MinValue;
            }            
        }

        public List<Entry> Entries { get; set; }

        public Backlogs()
        {
            Entries = new List<Entry>();
        }
    }
}