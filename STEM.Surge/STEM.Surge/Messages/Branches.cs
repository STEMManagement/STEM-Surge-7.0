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
    /// A list of BranchManagers
    /// </summary>
    public class Branches : STEM.Sys.Messaging.Message
    {
        public class Entry
        {
            public string BranchIP { get; set; }
            public string OSDescription { get; set; }
            public string BranchName { get; set; }
            public int ThreadCount { get; set; }
            public DateTime LastStateReport { get; set; }
            public List<string> ErrorIDs { get; set; }
            public int ProcessorCount { get; set; }
            public int MBRam { get; set; }
            public STEM.Surge.BranchState BranchState { get; set; }
            public List<string> StaticInstructionSets { get; set; }
            public int Assigned { get; set; }
            public int Processing { get; set; }

            public DateTime SurgeBuildDate { get; set; }
            public DateTime SysBuildDate { get; set; }
            public DateTime SurgeInternalBuildDate { get; set; }
            public DateTime SysInternalBuildDate { get; set; }

            public Entry()
            {
                LastStateReport = DateTime.MinValue;
                StaticInstructionSets = new List<string>();
                ErrorIDs = new List<string>();
            }

            internal Entry(_BranchEntry e)
            {
                LastStateReport = DateTime.MinValue;
                StaticInstructionSets = new List<string>();
                ErrorIDs = new List<string>();
                CopyFrom(e);
            }

            public void CopyFrom(Entry e)
            {
                if (LastStateReport > e.LastStateReport)
                    return;

                BranchIP = e.BranchIP;
                OSDescription = e.OSDescription;
                BranchName = e.BranchName;
                ThreadCount = e.ThreadCount;
                LastStateReport = e.LastStateReport;
                MBRam = e.MBRam;
                StaticInstructionSets = e.StaticInstructionSets;
                BranchState = e.BranchState;
                ErrorIDs = e.ErrorIDs.ToList();
                ProcessorCount = e.ProcessorCount;
                SurgeBuildDate = e.SurgeBuildDate;
                SysBuildDate = e.SysBuildDate;
                SurgeInternalBuildDate = e.SurgeInternalBuildDate;
                SysInternalBuildDate = e.SysInternalBuildDate;
            }

            internal void CopyFrom(_BranchEntry e)
            {
                if (LastStateReport > e.LastStateReport)
                    return;

                BranchIP = e.BranchIP;
                OSDescription = e.OSDescription;
                BranchName = e.BranchName;
                ThreadCount = e.Threads;
                LastStateReport = e.LastStateReport;
                MBRam = e.MBRam;
                StaticInstructionSets = e.StaticInstructionSets;
                BranchState = e.BranchState;
                ErrorIDs = e.ErrorIDs.ToList();
                ProcessorCount = e.ProcessorCount;
                Assigned = e.InstructionSetsAssigned;
                Processing = e.InstructionSetsProcessing;
                SurgeBuildDate = e.SurgeBuildDate;
                SysBuildDate = e.SysBuildDate;
                SurgeInternalBuildDate = e.SurgeInternalBuildDate;
                SysInternalBuildDate = e.SysInternalBuildDate;
            }
        }

        public List<Entry> Entries { get; set; }

        public Branches()
        {
            Entries = new List<Entry>();
        }

        public Branches(List<_BranchEntry> branches)
        {
            Entries = new List<Entry>();

            foreach (_BranchEntry e in branches)
            {
                Entries.Add(new Entry(e));
            }
        }
    }
}