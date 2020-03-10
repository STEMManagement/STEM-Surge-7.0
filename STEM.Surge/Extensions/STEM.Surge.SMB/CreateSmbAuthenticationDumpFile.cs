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
using System.Xml.Serialization;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using STEM.Sys.Security;

namespace STEM.Surge.SMB
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("CreateSmbAuthenticationDumpFile")]
    [Description("Create a dump file of all process wide SMB share accesses.")]
    public class CreateSmbAuthenticationDumpFile : Instruction
    {
        [DisplayName("Output File"), DescriptionAttribute("The file in which to write the dump.")]
        public string OutputFile { get; set; }

        public CreateSmbAuthenticationDumpFile()
            : base()
        {
            OutputFile = @"D:\netuse.dmp";
        }

        protected override void _Rollback()
        {
            // No Rollback
        }

        bool _Exited = false;
        protected override bool _Run()
        {
            try
            {
                bool isWindows = STEM.Sys.Control.IsWindows;

                if (!isWindows)
                    return true;

                ProcessStartInfo si = new ProcessStartInfo();
                si.CreateNoWindow = true;
                si.UseShellExecute = false;
                si.RedirectStandardError = true; 
                si.RedirectStandardOutput = true;

                si.FileName = @"C:\windows\system32\cmd";
                si.Arguments = "/c net use ";

                Process p = new Process();
                p.StartInfo = si;
                p.EnableRaisingEvents = true;
                p.Exited += Exited;

                p.Start();

                while (!_Exited)
                    System.Threading.Thread.Sleep(10);

                string dump = p.StandardOutput.ReadToEnd();

                if (File.Exists(OutputFile))
                    File.Delete(OutputFile);

                File.WriteAllText(OutputFile, dump);
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
                return false;
            }

            return Exceptions.Count == 0;
        }
        void Exited(object sender, EventArgs e)
        {
            _Exited = true;
        }
    }
}