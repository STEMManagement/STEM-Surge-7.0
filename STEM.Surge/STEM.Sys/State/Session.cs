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
using System.Threading;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace STEM.Sys.State
{
    [XmlType(TypeName = "STEM.Sys.State.Session")]
    public class Session
    {
        static Session()
        {
            STEM.Sys.Global.ThreadPool.BeginAsync(new ThreadStart(_TimeoutThread), TimeSpan.FromMinutes(5));
        }

        public Session()
        {
        }

        static void _TimeoutThread()
        {
            try
            {
                List<string> keys = new List<string>();

                lock (_SessionObjects)
                    keys.AddRange(_SessionObjects.Keys);

                foreach (string key in keys)
                    lock (_SessionObjects)
                        if (_SessionObjects.ContainsKey(key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)))
                            if ((DateTime.UtcNow - _SessionObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].LastAccess).TotalMinutes > 120)
                            {
                                _SessionObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].Dispose();
                                _SessionObjects.Remove(key.ToUpper(System.Globalization.CultureInfo.CurrentCulture));
                            }
            }
            catch { }
        }

        static Dictionary<string, SessionObject> _SessionObjects = new Dictionary<string, SessionObject>();

        public List<string> KeyList()
        {
            List<string> ret = new List<string>();

            try
            {
                lock (_SessionObjects)
                    ret.AddRange(_SessionObjects.Keys);
            }
            catch { }

            return ret;
        }

        public bool ContainsKey(string key)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            try
            {
                lock (_SessionObjects)
                    return _SessionObjects.ContainsKey(key.ToUpper(System.Globalization.CultureInfo.CurrentCulture));
            }
            catch { }

            return false;
        }

        public DateTime LastAccessTime(string key)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            try
            {
                lock (_SessionObjects)
                    return _SessionObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].LastAccess;
            }
            catch { }

            return DateTime.MinValue;
        }

        public DateTime LastReadTime(string key)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            try
            {
                lock (_SessionObjects)
                    return _SessionObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].LastRead;
            }
            catch { }

            return DateTime.MinValue;
        }

        public DateTime LastWriteTime(string key)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            try
            {
                lock (_SessionObjects)
                    return _SessionObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].LastWrite;
            }
            catch { }

            return DateTime.MinValue;
        }

        public object this[string key]
        {
            get
            {
                if (String.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));

                lock (_SessionObjects)
                    if (_SessionObjects.ContainsKey(key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)))
                        return _SessionObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].Value;

                return null;
            }

            set
            {
                if (String.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));

                lock (_SessionObjects)
                    if (value == null)
                        _SessionObjects.Remove(key.ToUpper(System.Globalization.CultureInfo.CurrentCulture));
                    else                        
                        _SessionObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)] = new SessionObject(key, value);
            }
        }
    }
}
