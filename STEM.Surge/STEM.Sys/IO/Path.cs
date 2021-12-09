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
using System.Linq;
using System.Text.RegularExpressions;

namespace STEM.Sys.IO
{
    public static class Path
    {
        public static string SmbPathStart = "" + System.IO.Path.DirectorySeparatorChar + System.IO.Path.DirectorySeparatorChar;

        /// <summary>
        /// Returns a clean, platform adjusted, directory (path without the last token, whether a filename or a subdirectory)
        /// </summary>
        /// <param name="path">Path to be evaluated</param>
        /// <returns>path without the last token</returns>
        public static string GetDirectoryName(string path)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (String.IsNullOrEmpty(path))
                return "";

            string fn = GetFileName(path);

            path = STEM.Sys.IO.Path.AdjustPath(path);

            return path.Substring(0, path.LastIndexOf(fn, StringComparison.InvariantCultureIgnoreCase)).TrimEnd('\\').TrimEnd('/');
        }

        /// <summary>
        /// Returns the last token in path (a filename or a subdirectory)
        /// </summary>
        /// <param name="path">Path to be evaluated</param>
        /// <returns>the last token in path</returns>
        public static string GetFileName(string path)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            path = STEM.Sys.IO.Path.AdjustPath(path);

            path = path.TrimEnd('\\').TrimEnd('/');

            int i = path.LastIndexOf('\\');

            if (i < 0)
            {
                i = path.LastIndexOf('/');

                if (i < 0)
                    return path;
            }

            return path.Substring(i+1);
        }

        /// <summary>
        /// Returns the last token in path (a filename or a subdirectory), excluding any token after the last '.' (if a '.' exists)
        /// </summary>
        /// <param name="path">Path to be evaluated</param>
        /// <returns>the last token in path, excluding any token after the last '.'</returns>
        public static string GetFileNameWithoutExtension(string path)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            string s = GetFileName(path);

            int i = s.LastIndexOf('.');
            if (i < 0)
                return s;

            return s.Substring(0, i);
        }

        /// <summary>
        /// Returns the token after the last '.' (if a '.' exists)
        /// </summary>
        /// <param name="path">Path to be evaluated</param>
        /// <returns>the token after the last '.' (if a '.' exists)</returns>
        public static string GetExtension(string path)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (String.IsNullOrEmpty(path))
                return "";

            string s = STEM.Sys.IO.Path.GetFileName(path);

            int i = s.LastIndexOf('.');
            if (i < 0)
                return "";

            return s.Substring(i);
        }

        /// <summary>
        /// Changes the ip (\\ip\path) to the machine name (\\machineName\path)
        /// Returns path if path does not start with '\\' or already matches machineName
        /// </summary>
        /// <param name="path">Path to be transformed</param>
        /// <returns>Path with machineName in place of IP address</returns>
        public static string ChangeIpToMachineName(string path)
        {
            if (!path.StartsWith("\\\\"))
                return path;

            string machine = FirstTokenOfPath(path);
            string machineName = STEM.Sys.IO.Net.MachineName(machine);

            if (machineName == System.Net.IPAddress.None.ToString())
                return path;

            int machineStart = path.IndexOf(machine);

            return (path.Substring(0, machineStart) + machineName + path.Substring(machineStart + machine.Length));
        }

        /// <summary>
        /// Changes the machineName (\\machineName\path) to the IP (\\ip\path)
        /// Returns path if path does not start with '\\' or already matches IP
        /// </summary>
        /// <param name="path">Path to be transformed</param>
        /// <returns>Path with IP in place of machineName</returns>
        public static string ChangeMachineNameToIp(string path)
        {
            if (!path.StartsWith("\\\\"))
                return path;

            string machine = FirstTokenOfPath(path);
            string machineIP = STEM.Sys.IO.Net.MachineAddress(machine);

            if (machineIP == System.Net.IPAddress.None.ToString())
                return path;

            int machineStart = path.IndexOf(machine);

            return (path.Substring(0, machineStart) + machineIP + path.Substring(machineStart + machine.Length));
        }


        public enum PathScope { IP, MachineName, Unknown };
        /// <summary>
        /// Determines if the path starts with IP or machineName
        /// </summary>
        /// <param name="path">Path to be transformed</param>
        /// <returns>PathScope</returns>
        public static PathScope PathStartsWith(string path)
        {
            if (!path.StartsWith("\\\\"))
                return PathScope.Unknown;

            string machine = FirstTokenOfPath(path);

            try
            {
                System.Net.IPAddress.Parse(machine);
                return PathScope.IP;
            }
            catch { }

            string machineName = STEM.Sys.IO.Net.MachineName(machine);
            string machineIP = STEM.Sys.IO.Net.MachineAddress(machine);

            if (machineName == machine)
                return PathScope.MachineName;
            
            if (machineIP == machine)
                return PathScope.IP;

            if (machine.StartsWith(machineName, StringComparison.InvariantCultureIgnoreCase))
                return PathScope.MachineName;

            return PathScope.Unknown;
        }

        public static string FirstTokenOfPath(string path)
        {
            string p = STEM.Sys.IO.Path.AdjustPath(path);

            string machine = p.TrimStart('\\').TrimStart('/');

            if (machine.IndexOf(System.IO.Path.DirectorySeparatorChar) != -1)
                machine = machine.Substring(0, machine.IndexOf(System.IO.Path.DirectorySeparatorChar));

            if (machine.Contains(":"))
                return Net.MachineIP();

            return machine;
        }

        /// <summary>
        /// Order pathList such that paths nearest to ipAddress are first, based on IPV4 addresses
        /// </summary>
        /// <param name="ipAddress">The basis for proximity</param>
        /// <param name="pathList">The list of paths to be ordered</param>
        /// <returns>List<string> of the ordered pathList</returns>
        public static List<string> OrderPathsWithSubnet(string ipAddress, string[] pathList)
        {
            if (pathList == null || pathList.Length == 0)
                throw new ArgumentNullException(nameof(pathList));

            if (String.IsNullOrEmpty(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));
            
            List<string> orderedPaths = new List<string>();

            string a = STEM.Sys.IO.Net.MachineAddress(ipAddress);
            if (a == null || a == System.Net.IPAddress.None.ToString())
                throw new Exception("Invalid address. (" + ipAddress + ")");

            string tgt = a;

            List<string> availPaths = new List<string>();

            foreach (string path in pathList)
                foreach (string p in ExpandRangedPath(path))
                {
                    availPaths.Add(STEM.Sys.IO.Path.AdjustPath(p));
                }

            Random rnd = new Random();

            availPaths = availPaths.Distinct().OrderBy(i => rnd.Next()).ToList();

            foreach (string path in availPaths)
            {
                if (orderedPaths.Contains(path))
                    continue;
                
                if (!path.StartsWith(SmbPathStart, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                string ip = IPFromPath(path);

                if (tgt.Equals(ip, StringComparison.CurrentCultureIgnoreCase))
                    orderedPaths.Add(path);
            }
            
            foreach (string path in availPaths)
            {
                if (orderedPaths.Contains(path))
                    continue;

                if (!path.StartsWith(SmbPathStart, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                string ip = IPFromPath(path);
                ip = STEM.Sys.IO.Path.GetFileNameWithoutExtension(ip) + ".";

                if (tgt.StartsWith(ip, StringComparison.CurrentCultureIgnoreCase))
                    orderedPaths.Add(path);
            }

            foreach (string path in availPaths)
            {
                if (orderedPaths.Contains(path))
                    continue;

                if (!path.StartsWith(SmbPathStart, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                string ip = IPFromPath(path);
                ip = STEM.Sys.IO.Path.GetFileNameWithoutExtension(ip);
                ip = STEM.Sys.IO.Path.GetFileNameWithoutExtension(ip) + ".";

                if (tgt.StartsWith(ip, StringComparison.CurrentCultureIgnoreCase))
                    orderedPaths.Add(path);
            }

            foreach (string path in availPaths)
            {
                if (orderedPaths.Contains(path))
                    continue;

                if (!path.StartsWith(SmbPathStart, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                string ip = IPFromPath(path);
                ip = STEM.Sys.IO.Path.GetFileNameWithoutExtension(ip);
                ip = STEM.Sys.IO.Path.GetFileNameWithoutExtension(ip);
                ip = STEM.Sys.IO.Path.GetFileNameWithoutExtension(ip) + ".";

                if (tgt.StartsWith(ip, StringComparison.CurrentCultureIgnoreCase))
                    orderedPaths.Add(path);
            }

            foreach (string path in availPaths)
            {
                if (orderedPaths.Contains(path))
                    continue;

                orderedPaths.Add(path);
            }

            return orderedPaths;
        }


        /// <summary>
        /// Order ipList such that ips nearest to ipAddress are first, based on IPV4 addresses
        /// </summary>
        /// <param name="ipAddress">The basis for proximity</param>
        /// <param name="ipList">The list of ips to be ordered</param>
        /// <returns>List<string> of the ordered ipList</returns>
        public static List<string> OrderIPsWithSubnet(string ipAddress, string[] ipList)
        {
            if (ipList == null || ipList.Length == 0)
                throw new ArgumentNullException(nameof(ipList));

            if (String.IsNullOrEmpty(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));

            List<string> orderedIPs = new List<string>();

            string a = STEM.Sys.IO.Net.MachineAddress(ipAddress);
            if (a == null || a == System.Net.IPAddress.None.ToString())
                throw new Exception("Invalid address. (" + ipAddress + ")");

            string tgt = a;

            List<string> availIPs = new List<string>();

            foreach (string ip in ipList)
                foreach (string i in ExpandRangedIP(ip))
                {
                    string x = STEM.Sys.IO.Net.MachineAddress(i);
                    if (x == null || x == System.Net.IPAddress.None.ToString())
                        continue;

                    availIPs.Add(x);
                }

            Random rnd = new Random();

            availIPs = availIPs.Distinct().OrderBy(i => rnd.Next()).ToList();

            foreach (string ip in availIPs)
            {
                if (orderedIPs.Contains(ip))
                    continue;
                
                if (tgt.Equals(ip, StringComparison.CurrentCultureIgnoreCase))
                    orderedIPs.Add(ip);
            }

            foreach (string ip in availIPs)
            {
                if (orderedIPs.Contains(ip))
                    continue;

                string x = STEM.Sys.IO.Path.GetFileNameWithoutExtension(ip) + ".";

                if (tgt.StartsWith(x, StringComparison.CurrentCultureIgnoreCase))
                    orderedIPs.Add(ip);
            }

            foreach (string ip in availIPs)
            {
                if (orderedIPs.Contains(ip))
                    continue;

                string x = STEM.Sys.IO.Path.GetFileNameWithoutExtension(ip);
                x = STEM.Sys.IO.Path.GetFileNameWithoutExtension(x) + ".";

                if (tgt.StartsWith(x, StringComparison.CurrentCultureIgnoreCase))
                    orderedIPs.Add(ip);
            }

            foreach (string ip in availIPs)
            {
                if (orderedIPs.Contains(ip))
                    continue;

                string x = STEM.Sys.IO.Path.GetFileNameWithoutExtension(ip);
                x = STEM.Sys.IO.Path.GetFileNameWithoutExtension(x);
                x = STEM.Sys.IO.Path.GetFileNameWithoutExtension(x) + ".";

                if (tgt.StartsWith(x, StringComparison.CurrentCultureIgnoreCase))
                    orderedIPs.Add(ip);
            }

            foreach (string ip in availIPs)
            {
                if (orderedIPs.Contains(ip))
                    continue;

                orderedIPs.Add(ip);
            }
            
            return orderedIPs;
        }

        /// <summary>
        /// Order rangedPath such that paths nearest to ipAddress are first, based on IPV4 addresses
        /// </summary>
        /// <param name="ipAddress">The basis for proximity</param>
        /// <param name="rangedPath">The list of paths to be ordered</param>
        /// <returns>List<string> of the ordered rangedPath</returns>
        public static List<string> OrderPathsWithSubnet(string rangedPath, string ipAddress)
        {
            if (String.IsNullOrEmpty(rangedPath))
                throw new ArgumentNullException(nameof(rangedPath));

            if (String.IsNullOrEmpty(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));

            return OrderPathsWithSubnet(ipAddress, ExpandRangedPath(rangedPath).ToArray());
        }

        /// <summary>
        /// Order rangedIP such that ips nearest to ipAddress are first, based on IPV4 addresses
        /// </summary>
        /// <param name="ipAddress">The basis for proximity</param>
        /// <param name="rangedIP">The list of ips to be ordered</param>
        /// <returns>List<string> of the ordered rangedIP</returns>
        public static List<string> OrderIPsWithSubnet(string rangedIP, string ipAddress)
        {
            if (String.IsNullOrEmpty(rangedIP))
                throw new ArgumentNullException(nameof(rangedIP));

            if (String.IsNullOrEmpty(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));

            return OrderIPsWithSubnet(ipAddress, ExpandRangedIP(rangedIP).ToArray());
        }

        /// <summary>
        /// Order rangedIP such that ips nearest to targetAddress are first, based on IPV4 addresses
        /// </summary>
        /// <param name="targetAddress">The basis for proximity</param>
        /// <param name="rangedIP">The list of ips to be ordered</param>
        /// <returns>List<string> of the ordered rangedIP</returns>
        public static List<string> NearestToTarget(string rangedIP, string targetAddress)
        {
            if (String.IsNullOrEmpty(rangedIP))
                throw new ArgumentNullException(nameof(rangedIP));

            if (String.IsNullOrEmpty(targetAddress))
                throw new ArgumentNullException(nameof(targetAddress));

            return NearestToTarget(ExpandRangedIP(rangedIP), targetAddress);
        }

        /// <summary>
        /// Order ips such that paths nearest to targetAddress are first, based on IPV4 addresses
        /// </summary>
        /// <param name="targetAddress">The basis for proximity</param>
        /// <param name="ips">The list of ips to be ordered</param>
        /// <returns>List<string> of the ordered rangedIP</returns>
        public static List<string> NearestToTarget(List<string> ips, string targetAddress)
        {
            if (ips == null || ips.Count == 0)
                throw new ArgumentNullException(nameof(ips));

            if (String.IsNullOrEmpty(targetAddress))
                throw new ArgumentNullException(nameof(targetAddress));

            return OrderIPsWithSubnet(targetAddress, ips.ToArray());
        }

        /// <summary>
        /// Select a path from rangedPath that is nearest to the executing machine, based on IPV4 addresses
        /// </summary>
        /// <param name="rangedPath">The list of paths to select from</param>
        /// <returns>the nearest path to the executing machine</returns>
        public static string NearestPath(string rangedPath)
        {
            if (String.IsNullOrEmpty(rangedPath))
                throw new ArgumentNullException(nameof(rangedPath));

            return NearestPath(ExpandRangedPath(rangedPath), STEM.Sys.IO.Net.MachineIP());
        }

        /// <summary>
        /// Select a path from paths that is nearest to the executing machine, based on IPV4 addresses
        /// </summary>
        /// <param name="paths">The list of paths to select from</param>
        /// <returns>the nearest path to the executing machine</returns>
        public static string NearestPath(List<string> paths)
        {
            if (paths == null || paths.Count == 0)
                throw new ArgumentNullException(nameof(paths));

            return NearestPath(paths, STEM.Sys.IO.Net.MachineIP());
        }

        /// <summary>
        /// Select a path from rangedPath that is nearest to ipAddress, based on IPV4 addresses
        /// </summary>
        /// <param name="ipAddress">The basis for proximity</param>
        /// <param name="rangedPath">The list of paths to select from</param>
        /// <returns>the nearest path to ipAddress</returns>
        public static string NearestPath(string rangedPath, string ipAddress)
        {
            if (String.IsNullOrEmpty(rangedPath))
                throw new ArgumentNullException(nameof(rangedPath));

            if (String.IsNullOrEmpty(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));

            return NearestPath(ExpandRangedPath(rangedPath), ipAddress);
        }

        /// <summary>
        /// Select a path from paths that is nearest to ipAddress, based on IPV4 addresses
        /// </summary>
        /// <param name="ipAddress">The basis for proximity</param>
        /// <param name="paths">The list of paths to select from</param>
        /// <returns>the nearest path to ipAddress</returns>
        public static string NearestPath(List<string> paths, string ipAddress)
        {
            if (paths == null || paths.Count == 0)
                throw new ArgumentNullException(nameof(paths));

            if (String.IsNullOrEmpty(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));

            return OrderPathsWithSubnet(ipAddress, paths.ToArray()).FirstOrDefault();
        }

        /// <summary>
        /// Order rangedIP such that ips nearest to path are first, based on IPV4 addresses
        /// </summary>
        /// <param name="path">The basis for proximity</param>
        /// <param name="rangedIP">The list of ips to be ordered</param>
        /// <returns>List<string> of the ordered rangedIP</returns>
        public static List<string> NearestToPath(string rangedIP, string path)
        {
            return NearestToPath(ExpandRangedIP(rangedIP), path);
        }

        /// <summary>
        /// Order ips such that ips nearest to path are first, based on IPV4 addresses
        /// </summary>
        /// <param name="path">The basis for proximity</param>
        /// <param name="ips">The list of ips to be ordered</param>
        /// <returns>List<string> of the ordered ips</returns>
        public static List<string> NearestToPath(List<string> ips, string path)
        {
            return NearestToTarget(ips, IPFromPath(path));
        }

        /// <summary>
        /// Order rangedIP such that ips nearest to the executing machine are first, based on IPV4 addresses
        /// </summary>
        /// <param name="rangedIP">The list of ips to be ordered</param>
        /// <returns>List<string> of the ordered rangedIP</returns>
        public static string NearestToIP(string rangedIP)
        {
            return NearestToIP(rangedIP, STEM.Sys.IO.Net.MachineIP());
        }

        /// <summary>
        /// Order rangedIP such that ips nearest to ipAddress are first, based on IPV4 addresses
        /// </summary>
        /// <param name="ipAddress">The basis for proximity</param>
        /// <param name="rangedIP">The list of ips to be ordered</param>
        /// <returns>List<string> of the ordered rangedIP</returns>
        public static string NearestToIP(string rangedIP, string ipAddress)
        {
            return NearestToIP(ExpandRangedIP(rangedIP), ipAddress);
        }

        /// <summary>
        /// Order ips such that ips nearest to ipAddress are first, based on IPV4 addresses
        /// </summary>
        /// <param name="ipAddress">The basis for proximity</param>
        /// <param name="ips">The list of ips to be ordered</param>
        /// <returns>List<string> of the ordered ips</returns>
        public static string NearestToIP(List<string> ips, string ipAddress)
        {
            return NearestToTarget(ips, ipAddress).FirstOrDefault();
        }

        /// <summary>
        /// Returns a clean, platform adjusted path
        /// </summary>
        /// <param name="path">Path to be evaluated</param>
        /// <returns>platform adjusted path</returns>
        public static string AdjustPath(string path)
        {
            if (path == null)
                return "";

            if (System.IO.Path.DirectorySeparatorChar == '/')
                path = path.Replace('\\', System.IO.Path.DirectorySeparatorChar).TrimEnd(System.IO.Path.DirectorySeparatorChar);
            else
                path = path.Replace('/', System.IO.Path.DirectorySeparatorChar).TrimEnd(System.IO.Path.DirectorySeparatorChar);

            return path;
        }

        /// <summary>
        /// Returns the IPv4 address based on the first token in a path, where path starts with a '/' or '\', otherwise it returns the executing machine address
        /// </summary>
        /// <param name="path">Path to be evaluated</param>
        /// <returns>the IPv4 address based on the first token in a path, where path starts with a '/' or '\'</returns>
        public static string IPFromPath(string path)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            path = STEM.Sys.IO.Path.AdjustPath(path);
                        
            if (path != null && (path.StartsWith("\\", StringComparison.InvariantCultureIgnoreCase) || path.StartsWith("/", StringComparison.InvariantCultureIgnoreCase)))
            {
                string ip = path.TrimStart('\\').TrimStart('/');

                if (ip.IndexOf(System.IO.Path.DirectorySeparatorChar) == -1)
                    return STEM.Sys.IO.Net.MachineAddress(ip);

                return STEM.Sys.IO.Net.MachineAddress(ip.Substring(0, ip.IndexOf(System.IO.Path.DirectorySeparatorChar)));
            }

            return Net.MachineIP();
        }

        /// <summary>
        /// Evaluates a rangedIP to check if it contains singleIP
        /// </summary>
        /// <param name="rangedIP"></param>
        /// <param name="singleIP"></param>
        /// <returns>True if rangedIP contains singleIP</returns>
        public static bool RangedIPContains(string rangedIP, string singleIP)
        {
            if (String.IsNullOrEmpty(rangedIP))
                return false;

            if (String.IsNullOrEmpty(singleIP))
                return false;

            foreach (string s in ExpandRangedIP(rangedIP))
                if (s.Equals(singleIP, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            foreach (string s in ExpandRangedIP(rangedIP))
                if (STEM.Sys.IO.Net.MachineAddress(s).Equals(singleIP, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }

        /// <summary>
        /// Evaluates a rangedPath to check if it contains singlePath
        /// </summary>
        /// <param name="rangedPath"></param>
        /// <param name="singlePath"></param>
        /// <returns>True if rangedPath contains singlePath</returns>
        public static bool RangedPathContains(string rangedPath, string singlePath)
        {
            if (String.IsNullOrEmpty(rangedPath))
                return false;

            if (String.IsNullOrEmpty(singlePath))
                return false;

            try
            {                                
                singlePath = STEM.Sys.IO.Path.AdjustPath(singlePath);
                rangedPath = STEM.Sys.IO.Path.AdjustPath(rangedPath);
                
                if (singlePath == (SmbPathStart) && rangedPath == (SmbPathStart))
                    return true;
                
                if (singlePath == (SmbPathStart) || rangedPath == (SmbPathStart))
                    return false;

                if (singlePath.StartsWith(SmbPathStart, StringComparison.InvariantCultureIgnoreCase))
                {
                    string singleIP = singlePath.TrimStart(System.IO.Path.DirectorySeparatorChar);
                    singleIP = singleIP.Substring(0, singleIP.IndexOf(System.IO.Path.DirectorySeparatorChar));

                    foreach (string p in rangedPath.Split(new string[] { "##" }, StringSplitOptions.RemoveEmptyEntries))
                        if (p.StartsWith(SmbPathStart, StringComparison.InvariantCultureIgnoreCase))
                        {
                            string rangedIP = p.TrimStart(System.IO.Path.DirectorySeparatorChar);
                            rangedIP = rangedIP.Substring(0, rangedIP.IndexOf(System.IO.Path.DirectorySeparatorChar));

                            if (p.Replace(rangedIP, singleIP).ToUpper(System.Globalization.CultureInfo.CurrentCulture).TrimEnd(System.IO.Path.DirectorySeparatorChar) == singlePath.ToUpper(System.Globalization.CultureInfo.CurrentCulture).TrimEnd(System.IO.Path.DirectorySeparatorChar))
                                if (RangedIPContains(rangedIP, singleIP))
                                    return true;
                        }
                        else if (p.ToUpper(System.Globalization.CultureInfo.CurrentCulture).TrimEnd(System.IO.Path.DirectorySeparatorChar) == singlePath.ToUpper(System.Globalization.CultureInfo.CurrentCulture).TrimEnd(System.IO.Path.DirectorySeparatorChar))
                        {
                            return true;
                        }
                }
                else
                {
                    foreach (string p in rangedPath.Split(new string[] { "##" }, StringSplitOptions.RemoveEmptyEntries))
                        if (!p.StartsWith(SmbPathStart, StringComparison.InvariantCultureIgnoreCase))
                            if (p.ToUpper(System.Globalization.CultureInfo.CurrentCulture).TrimEnd(System.IO.Path.DirectorySeparatorChar) == singlePath.ToUpper(System.Globalization.CultureInfo.CurrentCulture).TrimEnd(System.IO.Path.DirectorySeparatorChar))
                                return true;
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Expand a ranged path to the list of paths
        /// </summary>
        /// <param name="path"></param>
        /// <param name="expandWildcards"></param>
        /// <returns>The list of expanded paths</returns>
        public static List<string> ExpandRangedPath(string path, bool expandWildcards = false)
        {
            List<string> paths = new List<string>();

            if (path == null)
                return paths;
            
            foreach (string p in path.Split(new string[] { "##" }, StringSplitOptions.RemoveEmptyEntries))
            {
                string adj = STEM.Sys.IO.Path.AdjustPath(p);

                if (adj.StartsWith(SmbPathStart, StringComparison.InvariantCultureIgnoreCase))
                {
                    string rangedIP = adj.TrimStart(System.IO.Path.DirectorySeparatorChar);

                    if (rangedIP.IndexOf(System.IO.Path.DirectorySeparatorChar) == -1)
                    {
                        paths.Add(adj);
                        continue;
                    }

                    rangedIP = rangedIP.Substring(0, rangedIP.IndexOf(System.IO.Path.DirectorySeparatorChar));
                    List<string> ipList = _ExpandRangedIP(rangedIP, expandWildcards, false);

                    foreach (string ip in ipList)
                        paths.Add(adj.Replace(rangedIP, ip));
                }
                else
                {
                    paths.Add(adj);
                }
            }

            return paths.Distinct().ToList();
        }
        
        static Dictionary<string, int> _NumberMap = new Dictionary<string, int>();

        static Path()
        {
            for (int i = 0; i < 10000; i++)
            {
                _NumberMap[i.ToString(System.Globalization.CultureInfo.CurrentCulture)] = i;
                _NumberMap[i.ToString("D2", System.Globalization.CultureInfo.CurrentCulture)] = i;
                _NumberMap[i.ToString("D3", System.Globalization.CultureInfo.CurrentCulture)] = i;
                _NumberMap[i.ToString("D4", System.Globalization.CultureInfo.CurrentCulture)] = i;
            }
        }


        static string IPAddressNone = System.Net.IPAddress.None.ToString();
        static Dictionary<string, List<string>> _ExpandedIP = new Dictionary<string, List<string>>();
        static Dictionary<string, List<string>> _ExpandedName = new Dictionary<string, List<string>>();

        /// <summary>
        /// Expands the ranged IP/MachineName to a list of machine names
        /// </summary>
        /// <param name="rangedName"></param>
        /// <returns>The list of expanded machine names</returns>
        public static List<string> ExpandRangedMachineName(string ranged, bool expandWildcards = false)
        {
            return _ExpandRangedMachineName(ranged, expandWildcards, true);
        }

        static List<string> _ExpandRangedMachineName(string ranged, bool expandWildcards, bool convertIPsToMachineNames)
        {
            if (String.IsNullOrEmpty(ranged))
                return new List<string>();

            if (convertIPsToMachineNames && _ExpandedName.ContainsKey(ranged))
                return _ExpandedName[ranged].ToList();

            List<string> names = new List<string>();
            if (ranged.Contains("#"))
            {
                foreach (string s in ranged.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    names.AddRange(_ExpandRangedMachineName(s, expandWildcards, convertIPsToMachineNames));

                    if (names.Count > 10000)
                        throw new Exception("Expansion exceeds 10,000. Consider using STEM.Sys.IO.Path.RangedIPContains or STEM.Sys.IO.Path.RangedPathContains.");
                }
            }
            else if (ranged.Contains("(") && ranged.Contains(")"))
            {
                int startIndex = ranged.IndexOf('(');
                int endIndex = ranged.IndexOf(')');

                string head = ranged.Substring(0, startIndex);
                string tail = ranged.Substring(endIndex + 1);

                string span = ranged.Substring(startIndex + 1, endIndex - startIndex - 1);

                List<string> allSegments = new List<string>();
                foreach (string o in span.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (o.Contains("-"))
                    {
                        string[] oo = o.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                        string start = oo[0];
                        string end = oo[1];

                        int a = _NumberMap[start];
                        while (a <= _NumberMap[end])
                        {
                            allSegments.Add(a.ToString("".PadLeft(start.Length, '0'), System.Globalization.CultureInfo.CurrentCulture));
                            a++;
                        }
                    }
                    else
                    {
                        allSegments.Add(o);
                    }
                }

                foreach (string segment in allSegments.Distinct().OrderBy(i => i))
                {
                    if (tail.Contains("("))
                    {
                        names.AddRange(_ExpandRangedMachineName(head + segment + tail, expandWildcards, convertIPsToMachineNames));
                    }
                    else
                    {
                        names.Add(head + segment + tail);
                    }
                }
            }
            else
            {
                string[] octets = ranged.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                if (octets.Length == 4)
                {
                    names = _ExpandRangedIP(ranged, expandWildcards, false);
                }
                else
                {
                    names.Add(ranged);
                }
            }

            List<string> ret = new List<string>();
            foreach (string i in names)
            {
                if (i.StartsWith("[", StringComparison.InvariantCultureIgnoreCase))
                {
                    ret.Add(i);
                    continue;
                }

                if (i != IPAddressNone)
                {
                    if (convertIPsToMachineNames)
                    {
                        string a = STEM.Sys.IO.Net.MachineName(i);
                        if (a != null)
                        {
                            ret.Add(a);
                        }
                        else
                        {
                            ret.Add(IPAddressNone);
                        }
                    }
                    else
                    {
                        ret.Add(i);
                    }
                }
                else
                {
                    ret.Add(i);
                }
            }

            ret = ret.Distinct().ToList();
            
            if (convertIPsToMachineNames &&
                ret.Count > 0 &&
                !ret.Contains(IPAddressNone) &&
                !ret.Exists(i => i.Contains("(") || i.Contains(")")) && 
                !ret.Exists(i => i.StartsWith("[")))
            {
                lock (_ExpandedName)
                    _ExpandedName[ranged] = ret;
            }
            
            if (ret.Contains(IPAddressNone))
                ret.Remove(IPAddressNone);

            return ret;
        }

        /// <summary>
        /// Expands the ranged IP/MachineName to a list of ips
        /// </summary>
        /// <param name="ranged"></param>
        /// <param name="expandWildcards"></param>
        /// <returns>The list of expanded ips</returns>
        public static List<string> ExpandRangedIP(string ranged, bool expandWildcards = false)
        {
            return _ExpandRangedIP(ranged, expandWildcards, true);
        }

        static List<string> _ExpandRangedIP(string ranged, bool expandWildcards, bool convertMachineNamesToIPs)
        {
            if (String.IsNullOrEmpty(ranged))
                return new List<string>();
                        
            if (convertMachineNamesToIPs && _ExpandedIP.ContainsKey(ranged))
                return _ExpandedIP[ranged].ToList();

            List<string> ips = new List<string>();
            if (ranged.Contains("#"))
            {
                foreach (string s in ranged.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    ips.AddRange(_ExpandRangedIP(s, expandWildcards, convertMachineNamesToIPs));

                    if (ips.Count > 10000)
                        throw new Exception("Expansion exceeds 10,000. Consider using STEM.Sys.IO.Path.RangedIPContains or STEM.Sys.IO.Path.RangedPathContains.");
                }
            }
            else if (ranged.Contains("(") && ranged.Contains(")"))
            {
                ips = _ExpandRangedMachineName(ranged, expandWildcards, false);
            }
            else
            {
                string[] octets = ranged.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                if (octets.Length == 4)
                {
                    foreach (int a in ExpandOctet(octets[0], expandWildcards))
                        foreach (int b in ExpandOctet(octets[1], expandWildcards))
                            foreach (int c in ExpandOctet(octets[2], expandWildcards))
                                foreach (int d in ExpandOctet(octets[3], expandWildcards))
                                {
                                    ips.Add(a + "." + b + "." + c + "." + d);

                                    if (ips.Count > 10000)
                                        throw new Exception("Expansion exceeds 10,000. Consider using STEM.Sys.IO.Path.RangedIPContains or STEM.Sys.IO.Path.RangedPathContains.");
                                }
                }
                else
                {
                    ips.Add(ranged);
                }
            }

            List<string> ret = new List<string>();
            foreach (string i in ips)
            {
                if (i.StartsWith("[", StringComparison.InvariantCultureIgnoreCase))
                {
                    ret.Add(i);
                    continue;
                }

                if (i != IPAddressNone)
                {
                    if (convertMachineNamesToIPs)
                    {
                        string a = STEM.Sys.IO.Net.MachineAddress(i);
                        if (a != null)
                        {
                            ret.Add(a);
                        }
                        else
                        {
                            ret.Add(IPAddressNone);
                        }
                    }
                    else
                    {
                        ret.Add(i);
                    }
                }
                else
                {
                    ret.Add(i);
                }
            }

            ret = ret.Distinct().ToList();
            
            if (convertMachineNamesToIPs &&
                ret.Count > 0 &&
                !ret.Contains(IPAddressNone) &&
                !ret.Exists(i => i.Contains("(") || i.Contains(")")) &&
                !ret.Exists(i => i.StartsWith("[")))
            {
                lock (_ExpandedIP)
                    _ExpandedIP[ranged] = ret;
            }
            
            if (ret.Contains(IPAddressNone))
                ret.Remove(IPAddressNone);

            return ret;
        }
        
        static IEnumerable<int> ExpandOctet(string octet, bool allowWildcards)
        {
            List<int> allOcts = new List<int>();
            foreach (string o in octet.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (o.Contains("-"))
                {
                    string[] oo = o.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                    string start = oo[0];
                    string end = oo[1];

                    if (allowWildcards)
                    {
                        List<string> wcr = ParseWildcards(oo[0]);
                        foreach (string s in wcr)
                            allOcts.Add(_NumberMap[s]);
                        start = wcr.Last();
                        wcr = ParseWildcards(oo[1]);
                        foreach (string s in wcr)
                            allOcts.Add(_NumberMap[s]);
                        end = wcr.First();
                    }

                    int a = _NumberMap[start];
                    while (a <= _NumberMap[end])
                    {
                        allOcts.Add(a);
                        a++;
                    }
                }
                else
                {
                    List<string> wcr = ParseWildcards(o);
                    foreach (string s in wcr)
                        allOcts.Add(_NumberMap[s]);
                }
            }

            return allOcts.Distinct().OrderBy(i => i);
        }

        static List<string> ParseWildcards(string octet)
        {
            List<string> wild = new List<string>();
            wild.Add(octet);

            while (wild.Exists(i => i.Contains("*")))
                foreach (string ws in wild.ToList())
                {
                    wild.Remove(ws);

                    string w = ws;
                    while (w.Contains("**"))
                        w = ws.Replace("**", "*");

                    if (w.StartsWith("*", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int depth = 256;
                        if (w.Length == 2)
                            depth = 25;
                        if (w.Length == 3)
                            depth = 2;

                        string cReplace = w.Substring(1);
                        for (int c = 0; c < depth; c++)
                            wild.Add(c.ToString(System.Globalization.CultureInfo.CurrentCulture) + cReplace);
                    }
                    else if (w.EndsWith("*", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int depth = 256;
                        if (w.Length == 2)
                            depth = 56;
                        if (w.Length == 3)
                            depth = 10;

                        string cReplace = w.Remove(w.Length - 1);
                        for (int c = 0; c < depth; c++)
                            wild.Add(cReplace + c.ToString("0", System.Globalization.CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        int depth = 56;
                        if (w.Length == 3)
                            depth = 10;

                        string[] cReplace = w.Split(new char[] { '*' });
                        for (int c = 0; c < depth; c++)
                            wild.Add(cReplace[0] + c.ToString("0", System.Globalization.CultureInfo.CurrentCulture) + cReplace[1]);
                    }
                }

            while (wild.Exists(i => i.Contains("?")))
                foreach (string w in wild.ToList())
                {
                    if (w.StartsWith("?", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string cReplace = w.Substring(1);
                        for (int c = 0; c < 10; c++)
                            wild.Add(c.ToString("0", System.Globalization.CultureInfo.CurrentCulture) + cReplace);
                    }
                    else if (w.EndsWith("?", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string cReplace = w.Remove(w.Length - 1);
                        for (int c = 0; c < 10; c++)
                            wild.Add(cReplace + c.ToString("0", System.Globalization.CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        string[] cReplace = w.Split(new char[] { '?' });
                        for (int c = 0; c < 10; c++)
                            wild.Add(cReplace[0] + c.ToString("0", System.Globalization.CultureInfo.CurrentCulture) + cReplace[1]);
                    }

                    wild.Remove(w);
                }

            List<string> values = new List<string>();
            foreach (string v in wild)
            {
                try
                {
                    int i = _NumberMap[v];
                    if (i < 256)
                        if (!values.Contains(v))
                            values.Add(v);
                }
                catch { }
            }

            return values;
        }

        /// <summary>
        /// Builds a Regex representing the exclusive filter set from fullFilterSet
        /// </summary>
        /// <param name="fullFilterSet">The STEM File or Directory filter set (i.e. *.dll|*.exe|!Microsoft*)</param>
        /// <returns>Regex representing the exclusive filter set</returns>
        public static Regex BuildExclusiveFilter(string fullFilterSet)
        {
            string f = "";
            if (fullFilterSet != null)
                foreach (string pattern in fullFilterSet.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    if (pattern.Trim().StartsWith("<>", StringComparison.InvariantCultureIgnoreCase) || pattern.Trim().StartsWith("!", StringComparison.InvariantCultureIgnoreCase))
                        f += "(" + BuildRegExPattern(pattern.Trim().Replace("<>", "").Replace("!", "")) + ")|";

            f = f.TrimEnd(new char[] { '|' });

            if (f.Trim().Length > 0)
                return new Regex(f, RegexOptions.IgnoreCase);

            return null;
        }

        /// <summary>
        /// Builds a Regex representing the inclusive filter set from fullFilterSet
        /// </summary>
        /// <param name="fullFilterSet">The STEM File or Directory filter set (i.e. *.dll|*.exe|!Microsoft*)</param>
        /// <returns>Regex representing the inclusive filter set</returns>
        public static Regex BuildInclusiveFilter(string fullFilterSet)
        {
            string f = "";
            if (fullFilterSet != null)
                foreach (string pattern in fullFilterSet.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    if (!pattern.Trim().StartsWith("<>", StringComparison.InvariantCultureIgnoreCase) && !pattern.Trim().StartsWith("!", StringComparison.InvariantCultureIgnoreCase))
                        f += "(" + BuildRegExPattern(pattern.Trim().Replace("<>", "").Replace("!", "")) + ")|";

            f = f.TrimEnd(new char[] { '|' });

            if (f.Trim().Length > 0)
                return new Regex(f, RegexOptions.IgnoreCase);

            return null;
        }

        /// <summary>
        /// Selects the subset of listing that match the regex
        /// </summary>
        /// <param name="listing"></param>
        /// <param name="regex"></param>
        /// <returns>The matching subset from listing</returns>
        public static List<string> WhereStringsMatch(List<string> listing, Regex regex)
        {
            if (listing == null)
                return new List<string>();

            if (regex == null)
                return listing.ToList();

            return listing.Where(i => regex.IsMatch(i.ToUpper(System.Globalization.CultureInfo.CurrentCulture))).ToList();
        }

        /// <summary>
        /// Selects the subset of listing that do NOT match the regex
        /// </summary>
        /// <param name="listing"></param>
        /// <param name="regex"></param>
        /// <returns>The non-matching subset from listing</returns>
        public static List<string> WhereStringsNotMatch(List<string> listing, Regex regex)
        {
            if (listing == null)
                return new List<string>();

            if (regex == null)
                return listing.ToList();

            return listing.Where(i => !regex.IsMatch(i.ToUpper(System.Globalization.CultureInfo.CurrentCulture))).ToList();
        }


        /// <summary>
        /// Selects the subset of listing that match the regex (checking only the last token in each string)
        /// </summary>
        /// <param name="listing"></param>
        /// <param name="regex"></param>
        /// <returns>The matching subset from listing</returns>
        public static List<string> WhereFilesMatch(List<string> listing, Regex regex)
        {
            if (listing == null)
                return new List<string>();

            if (regex == null)
                return listing.ToList();

            return listing.Where(i => regex.IsMatch(STEM.Sys.IO.Path.GetFileName(i).ToUpper())).ToList();
        }


        /// <summary>
        /// Selects the subset of listing that do NOT match the regex (checking only the last token in each string)
        /// </summary>
        /// <param name="listing"></param>
        /// <param name="regex"></param>
        /// <returns>The non-matching subset from listing</returns>
        public static List<string> WhereFilesNotMatch(List<string> listing, Regex regex)
        {
            if (listing == null)
                return new List<string>();

            if (regex == null)
                return listing.ToList();

            return listing.Where(i => !regex.IsMatch(STEM.Sys.IO.Path.GetFileName(i).ToUpper())).ToList();
        }

        /// <summary>
        /// Checks str against the compoundFilter for a match
        /// </summary>
        /// <param name="str">The string to be evaluated</param>
        /// <param name="compoundFilter">The STEM File or Directory filter set (i.e. *.dll|*.exe|!Microsoft*)</param>
        /// <returns>True if str is a match</returns>
        public static bool StringMatches(string str, string compoundFilter)
        {
            if (compoundFilter == null)
                return true;

            try
            {
                Regex exclusiveFilterRegex = Path.BuildExclusiveFilter(compoundFilter);
                Regex inclusiveFilterRegex = Path.BuildInclusiveFilter(compoundFilter);

                if (exclusiveFilterRegex == null || !StringMatches(str, exclusiveFilterRegex))
                    if (StringMatches(str, inclusiveFilterRegex))
                        return true;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Checks str against regex for a match
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regex"></param>
        /// <returns>True if regex is null or matches</returns>
        public static bool StringMatches(string str, Regex regex)
        {
            if (String.IsNullOrEmpty(str))
                throw new ArgumentNullException(nameof(str));

            if (regex == null)
                return true;

            try
            {
                if (regex.IsMatch(str.ToUpper(System.Globalization.CultureInfo.CurrentCulture)))
                    return true;
            }
            catch { }

            return false;
        }
        
        static string BuildRegExPattern(string filter)
        {
            string capFilter = filter.ToUpper(System.Globalization.CultureInfo.CurrentCulture);

            string[] starParts = capFilter.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);

            string regEx = "^";
            if (capFilter.StartsWith("*", StringComparison.InvariantCultureIgnoreCase))
                regEx += ".*";

            int x = 0;
            foreach (string s in starParts)
            {
                if (x > 0)
                    regEx += ".*";

                string[] questionParts = s.Split(new char[] { '?' }, StringSplitOptions.None);

                if (s.StartsWith("?", StringComparison.InvariantCultureIgnoreCase))
                    regEx += ".?";

                int y = 0;
                foreach (string q in questionParts)
                {
                    if (y > 0)
                        regEx += ".?";

                    regEx += "(" + Regex.Escape(q) + "){1}";

                    y++;
                }

                x++;
            }

            if (capFilter.EndsWith("*", StringComparison.InvariantCultureIgnoreCase))
                regEx += ".*";

            if (capFilter.EndsWith("?", StringComparison.InvariantCultureIgnoreCase))
                regEx += ".?";

            return regEx + "$";
        }
    }
}