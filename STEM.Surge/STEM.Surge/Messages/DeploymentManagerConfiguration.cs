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
using System.IO;
using STEM.Sys.IO;

namespace STEM.Surge.Messages
{
    /// <summary>
    /// A message sent from a DeploymentManager to a SurgeActor with the configuration
    /// Also shared between DeploymentManagers that are coordinating work
    /// Also can be edited by a SurgeActor and sent back to a DeploymentManager to affect a configuration update
    /// </summary>
    public class DeploymentManagerConfiguration : STEM.Sys.Messaging.Message
    {
        public bool Update(DeploymentManagerConfiguration candidate)
        {
            lock (this)
            {
                DateTime lastUpdateC = candidate.LastUpdate;
                DateTime lastUpdateT = this.LastUpdate;

                try
                {
                    candidate.LastUpdate = DateTime.MinValue;
                    this.LastUpdate = DateTime.MinValue;

                    string orig = this.Serialize();

                    DeploymentManagerConfiguration test = DeploymentManagerConfiguration.Deserialize(orig) as DeploymentManagerConfiguration;

                    test.SwitchboardConfigurationDescription = candidate.SwitchboardConfigurationDescription;
                    test.AuthenticationConfigurations = candidate.AuthenticationConfigurations;
                    test.DeploymentControllers = candidate.DeploymentControllers;
                    test.InstructionSetTemplates = candidate.InstructionSetTemplates;
                    test.AdHocInstructionSets = candidate.AdHocInstructionSets;
                    test.InstructionSetStatics = candidate.InstructionSetStatics;

                    if (orig == test.Serialize())
                        return false;

                    STEM.Sys.EventLog.WriteEntry("DeploymentManagerConfiguration.Update", "Update applied.", STEM.Sys.EventLog.EventLogEntryType.Information);
                    
                    SwitchboardConfigurationDescription = candidate.SwitchboardConfigurationDescription;

                    AuthenticationConfigurations.Clear();
                    AuthenticationConfigurations.AddRange(candidate.AuthenticationConfigurations);

                    DeploymentControllers.Clear();
                    DeploymentControllers.AddRange(candidate.DeploymentControllers);

                    InstructionSetTemplates.Clear();
                    InstructionSetTemplates.AddRange(candidate.InstructionSetTemplates);
                    
                    AdHocInstructionSets.Clear();
                    AdHocInstructionSets.AddRange(candidate.AdHocInstructionSets);

                    foreach (string sub in candidate.InstructionSetStatics.Keys)
                    {
                        InstructionSetStatics[sub].Clear();
                        InstructionSetStatics[sub].AddRange(candidate.InstructionSetStatics[sub]);
                    }
                    
                    if (orig != this.Serialize())
                    {
                        if (lastUpdateC > DateTime.MinValue)
                            LastUpdate = lastUpdateC;
                        else
                            LastUpdate = DateTime.UtcNow;

                        return true;
                    }
                    else
                    {
                        LastUpdate = lastUpdateT;
                        if (LastUpdate < lastUpdateC)
                            LastUpdate = lastUpdateC;

                        return false;
                    }
                }
                finally
                {
                    if (LastUpdate == DateTime.MinValue)
                        LastUpdate = lastUpdateT;

                    if (LastUpdate == DateTime.MinValue)
                        LastUpdate = DateTime.UtcNow;
                }
            }
        }

        DateTime _LastConfigTime = DateTime.MinValue;
        SwitchboardConfig _SwitchboardConfiguration = null;
        public SwitchboardConfig SwitchboardConfiguration 
        {
            get
            {
                lock (SwitchboardConfigurationDescription)
                {
                    if (_LastConfigTime != SwitchboardConfigurationDescription.LastWriteTimeUtc)
                    {
                        _SwitchboardConfiguration = new SwitchboardConfig();
                        _LastConfigTime = SwitchboardConfigurationDescription.LastWriteTimeUtc;
                        _SwitchboardConfiguration.ReadXml(new StringReader(SwitchboardConfigurationDescription.StringContent));
                    }
                }

                return _SwitchboardConfiguration;
            }
        }
        
        public FileDescription SwitchboardConfigurationDescription { get; set; }
                
        public int DaysRemaining { get; set; }
        public int AllowableBranches { get; set; }
        public string Keys { get; set; }
        public bool IsSES { get; set; }

        public DateTime BuildDate { get; set; }

        public List<FileDescription> AuthenticationConfigurations { get; set; }
        public List<FileDescription> DeploymentControllers { get; set; }
        public List<FileDescription> InstructionSetTemplates { get; set; }
        public List<FileDescription> AdHocInstructionSets { get; set; }

        public STEM.Sys.Serialization.Dictionary<string, List<FileDescription>> InstructionSetStatics { get; set; }
        
        public List<FileDescription> HistoricalDescriptions { get; set; }

        public DateTime LastUpdate { get; set; }

        public DeploymentManagerConfiguration()
        {
            AuthenticationConfigurations = new List<FileDescription>();
            DeploymentControllers = new List<FileDescription>();
            InstructionSetTemplates = new List<FileDescription>();
            AdHocInstructionSets = new List<FileDescription>();
            InstructionSetStatics = new Sys.Serialization.Dictionary<string, List<FileDescription>>();
            HistoricalDescriptions = new List<FileDescription>();

            SwitchboardConfigurationDescription = new FileDescription();
            LastUpdate = DateTime.MinValue;
        }
    }
}
