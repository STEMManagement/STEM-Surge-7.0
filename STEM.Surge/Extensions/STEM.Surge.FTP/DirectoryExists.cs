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
using System.IO;
using FluentFTP;

namespace STEM.Surge.FTP
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("DirectoryExists")]
    [Description("Check to see if a directory exists on the FTP server and take a flow action.")]
    public class DirectoryExists : Instruction
    {
        [Category("FTP Server")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }

        [Category("FTP Server")]
        [DisplayName("FTP Server Address"), DescriptionAttribute("What is the FTP Server Address?")]
        public string ServerAddress { get; set; }

        [Category("FTP Server")]
        [DisplayName("FTP Port"), DescriptionAttribute("What is the FTP Port?")]
        public string Port { get; set; }

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
            ServerAddress = "[FtpServerAddress]";
            Port = "[FtpServerPort]";
            DirectoryName = "[TargetPath]";
            TargetLabel = "";
        }

        protected override bool _Run()
        {
            try
            {
                string address = Authentication.NextAddress(ServerAddress);

                if (address == null)
                {
                    Exception ex = new Exception("No valid address. (" + ServerAddress + ")");
                    Exceptions.Add(ex);
                    AppendToMessage(ex.Message);
                    return false;
                }

                Surge.FailureAction tgtAction = DirectoryNotExistsAction;

                FtpClient conn = Authentication.OpenClient(address, Int32.Parse(Port));

                try
                {
                    if (conn.DirectoryExists(Authentication.AdjustPath(address, DirectoryName)))
                        tgtAction = DirectoryExistsAction;

                    switch (tgtAction)
                    {
                        case Surge.FailureAction.SkipRemaining:

                            SkipRemaining();
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
                finally
                {
                    Authentication.RecycleClient(conn);
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
