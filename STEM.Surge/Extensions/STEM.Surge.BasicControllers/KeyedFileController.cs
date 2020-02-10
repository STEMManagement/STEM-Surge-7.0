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
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using STEM.Sys.State;

namespace STEM.Surge.BasicControllers
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("KeyedFileController")]
    [Description("Customize an InstructionSet Template using placeholders related to the file properties for each file discovered. (e.g. [TargetPath], [TargetName], [LastWriteTimeUtc]...) " +
        "Note that with this controller [DestinationPath] can be an expandable path where the controller will choose the best path based on network proximity and destination directory existence (if 'Check for directory existence' is enabled). " +
        "Files from this controller are addressed in alphabetical order. " +
        "This controller seeks to issue instruction sets, serially bound to a dedicated branch, based on an exclusive key generated for each file. (e.g. if the key was the filename extension, then all files with " +
        "extension '.typex' could be bound to 'branch-01' and assigned serially to that branch and only that branch unless or until that branch is no longer online, at that point typex files could be re-bound to another Branch)")]
    public abstract class KeyedFileController : BasicFileController
    {
        static Dictionary<string, BoundKey> _Keys = new Dictionary<string, BoundKey>();
        static Dictionary<string, long> _BranchBoundWeight = new Dictionary<string, long>();

        public KeyedFileController()
        {
            AllowThreadedAssignment = false;
            KeyTimeout = TimeSpan.FromMinutes(20);
            RequireTargetNameCoordination = false;
        }

        class BoundKey : IKeyLockOwner
        {
            internal BoundKey(KeyedFileController owner, string initiationSource, string key, string branchIP, long fileSize, bool assigned)
            {
                _Owner = owner;
                Key = key;
                InitiationSource = initiationSource;
                BranchIP = branchIP;
                FileSize = fileSize;
                LastAssigned = DateTime.UtcNow;
                Assigned = assigned;
            }
            
            /// <summary>
            /// This is for internal use only
            /// </summary>
            /// <param name="key"></param>
            public void Locked(string key)
            {
                key = key.ToUpper();

                lock (_Keys)
                    lock (this)
                        try
                        {
                            if (Key != null && _Keys.ContainsKey(Key))
                                _Keys.Remove(Key);

                            Key = key;

                            _Keys[key] = this;
                        }
                        catch { }
            }

            /// <summary>
            /// This is for internal use only
            /// </summary>
            /// <param name="key"></param>
            public void Unlocked(string key)
            {
                key = key.ToUpper();

                lock (_Keys)
                    lock (this)
                        try
                        {
                            if (InitiationSource != null)
                                FileSize = 0;

                            if (key != Key)
                            {
                                if (Key != null && _Keys.ContainsKey(Key))
                                    _Keys.Remove(Key);

                                _Owner.CoordinatedKeyManager.Unlock(Key, this);
                            }

                            if (key != null && _Keys.ContainsKey(key))
                                _Keys.Remove(key);
                        }
                        catch { }

                if (((TimeSpan)(DateTime.UtcNow - LastAssigned)) > _Owner.KeyTimeout)
                    if (_Owner.KeyExpired != null)
                        try
                        {
                            _Owner.KeyExpired(key);
                        }
                        catch { }
            }

            internal void Clear()
            {
                lock (this)
                {
                    FileSize = 0;
                    InitiationSource = null;
                    Assigned = false;
                }
            }

            /// <summary>
            /// This is for internal use only
            /// </summary>
            /// <param name="key"></param>
            public void Verify(string key)
            {
                key = key.ToUpper();

                lock (_Keys)
                    lock (this)
                        try
                        {
                            if (key != Key)
                                return;

                            if (InitiationSource != null)
                                if (!_Owner.CoordinatedKeyManager.Locked(InitiationSource))
                                {
                                    InitiationSource = null;
                                    Assigned = false;
                                    FileSize = 0;
                                }

                            if (((TimeSpan)(DateTime.UtcNow - LastAssigned)) > _Owner.KeyTimeout)
                                _Owner.CoordinatedKeyManager.Unlock(Key, this);
                        }
                        catch { }
            }

            KeyedFileController _Owner = null;

            internal string Key { get; set; }
            internal string InitiationSource { get; set; }
            internal string BranchIP { get; set; }

            long _fileSize = 0;
            internal long FileSize
            {
                get
                {
                    return _fileSize;
                }

                set
                {
                    if (_fileSize < value)
                    {
                        try
                        {
                            lock (_BranchBoundWeight)
                            {
                                _BranchBoundWeight[BranchIP] = _BranchBoundWeight[BranchIP] - _fileSize;
                                _BranchBoundWeight[BranchIP] = _BranchBoundWeight[BranchIP] + value;
                            }
                        }
                        catch { }

                        _fileSize = value;
                    }
                }
            }

            bool _assigned = false;
            internal bool Assigned
            {
                get
                {
                    return _assigned;
                }

                set
                {
                    if (_assigned == value)
                        return;

                    if (value == false)
                        InitiationSource = null;

                    _assigned = value;

                    if (_assigned)
                        LastAssigned = DateTime.UtcNow;
                }
            }

            internal DateTime LastAssigned { get; set; }

            public override string ToString()
            {
                return "Last Assigned: " + LastAssigned.ToString("G") + ", " + _Owner.GetType().Name + " - " + InitiationSource;
            }
        }

        public delegate void KeyExpiredHandler(string key);
        public event KeyExpiredHandler KeyExpired;

        public abstract string GenerateKey(string file);

        protected TimeSpan KeyTimeout { get; set; }

        string BestBranchByWeight(string branchIP)
        {
            string branch = null;

            long least = long.MaxValue;

            List<string> possibleIps = null;

            lock (_BranchBoundWeight)
            {
                string test = STEM.Sys.IO.Path.GetFileNameWithoutExtension(branchIP);

                possibleIps = _BranchBoundWeight.Keys.ToList().Where(i => i.StartsWith(test)).ToList();

                if (possibleIps.Count == 0)
                {
                    test = STEM.Sys.IO.Path.GetFileNameWithoutExtension(test);
                    possibleIps = _BranchBoundWeight.Keys.ToList().Where(i => i.StartsWith(test)).ToList();
                }
            }

            lock (_BranchBoundWeight)
                foreach (string b in _BranchBoundWeight.Keys)
                    if (possibleIps.Count == 0 || possibleIps.Contains(b))
                        if (_BranchBoundWeight[b] < least)
                        {
                            least = _BranchBoundWeight[b];
                            branch = b;
                        }

            return branch;
        }

        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            DeploymentDetails ret = null;

            try
            {
                recommendedBranchIP = LockSerializationKey(initiationSource, recommendedBranchIP);
                if (recommendedBranchIP != null)
                {
                    ret = GenerateDeploymentDetailsSerializationVerified(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);

                    if (ret == null)
                    {
                        lock (_Keys)
                        {
                            BoundKey binding = _Keys.Values.FirstOrDefault(i => i.InitiationSource != null && i.InitiationSource.ToUpper() == initiationSource.ToUpper());
                            if (binding != null)
                            {
                                binding.InitiationSource = null;
                                binding.Assigned = false;
                                binding.FileSize = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("KeyedFileController.GenerateDeploymentDetails", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                ReleaseSerializationKey(initiationSource);
                ret = null;
            }

            return ret;
        }


        /// <summary>
        /// This method generates InstructionSets upon request. When called, GenerateDeploymentDetails is assured that initiationSource is
        /// exclusively "locked" for assignment at that time and the SerializationKey is available for assignment.
        /// </summary>
        /// <param name="listPreprocessResult">The list of initiationSources returned from the most recent call to ListPreprocess</param>
        /// <param name="initiationSource">The initiationSource from the listPreprocessResult list for which an InstructionSet is being requested</param>
        /// <param name="recommendedBranchIP">The Branch IP that is preferred for this assignment</param>
        /// <returns>The DeploymentDetails instance containing the InstructionSet to be assigned, or null if no assignment should be made at this time</returns>
        public virtual DeploymentDetails GenerateDeploymentDetailsSerializationVerified(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            return base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);
        }

        string LockSerializationKey(string initiationSource, string branchIP)
        {
            try
            {
                string key = GenerateKey(initiationSource);

                lock (_Keys)
                {
                    BoundKey binding = _Keys.Values.FirstOrDefault(i => i.InitiationSource != null && i.InitiationSource.ToUpper() == initiationSource.ToUpper());
                    if (binding != null)
                        lock (binding)
                        {
                            if (key != null)
                            {
                                if (!binding.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                                    CoordinatedKeyManager.Unlock(binding.Key, binding);
                            }
                            else if (!CoordinatedKeyManager.Locked(binding.InitiationSource))
                            {
                                CoordinatedKeyManager.Unlock(binding.Key, binding);
                                return null;
                            }
                        }

                    if (key == null)
                        return branchIP;

                    key = key.ToUpper();

                    binding = null;

                    if (_Keys.ContainsKey(key))
                        binding = _Keys[key];

                    if (binding != null)
                    {
                        lock (binding)
                        {
                            if (binding.Assigned)
                            {
                                if (binding.InitiationSource != null)
                                {
                                    if (!CoordinatedKeyManager.Locked(binding.InitiationSource))
                                    {
                                        binding.InitiationSource = null;
                                        binding.Assigned = false;
                                    }
                                    else
                                    {
                                        return null;
                                    }
                                }
                                else
                                {
                                    binding.Assigned = false;
                                }

                                if (binding.Assigned)
                                    return null;
                            }

                            if (CoordinatedKeyManager.CoordinatedMachineHasLock(key, CoordinateWith))
                            {
                                CoordinatedKeyManager.Unlock(key, binding);
                                STEM.Sys.EventLog.WriteEntry("KeyedFileController.LockSerializationKey", "A previously bound key has been found to be owned by a different DeploymentService.", STEM.Sys.EventLog.EventLogEntryType.Error);
                                return null;
                            }

                            if (FileExists(initiationSource))
                                _Keys[key].FileSize = new FileInfo(initiationSource).Length;

                            _Keys[key].InitiationSource = initiationSource;
                            _Keys[key].Assigned = true;
                            return _Keys[key].BranchIP;
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(branchIP) || branchIP == STEM.Sys.IO.Net.EmptyAddress)
                            return null;

                        if (!_BranchBoundWeight.ContainsKey(branchIP))
                            return null;
                        
                        branchIP = BestBranchByWeight(branchIP);

                        if (branchIP == null)
                            return null;

                        long fileLen = 0;

                        if (FileExists(initiationSource))
                            fileLen = GetFileInfo(initiationSource).Size;                 
                        
                        binding = new BoundKey(this, initiationSource, key, branchIP, fileLen, true);

                        if (!CoordinatedKeyManager.Lock(key, binding, CoordinateWith))
                            return null;

                        return binding.BranchIP;
                    }
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("KeyedFileController.LockSerializationKey", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return branchIP;
        }

        void ReleaseSerializationKey(string initiationSource)
        {
            try
            {
                if (initiationSource == null)
                    return;

                string key = GenerateKey(initiationSource);

                if (key == null)
                {
                    lock (_Keys)
                        foreach (string k in _Keys.Keys.ToList())
                        {
                            if (_Keys[k].InitiationSource != null)
                                if (_Keys[k].InitiationSource.ToUpper() == initiationSource.ToUpper())
                                {
                                    _Keys[k].InitiationSource = null;
                                    _Keys[k].Assigned = false;
                                    return;
                                }
                        }

                    return;
                }

                key = key.ToUpper();

                lock (_Keys)
                {
                    if (_Keys.ContainsKey(key))
                    {
                        _Keys[key].InitiationSource = null;
                        _Keys[key].Assigned = false;
                    }
                    else
                    {
                        foreach (string k in _Keys.Keys.ToList())
                        {
                            if (_Keys[k].InitiationSource != null)
                                if (_Keys[k].InitiationSource.ToUpper() == initiationSource.ToUpper())
                                {
                                    _Keys[key] = _Keys[k];
                                    _Keys[key].Key = key;
                                    _Keys[key].InitiationSource = null;
                                    _Keys[key].Assigned = false;

                                    if (k != key)
                                        _Keys.Remove(k);

                                    return;
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("KeyedFileController.ReleaseSerializationKey", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// This is for internal use only
        /// </summary>
        /// <param name="key"></param>
        public override void RememberDeployment(DeploymentDetails details)
        {
            SafeRememberDeployment(details);
        }

        public virtual void SafeRememberDeployment(DeploymentDetails details)
        {
        }

        /// <summary>
        /// This is for internal use only
        /// </summary>
        /// <param name="key"></param>
        public override sealed void BranchStatusUpdate(string address, BranchState state)
        {
            try
            {
                if (String.IsNullOrEmpty(address) || address == STEM.Sys.IO.Net.EmptyAddress)
                    return;

                lock (_BranchBoundWeight)
                    if (!_BranchBoundWeight.ContainsKey(address) && state == BranchState.Online)
                        _BranchBoundWeight[address] = 0;

                if (state != BranchState.Online)
                    lock (_Keys)
                    {
                        List<string> bad = new List<string>();
                        foreach (string tag in _Keys.Keys)
                            if (_Keys[tag].BranchIP == address)
                                bad.Add(tag);

                        foreach (string tag in bad)
                        {
                            lock (_Keys[tag])
                            {
                                CoordinatedKeyManager.Unlock(tag, _Keys[tag]);
                            }
                        }

                        lock (_BranchBoundWeight)
                            _BranchBoundWeight.Remove(address);
                    }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("KeyedFileController.BranchStatusUpdate", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            SafeBranchStatusUpdate(address, state);
        }

        public virtual void SafeBranchStatusUpdate(string address, BranchState state)
        {
        }

        /// <summary>
        /// This is for internal use only
        /// </summary>
        /// <param name="key"></param>
        public override sealed void ExecutionComplete(DeploymentDetails details, List<Exception> exceptions)
        {
            try
            {
                base.ExecutionComplete(details, exceptions);
            }
            catch { }

            try
            {
                ReleaseSerializationKey(details.InitiationSource);
            }
            catch { }

            SafeExecutionComplete(details, exceptions);
        }

        public virtual void SafeExecutionComplete(DeploymentDetails details, List<Exception> exceptions)
        {
        }
    }
}
