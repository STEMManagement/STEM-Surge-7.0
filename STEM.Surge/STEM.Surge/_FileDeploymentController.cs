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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using STEM.Sys.Security;

namespace STEM.Surge
{
    /// <summary>
    /// This is the opensource base class for STEM.Surge.Internal FileDeploymentController
    /// See the bottom of this file for the Internal class implementation
    /// When creating a custom Deployment controller derive from either DeploymentController or FileDeploymentController not _DeploymentController or _FileDeploymentController
    /// 
    /// 
    /// virtual methods may be added in the future as opportunities for opensource developers to further customize
    /// </summary>
    public abstract class _FileDeploymentController : STEM.Surge._DeploymentController
    {
        [Category("Assignment Profile")]
        [DisplayName("Verify Target Exists"), DescriptionAttribute("Disregard targets that don't exist.")]
        public bool CheckTriggerExists { get; set; }

        [Category("Source Aging")]
        [DisplayName("Age Targets"), DescriptionAttribute("Disregard targets that haven't aged based on the age settings specified.")]
        public bool AgeTrigger { get; set; }

        [Category("Source Aging")]
        [DisplayName("Seconds To Age"), DescriptionAttribute("How many seconds from the Age Origin should we age targets before assigning?")]
        public int SecondsToAge { get; set; }

        [Category("Source Aging")]
        [DisplayName("Selected Origin"), DescriptionAttribute("What Age Origin should we age targets relative to?")]
        public AgeOrigin SelectedOrigin { get; set; }

        [Category("Assignment Profile")]
        [DisplayName("Require Additional Target Name Coordination"), DescriptionAttribute("Require coordination based on filename or directory name? (the full path is already a coordination key)")]
        public bool RequireTargetNameCoordination { get; set; }

        [Category("Recurse and Recreate")]
        [DisplayName("Recreate subdirectory from root of..."), DescriptionAttribute("From where should the directory tree recreation start?")]
        public string RecreateSubFromRootOf { get; set; }

        public _FileDeploymentController()
        {
            SelectedOrigin = AgeOrigin.LastWriteTime;
            AgeTrigger = false;
            SecondsToAge = -1;

            CheckTriggerExists = true;
            RequireTargetNameCoordination = false;

            RecreateSubFromRootOf = "";

            _ImpersonateUser = null;
            _ImpersonationPassword = null;
            _LocalUserImpersonation = true;

            TemplateKVP["[TargetPath]"] = "Reserved";
            TemplateKVP["[TargetName]"] = "Reserved";
            TemplateKVP["[TargetDirectoryName]"] = "Reserved";
            TemplateKVP["[SubDir]"] = "Reserved";
            TemplateKVP["[TargetNameWithoutExt]"] = "Reserved";
            TemplateKVP["[TargetExt]"] = "Reserved";
            TemplateKVP["[LastWriteTimeUtc]"] = "yyyy-MM-dd HH.mm.ss.fff";
            TemplateKVP["[LastAccessTimeUtc]"] = "yyyy-MM-dd HH.mm.ss.fff";
            TemplateKVP["[CreationTimeUtc]"] = "yyyy-MM-dd HH.mm.ss.fff";
            TemplateKVP["[SourceAddress]"] = "Reserved";
        }

        /// <summary>
        /// Customize an iSetTemplate by applying the TemplateKVP map to the InstructionsXml
        /// </summary>
        /// <param name="iSetTemplate">A clone of the template to be modified in this method</param>
        /// <param name="map">The TemplateKVP map used to modify the iSetTemplate</param>
        /// <param name="branchIP">The branchIP this will be assigned to</param>
        /// <param name="initiationSource">The initiationSource passed in to GenerateDeploymentDetails()</param>
        /// <param name="cloneMap">Should the map be cloned as it will be modified in this method</param>
        public override void CustomizeInstructionSet(_InstructionSet iSetTemplate, System.Collections.Generic.Dictionary<string, string> map, string branchIP, string initiationSource, bool cloneMap = true)
        {
            if (iSetTemplate == null)
                throw new ArgumentNullException(nameof(iSetTemplate));

            if (map == null)
                throw new ArgumentNullException(nameof(map));

            if (String.IsNullOrEmpty(branchIP))
                throw new ArgumentNullException(nameof(branchIP));

            if (String.IsNullOrEmpty(initiationSource))
                throw new ArgumentNullException(nameof(initiationSource));

            System.Collections.Generic.Dictionary<string, string> kvp = map;

            if (cloneMap)
                kvp = new System.Collections.Generic.Dictionary<string, string>(map);

            kvp["[TargetPath]"] = STEM.Sys.IO.Path.GetDirectoryName(initiationSource);
            
            kvp["[TargetDirectoryName]"] = STEM.Sys.IO.Path.GetFileName(kvp["[TargetPath]"]);
            
            if (!String.IsNullOrEmpty(kvp["[TargetPath]"]))
            {
                kvp["[TargetName]"] = STEM.Sys.IO.Path.GetFileName(initiationSource);
                kvp["[TargetNameWithoutExt]"] = STEM.Sys.IO.Path.GetFileNameWithoutExtension(initiationSource);
                kvp["[TargetExt]"] = STEM.Sys.IO.Path.GetExtension(initiationSource);
            }
            else
            {
                kvp["[TargetName]"] = "";
                kvp["[TargeteNameWithoutExt]"] = "";
                kvp["[TargetExt]"] = "";
            }

            kvp["[SourceAddress]"] = STEM.Sys.IO.Path.IPFromPath(initiationSource);

            string xml = iSetTemplate.SerializationSourceInstructionDocument;

            bool getFileInfo = false;

            if (xml.IndexOf("[LastWriteTimeUtc]", StringComparison.InvariantCultureIgnoreCase) >= 0)
                getFileInfo = true;

            if (getFileInfo == false)
                if (xml.IndexOf("[LastAccessTimeUtc]", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    getFileInfo = true;

            if (getFileInfo == false)
                if (xml.IndexOf("[CreationTimeUtc]", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    getFileInfo = true;

            if (getFileInfo == false)
                foreach (string k in kvp.Keys)
                {
                    if (kvp[k] != null)
                    {
                        if (kvp[k].IndexOf("[LastWriteTimeUtc]", StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            getFileInfo = true;
                            break;
                        }
                        if (kvp[k].IndexOf("[LastAccessTimeUtc]", StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            getFileInfo = true;
                            break;
                        }
                        if (kvp[k].IndexOf("[CreationTimeUtc]", StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            getFileInfo = true;
                            break;
                        }
                    }
                }

            if (getFileInfo)
            {
                FDCFileInfo info = GetFileInfo(initiationSource);

                foreach (string key in kvp.Keys.Where(i => i.Equals("[LastWriteTimeUtc]", StringComparison.InvariantCultureIgnoreCase)).ToList())
                    try
                    {
                        kvp[key] = info.LastWriteTimeUtc.ToString(kvp[key], System.Globalization.CultureInfo.CurrentCulture);
                    }
                    catch { }

                foreach (string key in kvp.Keys.Where(i => i.Equals("[LastAccessTimeUtc]", StringComparison.InvariantCultureIgnoreCase)).ToList())
                    try
                    {
                        kvp[key] = info.LastAccessTimeUtc.ToString(kvp[key], System.Globalization.CultureInfo.CurrentCulture);
                    }
                    catch { }

                foreach (string key in kvp.Keys.Where(i => i.Equals("[CreationTimeUtc]", StringComparison.InvariantCultureIgnoreCase)).ToList())
                    try
                    {
                        kvp[key] = info.CreationTimeUtc.ToString(kvp[key], System.Globalization.CultureInfo.CurrentCulture);
                    }
                    catch { }
            }

            string subDir = null;
            if (!String.IsNullOrEmpty(RecreateSubFromRootOf) && RecreateSubFromRootOf.Trim().Length > 0)
            {
                if (initiationSource.ToUpper(System.Globalization.CultureInfo.CurrentCulture).IndexOf(RecreateSubFromRootOf.ToUpper(System.Globalization.CultureInfo.CurrentCulture), StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    int startIndex = initiationSource.ToUpper(System.Globalization.CultureInfo.CurrentCulture).IndexOf(RecreateSubFromRootOf.ToUpper(System.Globalization.CultureInfo.CurrentCulture), StringComparison.InvariantCultureIgnoreCase) + RecreateSubFromRootOf.Length + 1;

                    if (startIndex < STEM.Sys.IO.Path.GetDirectoryName(initiationSource).Length)
                        subDir = initiationSource.Substring(startIndex, STEM.Sys.IO.Path.GetDirectoryName(initiationSource).Length - startIndex);
                }
            }

            if (subDir == null)
                subDir = "";

            kvp["[SubDir]"] = subDir;

            base.CustomizeInstructionSet(iSetTemplate, kvp, branchIP, initiationSource, false);
        }

        /// <summary>
        /// An impersonation safe way to get the time for which to base aging
        /// </summary>
        /// <param name="targetSource">The target of the time</param>
        /// <returns>The time to use for ageing sources</returns>
        public virtual DateTime GetAgeBasis(string targetSource)
        {
            FDCFileInfo fi = GetFileInfo(targetSource);

            if (fi == null)
            {
                FDCDirectoryInfo di = GetDirectoryInfo(targetSource);

                if (di == null)
                    return DateTime.MinValue;

                switch (SelectedOrigin)
                {
                    case STEM.Surge.AgeOrigin.LastWriteTime:
                        return di.LastWriteTimeUtc;

                    case STEM.Surge.AgeOrigin.LastAccessTime:
                        return di.LastAccessTimeUtc;

                    case STEM.Surge.AgeOrigin.CreationTime:
                        return di.CreationTimeUtc;
                }
            }
            
            switch (SelectedOrigin)
            {
                case STEM.Surge.AgeOrigin.LastWriteTime:
                    return fi.LastWriteTimeUtc;

                case STEM.Surge.AgeOrigin.LastAccessTime:
                    return fi.LastAccessTimeUtc;

                case STEM.Surge.AgeOrigin.CreationTime:
                    return fi.CreationTimeUtc;
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// An impersonation safe way to verify that targetSource meets any ageing requirements specified for this DeploymentController.
        /// </summary>
        /// <param name="targetSource">The targetSource to check</param>
        /// <returns>True if the targetSource meets any configured ageing requirements</returns>
        public virtual bool VerifyAge(string targetSource)
        {
            try
            {
                if (AgeTrigger)
                {
                    DateTime epoch = GetAgeBasis(targetSource);

                    if (epoch == DateTime.MinValue)
                        return false;

                    if ((DateTime.UtcNow - epoch).TotalSeconds < SecondsToAge)
                        return false;

                    return true;
                }
            }
            catch { }

            return false;
        }


        internal string _ImpersonateUser;
        internal string _ImpersonationPassword;
        internal bool _LocalUserImpersonation;
        
        /// <summary>
        /// Called with the parameters from the switchboard row for this source data
        /// </summary>
        /// <param name="impersonateUser"></param>
        /// <param name="impersonationPassword"></param>
        /// <param name="localUserImpersonation"></param>
        public void UseImpersonation(string impersonateUser, string impersonationPassword, bool localUserImpersonation)
        {
            _ImpersonateUser = impersonateUser;
            _ImpersonationPassword = impersonationPassword;
            _LocalUserImpersonation = localUserImpersonation;
        }
        
        /// <summary>
        /// An impersonation safe way to get FileInfo for this DeploymentController.
        /// </summary>
        /// <param name="file">The file for which FileInfo is being requested</param>
        /// <returns>The FileInfo for the specified file</returns>
        public virtual FDCFileInfo GetFileInfo(string file)
        {
            return GetFileInfo(file, _ImpersonateUser, _ImpersonationPassword, _LocalUserImpersonation);
        }

        /// <summary>
        /// An impersonation safe way to get FileInfo for this DeploymentController.
        /// </summary>
        /// <param name="file">The file for which FileInfo is being requested</param>
        /// <param name="user">The user to impersonate</param>
        /// <param name="password">The impersonated user password</param>
        /// <param name="isLocal">Whether the impersonated user is local to this server</param>
        /// <returns>The FileInfo for the specified file</returns>
        public virtual FDCFileInfo GetFileInfo(string file, string user, string password, bool isLocal)
        {
            Impersonation impersonated = null;
            FileInfo fi = null;
            try
            {
                if (!String.IsNullOrEmpty(user) && !String.IsNullOrEmpty(password))
                {
                    impersonated = new Impersonation();
                    impersonated.Impersonate(user, password, !isLocal);
                }

                if (File.Exists(file))
                    fi = new FileInfo(file);
            }
            finally
            {
                if (impersonated != null)
                {
                    impersonated.UnImpersonate();
                    impersonated = null;
                }
            }

            if (fi == null)
                return null;

            return new FDCFileInfo { CreationTimeUtc = fi.CreationTimeUtc, LastAccessTimeUtc = fi.LastAccessTimeUtc, LastWriteTimeUtc = fi.LastWriteTimeUtc, Size = fi.Length }; ;
        }
                
        /// <summary>
        /// An impersonation safe way to get DirectoryInfo for this DeploymentController.
        /// </summary>
        /// <param name="directory">The directory for which DirectoryInfo is being requested</param>
        /// <returns>The FileInfo for the specified file</returns>
        public virtual FDCDirectoryInfo GetDirectoryInfo(string directory)
        {
            return GetDirectoryInfo(directory, _ImpersonateUser, _ImpersonationPassword, _LocalUserImpersonation);
        }

        /// <summary>
        /// An impersonation safe way to get DirectoryInfo for this DeploymentController.
        /// </summary>
        /// <param name="directory">The directory for which DirectoryInfo is being requested</param>
        /// <param name="user">The user to impersonate</param>
        /// <param name="password">The impersonated user password</param>
        /// <param name="isLocal">Whether the impersonated user is local to this server</param>
        /// <returns>The FileInfo for the specified file</returns>
        public virtual FDCDirectoryInfo GetDirectoryInfo(string directory, string user, string password, bool isLocal)
        {
            Impersonation impersonated = null;
            DirectoryInfo di = null;
            try
            {
                if (!String.IsNullOrEmpty(user) && !String.IsNullOrEmpty(password))
                {
                    impersonated = new Impersonation();
                    impersonated.Impersonate(user, password, !isLocal);
                }

                if (Directory.Exists(directory))
                    di = new DirectoryInfo(directory);
            }
            finally
            {
                if (impersonated != null)
                {
                    impersonated.UnImpersonate();
                    impersonated = null;
                }
            }

            if (di == null)
                return null;

            return new FDCDirectoryInfo { CreationTimeUtc = di.CreationTimeUtc, LastAccessTimeUtc = di.LastAccessTimeUtc, LastWriteTimeUtc = di.LastWriteTimeUtc }; ;
        }

        /// <summary>
        /// An impersonation safe way to check file existence for this DeploymentController.
        /// </summary>
        /// <param name="file">The file for which an existence check is being requested</param>
        /// <returns>True if the file can be found</returns>
        public virtual bool FileExists(string file)
        {
            return FileExists(file, _ImpersonateUser, _ImpersonationPassword, _LocalUserImpersonation);
        }

        /// <summary>
        /// An impersonation safe way to check file existence for this DeploymentController.
        /// </summary>
        /// <param name="file">The file for which an existence check is being requested</param>
        /// <param name="user">The user to impersonate</param>
        /// <param name="password">The impersonated user password</param>
        /// <param name="isLocal">Whether the impersonated user is local to this server</param>
        /// <returns>True if the file can be found</returns>
        public virtual bool FileExists(string file, string user, string password, bool isLocal)
        {
            Impersonation impersonated = null;
            bool fileExists = false;
            try
            {
                if (!String.IsNullOrEmpty(user) && !String.IsNullOrEmpty(_ImpersonationPassword))
                {
                    impersonated = new Impersonation();
                    impersonated.Impersonate(user, _ImpersonationPassword, !isLocal);
                }

                fileExists = File.Exists(file);

                if (!fileExists)
                    fileExists = Directory.Exists(file);
            }
            finally
            {
                if (impersonated != null)
                {
                    impersonated.UnImpersonate();
                    impersonated = null;
                }
            }

            return fileExists;
        }

        /// <summary>
        /// An impersonation safe way to create a directory for this DeploymentController.
        /// </summary>
        /// <param name="directory">The directory to be created</param>
        public virtual void CreateDirectory(string directory)
        {
            CreateDirectory(directory, _ImpersonateUser, _ImpersonationPassword, _LocalUserImpersonation);
        }

        /// <summary>
        /// An impersonation safe way to create a directory for this DeploymentController.
        /// </summary>
        /// <param name="directory">The directory to be created</param>
        /// <param name="user">The user to impersonate</param>
        /// <param name="password">The impersonated user password</param>
        /// <param name="isLocal">Whether the impersonated user is local to this server</param>
        public virtual void CreateDirectory(string directory, string user, string password, bool isLocal)
        {
            if (!DirectoryExists(directory, user, password, isLocal))
            {
                Impersonation impersonated = null;
                try
                {
                    if (!String.IsNullOrEmpty(user) && !String.IsNullOrEmpty(password))
                    {
                        impersonated = new Impersonation();
                        impersonated.Impersonate(user, password, !isLocal);
                    }

                    Directory.CreateDirectory(directory);
                }
                finally
                {
                    if (impersonated != null)
                    {
                        impersonated.UnImpersonate();
                        impersonated = null;
                    }
                }
            }
        }

        /// <summary>
        /// An impersonation safe way to get a file listing for this DeploymentController.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="directoryFilter"></param>
        /// <param name="fileFilter"></param>
        /// <param name="recurse"></param>
        /// <returns>The file list</returns>
        public List<string> ListFiles(string directory, string directoryFilter, string fileFilter, bool recurse)
        {
            return ListFiles(directory, directoryFilter, fileFilter, recurse, _ImpersonateUser, _ImpersonationPassword, _LocalUserImpersonation);
        }

        /// <summary>
        /// An impersonation safe way to get a file listing for this DeploymentController.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="directoryFilter"></param>
        /// <param name="fileFilter"></param>
        /// <param name="recurse"></param>
        /// <param name="user">The user to impersonate</param>
        /// <param name="password">The impersonated user password</param>
        /// <param name="isLocal">Whether the impersonated user is local to this server</param>
        /// <returns>The file list</returns>
        public List<string> ListFiles(string directory, string directoryFilter, string fileFilter, bool recurse, string user, string password, bool isLocal)
        {
            try
            {
                if (!DirectoryExists(directory, user, password, isLocal))
                {
                    Impersonation impersonated = null;
                    try
                    {
                        if (!String.IsNullOrEmpty(user) && !String.IsNullOrEmpty(password))
                        {
                            impersonated = new Impersonation();
                            impersonated.Impersonate(user, password, !isLocal);
                        }

                        return STEM.Sys.IO.Directory.STEM_GetFiles(directory, fileFilter, directoryFilter, recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly, true);
                    }
                    finally
                    {
                        if (impersonated != null)
                        {
                            impersonated.UnImpersonate();
                            impersonated = null;
                        }
                    }
                }
            }
            catch { }

            return new List<string>();
        }

        /// <summary>
        /// An impersonation safe way to get a directory listing for this DeploymentController.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="directoryFilter"></param>
        /// <param name="recurse"></param>
        /// <returns>The directory list</returns>
        public List<string> ListDirectories(string directory, string directoryFilter, bool recurse)
        {
            return ListDirectories(directory, directoryFilter, recurse, _ImpersonateUser, _ImpersonationPassword, _LocalUserImpersonation);
        }

        /// <summary>
        /// An impersonation safe way to get a directory listing for this DeploymentController.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="directoryFilter"></param>
        /// <param name="recurse"></param>
        /// <param name="user">The user to impersonate</param>
        /// <param name="password">The impersonated user password</param>
        /// <param name="isLocal">Whether the impersonated user is local to this server</param>
        /// <returns>The directory list</returns>
        public List<string> ListDirectories(string directory, string directoryFilter, bool recurse, string user, string password, bool isLocal)
        {
            try
            {
                if (!DirectoryExists(directory, user, password, isLocal))
                {
                    Impersonation impersonated = null;
                    try
                    {
                        if (!String.IsNullOrEmpty(user) && !String.IsNullOrEmpty(password))
                        {
                            impersonated = new Impersonation();
                            impersonated.Impersonate(user, password, !isLocal);
                        }

                        return STEM.Sys.IO.Directory.STEM_GetDirectories(directory, directoryFilter, recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly, true);
                    }
                    finally
                    {
                        if (impersonated != null)
                        {
                            impersonated.UnImpersonate();
                            impersonated = null;
                        }
                    }
                }
            }
            catch { }

            return new List<string>();
        }

        /// <summary>
        /// An impersonation safe way to check directory existence for this DeploymentController.
        /// </summary>
        /// <param name="directory">The directory for which an existence check is being requested</param>
        /// <returns>True if the directory can be found</returns>
        public virtual bool DirectoryExists(string directory)
        {
            return DirectoryExists(directory, _ImpersonateUser, _ImpersonationPassword, _LocalUserImpersonation);
        }

        /// <summary>
        /// An impersonation safe way to check directory existence for this DeploymentController.
        /// </summary>
        /// <param name="directory">The directory for which an existence check is being requested</param>
        /// <param name="user">The user to impersonate</param>
        /// <param name="password">The impersonated user password</param>
        /// <param name="isLocal">Whether the impersonated user is local to this server</param>
        /// <returns>True if the directory can be found</returns>
        public virtual bool DirectoryExists(string directory, string user, string password, bool isLocal)
        {
            Impersonation impersonated = null;
            bool directoryExists = false;
            try
            {
                if (!String.IsNullOrEmpty(user) && !String.IsNullOrEmpty(password))
                {
                    impersonated = new Impersonation();
                    impersonated.Impersonate(user, password, !isLocal);
                }

                directoryExists = Directory.Exists(directory);

                if (!directoryExists)
                    directoryExists = File.Exists(directory);
            }
            finally
            {
                if (impersonated != null)
                {
                    impersonated.UnImpersonate();
                    impersonated = null;
                }
            }

            return directoryExists;
        }
    }
}



//////[TypeConverter(typeof(ExpandableObjectConverter))]
//////public abstract class FileDeploymentController : _FileDeploymentController
//////{
//////    public FileDeploymentController()
//////    {
//////    }

//////    internal List<string> _CoordinateWith { get; set; }

//////    public sealed override List<string> CoordinateWith
//////    {
//////        get
//////        {
//////            return _CoordinateWith;
//////        }
//////    }

//////    internal CoordinatedKeyManager _CoordinatedKeyManager = null;

//////    /// <summary>
//////    /// This KeyManager can be used to "lock" values across all coordinated Deployment Manager Services 
//////    /// in order to affect coordination control beyond the default "targetSource exclusive lock" provided by the STEM platform.
//////    /// </summary>
//////    protected sealed override CoordinatedKeyManager CoordinatedKeyManager { get { return _CoordinatedKeyManager; } }

//////    internal void _InstructionMessageReceived(InstructionMessage m, DeploymentDetails d)
//////    {
//////        try
//////        {
//////            if (m is ExecutionCompleted)
//////                ExecutionComplete(d, ((ExecutionCompleted)m).Exceptions);

//////            InstructionMessageReceived(m, d);
//////            MessageReceived(m);
//////        }
//////        catch (Exception ex)
//////        {
//////            STEM.Sys.EventLog.WriteEntry("DeploymentController._InstructionMessageReceived", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
//////        }
//////    }
//////}