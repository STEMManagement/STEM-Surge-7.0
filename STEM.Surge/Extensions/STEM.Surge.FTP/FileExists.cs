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
    [DisplayName("FileExists")]
    [Description("Check to see if a file exists on the FTP server and take a flow action.")]
    public class FileExists : Instruction
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

        [DisplayName("FileName")]
        [Description("The file to be evaluated")]
        public string FileName { get; set; }

        [DisplayName("If file exists")]
        [Description("What action should be taken if the file exists?")]
        public Surge.FailureAction FileExistsAction { get; set; }

        [DisplayName("If file does not exist")]
        [Description("What action should be taken if the file does not exist?")]
        public Surge.FailureAction FileNotExistsAction { get; set; }

        public FileExists()
            : base()
        {
            Authentication = new Authentication();
            ServerAddress = "[FtpServerAddress]";
            Port = "[FtpServerPort]";
            FileName = "[TargetPath]\\[TargetName]";
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

                Surge.FailureAction tgtAction = FileNotExistsAction;

                FtpClient conn = Authentication.OpenClient(address, Int32.Parse(Port));

                try
                {
                    if (conn.FileExists(Authentication.AdjustPath(address, FileName)))
                        tgtAction = FileExistsAction;

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
