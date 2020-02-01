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
using STEM.Sys.IO.TCP;
using STEM.Sys.Messaging;

namespace STEM.Surge
{
    /// <summary>
    /// This is the opensource base class for STEM.Surge.Internal BranchEntry
    /// The internal class simply performs bookkeeping on DeploymentDetails objects assigned to a BranchManager
    /// virtual methods may be added in the future as opportunities for opensource developers to customize
    /// </summary>
    public abstract class _BranchEntry
    {
        public _BranchEntry()
        {
        }

        /// <summary>
        /// IPV4 address of this branch
        /// </summary>
        public abstract string BranchIP { get; }

        /// <summary>
        /// OS Description of this branch
        /// </summary>
        public abstract string OSDescription { get; }

        /// <summary>
        /// Hostname of this branch
        /// </summary>
        public abstract string BranchName { get; }

        /// <summary>
        /// State of this branch
        /// </summary>
        public abstract STEM.Surge.BranchState BranchState { get; }

        /// <summary>
        /// InstructionSet IDs of current errors (includes sandboxes)
        /// </summary>
        public abstract List<string> ErrorIDs { get; }

        /// <summary>
        /// Number of threads in the main Surge Branch service
        /// </summary>
        public abstract int Threads { get; }

        /// <summary>
        /// MB Ram used by the main Surge Branch service
        /// </summary>
        public abstract int MBRam { get; }

        /// <summary>
        /// The number of processors on the server * ProcessorOverload
        /// </summary>
        public abstract int ProcessorCount { get; }

        /// <summary>
        /// The names of the Static InstructionSets running on this Branch
        /// </summary>
        public abstract List<string> StaticInstructionSets { get; }

        /// <summary>
        /// The last time (UTC) a Branch state was received
        /// </summary>
        public abstract DateTime LastStateReport { get; }

        /// <summary>
        /// The last time (UTC) that an InstructionSet was issued (includes sandboxes)
        /// </summary>
        public abstract DateTime LastDeployment { get; }

        /// <summary>
        /// The number of InstructionSets assigned to this Branch (includes sandboxes)
        /// </summary>
        public abstract int InstructionSetsAssigned { get; }

        /// <summary>
        /// The number of assigned InstructionSets actively processing on this Branch (includes sandboxes)
        /// </summary>
        public abstract int InstructionSetsProcessing { get; }

        /// <summary>
        /// The build dateTime of the STEM.Surge.dll
        /// </summary>
        public abstract DateTime SurgeBuildDate { get; }

        /// <summary>
        /// The build dateTime of the STEM.Sys.dll
        /// </summary>
        public abstract DateTime SysBuildDate { get; }
        
        /// <summary>
        /// The build dateTime of the STEM.Surge.Internal.dll
        /// </summary>
        public abstract DateTime SurgeInternalBuildDate { get; }
        
        /// <summary>
        /// The build dateTime of the STEM.Sys.Internal.dll
        /// </summary>
        public abstract DateTime SysInternalBuildDate { get; }

        /// <summary>
        /// The current InstructionSet load level of this Branch (includes sandboxes)
        /// </summary>
        /// <param name="switchboardRowID">Optional arg to focus on a specific load level</param>
        /// <returns>Number of active InstructionSets loaded on the Branch</returns>
        public abstract int CurrentLoad(string switchboardRowID = null);
        
        /// <summary>
        /// Disassociate an assignable InitiationSource with this Branch
        /// Only used by InitiationSourceLockOwner
        /// </summary>
        /// <param name="lockOwner"></param>
        public abstract void Remove(InitiationSourceLockOwner lockOwner);
        
        /// <summary>
        /// Associate an assignable InitiationSource with this Branch
        /// Only used by InitiationSourceLockOwner
        /// </summary>
        /// <param name="lockOwner"></param>
        public abstract void Add(InitiationSourceLockOwner lockOwner);

        /// <summary>
        /// This is implemented in STEM.Surge.Internal.BranchEntry and defined here in case an opensource developer wants to 
        /// handle messages received from BranchManagers
        /// </summary>
        /// <param name="connection">The message connection to the BranchManager sending this message</param>
        /// <param name="message">The received message</param>
        /// <returns>True if the derived BranchEntry class is to handle the message as well, False if the derived class is to ignore this message</returns>
        protected virtual bool Receive(MessageConnection connection, Message message)
        {
            return true;
        }
    }
}
