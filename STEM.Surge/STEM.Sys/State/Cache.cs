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
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace STEM.Sys.State
{
    [XmlType(TypeName = "STEM.Sys.State.Cache")]
    public class Cache
    {
        public Cache() { }
        
        static Cache()
        {
            STEM.Sys.Global.ThreadPool.BeginAsync(new ThreadStart(_TimeoutThread), TimeSpan.FromMinutes(5));

            lock (Cache._CacheObjects)
            {
                if (Cache._CacheObjects.Count == 0)
                {
                    foreach (string file in Directory.GetFiles(CacheDirectory, "*.cache"))
                    {
                        try
                        {
                            SessionObject so = SessionObject.Deserialize(File.ReadAllText(file));

                            if (so != null)
                                _CacheObjects[so.Key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)] = so;
                            else
                                File.Delete(file);
                        }
                        catch { }
                    }
                }
            }
        }

        static void _TimeoutThread()
        {
            try
            {
                List<string> keys = new List<string>();

                lock (Cache._CacheObjects)
                    keys.AddRange(Cache._CacheObjects.Keys);

                foreach (string key in keys)
                    lock (Cache._CacheObjects)
                        if (Cache._CacheObjects.ContainsKey(key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)))
                            if ((DateTime.UtcNow - Cache._CacheObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].LastAccess).TotalMinutes > 120)
                            {
                                _CacheObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].Dispose();
                                _CacheObjects.Remove(key.ToUpper(System.Globalization.CultureInfo.CurrentCulture));
                            }
            }
            catch { }
        }

        static Dictionary<string, SessionObject> _CacheObjects = new Dictionary<string, SessionObject>();

        static string CacheDirectory
        {
            get
            {
                try
                {
                    if (!Directory.Exists(Path.Combine(System.Environment.CurrentDirectory, "Cache")))
                        Directory.CreateDirectory(Path.Combine(System.Environment.CurrentDirectory, "Cache"));
                }
                catch { }

                return Path.Combine(System.Environment.CurrentDirectory, "Cache");
            }
        }

        public List<string> KeyList()
        {
            List<string> ret = new List<string>();

            try
            {
                lock (_CacheObjects)
                    ret.AddRange(_CacheObjects.Keys);
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
                return _CacheObjects.ContainsKey(key.ToUpper(System.Globalization.CultureInfo.CurrentCulture));
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
                lock (_CacheObjects)
                    return _CacheObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].LastAccess;
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
                lock (_CacheObjects)
                    return _CacheObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].LastRead;
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
                lock (_CacheObjects)
                    return _CacheObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].LastWrite;
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

                try
                {
                    if (_CacheObjects.ContainsKey(key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)))
                    {
                        object ret = _CacheObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].Value;
                        return ret;
                    }
                }
                catch { }

                return null;
            }

            set
            {
                if (String.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));

                if (value == null)
                {
                    lock (_CacheObjects)
                        _CacheObjects.Remove(key.ToUpper(System.Globalization.CultureInfo.CurrentCulture));

                    try
                    {
                        if (File.Exists(Path.Combine(CacheDirectory, STEM.Sys.State.KeyManager.GetHash(key) + ".cache")))
                            File.Delete(Path.Combine(CacheDirectory, STEM.Sys.State.KeyManager.GetHash(key) + ".cache"));
                    }
                    catch { }
                }
                else
                {
                    lock (_CacheObjects)
                        _CacheObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)] = new SessionObject(key, value);

                    try
                    {
                        File.WriteAllText(Path.Combine(CacheDirectory, STEM.Sys.State.KeyManager.GetHash(key) + ".cache"), _CacheObjects[key.ToUpper(System.Globalization.CultureInfo.CurrentCulture)].Serialize());
                    }
                    catch (Exception ex)
                    {
                        STEM.Sys.EventLog.WriteEntry("STEM.Sys.State.Cache", new Exception("Failed to write cache file for key (" + key + ")", ex));
                    }
                }
            }
        }
    }
}
