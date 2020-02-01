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
    
    public class InstructionMessage : STEM.Sys.Messaging.Message
    {
        public Guid InstructionSetID { get; set; }
        public string DeploymentControllerID { get; set; }

        public InstructionMessage()
        {
            InstructionSetID = Guid.NewGuid();
            DeploymentControllerID = "";
        }

        public InstructionMessage(Guid instructionSetID, string deploymentControllerID)
            : base()
        {
            InstructionSetID = instructionSetID;
            DeploymentControllerID = deploymentControllerID;
        }

        public InstructionMessage(_InstructionSet iSet)
            : base()
        {
            if (iSet != null)
            {
                InstructionSetID = iSet.ID;
                DeploymentControllerID = iSet.DeploymentControllerID;
            }
        }

        public override void CopyFrom(Message source)
        {
            InstructionSetID = ((InstructionMessage)source).InstructionSetID;
            DeploymentControllerID = ((InstructionMessage)source).DeploymentControllerID;
        }
    }
}
