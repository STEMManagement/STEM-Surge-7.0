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
using System.Security.Authentication;
using System.Text.RegularExpressions;
using STEM.Sys.Security;
using STEM.Sys.IO.Listing;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace STEM.Listing.SSH
{
    public class Authentication : STEM.Sys.IO.Listing.IAuthentication
    {
        [Category("SSH")]
        [DisplayName("SSH Server Address"), DescriptionAttribute("What is the SSH Server Address?")]
        public string ServerAddress { get; set; }

        [Category("SSH")]
        [DisplayName("SSH Port"), DescriptionAttribute("What is the SSH Port?")]
        public string Port { get; set; }

        [Category("SSH")]
        [DisplayName("Connection Timeout (Seconds)"), DescriptionAttribute("How long should we wait to connect?")]
        public int TimeoutSeconds { get; set; }

        [Category("SSH")]
        [DisplayName("User"), DescriptionAttribute("What is the SSH user?")]
        public string User { get; set; }

        [Category("SSH")]
        [DisplayName("Key File"), DescriptionAttribute("Where is the SSH users key file?")]
        public string KeyFile { get; set; }

        [Category("SSH")]
        [DisplayName("Password"), DescriptionAttribute("What is the SSH password?")]
        [XmlIgnore]
        [PasswordPropertyText(true)]
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

        public Authentication()
        {
            ServerAddress = "[SshServerAddress]";
            Port = "[SshServerPort]";
            TimeoutSeconds = 5;
            User = "";
            KeyFile = "";
            Password = "";
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

                i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "User");
                if (i != null)
                {
                    string k = i.GetValue(source) as string;
                    if (!String.IsNullOrEmpty(k) && String.IsNullOrEmpty(User))
                    {
                        User = k;

                        i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "Password");
                        if (i != null)
                            Password = i.GetValue(source) as string;

                        i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "KeyFile");
                        if (i != null)
                            KeyFile = i.GetValue(source) as string;
                    }
                }
            }
            else
            {
                throw new Exception("IAuthentication Type mismatch.");
            }
        }

        static Dictionary<string, Queue<SftpClient>> _SftpClients = new Dictionary<string, Queue<SftpClient>>();
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
            lock (_SftpClients)
            {
                if (_SelectedAddress != null)
                    return _SelectedAddress;

                if (!String.IsNullOrEmpty(target))
                {
                    _SelectedAddress = ServerFromPath(target);

                    if (!String.IsNullOrEmpty(_SelectedAddress))
                        _SelectedAddress = null;
                }

                SftpClient client = null;

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
        public SftpClient OpenClient()
        {
            lock (_SftpClients)
            {
                if (_SelectedAddress != null)
                {
                    try
                    {
                        SftpClient conn = OpenClient(_SelectedAddress);

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
                        SftpClient conn = OpenClient(server);

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
        public SftpClient OpenClient(string path)
        {
            lock (_SftpClients)
            {
                SftpClient conn = null;

                try
                {
                    string server = null;

                    if (path == null)
                        path = _SelectedAddress;

                    if (path == _SelectedAddress)
                        server = _SelectedAddress;

                    if (server == null)
                        server = ServerFromPath(path);

                    if (String.IsNullOrEmpty(server))
                        server = _SelectedAddress;

                    if (String.IsNullOrEmpty(server))
                        throw new System.IO.IOException("System.Listing.SSH.OpenClient called with empty server value.");

                    if (_SelectedAddress == null)
                        _SelectedAddress = server;

                    string clientKey = server + ":" + Port + ":" + User + ":" + Password + ":" + KeyFile;

                    if (!_SftpClients.ContainsKey(clientKey))
                        _SftpClients[clientKey] = new Queue<SftpClient>();

                    if (_SftpClients[clientKey].Count > 0)
                        conn = _SftpClients[clientKey].Dequeue();

                    if (conn != null)
                    {
                        if (!conn.IsConnected)
                            conn = OpenClient(server);

                        if (conn != null)
                            return conn;
                    }

                    AuthenticationMethod authenticationMethod = null;

                    if (!String.IsNullOrEmpty(KeyFile))
                    {
                        string kf = null;

                        foreach (string f in STEM.Sys.IO.Path.OrderPathsWithSubnet(KeyFile, STEM.Sys.IO.Net.MachineIP()))
                            if (System.IO.File.Exists(f))
                            {
                                kf = f;
                                break;
                            }

                        if (kf == null)
                            throw new System.Exception("No file could be found from " + KeyFile);

                        if (!String.IsNullOrEmpty(Password))
                        {
                            authenticationMethod = new PrivateKeyAuthenticationMethod(User, new PrivateKeyFile[] { new PrivateKeyFile(kf, Password) });
                        }
                        else
                        {
                            authenticationMethod = new PrivateKeyAuthenticationMethod(User, new PrivateKeyFile[] { new PrivateKeyFile(kf) });
                        }
                    }
                    else
                    {
                        authenticationMethod = new PasswordAuthenticationMethod(User, Password);
                    }

                    conn = new SftpClient(new ConnectionInfo(server, Int32.Parse(Port), User, new AuthenticationMethod[] { authenticationMethod }));

                    conn.ConnectionInfo.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);

                    conn.Connect();

                    return conn;
                }
                catch (Exception ex)
                {
                    DisposeClient(conn);
                    conn = null;

                    STEM.Sys.EventLog.WriteEntry("SSH.Authentication.OpenClient", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                    throw ex;
                }
            }
        }

        public void DisposeClient(SftpClient client)
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

        public void RecycleClient(SftpClient client)
        {
            try
            {
                if (client == null)
                    return;

                if (client.IsConnected)
                {
                    lock (_SftpClients)
                    {
                        string clientKey = client.ConnectionInfo.Host + ":" + Port + ":" + User + ":" + Password + ":" + KeyFile;

                        if (!_SftpClients.ContainsKey(clientKey))
                            _SftpClients[clientKey] = new Queue<SftpClient>();

                        if (_SftpClients[clientKey].Count < 5)
                        {
                            _SftpClients[clientKey].Enqueue(client);
                            client = null;
                        }
                    }
                }

                DisposeClient(client);
                client = null;
            }
            catch { }
        }

        public string ToString(SftpFile item)
        {
            return  '/' + _SelectedAddress + '/' + item.FullName.Replace('\\', '/');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory">Should take the form \server\dirx\diry\...</param>
        /// <param name="listType"></param>
        /// <param name="recurse"></param>
        /// <param name="directoryFilter">Can be a STEM compound filter</param>
        /// <param name="fileFilter">Can be a STEM compound filter</param>
        /// <returns></returns>
        public List<SftpFile> ListDirectory(string directory, ListingType listType, bool recurse, string directoryFilter, string fileFilter)
        {
            List<SftpFile> ret = new List<SftpFile>();

            try
            {
                List<string> directoriesListed = new List<string>();

                Regex exclusiveFileFilters = STEM.Sys.IO.Path.BuildExclusiveFilter(fileFilter);
                Regex inclusiveFileFilters = STEM.Sys.IO.Path.BuildInclusiveFilter(fileFilter);
                Regex exclusiveDirectoryFilters = STEM.Sys.IO.Path.BuildExclusiveFilter(directoryFilter);
                Regex inclusiveDirectoryFilters = STEM.Sys.IO.Path.BuildInclusiveFilter(directoryFilter);

                ListDirectory(ret, directoriesListed,
                    directory, listType, recurse,
                    exclusiveDirectoryFilters, inclusiveDirectoryFilters,
                    exclusiveFileFilters, inclusiveFileFilters);
            }
            catch
            {
                throw;
            }

            return ret;
        }

        void ListDirectory(List<SftpFile> sshListItems, List<string> directoriesListed,
            string directory, ListingType listType, bool recurse,
            Regex exclusiveDirectoryFilters, Regex inclusiveDirectoryFilters,
            Regex exclusiveFileFilters, Regex inclusiveFileFilters, SftpClient client = null)
        {
            bool clientOwner = false;

            try
            {
                if (client == null)
                {
                    clientOwner = true;
                    client = OpenClient(directory);
                }

                directory = AdjustPath(client.ConnectionInfo.Host, directory);

                foreach (SftpFile i in client.ListDirectory(directory))
                {
                    if (i.Name == ".")
                        continue;

                    if (i.Name == "..")
                        continue;
                    
                    if (i.IsDirectory)
                    {
                        if (exclusiveDirectoryFilters != null && exclusiveDirectoryFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.FullName).ToUpper()))
                            continue;

                        if (inclusiveDirectoryFilters != null && !inclusiveDirectoryFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.FullName).ToUpper()))
                            continue;

                        if (listType == ListingType.Directory || listType == ListingType.All)
                            sshListItems.Add(i);

                        if (recurse && !directoriesListed.Contains(i.FullName))
                        {
                            directoriesListed.Add(i.FullName);
                            ListDirectory(sshListItems, directoriesListed,
                                            i.FullName, listType, recurse,
                                            exclusiveDirectoryFilters, inclusiveDirectoryFilters,
                                            exclusiveFileFilters, inclusiveFileFilters, client);
                        }
                    }
                    else if (i.IsRegularFile)
                    {
                        if (listType == ListingType.File || listType == ListingType.All)
                        {
                            if (exclusiveFileFilters != null && exclusiveFileFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.FullName).ToUpper()))
                                continue;

                            if (inclusiveFileFilters != null && !inclusiveFileFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.FullName).ToUpper()))
                                continue;

                            sshListItems.Add(i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (clientOwner)
                {
                    DisposeClient(client);
                    client = null;

                    STEM.Sys.EventLog.WriteEntry("Authentication.ListDirectory", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                if (clientOwner && client != null)
                {
                    RecycleClient(client);
                }
            }
        }

        public void DownloadFile(string sourceFilePath, System.IO.Stream destinationStream)
        {
            SftpClient conn = null;

            try
            {
                conn = OpenClient(sourceFilePath);

                sourceFilePath = AdjustPath(conn.ConnectionInfo.Host, sourceFilePath);

                conn.DownloadFile(sourceFilePath, destinationStream);
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
            SftpClient conn = null;

            try
            {
                conn = OpenClient(destinationFilePath);

                destinationFilePath = AdjustPath(conn.ConnectionInfo.Host, destinationFilePath);

                conn.UploadFile(sourceStream, destinationFilePath);
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
            SftpClient conn = null;

            try
            {
                // Not yet implemented in Renci

                //conn = OpenClient(file);

                //file = AdjustPath(conn.ConnectionInfo.Host, file);

                //conn.SetLastWriteTimeUtc(file, lwt);
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
            SftpClient conn = null;

            try
            {
                conn = OpenClient(filename);

                filename = AdjustPath(conn.ConnectionInfo.Host, filename);

                conn.ChangeDirectory(AdjustPath(conn.ConnectionInfo.Host, STEM.Sys.IO.Path.GetDirectoryName(filename)));

                string unique = STEM.Sys.IO.Path.GetFileName(filename);

                if (filename.ToUpper().Contains("/DEV/NULL"))
                    return filename;

                int cnt = 1;

                while (conn.Exists(unique))
                {
                    unique = string.Format("{0}_{1}{2}",
                        STEM.Sys.IO.Path.GetFileNameWithoutExtension(filename),
                        (cnt++).ToString("0000"),
                        STEM.Sys.IO.Path.GetExtension(filename));
                }

                return AdjustPath(conn.ConnectionInfo.Host, STEM.Sys.IO.Path.GetDirectoryName(filename)) + "/" + unique;
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

            path = path.TrimStart('/', '\\');

            if (path == "")
                path = ".";

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
            SftpClient conn = null;

            try
            {
                conn = OpenClient(directory);

                directory = AdjustPath(conn.ConnectionInfo.Host, directory);

                if (!conn.Exists(directory))
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

        void DeleteDirectory(string directory, bool recurse, bool deleteFiles, SftpClient conn)
        {
            try
            {
                directory = AdjustPath(conn.ConnectionInfo.Host, directory);

                if (conn.Exists(directory))
                {
                    List<SftpFile> items = conn.ListDirectory(directory).ToList();

                    if (recurse)
                    {
                        foreach (SftpFile i in items.Where(i => i.IsDirectory))
                            DeleteDirectory(i.FullName, recurse, deleteFiles);
                    }

                    if (!deleteFiles && items.Count(i => !i.IsDirectory) > 0)
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
            SftpClient conn = null;

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
            SftpClient conn = null;

            try
            {
                conn = OpenClient(file);

                file = AdjustPath(conn.ConnectionInfo.Host, file);

                if (conn.Exists(file))
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
            SftpClient conn = null;
            try
            {
                conn = OpenClient(directory);

                directory = AdjustPath(conn.ConnectionInfo.Host, directory);

                return conn.Exists(directory);
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
            SftpClient conn = null;

            try
            {
                conn = OpenClient(file);

                file = AdjustPath(conn.ConnectionInfo.Host, file);

                return conn.Exists(file);
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
            SftpClient conn = null;

            try
            {
                conn = OpenClient(directory);

                directory = AdjustPath(conn.ConnectionInfo.Host, directory);

                SftpFile a = conn.Get(directory);

                if (a.IsDirectory)
                    return new DirectoryInfo { CreationTimeUtc = a.LastWriteTimeUtc, LastAccessTimeUtc = a.LastAccessTimeUtc, LastWriteTimeUtc = a.LastWriteTimeUtc };
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
            SftpClient conn = null;

            try
            {
                conn = OpenClient(file);

                file = AdjustPath(conn.ConnectionInfo.Host, file);

                SftpFile a = conn.Get(file);

                if (!a.IsDirectory)
                    return new FileInfo { CreationTimeUtc = a.LastWriteTimeUtc, LastAccessTimeUtc = a.LastAccessTimeUtc, LastWriteTimeUtc = a.LastWriteTimeUtc, Size = a.Length };
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
            SftpClient conn = null;

            try
            {
                conn = OpenClient(oldPath);

                oldPath = AdjustPath(conn.ConnectionInfo.Host, oldPath);
                newPath = AdjustPath(conn.ConnectionInfo.Host, newPath);
                conn.RenameFile(oldPath, newPath);
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
