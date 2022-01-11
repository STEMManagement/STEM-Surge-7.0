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
using System.ComponentModel;
using System.Collections.Generic;
using STEM.Sys.State;

namespace STEM.Surge.BasicControllers
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("SerialFileController")]
    [Description("Customize an InstructionSet Template using placeholders related to the file properties for each file discovered. (e.g. [TargetPath], [TargetName], [LastWriteTimeUtc]...) " +
        "Note that with this controller [DestinationPath] can be an expandable path where the controller will choose the best path based on network proximity and destination directory existence (if 'Check for directory existence' is enabled). " +
        "Files from this controller are addressed in alphabetical order. " +
        "This controller seeks to issue instruction sets based on an exclusive key generated for each file. (e.g. if the key was the filename extension, then all files with " +
        "extension '.typex' would assigned serially)")]
    public abstract class SerialFileController : BasicFileController
    {
        static Dictionary<string, BoundKey> _Keys = new Dictionary<string, BoundKey>();

        public SerialFileController()
        {
            AllowThreadedAssignment = false;
            KeyTimeout = TimeSpan.FromMinutes(20);
            RequireTargetNameCoordination = false;
        }
                
        class BoundKey : IKeyLockOwner
        {
            internal BoundKey(SerialFileController owner, string initiationSource, string key, bool assigned)
            {
                _Owner = owner;
                Key = key;
                InitiationSource = initiationSource;
                LastAssigned = DateTime.UtcNow;
                LastSuccessfulAssignment = DateTime.UtcNow;
                Assigned = assigned;
            }

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

            public void Unlocked(string key)
            {
                key = key.ToUpper();

                lock (_Keys)
                    lock (this)
                        try
                        {
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

                if (((TimeSpan)(DateTime.UtcNow - LastSuccessfulAssignment)) > _Owner.KeyTimeout)
                    if (_Owner.KeyExpired != null)
                        try
                        {
                            _Owner.KeyExpired(key);
                        }
                        catch { }
            }

            public void Verify(string key)
            {
                key = key.ToUpper();

                lock (_Keys)
                    lock (this)
                        try
                        {
                            if (key != Key)
                                return;

                            //if (InitiationSource != null)
                            //    if (!_Owner.CoordinatedKeyManager.Locked(InitiationSource))
                            //    {
                            //        InitiationSource = null;
                            //        Assigned = false;
                            //    }

                            if (((TimeSpan)(DateTime.UtcNow - LastSuccessfulAssignment)) > _Owner.KeyTimeout)
                                _Owner.CoordinatedKeyManager.Unlock(Key, this);
                        }
                        catch { }
            }

            SerialFileController _Owner = null;

            internal string Key { get; set; }
            internal string InitiationSource { get; set; }

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
            internal DateTime LastSuccessfulAssignment { get; set; }

            public override string ToString()
            {
                return "Last Successful Assignment: " + LastSuccessfulAssignment.ToString("G") + ", " + _Owner.GetType().Name + " - " + InitiationSource;
            }
        }
        
        public delegate void KeyExpiredHandler(string key);
        public event KeyExpiredHandler KeyExpired;

        /// <summary>
        /// This method is defined by derived classes to create a key generation algorithm.
        /// </summary>
        /// <param name="initiationSource">The initiationSource string for which a key used to achieve serialization is generated.</param>
        /// <returns>The serialization key for this initiationSource, else null if no serialization is required.</returns>
        public abstract string GenerateKey(string initiationSource);

        protected TimeSpan KeyTimeout { get; set; }

        protected bool IsSerializationKeyLocked(string initiationSource)
        {
            string key = GenerateKey(initiationSource);
            if (key == null)
                return false;

            key = key.ToUpper();

            lock (_Keys)
                if (_Keys.ContainsKey(key))
                {
                    if (_Keys[key].Assigned)
                        return true;
                }

            return false;
        }

        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {           
            DeploymentDetails ret = null;

            try
            {
                if (LockSerializationKey(initiationSource))
                {
                    ret = GenerateDeploymentDetailsSerializationVerified(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);

                    if (ret == null)
                    {
                        ReleaseSerializationKey(initiationSource, false);
                    }
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("SerialFileController.LockSerializationKey", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                ReleaseSerializationKey(initiationSource, false);
                ret = null;
            }
            
            return ret;
        }

        /// <summary>
        /// This method generates InstructionSets upon request. When called, GenerateDeploymentDetails is assured that initiationSource is
        /// exclusively "locked" for assignment at that time and the SerializationKey is available for assignment.
        /// </summary>
        /// <param name="listPreprocessResult">The list of initiationSources returned from the most recent call to ListPreprocess</param>
        /// <param name="initiationSource">The initiationSource from the fileListPreprocessResult list for which an InstructionSet is being requested</param>
        /// <param name="recommendedBranchIP">The Branch IP that is preferred for this assignment</param>
        /// <returns>The DeploymentDetails instance containing the InstructionSet to be assigned, or null if no assignment should be made at this time</returns>
        public virtual DeploymentDetails GenerateDeploymentDetailsSerializationVerified(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            return base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);
        }

        bool LockSerializationKey(string initiationSource)
        {
            try
            {
                string key = GenerateKey(initiationSource);
                BoundKey binding = null;

                while (true)
                {
                    try
                    {
                        binding = _Keys.Values.FirstOrDefault(i => i.InitiationSource != null && i.InitiationSource.ToUpper() == initiationSource.ToUpper());
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
                                    return false;
                                }
                            }

                        break;
                    }
                    catch { }
                }

                if (key == null)
                    return true;

                lock (_Keys)
                {
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
                                        return false;
                                    }
                                }
                                else
                                {
                                    binding.Assigned = false;
                                }

                                if (binding.Assigned)
                                    return false;
                            }

                            if (CoordinatedKeyManager.CoordinatedMachineHasLock(key, CoordinateWith) == CoordinatedKeyManager.RemoteLockExists.True)
                            {
                                CoordinatedKeyManager.Unlock(key, binding);
                                STEM.Sys.EventLog.WriteEntry("SerialFileController.KeyedBranchIP", "A previously bound key has been found to be owned by a different DeploymentService.", STEM.Sys.EventLog.EventLogEntryType.Error);
                                return false;
                            }

                            _Keys[key].InitiationSource = initiationSource;
                            _Keys[key].Assigned = true;
                        }
                    }
                    else
                    {
                        binding = new BoundKey(this, initiationSource, key, true);

                        if (UseSubnetCoordination)
                        {
                            if (!CoordinatedKeyManager.Lock(key, binding, CoordinateWith))
                                return false;
                        }
                        else
                        {
                            if (!CoordinatedKeyManager.Lock(key, binding))
                                return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("SerialFileController.LockSerializationKey", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                return false;
            }

            return true;
        }

        protected void ReleaseSerializationKey(string initiationSource, bool successfulAssignment)
        {
            try
            {
                if (initiationSource == null)
                    return;

                while (true)
                {
                    try
                    {
                        BoundKey binding = _Keys.Values.FirstOrDefault(i => i.InitiationSource != null && i.InitiationSource.ToUpper() == initiationSource.ToUpper());
                        if (binding != null)
                            lock (binding)
                            {
                                binding.InitiationSource = null;
                                binding.Assigned = false;

                                if (successfulAssignment)
                                    binding.LastSuccessfulAssignment = DateTime.UtcNow;
                            }

                        break;
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("SerialFileController.ReleaseSerializationKey", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// This is called from Surge 
        /// </summary>
        /// <param name="key"></param>
        public override sealed void RememberDeployment(DeploymentDetails details)
        {
            //try
            //{
            //    string key = GenerateKey(details.InitiationSource);
            //    if (key == null)
            //        return;

            //    key = key.ToUpper();

            //    if (!CoordinatedKeyManager.Lock(key, this) || !CoordinatedKeyManager.Locked(key))
            //        return;

            //    lock (_Keys)
            //        _Keys[key] = new BoundKey(this, details.InitiationSource, key, true);
            //}
            //catch (Exception ex)
            //{
            //    STEM.Sys.EventLog.WriteEntry("SerialFileController.RememberDeployment", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            //}

            SafeRememberDeployment(details);
        }

        public virtual void SafeRememberDeployment(DeploymentDetails details)
        {
        }

        /// <summary>
        /// This is called from Surge causing this DeploymentController to ReleaseSerializationKey 
        /// </summary>
        /// <param name="details"></param>
        public override sealed void ExecutionComplete(DeploymentDetails details, List<Exception> exceptions)
        {
            try
            {
                base.ExecutionComplete(details, exceptions);
            }
            catch { }

            try
            {
                ReleaseSerializationKey(details.InitiationSource, exceptions.Count == 0);
            }
            catch { }

            SafeExecutionComplete(details, exceptions);
        }

        public virtual void SafeExecutionComplete(DeploymentDetails details, List<Exception> exceptions)
        {
        }
    }
}