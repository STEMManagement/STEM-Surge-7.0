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
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace STEM.Sys.IO
{
    public static class Net
    {
        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;

            using (Ping pinger = new Ping())
            {
                try
                {
                    PingReply reply = pinger.Send(nameOrAddress, 3000);

                    pingable = reply.Status == IPStatus.Success;
                }
                catch
                {
                    // Discard Exceptions and return false;
                }
            }

            return pingable;
        }

        static string IPAddressNone = System.Net.IPAddress.None.ToString();
        public const string EmptyAddress = "0.0.0.0";

        public static string LoopbackAddress()
        {
            return IPAddress.Loopback.ToString();
        }

        public static int AvailableLocalTcpPort()
        {
            TcpListener listener = null;
            int port = 0;
            try
            {
                listener = new TcpListener(IPAddress.Loopback, 0);
                listener.Start();
                port = ((IPEndPoint)listener.LocalEndpoint).Port;
            }
            catch { }
            finally
            {
                if (listener != null)
                {
                    try
                    {
                        listener.Stop();
                    }
                    catch { }
                }
            }

            return port;
        }

        public static bool IsLocal(string address)
        {
            if (String.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            if (address.Equals("localhost", StringComparison.InvariantCultureIgnoreCase) || address == "127.0.0.1")
                return true;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                if (nic.Name.ToUpper(System.Globalization.CultureInfo.CurrentCulture) == address.ToUpper(System.Globalization.CultureInfo.CurrentCulture))
                    return true;
                else if (STEM.Sys.IO.Net.V4Address(nic) == System.Net.IPAddress.Parse(address))
                    return true;

            return false;
        }

        internal static List<string> AllAdaptersByName()
        {
            List<string> ret = new List<string>();

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                ret.Add(ni.Name);

            return ret;
        }

        internal static List<string> EnabledAdaptersByName()
        {
            List<string> ret = new List<string>();

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                if (ni.OperationalStatus == OperationalStatus.Up)
                    ret.Add(ni.Name);

            return ret;
        }

        internal static NetworkInterface Interface(System.Net.IPAddress localAddress)
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    if (ip.Address.ToString() == localAddress.ToString())
                        return ni;

            return null;
        }

        internal static NetworkInterface Interface(string localAdapterName)
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                if (ni.Name.ToUpper(System.Globalization.CultureInfo.CurrentCulture) == localAdapterName.ToUpper(System.Globalization.CultureInfo.CurrentCulture))
                    return ni;

            return null;
        }

        internal static NetworkInterface AdapterByIP(string address)
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                if (nic.Name.ToUpper(System.Globalization.CultureInfo.CurrentCulture) == address.ToUpper(System.Globalization.CultureInfo.CurrentCulture))
                    return nic;

            return null;
        }

        internal static System.Net.IPAddress LocalAddressFromRemoteAddress(string remoteAddress)
        {
            try
            {
                if (remoteAddress.Trim().Length == 0 || remoteAddress.Trim().ToUpper(System.Globalization.CultureInfo.CurrentCulture) == "ANY")
                {
                    return System.Net.IPAddress.Any;
                }
                else if (remoteAddress.Equals("localhost", StringComparison.InvariantCultureIgnoreCase) || remoteAddress == "127.0.0.1")
                {
                    return System.Net.IPAddress.Parse(MachineIP());
                }
                else
                {
                    NetworkInterface nic = STEM.Sys.IO.Net.Interface(remoteAddress);
                    if (nic == null)
                        nic = STEM.Sys.IO.Net.Interface(System.Net.IPAddress.Parse(remoteAddress));

                    System.Net.IPAddress a = null;
                    if (nic == null)
                    {
                        a = V4Address(remoteAddress);

                        if (a == null)
                            a = V6Address(remoteAddress);
                    }
                    else
                    {
                        a = STEM.Sys.IO.Net.V4Address(nic);

                        if (a == null)
                            a = STEM.Sys.IO.Net.V6Address(nic);
                    }

                    if (a != null)
                        return a;
                }
            }
            catch { }

            return System.Net.IPAddress.None;
        }

        internal static System.Net.IPAddress V4Address(NetworkInterface localInterface)
        {
            foreach (UnicastIPAddressInformation ip in localInterface.GetIPProperties().UnicastAddresses)
                if (!ip.Address.AddressFamily.ToString().Contains("V6"))
                    return ip.Address;

            return null;
        }

        internal static List<System.Net.IPAddress> V4Address()
        {
            List<System.Net.IPAddress> list = new List<System.Net.IPAddress>();
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                if ((ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback) &&
                    (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up))
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        if (!ip.Address.AddressFamily.ToString().Contains("V6"))
                            list.Add(ip.Address);

            return list;
        }

        internal static System.Net.IPAddress V6Address(NetworkInterface localInterface)
        {
            foreach (UnicastIPAddressInformation ip in localInterface.GetIPProperties().UnicastAddresses)
                if (ip.Address.AddressFamily.ToString().Contains("V6"))
                    return ip.Address;

            return null;
        }

        internal static List<System.Net.IPAddress> V6Address()
        {
            List<System.Net.IPAddress> list = new List<System.Net.IPAddress>();
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                if ((ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback) &&
                    (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up))
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        if (ip.Address.AddressFamily.ToString().Contains("V6"))
                            list.Add(ip.Address);

            return list;
        }

        internal static System.Net.IPAddress V4Address(string externalIP)
        {
            foreach (System.Net.IPAddress ip in V4Address())
                if (!ip.AddressFamily.ToString().Contains("V6"))
                {
                    if (externalIP.Equals("localhost", StringComparison.InvariantCultureIgnoreCase) || externalIP == "127.0.0.1" || externalIP == ip.ToString())
                        return ip;

                    if (PingHost(externalIP))
                        return ip;
                }

            return System.Net.IPAddress.None;
        }

        internal static System.Net.IPAddress V6Address(string externalIP)
        {
            foreach (System.Net.IPAddress ip in V4Address())
                if (ip.AddressFamily.ToString().Contains("V6"))
                {
                    if (externalIP.Equals("localhost", StringComparison.InvariantCultureIgnoreCase) || externalIP == "127.0.0.1" || externalIP == ip.ToString())
                        return ip;

                    if (PingHost(externalIP))
                        return ip;
                }

            return System.Net.IPAddress.None;
        }

        static string LocalInterfaceIP = "0.0.0.0";
        public static string MachineIP()
        {
            while (LocalInterfaceIP == "0.0.0.0" || LocalInterfaceIP == "255.255.255.255" || LocalInterfaceIP == "127.0.0.1")
            {
                LocalInterfaceIP = V4Address("127.0.0.1").ToString();
                System.Threading.Thread.Sleep(10);
            }

            return LocalInterfaceIP;
        }

        public static string MachineIP(string remoteIP)
        {
            return V4Address(remoteIP).ToString();
        }

        public static string LocalToUnc(string local)
        {
            if (String.IsNullOrEmpty(local))
                throw new ArgumentNullException(nameof(local));

            if (local.StartsWith("" + System.IO.Path.DirectorySeparatorChar, StringComparison.InvariantCultureIgnoreCase))
                return local;

            if (local[1] != ':')
                return local;

            if (local.Length < 3)
                return local;

            return System.IO.Path.DirectorySeparatorChar + System.IO.Path.DirectorySeparatorChar + MachineIP() + local.Substring(2);
        }


        static Net()
        {
            STEM.Sys.Global.ThreadPool.BeginAsync(new System.Threading.ThreadStart(RefreshResolved), TimeSpan.FromMinutes(1));
        }

        static void RefreshResolved()
        {
            try
            {
                foreach (string k in _Resolved.Keys.ToList())
                {
                    if (k == IPAddressNone)
                    {
                        _Resolved.Remove(k);
                        continue;
                    }

                    if ((DateTime.UtcNow - _Resolved[k].LastResolved).TotalMinutes > 1)
                        try
                        {
                            string name = System.Net.Dns.GetHostEntry(k).HostName;
                            string ip = System.Net.Dns.GetHostAddresses(name).FirstOrDefault(i => i.AddressFamily == AddressFamily.InterNetwork).ToString();

                            if (name != _Resolved[k].MachineName || ip != _Resolved[k].IPV4Address)
                            {
                                Resolved n = new Resolved { IPV4Address = ip, MachineName = name, LastResolved = DateTime.UtcNow };

                                lock (_Resolved)
                                {
                                    foreach (string key in _Resolved.Keys.ToList())
                                    {
                                        if (k != key)
                                            if (_Resolved[k] == _Resolved[key])
                                                _Resolved.Remove(key);
                                    }

                                    if (name != IPAddressNone)
                                        _Resolved[name] = n;

                                    if (ip != IPAddressNone)
                                        _Resolved[ip] = n;

                                    if (k != IPAddressNone)
                                        _Resolved[k] = n;
                                }
                            }

                            _Resolved[k].LastResolved = DateTime.UtcNow;
                        }
                        catch { }
                }
            }
            catch { }
        }
        
        class Resolved
        {
            public string IPV4Address { get; set; }
            public string MachineName { get; set; }
            public DateTime LastResolved { get; set; }
        }

        static Dictionary<string, Resolved> _Resolved = new Dictionary<string, Resolved>(StringComparer.InvariantCultureIgnoreCase);
        public static string MachineAddress(string nameOrAddress)
        {
            return _MachineAddress(nameOrAddress, false);
        }

        static string _MachineAddress(string nameOrAddress, bool forceMachineNameLookup)
        {
            if (String.IsNullOrEmpty(nameOrAddress))
                throw new ArgumentNullException(nameof(nameOrAddress));

            try
            {
                if (_Resolved.ContainsKey(nameOrAddress))
                    return _Resolved[nameOrAddress].IPV4Address;
            }
            catch { }

            try
            {
                if (nameOrAddress.Trim().Length == 0 || nameOrAddress.Trim().ToUpper(System.Globalization.CultureInfo.CurrentCulture) == "ANY")
                {
                    return System.Net.IPAddress.Any.ToString();
                }
                else if (nameOrAddress.Equals(".", StringComparison.InvariantCultureIgnoreCase) ||
                         nameOrAddress.Equals("local", StringComparison.InvariantCultureIgnoreCase) || nameOrAddress.Equals("localhost", StringComparison.InvariantCultureIgnoreCase) ||
                         nameOrAddress == "127.0.0.1")
                {
                    lock (_Resolved)
                    {
                        _Resolved["."] = new Resolved { IPV4Address = MachineIP(), MachineName = System.Net.Dns.GetHostEntry("127.0.0.1").HostName, LastResolved = DateTime.UtcNow };
                        _Resolved["local"] = _Resolved["."];
                        _Resolved["localhost"] = _Resolved["."];
                        _Resolved["127.0.0.1"] = _Resolved["."];


                        if (_Resolved["."].IPV4Address != IPAddressNone)
                            _Resolved[_Resolved["."].IPV4Address] = _Resolved["."];

                        if (_Resolved["."].MachineName != IPAddressNone)
                            _Resolved[_Resolved["."].MachineName] = _Resolved["."];
                    }
                }
                else
                {
                    try
                    {
                        string name = "";
                        string ip = "";

                        try
                        {
                            IPAddress.Parse(nameOrAddress);

                            name = nameOrAddress;

                            if (forceMachineNameLookup)
                            {
                                try
                                {
                                    name = System.Net.Dns.GetHostEntry(nameOrAddress).HostName;
                                }
                                catch { }
                            }

                            try
                            {
                                ip = System.Net.Dns.GetHostAddresses(name).FirstOrDefault(i => i.AddressFamily == AddressFamily.InterNetwork).ToString();
                            }
                            catch
                            {
                                ip = nameOrAddress;
                            }
                        }
                        catch
                        {
                            try
                            {
                                name = System.Net.Dns.GetHostEntry(nameOrAddress).HostName;
                            }
                            catch
                            {
                                name = nameOrAddress;
                            }

                            try
                            {
                                ip = System.Net.Dns.GetHostAddresses(name).FirstOrDefault(i => i.AddressFamily == AddressFamily.InterNetwork).ToString();
                            }
                            catch
                            {
                                ip = IPAddressNone;
                            }
                        }

                        lock (_Resolved)
                        {
                            Resolved r = new Resolved { IPV4Address = ip, MachineName = name, LastResolved = DateTime.UtcNow };

                            if (nameOrAddress != IPAddressNone)
                                _Resolved[nameOrAddress] = r;

                            if (name != IPAddressNone)
                                _Resolved[name] = r;

                            if (ip != IPAddressNone)
                                _Resolved[ip] = r;
                        }
                    }
                    catch { }
                }

                if (_Resolved.ContainsKey(nameOrAddress))
                    return _Resolved[nameOrAddress].IPV4Address;
            }
            catch { }

            if (nameOrAddress != IPAddressNone)
            {
                lock (_Resolved)
                    _Resolved[nameOrAddress] = new Resolved { IPV4Address = IPAddressNone, MachineName = IPAddressNone, LastResolved = DateTime.UtcNow };

                return _Resolved[nameOrAddress].IPV4Address;
            }

            return IPAddressNone;
        }

        public static string MachineName()
        {
            try
            {
                return _Resolved[_MachineAddress(".", true)].MachineName;
            }
            catch
            {
                try
                {
                    return _Resolved["."].MachineName;
                }
                catch { }
            }

            return IPAddressNone;
        }

        public static string MachineName(string nameOrAddress)
        {
            if (String.IsNullOrEmpty(nameOrAddress))
                throw new ArgumentNullException(nameof(nameOrAddress));

            try
            {
                return _Resolved[_MachineAddress(nameOrAddress, true)].MachineName;
            }
            catch 
            {
                try
                {
                    return _Resolved[nameOrAddress].MachineName;
                }
                catch { }
            }

            return IPAddressNone;
        }
    }
}
