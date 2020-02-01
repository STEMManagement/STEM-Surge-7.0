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

namespace STEM.Surge
{
    /// <summary>
    /// An optomization for applying [token] = "value" replacement within a string
    /// </summary>
    public static class KVPMapUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="kvp"></param>
        /// <param name="escapeForXml"></param>
        /// <returns></returns>
        public static string ApplyKVP(string target, System.Collections.Generic.Dictionary<string, string> kvp, bool escapeForXml, StemStr reuseStemStr = null)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (kvp == null)
                throw new ArgumentNullException(nameof(kvp));

            IEnumerable<string> keysWithTokensInValues = kvp.Keys.Where(i => kvp[i] != null && kvp[i].Contains("[") && kvp[i].Contains("]"));

            foreach (string key in keysWithTokensInValues)
                if (kvp[key].IndexOf(key, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    throw new Exception("Map error - key in value (" + key + ")");

            // Check for cross collision
            foreach (string key in keysWithTokensInValues)
            {
                foreach (string k in keysWithTokensInValues)
                {
                    if (kvp[key].IndexOf(k, StringComparison.InvariantCultureIgnoreCase) >= 0 && kvp[k].IndexOf(key, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        throw new Exception("Map error - cross key reference (" + key + ", " + k + ")");
                }
            }

            DateTime now = DateTime.UtcNow;
            List<string> keysPresent = new List<string>();

            StemStr ss = reuseStemStr;

            if (ss == null)
            {
                ss = new StemStr(target, target.Length * 2);
            }
            else
            {
                ss.Reset(target);
            }

            List<string> keys = new List<string>();

            while (true)
            {
                bool matchFound = false;

                int masterIndex = 0;

                keys.Clear();

                while ((masterIndex = ss.IndexOf("[", masterIndex)) >= 0)
                {
                    if (ss.IndexOf("]", masterIndex) < 0)
                        break;

                    string k = ss.Substring(masterIndex, ss.IndexOf("]", masterIndex) - masterIndex + 1);

                    if (k.Length == 0)
                    {
                        masterIndex++;
                        continue;
                    }

                    masterIndex += k.Length;

                    if (keys.Contains(k))
                        continue;

                    keys.Add(k);

                    string v = null;

                    if (kvp.ContainsKey(k))
                        v = kvp[k];
                    else
                        v = kvp.Where(i => i.Key.Equals(k, StringComparison.InvariantCultureIgnoreCase)).Select(i => i.Value).FirstOrDefault();

                    if (v == null)
                        continue;

                    v = v.Trim();

                    if (k.Equals("[NewGuid]", StringComparison.InvariantCultureIgnoreCase))
                    {
                        matchFound = true;

                        if (target == k)
                        {
                            return Guid.NewGuid().ToString();
                        }

                        string[] parts = target.Split(new string[] { k }, StringSplitOptions.None);

                        string tmp = parts[0];

                        for (int i = 1; i < parts.Length; i++)
                            tmp = tmp + Guid.NewGuid().ToString() + parts[i];

                        ss.Reset(tmp);
                    }
                    else
                    {
                        if (v.Equals("Reserved", StringComparison.InvariantCultureIgnoreCase))
                            continue;

                        if (v.ToUpper().Contains(k.ToUpper()))
                            throw new Exception("Key in value! (" + k + ", " + v + ")");

                        if (k.Equals("[UtcNow]", StringComparison.InvariantCultureIgnoreCase))
                        {
                            v = now.ToString(v, System.Globalization.CultureInfo.CurrentCulture);
                        }

                        if (escapeForXml)
                            v = System.Security.SecurityElement.Escape(v);

                        matchFound = true;

                        ss.Replace(k, v, 0);
                    }
                }

                if (!matchFound)
                    return ss.ToString();
            }
        }
    }
}

