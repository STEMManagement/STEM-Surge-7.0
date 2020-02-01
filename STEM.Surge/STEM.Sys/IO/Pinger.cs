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

namespace STEM.Sys.IO
{
    public class Pinger
    {
        public string Address { get; private set; }
        public DateTime LastAttempt { get; private set; }
        public bool Pingable { get; private set; }

        public Pinger(string ip)
        {
            Address = ip;
            LastAttempt = DateTime.MinValue;
            Pingable = false;
        }

        public bool IsPingable()
        {
            if (System.Threading.Monitor.TryEnter(this))
                try
                {
                    if ((DateTime.UtcNow - LastAttempt).TotalSeconds > 15)
                    {
                        try
                        {
                            LastAttempt = DateTime.UtcNow;
                            if (STEM.Sys.IO.Net.PingHost(Address) || STEM.Sys.IO.Net.PingHost(Address) || STEM.Sys.IO.Net.PingHost(Address))
                            {
                                Pingable = true;
                            }
                            else
                            {
                                Pingable = false;
                            }
                        }
                        catch
                        {
                            Pingable = false;
                        }
                    }
                }
                catch { }
                finally
                {
                    System.Threading.Monitor.Exit(this);
                }

            return Pingable;
        }
    }
}
