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
using System.ComponentModel;
using STEM.Surge;

namespace STEM.Surge.FlowControl
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Operating System")]
    [Description("Take Action if Operating System matches selected OS.")]
    public class OperatingSystem : Instruction
    {
        [Category("Flow")]
        [DisplayName("Operating System Type")]
        [Description("The Operating System to check for")]
        public OSType OS { get; set; }

        [Category("Flow")]
        [DisplayName("If OS Matches"), DescriptionAttribute("The action to take if the OS matches the selected Operating System.")]
        public Surge.FailureAction ActionIfMatch { get; set; }

        [Category("Flow")]
        [DisplayName("If OS Does Not Match"), DescriptionAttribute("The action to take if the OS does not match the selected Operating System.")]
        public Surge.FailureAction ActionIfNotMatch { get; set; }

        public OperatingSystem() : base()
        {
            OS = OSType.Windows;
            ActionIfMatch = FailureAction.SkipRemaining;
            ActionIfNotMatch = FailureAction.SkipRemaining;
        }

        protected override bool _Run()
        {
            try
            {
                bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

                Surge.FailureAction tgtAction = ActionIfNotMatch;

                if (isWindows && OS == OSType.Windows)
                    tgtAction = ActionIfMatch;
                
                if (!isWindows && OS == OSType.Linux)
                    tgtAction = ActionIfMatch;

                switch (tgtAction)
                {
                    case Surge.FailureAction.SkipRemaining:

                        SkipRemaining();
                        break;

                    case Surge.FailureAction.SkipNext:

                        SkipNext();
                        break;

                    case Surge.FailureAction.Rollback:

                        RollbackAllPreceedingAndSkipRemaining();
                        break;

                    case Surge.FailureAction.Continue:

                        break;
                }
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.Message);
                Exceptions.Add(ex);
            }

            return Exceptions.Count == 0;
        }

        protected override void _Rollback()
        {
        }
    }
}
