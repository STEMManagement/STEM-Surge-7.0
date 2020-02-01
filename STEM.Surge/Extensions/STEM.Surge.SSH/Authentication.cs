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
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using STEM.Sys.Security;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace STEM.Surge.SSH
{
    public enum SSHListType { File, Directory, All }

    public class Authentication : IAuthentication
    {
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

        static Dictionary<string, Queue<SftpClient>> _SftpClients = new Dictionary<string, Queue<SftpClient>>();

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

        public SftpClient OpenSftpClient(string server, int port)
        {
            lock (_SftpClients)
            {
                string clientKey = server + ":" + port + ":" + User + ":" + Password + ":" + KeyFile;

                if (!_SftpClients.ContainsKey(clientKey))
                    _SftpClients[clientKey] = new Queue<SftpClient>();

                SftpClient conn = null;

                if (_SftpClients[clientKey].Count > 0)
                    conn = _SftpClients[clientKey].Dequeue();

                if (conn != null)
                {
                    if (!conn.IsConnected)
                        conn.Connect();

                    return conn;
                }

                AuthenticationMethod authenticationMethod = null;

                if (!String.IsNullOrEmpty(KeyFile))
                {
                    if (!String.IsNullOrEmpty(Password))
                    {
                        authenticationMethod = new PrivateKeyAuthenticationMethod(User, new PrivateKeyFile[] { new PrivateKeyFile(KeyFile, Password) });
                    }
                    else
                    {
                        authenticationMethod = new PrivateKeyAuthenticationMethod(User, new PrivateKeyFile[] { new PrivateKeyFile(KeyFile) });
                    }
                }
                else
                {
                    authenticationMethod = new PasswordAuthenticationMethod(User, Password);
                }

                conn = new SftpClient(new ConnectionInfo(server, port, User, new AuthenticationMethod[] { authenticationMethod }));

                conn.Connect();

                return conn;
            }
        }

        public void RecycleClient(SftpClient client)
        {
            lock (_SftpClients)
            {
                string clientKey = client.ConnectionInfo.Host + ":" + client.ConnectionInfo.Port + ":" + User + ":" + Password + ":" + KeyFile;

                if (!_SftpClients.ContainsKey(clientKey))
                    _SftpClients[clientKey] = new Queue<SftpClient>();

                if (_SftpClients[clientKey].Count < 20)
                {
                    _SftpClients[clientKey].Enqueue(client);
                    return;
                }

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

        static Dictionary<string, Queue<SshClient>> _SshClients = new Dictionary<string, Queue<SshClient>>();

        public SshClient OpenSshClient(string server, int port)
        {
            lock (_SshClients)
            {
                string clientKey = server + ":" + port + ":" + User + ":" + Password + ":" + KeyFile;

                if (!_SshClients.ContainsKey(clientKey))
                    _SshClients[clientKey] = new Queue<SshClient>();

                SshClient conn = null;

                if (_SshClients[clientKey].Count > 0)
                    conn = _SshClients[clientKey].Dequeue();

                if (conn != null)
                {
                    if (!conn.IsConnected)
                        conn.Connect();

                    return conn;
                }

                AuthenticationMethod authenticationMethod = null;

                if (!String.IsNullOrEmpty(KeyFile))
                {
                    if (!String.IsNullOrEmpty(Password))
                    {
                        authenticationMethod = new PrivateKeyAuthenticationMethod(User, new PrivateKeyFile[] { new PrivateKeyFile(KeyFile, Password) });
                    }
                    else
                    {
                        authenticationMethod = new PrivateKeyAuthenticationMethod(User, new PrivateKeyFile[] { new PrivateKeyFile(KeyFile) });
                    }
                }
                else
                {
                    authenticationMethod = new PasswordAuthenticationMethod(User, Password);
                }

                conn = new SshClient(new ConnectionInfo(server, port, User, new AuthenticationMethod[] { authenticationMethod }));

                conn.KeepAliveInterval = TimeSpan.FromSeconds(10);
                conn.ConnectionInfo.Timeout = TimeSpan.FromSeconds(10); 
                conn.Connect();

                return conn;
            }
        }

        public void RecycleClient(SshClient client)
        {
            lock (_SshClients)
            {
                string clientKey = client.ConnectionInfo.Host + ":" + client.ConnectionInfo.Port + ":" + User + ":" + Password + ":" + KeyFile;

                if (!_SshClients.ContainsKey(clientKey))
                    _SshClients[clientKey] = new Queue<SshClient>();

                if (_SshClients[clientKey].Count < 20)
                {
                    _SshClients[clientKey].Enqueue(client);
                    return;
                }

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

        protected override void Dispose(bool dispose)
        {
        }

        public List<SftpFile> ListDirectory(string server, int port,
            string directory, SSHListType listType, bool recurse, string directoryFilter, string fileFilter)
        {
            directory = AdjustPath(server, directory);

            List<SftpFile> ret = new List<SftpFile>();
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
            List<SftpFile> sshListItems, List<string> directoriesListed,
            string directory, SSHListType listType, bool recurse,
            Regex exclusiveDirectoryFilters, Regex inclusiveDirectoryFilters,
            Regex exclusiveFileFilters, Regex inclusiveFileFilters)
        {
            List<SftpFile> list = null;

            try
            {
                SftpClient client = OpenSftpClient(server, port);

                try
                {
                    foreach (SftpFile i in client.ListDirectory(directory))
                    {
                        if (i.IsDirectory)
                        {
                            if (exclusiveDirectoryFilters != null && exclusiveDirectoryFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.FullName).ToUpper()))
                                continue;

                            if (inclusiveDirectoryFilters != null && !inclusiveDirectoryFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(i.FullName).ToUpper()))
                                continue;

                            if (listType == SSHListType.Directory || listType == SSHListType.All)
                                sshListItems.Add(i);

                            if (recurse && !directoriesListed.Contains(i.FullName))
                            {
                                directoriesListed.Add(i.FullName);
                                ListDirectory(server, port,
                                                sshListItems, directoriesListed,
                                                i.FullName, listType, recurse,
                                                exclusiveDirectoryFilters, inclusiveDirectoryFilters,
                                                exclusiveFileFilters, inclusiveFileFilters);
                            }
                        }
                        else if (i.IsRegularFile)
                        {
                            if (listType == SSHListType.File || listType == SSHListType.All)
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
                finally
                {
                    RecycleClient(client);
                }

                if (list == null)
                    return;
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Authentication.ListDirectory", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }
        }

        public string UniqueFilename(string server, int port, string file)
        {
            file = AdjustPath(server, file);

            string unique = file;

            try
            {
                if (file.ToUpper().Contains("/DEV/NULL"))
                    return unique;

                int cnt = 1;

                SftpClient client = OpenSftpClient(server, port);

                try
                {
                    while (client.Exists(unique))
                    {
                        unique = string.Format("{0}/{1}_{2}{3}",
                            STEM.Sys.IO.Path.GetDirectoryName(file),
                            STEM.Sys.IO.Path.GetFileNameWithoutExtension(file),
                            (cnt++).ToString("0000"),
                            STEM.Sys.IO.Path.GetExtension(file));
                    }
                }
                finally
                {
                    RecycleClient(client);
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Authentication.UniqueFilename", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return unique;
        }

        public bool FileExists(string server, int port, string file)
        {
            file = AdjustPath(server, file);

            try
            {
                SftpClient client = OpenSftpClient(server, port);

                try
                {
                    SftpFileAttributes a = client.GetAttributes(file);

                    if (a.IsRegularFile)
                        return true;
                }
                catch
                {
                }
                finally
                {
                    RecycleClient(client);
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Authentication.FileExists", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return false;
        }

        public FDCFileInfo GetFileInfo(string server, int port, string file)
        {
            file = AdjustPath(server, file);

            try
            {
                SftpClient client = OpenSftpClient(server, port);

                try
                {
                    SftpFile a = client.Get(file);

                    if (a.IsRegularFile)
                        return new FDCFileInfo { CreationTimeUtc = a.LastWriteTimeUtc, LastAccessTimeUtc = a.LastAccessTimeUtc, LastWriteTimeUtc = a.LastWriteTimeUtc, Size = a.Attributes.Size };
                }
                catch
                {
                }
                finally
                {
                    RecycleClient(client);
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Authentication.GetFileInfo", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return null;
        }

        public bool DirectoryExists(string server, int port, string directory)
        {
            directory = AdjustPath(server, directory);

            try
            {
                SftpClient client = OpenSftpClient(server, port);

                try
                {
                    SftpFileAttributes a = client.GetAttributes(directory);

                    if (a.IsDirectory)
                        return true;
                }
                catch
                {
                }
                finally
                {
                    RecycleClient(client);
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Authentication.DirectoryExists", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return false;
        }


        public FDCDirectoryInfo GetDirectoryInfo(string server, int port, string directory)
        {
            directory = AdjustPath(server, directory);

            try
            {
                SftpClient client = OpenSftpClient(server, port);

                try
                {
                    SftpFile a = client.Get(directory);

                    if (a.IsDirectory)
                        return new FDCDirectoryInfo { CreationTimeUtc = a.LastWriteTimeUtc, LastAccessTimeUtc = a.LastAccessTimeUtc, LastWriteTimeUtc = a.LastWriteTimeUtc };
                }
                catch
                {
                }
                finally
                {
                    RecycleClient(client);
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Authentication.GetDirectoryInfo", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return null;
        }

        public void RenameFile(string server, int port, string oldPath, string newPath)
        {
            oldPath = AdjustPath(server, oldPath);

            newPath = AdjustPath(server, newPath);

            SftpClient client = OpenSftpClient(server, port);
            try
            {
                client.RenameFile(oldPath, newPath);
            }
            finally
            {
                RecycleClient(client);
            }
        }

        public void CreateDirectory(string server, int port, string directory)
        {
            directory = AdjustPath(server, directory);

            SftpClient client = OpenSftpClient(server, port);
            try
            {
                client.CreateDirectory(directory);
            }
            finally
            {
                RecycleClient(client);
            }
        }

        public void DeleteDirectory(string server, int port, string directory)
        {
            directory = AdjustPath(server, directory);

            SftpClient client = OpenSftpClient(server, port);
            try
            {
                client.DeleteDirectory(directory);
            }
            finally
            {
                RecycleClient(client);
            }
        }

        public void DeleteFile(string server, int port, string file)
        {
            file = AdjustPath(server, file);
            
            SftpClient client = OpenSftpClient(server, port);
            try
            {
                client.DeleteFile(file);
            }
            finally
            {
                RecycleClient(client);
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