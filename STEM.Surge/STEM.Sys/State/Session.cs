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
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace STEM.Sys.State
{
    [XmlType(TypeName = "STEM.Sys.State.Session")]
    public class Session : IDisposable
    {
        static Session()
        {
            STEM.Sys.Global.ThreadPool.BeginAsync(new ThreadStart(_TimeoutThread), TimeSpan.FromMinutes(5));
        }

        public Session()
        {
            _TargetObjects = _SessionObjects;
        }

        public Session(bool dedicated)
        {
            if (dedicated)
            {
                _TargetObjects = new Dictionary<string, SessionObject>();
                STEM.Sys.Global.ThreadPool.BeginAsync(new ThreadStart(__TimeoutThread), TimeSpan.FromMinutes(5));
            }
            else
            {
                _TargetObjects = _SessionObjects;
            }
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

        void __TimeoutThread()
        {
            try
            {
                List<string> keys = new List<string>();

                lock (_TargetObjects)
                    keys.AddRange(_TargetObjects.Keys);

                foreach (string key in keys)
                    lock (_TargetObjects)
                        if (_TargetObjects.ContainsKey(key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)))
                            if ((DateTime.UtcNow - _TargetObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].LastAccess).TotalMinutes > 120)
                            {
                                _TargetObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].Dispose();
                                _TargetObjects.Remove(key.ToUpper(System.Globalization.CultureInfo.CurrentCulture));
                            }
            }
            catch { }
        }

        static Dictionary<string, SessionObject> _SessionObjects = new Dictionary<string, SessionObject>();

        Dictionary<string, SessionObject> _TargetObjects = new Dictionary<string, SessionObject>();

        public List<string> KeyList()
        {
            List<string> ret = new List<string>();

            try
            {
                lock (_TargetObjects)
                    ret.AddRange(_TargetObjects.Keys);
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
                lock (_TargetObjects)
                    return _TargetObjects.ContainsKey(key.ToUpper(System.Globalization.CultureInfo.CurrentCulture));
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
                lock (_TargetObjects)
                    return _TargetObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].LastAccess;
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
                lock (_TargetObjects)
                    return _TargetObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].LastRead;
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
                lock (_TargetObjects)
                    return _TargetObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].LastWrite;
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

                lock (_TargetObjects)
                    if (_TargetObjects.ContainsKey(key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)))
                        return _TargetObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].Value;

                return null;
            }

            set
            {
                if (String.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));

                lock (_TargetObjects)
                    if (value == null)
                        _TargetObjects.Remove(key.ToUpper(System.Globalization.CultureInfo.CurrentCulture));
                    else
                        _TargetObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)] = new SessionObject(key, value);
            }
        }

        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch { }

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            lock (_TargetObjects)
            {
                List<SessionObject> values = _TargetObjects.Values.ToList();

                _TargetObjects.Clear();

                foreach (SessionObject o in values)
                {
                    o.Dispose();
                }
            }
        }
    }
}
