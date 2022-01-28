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
using System.Reflection;
using System.Xml.Serialization;
using System.Net;
using FluentFTP;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using STEM.Sys.Security;
using STEM.Sys.IO.Listing;

namespace STEM.Listing.FTP
{
    public class Authentication : STEM.Sys.IO.Listing.IAuthentication
    {
        [Category("FTP Server")]
        [DisplayName("FTP Server Address"), DescriptionAttribute("What is the FTP Server Address?")]
        public string ServerAddress { get; set; }

        [Category("FTP Server")]
        [DisplayName("FTP Port"), DescriptionAttribute("What is the FTP Port?")]
        public string Port { get; set; }

        [Category("FTP Server")]
        [DisplayName("Connection Timeout (Seconds)"), DescriptionAttribute("How long should we wait to connect?")]
        public int TimeoutSeconds { get; set; }

        [Category("FTP Server")]
        [DisplayName("Working Directory"), DescriptionAttribute("What is the FTP server working directory? (Blank for default)")]
        public string WorkingDirectory { get; set; }

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
            ServerAddress = "[FtpServerAddress]";
            Port = "[FtpServerPort]";
            TimeoutSeconds = 15;
            WorkingDirectory = "";
            FTPUser = "";
            FTPPassword = "";
            RequireCertificateValidation = false;
            UseTLS = false;
        }

        public override void PopulateFrom(Sys.Security.IAuthentication source)
        {
            if (source.VersionDescriptor.TypeName == VersionDescriptor.TypeName)
            {
                PropertyInfo i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "ServerAddress");
                if (i != null)
                {
                    string k = i.GetValue(source) as string;
                    if (!String.IsNullOrEmpty(k) && String.IsNullOrEmpty(ServerAddress))
                        ServerAddress = k;
                }

                i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "Port");
                if (i != null)
                {
                    string k = i.GetValue(source) as string;
                    if (!String.IsNullOrEmpty(k) && String.IsNullOrEmpty(Port))
                        Port = k;
                }

                i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "TimeoutSeconds");
                if (i != null)
                {
                    TimeoutSeconds = (int)i.GetValue(source);
                }

                i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "FTPUser");
                if (i != null)
                {
                    string k = i.GetValue(source) as string;
                    if (!String.IsNullOrEmpty(k) && String.IsNullOrEmpty(FTPUser))
                    {
                        FTPUser = k;

                        i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "FTPPassword");
                        if (i != null)
                            FTPPassword = i.GetValue(source) as string;

                        i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "RequireCertificateValidation");
                        if (i != null)
                            RequireCertificateValidation = (bool)i.GetValue(source);

                        i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "UseTLS");
                        if (i != null)
                            UseTLS = (bool)i.GetValue(source);
                    }
                }
            }
            else
            {
                throw new Exception("IAuthentication Type mismatch.");
            }
        }

        static Dictionary<string, Queue<FtpClient>> _FtpClients = new Dictionary<string, Queue<FtpClient>>();
        static Dictionary<string, STEM.Sys.State.GrabBag<string>> _ServerAddresses = new Dictionary<string, Sys.State.GrabBag<string>>(StringComparer.InvariantCultureIgnoreCase);

        public static string NextAddress(string rangedAddress, out int available)
        {
            if (!_ServerAddresses.ContainsKey(rangedAddress))
                lock (_ServerAddresses)
                    if (!_ServerAddresses.ContainsKey(rangedAddress))
                        _ServerAddresses[rangedAddress] = new Sys.State.GrabBag<string>(STEM.Sys.IO.Path.ExpandRangedIP(rangedAddress), rangedAddress);

            available = _ServerAddresses[rangedAddress].Available();

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

        string _SelectedAddress = null;

        public string TargetAddress(string target)
        {
            lock (_FtpClients)
            {
                if (_SelectedAddress != null)
                    return _SelectedAddress;

                if (!String.IsNullOrEmpty(target))
                {
                    _SelectedAddress = ServerFromPath(target);

                    if (!String.IsNullOrEmpty(_SelectedAddress))
                        _SelectedAddress = null;
                }

                FtpClient client = null;

                try
                {
                    client = OpenClient();
                }
                catch { }
                finally
                {
                    RecycleClient(client);
                }

                return _SelectedAddress;
            }
        }

        public string FullPath(string path)
        {
            if (_SelectedAddress == null)
            {
                TargetAddress(path);

                if (_SelectedAddress == null)
                    throw new System.IO.IOException("No server connection could be established given path: " + path);

                return '/' + _SelectedAddress + '/' + AdjustPath(_SelectedAddress, path);
            }

            string machine = ServerFromPath(path);

            return '/' + machine + '/' + AdjustPath(machine, path);
        }

        /// <summary>
        /// Opens a connection to one of the servers specified in the ServerAddress field and maintains the selected address as the target for future calls
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.IO.IOException"></exception>
        public FtpClient OpenClient()
        {
            lock (_FtpClients)
            {
                if (_SelectedAddress != null)
                {
                    try
                    {
                        FtpClient conn = OpenClient(_SelectedAddress);

                        if (conn != null)
                            return conn;
                    }
                    catch (Exception ex)
                    {
                        throw new System.IO.IOException("No connection could be established. (" + _SelectedAddress + ")", ex);
                    }
                }

                int available = 1;
                string server = Authentication.NextAddress(ServerAddress, out available);

                do
                {
                    if (server == null)
                        throw new System.IO.IOException("No connection could be established. (" + ServerAddress + ")");

                    try
                    {
                        FtpClient conn = OpenClient(server);

                        if (conn != null)
                        {
                            _SelectedAddress = server;
                            return conn;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (available == 0)
                            throw new System.IO.IOException("No connection could be established. (" + ServerAddress + ")", ex);
                    }

                    server = Authentication.NextAddress(ServerAddress, out _);

                } while (available-- > 0);

                throw new System.IO.IOException("No connection could be established. (" + ServerAddress + ")");
            }
        }

        /// <summary>
        /// Opens a client connected to the server specified in path
        /// </summary>
        /// <param name="path">Could take the form of serverNameOrAddress or \server\dirx\diry\...</param>
        /// <returns></returns>
        /// <exception cref="System.IO.IOException"></exception>
        public FtpClient OpenClient(string path)
        {
            lock (_FtpClients)
            {
                FtpClient conn = null;
                
                try
                {
                    string server = null;

                    if (path == null)
                        path = _SelectedAddress;

                    if (path == _SelectedAddress)
                        server = _SelectedAddress;

                    if (server == null)
                    {
                        server = ServerFromPath(path);
                    }

                    if (String.IsNullOrEmpty(server))
                        server = _SelectedAddress;

                    if (String.IsNullOrEmpty(server))
                        throw new System.IO.IOException("System.Listing.FTP.OpenClient called with empty server value.");

                    if (_SelectedAddress == null)
                        _SelectedAddress = server;

                    string clientKey = server + ":" + Port + ":" + FTPUser + ":" + FTPPassword;

                    if (!_FtpClients.ContainsKey(clientKey))
                        _FtpClients[clientKey] = new Queue<FtpClient>();

                    if (_FtpClients[clientKey].Count > 0)
                        conn = _FtpClients[clientKey].Dequeue();

                    if (conn != null)
                    {
                        if (!conn.IsConnected)
                            conn = OpenClient(server);

                        if (conn != null)
                            return conn;
                    }

                    conn = new FtpClient();

                    conn.Host = server;
                    conn.Port = Int32.Parse(Port);
                    conn.ConnectTimeout = TimeoutSeconds * 1000;
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

                    if (!String.IsNullOrEmpty(WorkingDirectory))
                        conn.SetWorkingDirectory(WorkingDirectory);

                    return conn;
                }
                catch (Exception ex)
                {
                    DisposeClient(conn);
                    conn = null;

                    STEM.Sys.EventLog.WriteEntry("FTP.Authentication.OpenClient", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);

                    throw ex;
                }
            }
        }

        public void DisposeClient(FtpClient client)
        {
            if (client != null)
            {
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

        public void RecycleClient(FtpClient client)
        {
            try
            {
                if (client == null)
                    return;

                if (client.IsConnected)
                {
                    lock (_FtpClients)
                    {
                        string clientKey = client.Host + ":" + client.Port + ":" + FTPUser + ":" + FTPPassword;

                        if (!_FtpClients.ContainsKey(clientKey))
                            _FtpClients[clientKey] = new Queue<FtpClient>();

                        if (_FtpClients[clientKey].Count < 5)
                        {
                            _FtpClients[clientKey].Enqueue(client);
                            client = null;
                        }
                    }
                }

                DisposeClient(client);
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

        public string ToString(FtpListItem item)
        {
            string ret = '/' + _SelectedAddress + '/' + item.Name.Replace('\\', '/');

            while (ret.Contains("//"))
                ret = ret.Replace("//", "/");

            return ret;
        }

        static Dictionary<string, Regex> _InclusiveDirFilter = new Dictionary<string, Regex>(StringComparer.InvariantCultureIgnoreCase);
        static Dictionary<string, Regex> _ExclusiveDirFilter = new Dictionary<string, Regex>(StringComparer.InvariantCultureIgnoreCase);
        static Dictionary<string, Regex> _InclusiveFileFilter = new Dictionary<string, Regex>(StringComparer.InvariantCultureIgnoreCase);
        static Dictionary<string, Regex> _ExclusiveFileFilter = new Dictionary<string, Regex>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory">Should take the form \server\dirx\diry\...</param>
        /// <param name="listType"></param>
        /// <param name="recurse"></param>
        /// <param name="directoryFilter">Can be a STEM compound filter</param>
        /// <param name="fileFilter">Can be a STEM compound filter</param>
        /// <returns></returns>
        public List<FtpListItem> ListDirectory(string directory, ListingType listType, bool recurse, string directoryFilter, string fileFilter)
        {
            List<FtpListItem> ret = new List<FtpListItem>();
            List<string> directoriesListed = new List<string>();

            Regex inclusiveDirectoryFilters = null;
            if (_InclusiveDirFilter.ContainsKey(directoryFilter))
            {
                inclusiveDirectoryFilters = _InclusiveDirFilter[directoryFilter];
            }
            else
            {
                inclusiveDirectoryFilters = STEM.Sys.IO.Path.BuildInclusiveFilter(directoryFilter);
                if (inclusiveDirectoryFilters != null)
                    _InclusiveDirFilter[directoryFilter] = inclusiveDirectoryFilters;
            }

            Regex exclusiveDirectoryFilters = null;
            if (_ExclusiveDirFilter.ContainsKey(directoryFilter))
            {
                exclusiveDirectoryFilters = _ExclusiveDirFilter[directoryFilter];
            }
            else
            {
                exclusiveDirectoryFilters = STEM.Sys.IO.Path.BuildExclusiveFilter(directoryFilter);
                if (exclusiveDirectoryFilters != null)
                    _ExclusiveDirFilter[directoryFilter] = exclusiveDirectoryFilters;
            }

            Regex inclusiveFileFilters = null;
            if (_InclusiveFileFilter.ContainsKey(fileFilter))
            {
                inclusiveFileFilters = _InclusiveFileFilter[fileFilter];
            }
            else
            {
                inclusiveFileFilters = STEM.Sys.IO.Path.BuildInclusiveFilter(fileFilter);
                if (inclusiveFileFilters != null)
                    _InclusiveFileFilter[fileFilter] = inclusiveFileFilters;
            }

            Regex exclusiveFileFilters = null;
            if (_ExclusiveFileFilter.ContainsKey(fileFilter))
            {
                exclusiveFileFilters = _ExclusiveFileFilter[fileFilter];
            }
            else
            {
                exclusiveFileFilters = STEM.Sys.IO.Path.BuildExclusiveFilter(fileFilter);
                if (exclusiveFileFilters != null)
                    _ExclusiveFileFilter[fileFilter] = exclusiveFileFilters;
            }

            ListDirectory(ret, directoriesListed,
                directory, listType, recurse,
                exclusiveDirectoryFilters, inclusiveDirectoryFilters,
                exclusiveFileFilters, inclusiveFileFilters);

            return ret;
        }

        void ListDirectory(List<FtpListItem> ftpListItems, List<string> directoriesListed,
            string directory, ListingType listType, bool recurse,
            Regex exclusiveDirectoryFilters, Regex inclusiveDirectoryFilters,
            Regex exclusiveFileFilters, Regex inclusiveFileFilters, FtpClient conn = null)
        {
            bool clientOwner = false;

            try
            {
                if (conn == null)
                {
                    clientOwner = true;
                    string address = ServerFromPath(directory);
                    conn = OpenClient(address);
                }

                directory = AdjustPath(conn.Host, directory);

                foreach (FtpListItem i in conn.GetListing(directory, FtpListOption.Modify | FtpListOption.Size | FtpListOption.DerefLinks | FtpListOption.AllFiles))
                {
                    string fn = i.FullName;
                    int ind = fn.IndexOf(directory, StringComparison.OrdinalIgnoreCase);
                    if (ind != -1)
                    {
                        i.Name = fn.Substring(ind);
                    }

                    switch (i.Type)
                    {
                        case FtpFileSystemObjectType.Directory:

                            string sub = ToString(i);

                            if (exclusiveDirectoryFilters != null && exclusiveDirectoryFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(sub).ToUpper()))
                                continue;

                            if (inclusiveDirectoryFilters != null && !inclusiveDirectoryFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(sub).ToUpper()))
                                continue;

                            if (listType == ListingType.Directory || listType == ListingType.All)
                                ftpListItems.Add(i);

                            if (recurse && !directoriesListed.Contains(sub))
                            {
                                directoriesListed.Add(sub);
                                ListDirectory(ftpListItems, directoriesListed,
                                              sub, listType, recurse,
                                              exclusiveDirectoryFilters, inclusiveDirectoryFilters,
                                              exclusiveFileFilters, inclusiveFileFilters, conn);
                            }

                            break;

                        case FtpFileSystemObjectType.File:

                            if (listType == ListingType.File || listType == ListingType.All)
                            {
                                if (exclusiveFileFilters != null && exclusiveFileFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.Name).ToUpper()))
                                    continue;

                                if (inclusiveFileFilters != null && !inclusiveFileFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.Name).ToUpper()))
                                    continue;

                                ftpListItems.Add(i);
                            }

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                if (clientOwner)
                {
                    DisposeClient(conn);
                    conn = null;

                    STEM.Sys.EventLog.WriteEntry("Authentication.ListDirectory", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                if (clientOwner && conn != null)
                {
                    RecycleClient(conn);
                }
            }
        }

        public void DownloadFile(string sourceFilePath, System.IO.Stream destinationStream)
        {
            FtpClient conn = null;

            try
            {
                conn = OpenClient(sourceFilePath);

                sourceFilePath = AdjustPath(conn.Host, sourceFilePath);

                using (System.IO.Stream sStream = conn.OpenRead(sourceFilePath, FtpDataType.Binary))
                {
                    sStream.CopyTo(destinationStream);
                }

                FtpReply reply = conn.GetReply();

                if (!reply.Success)
                    throw new Exception("There was an error reading from the FTP server: " + reply.Message);
            }
            catch (Exception ex)
            {
                DisposeClient(conn);
                conn = null;

                throw ex;
            }
            finally
            {
                RecycleClient(conn);
            }
        }

        public void UploadFile(System.IO.Stream sourceStream, string destinationFilePath)
        {
            FtpClient conn = null;

            try
            {
                conn = OpenClient(destinationFilePath);

                destinationFilePath = AdjustPath(conn.Host, destinationFilePath);

                using (System.IO.Stream dStream = conn.OpenWrite(destinationFilePath, FtpDataType.Binary))
                {
                    sourceStream.CopyTo(dStream);
                }

                FtpReply reply = conn.GetReply();

                if (!reply.Success)
                    throw new Exception("There was an error writing to the FTP server: " + reply.Message);
            }
            catch (Exception ex)
            {
                DisposeClient(conn);
                conn = null;

                throw ex;
            }
            finally
            {
                RecycleClient(conn);
            }
        }

        public void SetLastWriteTimeUtc(string file, DateTime lwt)
        {
            FtpClient conn = null;

            try
            {
                //conn = OpenClient(file);

                //file = AdjustPath(conn.Host, file);

                //conn.SetModifiedTime(file, lwt);

                //FtpReply reply = conn.GetReply();

                //if (!reply.Success)
                //    throw new Exception("There was an error in SetModifiedTime on the FTP server: " + reply.Message);
            }
            catch (Exception ex)
            {
                DisposeClient(conn);
                conn = null;

                throw ex;
            }
            finally
            {
                RecycleClient(conn);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">Should take the form \server\dirx\...\filename</param>
        /// <returns></returns>
        public override string UniqueFilename(string filename)
        {
            FtpClient conn = null;

            try
            {
                conn = OpenClient(filename);

                filename = AdjustPath(conn.Host, filename);

                string directory = STEM.Sys.IO.Path.GetDirectoryName(filename);

                string unique = STEM.Sys.IO.Path.GetFileName(filename);

                unique = AdjustPath(conn.Host, directory) + "/" + unique;

                if (filename.ToUpper().Contains("/DEV/NULL"))
                    return filename;

                int cnt = 1;

                while (conn.FileExists(unique))
                {
                    unique = string.Format("{0}_{1}{2}",
                        STEM.Sys.IO.Path.GetFileNameWithoutExtension(filename),
                        (cnt++).ToString("0000"),
                        STEM.Sys.IO.Path.GetExtension(filename));

                    unique = AdjustPath(conn.Host, directory) + "/" + unique;
                }

                return unique;
            }
            catch (Exception ex)
            {
                DisposeClient(conn);
                conn = null;

                throw ex;
            }
            finally
            {
                RecycleClient(conn);
            }
        }

        /// <summary>
        /// Removes the server address if path takes the form \server\dirx\diry\... 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        string AdjustPath(string server, string path)
        {
            path = path.Replace("\\", "/");

            while (path.IndexOf("//") >= 0)
                path = path.Replace("//", "/");

            string machine = STEM.Sys.IO.Path.FirstTokenOfPath(path);

            if (!String.IsNullOrEmpty(machine))
                if (server.Equals(STEM.Sys.IO.Net.MachineName(machine), StringComparison.InvariantCultureIgnoreCase) ||
                    server.Equals(STEM.Sys.IO.Net.MachineAddress(machine), StringComparison.InvariantCultureIgnoreCase))
                {
                    path = path.Substring(("/" + machine).Length);
                }

            return path;
        }

        /// <summary>
        /// Returns the server address if path takes the form \server\dirx\diry\...
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string ServerFromPath(string path)
        {
            path = path.Replace("\\", "/");

            while (path.IndexOf("//") >= 0)
                path = path.Replace("//", "/");

            string machine = STEM.Sys.IO.Path.FirstTokenOfPath(path);

            if (String.IsNullOrEmpty(machine))
                return null;

            machine = STEM.Sys.IO.Net.MachineAddress(machine);

            if (machine == "0.0.0.0" || machine == "255.255.255.255")
            {
                machine = _SelectedAddress;
            }

            return machine;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listingType"></param>
        /// <param name="path">Should take the form \server\dirx\diry\...</param>
        /// <param name="fileFilter">Can be a STEM compound filter</param>
        /// <param name="subpathFilter">Can be a STEM compound filter</param>
        /// <param name="recurse"></param>
        /// <returns></returns>
        public override IListingAgent ConstructListingAgent(ListingType listingType, string path, string fileFilter, string subpathFilter, bool recurse)
        {
            return new ListingAgent(this, listingType, path, fileFilter, subpathFilter, recurse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory">Should take the form \server\dirx\diry\...</param>
        public override void CreateDirectory(string directory)
        {
            FtpClient conn = null;

            try
            {
                conn = OpenClient(directory);

                directory = AdjustPath(conn.Host, directory);

                if (!conn.DirectoryExists(directory))
                    conn.CreateDirectory(directory);
            }
            catch (Exception ex)
            {
                DisposeClient(conn);
                conn = null;

                throw ex;
            }
            finally
            {
                RecycleClient(conn);
            }
        }

        void DeleteDirectory(string directory, bool recurse, bool deleteFiles, FtpClient conn)
        {
            try
            {
                directory = AdjustPath(conn.Host, directory);

                if (conn.DirectoryExists(directory))
                {
                    List<FtpListItem> items = conn.GetListing(directory, FtpListOption.AllFiles).ToList();

                    if (recurse)
                    {
                        foreach (FtpListItem i in items.Where(i => i.Type == FtpFileSystemObjectType.Directory))
                            DeleteDirectory(ToString(i), recurse, deleteFiles, conn);
                    }

                    if (!deleteFiles && items.Count(i => i.Type != FtpFileSystemObjectType.Directory) > 0)
                        throw new Exception("Directory (" + directory + ") is not empty.");

                    conn.DeleteDirectory(directory);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory">Should take the form \server\dirx\diry\...</param>
        /// <param name="recurse"></param>
        /// <param name="deleteFiles"></param>
        public override void DeleteDirectory(string directory, bool recurse, bool deleteFiles)
        {
            FtpClient conn = null;

            try
            {
                conn = OpenClient(directory);
                DeleteDirectory(directory, recurse, deleteFiles, conn);                
            }
            catch (Exception ex)
            {
                DisposeClient(conn);
                conn = null;

                throw ex;
            }
            finally
            {
                RecycleClient(conn);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">Should take the form \server\dirx\...\filename</param>
        public override void DeleteFile(string file)
        {
            FtpClient conn = null;

            try
            {
                conn = OpenClient(file);

                file = AdjustPath(conn.Host, file);

                if (conn.FileExists(file))
                    conn.DeleteFile(file);
            }
            catch (Exception ex)
            {
                DisposeClient(conn);
                conn = null;

                throw ex;
            }
            finally
            {
                RecycleClient(conn);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory">Should take the form \server\dirx\diry\...</param>
        /// <returns></returns>
        public override bool DirectoryExists(string directory)
        {
            FtpClient conn = null;

            try
            {
                conn = OpenClient(directory);

                directory = AdjustPath(conn.Host, directory);

                return conn.DirectoryExists(directory);
            }
            catch
            {
                DisposeClient(conn);
                conn = null;
            }
            finally
            {
                RecycleClient(conn);
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">Should take the form \server\dirx\...\filename</param>
        /// <returns></returns>
        public override bool FileExists(string file)
        {
            FtpClient conn = null;

            try
            {
                conn = OpenClient(file);

                file = AdjustPath(conn.Host, file);

                return conn.FileExists(file);
            }
            catch
            {
                DisposeClient(conn);
                conn = null;
            }
            finally
            {
                RecycleClient(conn);
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory">Should take the form \server\dirx\diry\...</param>
        /// <returns></returns>
        public override DirectoryInfo GetDirectoryInfo(string directory)
        {
            FtpClient conn = null;

            try
            {
                conn = OpenClient(directory);

                directory = AdjustPath(conn.Host, directory);

                DateTime m = conn.GetModifiedTime(directory);

                return new DirectoryInfo { CreationTimeUtc = DateTime.MinValue, LastAccessTimeUtc = m, LastWriteTimeUtc = m };
            }
            catch
            {
                DisposeClient(conn);
                conn = null;
            }
            finally
            {
                RecycleClient(conn);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">Should take the form \server\dirx\...\filename</param>
        /// <returns></returns>
        public override FileInfo GetFileInfo(string file)
        {
            FtpClient conn = null;

            try
            {
                conn = OpenClient(file);

                file = AdjustPath(conn.Host, file);

                FtpListItem i = conn.GetListing(file, FtpListOption.AllFiles).FirstOrDefault();

                return new FileInfo { CreationTimeUtc = i.Created, LastAccessTimeUtc = i.Modified, LastWriteTimeUtc = i.Modified, Size = i.Size };
            }
            catch
            {
                DisposeClient(conn);
                conn = null;
            }
            finally
            {
                RecycleClient(conn);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldPath">Should take the form \server\dirx\...\filename</param>
        /// <param name="newPath">Should take the form \server\dirx\...\filename</param>
        /// <returns></returns>
        public void RenameFile(string oldPath, string newPath)
        {
            FtpClient conn = null;

            try
            {
                conn = OpenClient(oldPath);

                oldPath = AdjustPath(conn.Host, oldPath);
                newPath = AdjustPath(conn.Host, newPath);

                conn.Rename(oldPath, newPath);
            }
            catch (Exception ex)
            {
                DisposeClient(conn);
                conn = null; 
                
                throw ex;
            }
            finally
            {
                RecycleClient(conn);
            }
        }
    }
}