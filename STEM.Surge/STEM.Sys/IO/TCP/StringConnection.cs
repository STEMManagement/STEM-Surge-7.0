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
using System.Security.Cryptography.X509Certificates;

namespace STEM.Sys.IO.TCP
{
    public class StringConnection : Connection
    {
        internal StringConnection(System.Net.Sockets.TcpClient client, X509Certificate2 certificate) : base(client, certificate) { }
        internal StringConnection(string address, int port, bool sslConnection, bool autoReconnect = false) : base(address, port, sslConnection, autoReconnect) { }
                
        List<byte> _Tailing = null;
        public override void Receive(byte[] message, int length, DateTime received)
        {
            List<byte> fullBuf = null;

            if (_Tailing != null)
            {
                fullBuf = _Tailing;
                _Tailing = null;
            }

            byte[] buf = message;
            if (fullBuf != null)
            {
                fullBuf.AddRange(buf.Take(length));
                buf = fullBuf.ToArray();
                length = buf.Length;
            }

            int pos = 0;

            while (pos < length)
            {
                int len = 0;

                try
                {
                    string s  = STEM.Sys.IO.StringCompression.DecompressString(buf, pos, length, ref len);
                    
                    pos += len;

                    if (s == null)
                    {
                        if ((length - pos) > 0 && pos > 0)
                            _Tailing = buf.Skip(pos).Take(length-pos).ToList();
                        else if (pos == 0)
                            _Tailing = buf.Take(length).ToList();                            

                        pos = length;
                    }
                    else
                    {
                        Receive(s, received);
                    }
                }
                catch (Exception ex)
                {
                    _Tailing = null;
                    pos = length;
                    STEM.Sys.EventLog.WriteEntry("StringConnection._Receive", "(" + RemoteAddress + ":" + RemotePort + ") \r\n" + ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                }
            }

            Recycle(buf);
        }

        public virtual void Receive(string message, DateTime received) { }
        
        public bool Send(string message)
        {
            if (!String.IsNullOrEmpty(message))
                try
                {
                    byte[] b = STEM.Sys.IO.StringCompression.CompressString(message);

                    if (b != null)
                        return Send(b);
                }
                catch { }

            return false;
        }
    }
}
