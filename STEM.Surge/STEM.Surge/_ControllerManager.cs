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

using System.Collections.Generic;
using STEM.Sys.Threading;
using STEM.Sys.State;

namespace STEM.Surge
{
    /// <summary>
    /// This is the opensource base class for STEM.Surge.Internal ControllerManager
    /// The internal class manages a specific DeploymentController instance, calling ListPreprocess(), walking the resulting list, and calling GenerateDeploymentDetails()
    /// 
    /// This is a stub for now, virtual methods may be added in the future as opportunities for opensource developers to further customize
    /// </summary>
    public abstract class _ControllerManager : IThreadable
    {
        /// <summary>
        /// The phases of a DeploymentControllers life
        /// </summary>
        public enum ExecutionPhase { Active, Disposing, Idle, UnInitialized }

        /// <summary>
        /// The ID generated from GenerateSwitchboardRowIDs.GenerateSwitchboardRowID() for this row
        /// </summary>
        public abstract string SwitchboardRowID { get; }

        /// <summary>
        /// A unique string describing this controller instance 
        /// </summary>
        public abstract string DeploymentControllerDescription { get; }

        /// <summary>
        /// The ID generated from GenerateSwitchboardRowIDs.GenerateDeploymentControllerID() for this row
        /// </summary>
        public abstract string DeploymentControllerID { get; }

        /// <summary>
        /// The name of this Authentication Configuration
        /// </summary>
        public abstract string AuthenticationConfiguration { get; }

        /// <summary>
        /// The name of this Deployment Controller
        /// </summary>
        public abstract string DeploymentController { get; }

        /// <summary>
        /// IPV4 addresses of other Deployment Managers performing this deployment
        /// </summary>
        public abstract List<string> CoordinateWith { get; }

        /// <summary>
        /// A list optionally used by controllers to order assignments (mostly used by ListPreprocess)
        /// </summary>
        public abstract List<string> PriorityFilters { get; }

        /// <summary>
        /// The CoordinatedKeyManager used to obtain locks for this assignment
        /// </summary>
        public abstract CoordinatedKeyManager KeyManager { get; }

        /// <summary>
        /// The IAuthentication instance being used in active listings
        /// </summary>
        public abstract STEM.Sys.IO.Listing.IAuthentication ValidatedAuth { get; }

        /// <summary>
        /// The DeploymentController instance being used in active deployment
        /// </summary>
        public abstract _DeploymentController ValidatedController { get; }

        /// <summary>
        /// The current phase of this DeploymentController in its lifespan
        /// </summary>
        public abstract ExecutionPhase CurrentPhase { get; protected set; }
    }
}