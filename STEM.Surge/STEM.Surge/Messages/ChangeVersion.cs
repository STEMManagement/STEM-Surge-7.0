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
using STEM.Sys.Messaging;

namespace STEM.Surge.Messages
{
    /// <summary>
    /// Message sent from a SurgeActor to a DeploymentManager
    /// </summary>
    public class ChangeVersion : STEM.Sys.Messaging.Message
    {
        public enum TargetType { AuthenticationConfiguration, InstructionSetTemplate, DeploymentController, Static, All }

        public string TypeName { get; set; }
        public string OldVersion { get; set; }
        public string NewVersion { get; set; }
        public string OldAssemblyName { get; set; }
        public string NewAssemblyName { get; set; }        
        public TargetType Target { get; set; }
        public string Filter { get; set; }

        public ChangeVersion()
        {
            TypeName = "STEM.Surge.Compatibility.TokenReplace";
            OldVersion = "7.0.0.0";
            NewVersion = "7.0.1.0";
            OldAssemblyName = "STEM.Surge.Compatibility";
            NewAssemblyName = "STEM.Surge.Compatibility";
            Target = TargetType.All;
            Filter = "*";
        }
    }
}
