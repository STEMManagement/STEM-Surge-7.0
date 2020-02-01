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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace STEM.Sys.IO.UDP
{
    public class SocketHelper
    {
        public int MulticastPort { get; set; }
        public string LocalNetworkAdapter { get; set; }
        public string MulticastIP { get; set; }

        public const int LargestBlockSize = 65507;
                     
        public SocketHelper(int multicastPort, string localNetworkAdapter, string multicastIP)
        {
            MulticastPort = multicastPort;
            LocalNetworkAdapter = STEM.Sys.IO.Net.MachineAddress(localNetworkAdapter);
            MulticastIP = multicastIP;
        }
                
        public Socket BuildSendSocket()
        {
            Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            soc.SendBufferSize = 1024 * 1024 * 256;
            soc.Bind(new IPEndPoint(IPAddress.Parse(STEM.Sys.IO.Net.MachineAddress(LocalNetworkAdapter)), 0));
            soc.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
            soc.Connect(new IPEndPoint(System.Net.IPAddress.Parse(MulticastIP), MulticastPort));
            
            return soc;
        }

        public Socket BuildReceiveSocket()
        {
            Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            soc.ReceiveBufferSize = 1024 * 1024 * 256;
            soc.Bind(new IPEndPoint(IPAddress.Parse(STEM.Sys.IO.Net.MachineAddress(LocalNetworkAdapter)), MulticastPort));
            soc.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(System.Net.IPAddress.Parse(MulticastIP), IPAddress.Parse(STEM.Sys.IO.Net.MachineAddress(LocalNetworkAdapter))));
            
            return soc;
        }

        public static void Close(Socket soc)
        {
            try
            {
                if (soc != null)
                {
                    try
                    {
                        soc.Shutdown(SocketShutdown.Both);
                    }
                    catch { }

                    try
                    {
                        soc.Close();
                    }
                    catch { }

                    try
                    {
                        soc.Dispose();
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
