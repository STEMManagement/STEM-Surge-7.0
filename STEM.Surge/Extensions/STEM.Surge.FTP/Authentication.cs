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
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using System.Net;
using FluentFTP;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using STEM.Sys.Security;

namespace STEM.Surge.FTP
{    
    public enum FTPListType { File, Directory, All }

    public class Authentication : IAuthentication
    {
        [Category("FTP Server")]
        [DisplayName("FTP User"), DescriptionAttribute("What is the FTP user?")]
        public string FTPUser { get; set; }

        [Category("FTP Server")]
        [DisplayName("FTP Password"), DescriptionAttribute("What is the FTP password?")]
        [XmlIgnore]
        [PasswordPropertyText(true)]
        public string FTPPassword { get; set; }
        [Browsable(false)]
        public string FTPPasswordEncoded
        {
            get
            {
                return this.Entangle(FTPPassword);
            }

            set
            {
                FTPPassword = this.Detangle(value);
            }
        }

        [Category("FTP Server")]
        [DisplayName("Use TLS"), DescriptionAttribute("Should a TLS connection be used?")]
        public bool UseTLS { get; set; }

        [Category("FTP Server")]
        [DisplayName("Require Certificate Validation"), DescriptionAttribute("Should a server certificate be required?")]
        public bool RequireCertificateValidation { get; set; }

        public Authentication()
        {
            RequireCertificateValidation = false;
            UseTLS = false;
        }

        static Dictionary<string, Queue<FtpClient>> _FtpClients = new Dictionary<string, Queue<FtpClient>>();
        static Dictionary<string, STEM.Sys.State.GrabBag<string>> _ServerAddresses = new Dictionary<string, Sys.State.GrabBag<string>>(StringComparer.InvariantCultureIgnoreCase);

        public static string NextAddress(string rangedAddress)
        {
            if (!_ServerAddresses.ContainsKey(rangedAddress))
                lock (_ServerAddresses)
                    if (!_ServerAddresses.ContainsKey(rangedAddress))
                        _ServerAddresses[rangedAddress] = new Sys.State.GrabBag<string>(STEM.Sys.IO.Path.ExpandRangedIP(rangedAddress), rangedAddress);

            return _ServerAddresses[rangedAddress].Next();
        }

        public static void SuspendAddress(string rangedAddress, string suspendAddress)
        {
            if (!_ServerAddresses.ContainsKey(rangedAddress))
                lock (_ServerAddresses)
                    if (!_ServerAddresses.ContainsKey(rangedAddress))
                        _ServerAddresses[rangedAddress] = new Sys.State.GrabBag<string>(STEM.Sys.IO.Path.ExpandRangedIP(rangedAddress), rangedAddress);

            _ServerAddresses[rangedAddress].Suspend(suspendAddress);
        }

        public static void ResumeAddress(string rangedAddress, string resumeAddress)
        {
            if (!_ServerAddresses.ContainsKey(rangedAddress))
                lock (_ServerAddresses)
                    if (!_ServerAddresses.ContainsKey(rangedAddress))
                        _ServerAddresses[rangedAddress] = new Sys.State.GrabBag<string>(STEM.Sys.IO.Path.ExpandRangedIP(rangedAddress), rangedAddress);

            _ServerAddresses[rangedAddress].Resume(resumeAddress);
        }

        public FtpClient OpenClient(string server, int port)
        {
            lock (_FtpClients)
            {
                string clientKey = server + ":" + port + ":" + FTPUser + ":" + FTPPassword;

                if (!_FtpClients.ContainsKey(clientKey))
                    _FtpClients[clientKey] = new Queue<FtpClient>();

                FtpClient conn = null;

                if (_FtpClients[clientKey].Count > 0)
                    conn = _FtpClients[clientKey].Dequeue();

                if (conn != null)
                {
                    if (!conn.IsConnected)
                        conn = OpenClient(server, port);

                    if (conn != null)
                        return conn;
                }

                conn = new FtpClient();

                conn.Host = server;
                conn.Port = port;
                conn.Credentials = new NetworkCredential(FTPUser, FTPPassword);

                if (RequireCertificateValidation)
                {
                    conn.ValidateCertificate += RefuseInvalid;
                }
                else
                {
                    conn.ValidateCertificate += AcceptInvalid;
                }

                if (UseTLS)
                {
                    conn.EncryptionMode = FtpEncryptionMode.Explicit;
                    conn.SslProtocols = SslProtocols.None;
                }

                conn.Connect();

                return conn;
            }
        }

        public void RecycleClient(FtpClient client)
        {
            try
            {
                if (client == null)
                    return;

                //if (!client.IsConnected)
                //    return;

                lock (_FtpClients)
                {
                    string clientKey = client.Host + ":" + client.Port + ":" + FTPUser + ":" + FTPPassword;

                    if (!_FtpClients.ContainsKey(clientKey))
                        _FtpClients[clientKey] = new Queue<FtpClient>();

                    //if (_FtpClients[clientKey].Count < 20)
                    //{
                    //    _FtpClients[clientKey].Enqueue(client);
                    //    return;
                    //}

                    try
                    {
                        client.Disconnect();
                    }
                    catch { }

                    try
                    {
                        client.Dispose();
                    }
                    catch { }
                }
            }
            catch { }
        }

        private static void RefuseInvalid(FtpClient control, FtpSslValidationEventArgs e)
        {
            if (e.PolicyErrors != System.Net.Security.SslPolicyErrors.None)
            {
                e.Accept = false;
            }
        }
        private static void AcceptInvalid(FtpClient control, FtpSslValidationEventArgs e)
        {
            if (e.PolicyErrors != System.Net.Security.SslPolicyErrors.None)
            {
                e.Accept = true;
            }
        }

        protected override void Dispose(bool dispose)
        {
            
        }

        public List<FtpListItem> ListDirectory(string server, int port,
            string directory, FTPListType listType, bool recurse, string directoryFilter, string fileFilter)
        {
            directory = AdjustPath(server, directory);

            List <FtpListItem> ret = new List<FtpListItem>();
            List<string> directoriesListed = new List<string>();

            Regex exclusiveFileFilters = STEM.Sys.IO.Path.BuildExclusiveFilter(fileFilter);
            Regex inclusiveFileFilters = STEM.Sys.IO.Path.BuildInclusiveFilter(fileFilter);
            Regex exclusiveDirectoryFilters = STEM.Sys.IO.Path.BuildExclusiveFilter(directoryFilter);
            Regex inclusiveDirectoryFilters = STEM.Sys.IO.Path.BuildInclusiveFilter(directoryFilter);

            ListDirectory(server, port,
                ret, directoriesListed,
                directory, listType, recurse, 
                exclusiveDirectoryFilters, inclusiveDirectoryFilters,
                exclusiveFileFilters, inclusiveFileFilters);

            return ret;
        }

        void ListDirectory(string server, int port,
            List<FtpListItem> ftpListItems, List<string> directoriesListed,
            string directory, FTPListType listType, bool recurse,
            Regex exclusiveDirectoryFilters, Regex inclusiveDirectoryFilters,
            Regex exclusiveFileFilters, Regex inclusiveFileFilters)
        {
            FtpClient conn = OpenClient(server, port);

            try
            {
                foreach (FtpListItem i in conn.GetListing(directory, FtpListOption.Modify | FtpListOption.Size | FtpListOption.DerefLinks | FtpListOption.AllFiles))
                {
                    switch (i.Type)
                    {
                        case FtpFileSystemObjectType.Directory:

                            if (exclusiveDirectoryFilters != null && exclusiveDirectoryFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.FullName).ToUpper()))
                                continue;

                            if (inclusiveDirectoryFilters != null && !inclusiveDirectoryFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.FullName).ToUpper()))
                                continue;

                            if (listType == FTPListType.Directory || listType == FTPListType.All)
                                ftpListItems.Add(i);

                            if (recurse && !directoriesListed.Contains(i.FullName))
                            {
                                directoriesListed.Add(i.FullName);
                                ListDirectory(server, port,
                                                ftpListItems, directoriesListed,
                                                i.FullName, listType, recurse,
                                                exclusiveDirectoryFilters, inclusiveDirectoryFilters,
                                                exclusiveFileFilters, inclusiveFileFilters);
                            }

                            break;

                        case FtpFileSystemObjectType.File:

                            if (listType == FTPListType.File || listType == FTPListType.All)
                            {
                                if (exclusiveFileFilters != null && exclusiveFileFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.FullName).ToUpper()))
                                    continue;

                                if (inclusiveFileFilters != null && !inclusiveFileFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.FullName).ToUpper()))
                                    continue;

                                ftpListItems.Add(i);
                            }

                            break;

                        case FtpFileSystemObjectType.Link:
                            if (i.LinkObject != null)
                            {
                                switch (i.LinkObject.Type)
                                {
                                    case FtpFileSystemObjectType.Directory:

                                        if (exclusiveDirectoryFilters != null && exclusiveDirectoryFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.LinkObject.FullName).ToUpper()))
                                            continue;

                                        if (inclusiveDirectoryFilters != null && !inclusiveDirectoryFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.LinkObject.FullName).ToUpper()))
                                            continue;

                                        if (listType == FTPListType.Directory || listType == FTPListType.All)
                                            ftpListItems.Add(i.LinkObject);

                                        if (recurse && !directoriesListed.Contains(i.LinkObject.FullName))
                                        {
                                            directoriesListed.Add(i.LinkObject.FullName);
                                            ListDirectory(server, port,
                                                            ftpListItems, directoriesListed,
                                                            i.LinkObject.FullName, listType, recurse,
                                                            exclusiveDirectoryFilters, inclusiveDirectoryFilters,
                                                            exclusiveFileFilters, inclusiveFileFilters);
                                        }

                                        break;

                                    case FtpFileSystemObjectType.File:

                                        if (listType == FTPListType.File || listType == FTPListType.All)
                                        {
                                            if (exclusiveFileFilters != null && exclusiveFileFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.LinkObject.FullName).ToUpper()))
                                                continue;

                                            if (inclusiveFileFilters != null && !inclusiveFileFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.LinkObject.FullName).ToUpper()))
                                                continue;

                                            ftpListItems.Add(i.LinkObject);
                                        }

                                        break;

                                }
                            }

                            break;
                    }
                }
            }
            finally
            {
                RecycleClient(conn);
            }
        }

        public string UniqueFilename(string server, int port, string filename)
        {
            filename = AdjustPath(server, filename);

            FtpClient conn = OpenClient(server, port);

            try
            {
                conn.SetWorkingDirectory(AdjustPath(server, STEM.Sys.IO.Path.GetDirectoryName(filename)));

                string unique = STEM.Sys.IO.Path.GetFileName(filename);

                if (filename.ToUpper().Contains("/DEV/NULL"))
                    return filename;

                int cnt = 1;

                while (conn.FileExists(unique))
                {
                    unique = string.Format("{0}_{1}{2}",
                        STEM.Sys.IO.Path.GetFileNameWithoutExtension(filename),
                        (cnt++).ToString("0000"),
                        STEM.Sys.IO.Path.GetExtension(filename));
                }

                return AdjustPath(server, STEM.Sys.IO.Path.GetDirectoryName(filename)) + "/" + unique;
            }
            finally
            {
                RecycleClient(conn);
            }
        }

        public string AdjustPath(string server, string path)
        {
            path = path.Replace("\\", "/");

            while (path.IndexOf("//") >= 0)
                path = path.Replace("//", "/");

            string machine = STEM.Sys.IO.Path.FirstTokenOfPath(path);

            if (server.Equals(STEM.Sys.IO.Net.MachineName(machine), StringComparison.InvariantCultureIgnoreCase) ||
                server.Equals(STEM.Sys.IO.Net.MachineAddress(machine), StringComparison.InvariantCultureIgnoreCase))
            {
                path = path.Substring(("/" + machine).Length);
            }

            return path;
        }
    }
}
