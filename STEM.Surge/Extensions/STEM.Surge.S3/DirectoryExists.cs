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
using System.ComponentModel;

namespace STEM.Surge.S3
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("DirectoryExists")]
    [Description("Check to see if a directory exists in the S3 bucket and take a flow action.")]
    public class DirectoryExists : Instruction
    {
        [Category("S3")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }
        
        [DisplayName("DirectoryName")]
        [Description("The directory to be evaluated")]
        public string DirectoryName { get; set; }

        [DisplayName("If directory exists")]
        [Description("What action should be taken if the directory exists?")]
        public Surge.FailureAction DirectoryExistsAction { get; set; }

        [DisplayName("If directory does not exist")]
        [Description("What action should be taken if the directory does not exist?")]
        public Surge.FailureAction DirectoryNotExistsAction { get; set; }

        [DisplayName("Target Label")]
        [Description("The label to skip forward to when Action == SkipToLabel")]
        public string TargetLabel { get; set; }

        public DirectoryExists()
            : base()
        {
            Authentication = new Authentication();
            DirectoryName = "[TargetPath]";
            TargetLabel = "";
        }

        protected override bool _Run()
        {
            try
            {
                Surge.FailureAction tgtAction = DirectoryNotExistsAction;
                
                if (Authentication.DirectoryExists(DirectoryName))
                    tgtAction = DirectoryExistsAction;

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

                    case Surge.FailureAction.SkipToLabel:

                        SkipForwardToFlowControlLabel(TargetLabel);
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
