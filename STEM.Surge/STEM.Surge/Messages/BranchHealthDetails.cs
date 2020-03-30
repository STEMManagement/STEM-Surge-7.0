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
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using STEM.Sys.Messaging;

namespace STEM.Surge.Messages
{    
    /// <summary>
    /// This message is sent from BranchManagers to DeploymentManagers informing on current health details
    /// </summary>
    public class BranchHealthDetails : Message
    {
        public string BranchIP { get; set; }
        public string BranchName { get; set; }
        public string OSDescription { get; set; }
        public int ThreadCount { get; set; }
        public int ErrorCount { get; set; }
        public int ProcessorCount { get; set; }
        public double ProcessorOverload { get; set; }
        public int MBRam { get; set; }
        public STEM.Surge.BranchState BranchState { get; set; }
        public List<string> LoadedInstructionSets { get; set; }
        public List<string> StaticInstructionSets { get; set; }
        public DateTime SurgeBuildDate { get; set; }
        public DateTime SysBuildDate { get; set; }
        public DateTime SurgeInternalBuildDate { get; set; }
        public DateTime SysInternalBuildDate { get; set; }
        public DateTime GenerationTime { get; set; }
        public int MessageBacklog { get; set; }

        public BranchHealthDetails()
        {
            MessageBacklog = 0;

            BranchIP = STEM.Sys.IO.Net.MachineIP();
            BranchName = STEM.Sys.IO.Net.MachineName();
            OSDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription;

            System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
            ThreadCount = p.Threads.Count;
            MBRam = (int)(p.WorkingSet64 / 1048576);

            foreach (string file in Directory.GetFiles(System.Environment.CurrentDirectory, "STEM.*"))
            {
                if (STEM.Sys.IO.Path.GetFileName(file).StartsWith("STEM.Sys.Internal.", StringComparison.InvariantCultureIgnoreCase))
                {
                    SysInternalBuildDate = System.IO.File.GetLastWriteTimeUtc(file);
                }
                else if (STEM.Sys.IO.Path.GetFileName(file).StartsWith("STEM.Sys.", StringComparison.InvariantCultureIgnoreCase))
                {
                    SysBuildDate = System.IO.File.GetLastWriteTimeUtc(file);
                }
                else if (STEM.Sys.IO.Path.GetFileName(file).StartsWith("STEM.Surge.Internal.", StringComparison.InvariantCultureIgnoreCase))
                {
                    SurgeInternalBuildDate = System.IO.File.GetLastWriteTimeUtc(file);
                }
                else if (STEM.Sys.IO.Path.GetFileName(file).StartsWith("STEM.Surge.", StringComparison.InvariantCultureIgnoreCase))
                {
                    SurgeBuildDate = System.IO.File.GetLastWriteTimeUtc(file);
                }
            }

            ProcessorOverload = 1;
            LoadedInstructionSets = new List<string>();
            StaticInstructionSets = new List<string>();
            BranchState = STEM.Surge.BranchState.Online;
            ErrorCount = 0;
            ProcessorCount = (int)(STEM.Sys.Threading.ThreadPool.ProcessorOverload * Environment.ProcessorCount);

            if (ProcessorCount < 1)
                ProcessorCount = 1;

            GenerationTime = DateTime.UtcNow;
        }

        public void Refresh(List<string> loaded, List<string> statics, string errorDir)
        {
            BranchIP = STEM.Sys.IO.Net.MachineIP();
            OSDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription;

            System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
            ThreadCount = p.Threads.Count;
            MBRam = (int)(p.WorkingSet64 / 1048576);
                        
            LoadedInstructionSets = loaded;
            StaticInstructionSets = statics;

            ProcessorCount = (int)(STEM.Sys.Threading.ThreadPool.ProcessorOverload * Environment.ProcessorCount);

            if (ProcessorCount < 1)
                ProcessorCount = 1;

            ErrorCount = 0;
            if (Directory.Exists(errorDir))
                ErrorCount = Directory.GetFiles(errorDir).Count();

            GenerationTime = DateTime.UtcNow;
        }
    }
}

