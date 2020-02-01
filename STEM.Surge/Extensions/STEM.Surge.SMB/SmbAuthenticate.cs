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
using STEM.Sys.Security;
using STEM.Sys.Threading;

namespace STEM.Surge.SMB
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("SmbAuthenticate")]
    [Description("Perform a process wide net use to an SMB share or set of shares.")]
    public class SmbAuthenticate : Instruction
    {
        [DisplayName("FileShares to authenticate to"), DescriptionAttribute("This is a list of share roots (1 per line)")]
        public List<string> FileShares { get; set; }
        
        [DisplayName("Domain"), DescriptionAttribute("The Domain of the user")]
        public string Domain { get; set; }

        [DisplayName("User"), DescriptionAttribute("The user name")]
        public string User { get; set; }

        [XmlIgnore]
        [PasswordPropertyText(true)]
        [DisplayName("Password"), DescriptionAttribute("The user password")]
        public string Password { get; set; }
        [Browsable(false)]
        public string PasswordEncoded
        {
            get
            {
                return this.Entangle(Password);
            }

            set
            {
                Password = this.Detangle(value);
            }
        }
        
        [DisplayName("Pingable"), DescriptionAttribute("Are the remote computers pingable.")]
        public bool Pingable { get; set; }

        public SmbAuthenticate()
            : base()
        {
            FileShares = new List<string>();
            Domain = ".";
            User = "Username";
            Password = "Password";
            Pingable = true;
        }

        protected override void _Rollback()
        {
            // No Rollback
        }

        protected override bool _Run()
        {
            try
            {
                bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

                if (!isWindows)
                    return true;

                List<IThreadable> threads = new List<IThreadable>();

                foreach (string s in FileShares)
                    foreach (string share in STEM.Sys.IO.Path.ExpandRangedPath(s))
                    {
                        string machineName = STEM.Sys.IO.Path.ChangeIpToMachineName(share);

                        if (machineName != share)
                            threads.Add(new NetUse { FileShare = machineName, Domain = this.Domain, User = this.User, Password = this.Password, Pingable = this.Pingable, Exceptions = this.Exceptions });
                     
                        threads.Add(new NetUse { FileShare = share, Domain = this.Domain, User = this.User, Password = this.Password, Pingable = this.Pingable, Exceptions = this.Exceptions });
                    }

                if (threads.Count == 1)
                {
                    ((NetUse)threads[0]).DirectCall();
                }
                else
                {
                    threads = _Pool.ExecuteBatch(threads, TimeSpan.FromMinutes(5));

                    foreach (IThreadable i in threads)
                    {
                        NetUse n = i as NetUse;
                        Exceptions.Add(new Exception("net use " + n.FileShare + " / user:" + n.Domain + "\\" + n.User + " execution timeout."));
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
                return false;
            }

            foreach (Exception ex in Exceptions)
            {
                STEM.Sys.EventLog.WriteEntry(InstructionSet.ProcessName, ex, Sys.EventLog.EventLogEntryType.Error);
            }

            return Exceptions.Count == 0;
        }
        
        static ThreadPool _Pool = new ThreadPool(Environment.ProcessorCount);

        class NetUse : STEM.Sys.Threading.IThreadable
        {
            public string FileShare { get; set; }
            public string Domain { get; set; }
            public string User { get; set; }
            public string Password { get; set; }
            public bool Pingable { get; set; }

            public List<Exception> Exceptions { get; set; }

            bool _Exited = false;

            public void DirectCall()
            {
                Execute(null);
            }

            Process _Process = null;

            protected override void Execute(ThreadPool owner)
            {
                try
                {
                    if (Pingable)
                    {
                        if (!STEM.Sys.IO.Net.PingHost(STEM.Sys.IO.Path.IPFromPath(FileShare)))
                            throw new Exception("Ping failure.");
                    }

                    ProcessStartInfo si = new ProcessStartInfo();
                    si.CreateNoWindow = true;
                    si.UseShellExecute = false;
                    si.RedirectStandardError = true;

                    si.FileName = @"C:\windows\system32\cmd";

                    if (String.IsNullOrEmpty(Domain) || Domain == ".")
                        si.Arguments = "/c net use " + FileShare + " /user:" + User + " " + Password;
                    else
                        si.Arguments = "/c net use " + FileShare + " /user:" + Domain + "\\" + User + " " + Password;

                    _Process = new Process();
                    _Process.StartInfo = si;
                    _Process.EnableRaisingEvents = true;
                    _Process.Exited += Exited;

                    _Process.Start();

                    while (!_Exited)
                        System.Threading.Thread.Sleep(10);

                    string err = _Process.StandardError.ReadToEnd();

                    if (err != "")
                        throw new Exception(err);
                }
                catch (Exception ex)
                {
                    lock (Exceptions)
                        Exceptions.Add(new Exception("net use " + FileShare + " / user:" + Domain + "\\" + User, ex));
                }
            }

            protected override void Dispose(bool dispose)
            {
                base.Dispose(dispose);

                if (_Exited == false)
                {
                    try
                    {
                        _Process.Kill();
                    }
                    catch { }

                    Exceptions.Add(new Exception("net use " + FileShare + " / user:" + Domain + "\\" + User + " execution timeout."));
                }
            }

            void Exited(object sender, EventArgs e)
            {
                _Exited = true;
            }
        }
    }
}
