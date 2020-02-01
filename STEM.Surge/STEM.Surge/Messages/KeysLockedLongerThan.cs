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
using STEM.Sys.Messaging;

namespace STEM.Surge.Messages
{    
    public class KeysLockedLongerThan : STEM.Sys.Messaging.Message
    {
        public class LockInfo
        {
            public LockInfo(STEM.Sys.State.LockInfo info)
            {
                if (info == null)
                    throw new ArgumentNullException(nameof(info));

                Key = info.Key;
                LockTime = info.LockTime;
                LastLockAttempt = info.LastLockAttempt;

                Description = "";
                if (info.LockOwner != null)
                {
                    Description = info.LockOwner.ToString();
                }
            }

            public LockInfo() { }

            public string Key { get; set; }
            public string Description { get; set; }
            public DateTime LockTime { get; set; }
            public DateTime LastLockAttempt { get; set; }
        }

        public List<LockInfo> LockedKeys { get; set; }
        public int Seconds { get; set; }

        public KeysLockedLongerThan() 
        {
            LockedKeys = new List<LockInfo>();
            Seconds = 150;
        }
    }
}
