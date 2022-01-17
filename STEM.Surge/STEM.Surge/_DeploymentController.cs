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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using STEM.Sys.State;
using STEM.Sys.Messaging;
using STEM.Surge.Messages;

namespace STEM.Surge
{
    /// <summary>
    /// This is the opensource base class for STEM.Surge.Internal DeploymentController
    /// See the bottom of this file for the Internal class implementation
    /// When creating a custom Deployment controller derive from either DeploymentController or FileDeploymentController not _DeploymentController or _FileDeploymentController
    /// 
    /// 
    /// virtual methods may be added in the future as opportunities for opensource developers to further customize
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class _DeploymentController : STEM.Sys.Serializable, IDisposable
    {
        public _DeploymentController()
        {
            AllowThreadedAssignment = true;
            UseSubnetCoordination = true;
            HonorPriorityFilters = false;
            PollError = "";
            PreprocessPerformsDiscovery = false;

            InstructionSetTemplate = "MyTemplate";
            OverrideInstructionSetProcessName = "";

            SandboxAppConfigXml = new List<string>();

            TemplateKVP = new STEM.Sys.Serialization.Dictionary<string, string>();
            PostMortemMetaData = new Sys.Serialization.Dictionary<string, string>();

            TemplateKVP["[MANAGERS]"] = "Reserved";
            TemplateKVP["[BRANCHES]"] = "Reserved";
            TemplateKVP["[NOSOURCE]"] = "Reserved";

            TemplateKVP["[ControllerVersionNumber]"] = "Reserved";
            TemplateKVP["[BranchIP]"] = "Reserved";
            TemplateKVP["[BranchName]"] = "Reserved";
            TemplateKVP["[ISetID]"] = "Reserved";
            TemplateKVP["[NewGuid]"] = "Reserved";
            TemplateKVP["[DeploymentControllerName]"] = "Reserved";
            TemplateKVP["[DeploymentControllerID]"] = "Reserved";
            TemplateKVP["[SwitchboardRowID]"] = "Reserved";
            TemplateKVP["[DeploymentManagerIP]"] = "Reserved";
            TemplateKVP["[InitiationSource]"] = "Reserved";
            TemplateKVP["[InstructionSetProcessName]"] = "Reserved";
            TemplateKVP["[PollerSourceString]"] = "Reserved";
            TemplateKVP["[PollerFileFilter]"] = "Reserved";
            TemplateKVP["[PollerDirectoryFilter]"] = "Reserved";
            TemplateKVP["[UtcNow]"] = "yyyy-MM-dd HH.mm.ss.fff";
        }


        STEM.Sys.IO.Listing.IAuthentication _SourceAuthentication = null;
        public STEM.Sys.IO.Listing.IAuthentication SourceAuthentication()
        {
            return _SourceAuthentication;
        }
        public void AssignSourceAuthentication(STEM.Sys.IO.Listing.IAuthentication sa)
        {
            _SourceAuthentication = sa;
        }

        /// <summary>
        /// An internal ID unique to this instance of a DeploymentController
        /// generated using source, file filter, directory filter
        /// such that it's repeatable
        /// </summary>
        [Browsable(false)]
        public string DeploymentControllerID { get; set; }

        /// <summary>
        /// An internal ID unique to all instances of DeploymentControllers
        /// created for the pool results from an expandable switchboard row,
        /// generated using expandable-source, file filter, directory filter
        /// such that it's repeatable
        /// </summary>
        [Browsable(false)]
        public string SwitchboardRowID { get; set; }

        /// <summary>
        /// The name of this Deployment Controller
        /// </summary>
        [Browsable(false)]
        public string DeploymentControllerName { get; set; }

        /// <summary>
        /// The source string for this configured Deployment Controller from the switchboard configuration row
        /// A single element from expandable ranges
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public string PollerSourceString { get; set; }

        /// <summary>
        /// The file filter string for this configured Deployment Controller from the switchboard configuration row
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public string PollerFileFilter { get; set; }

        /// <summary>
        /// The directory filter string for this configured Deployment Controller from the switchboard configuration row
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public string PollerDirectoryFilter { get; set; }

        /// <summary>
        /// The recurse boolean for this configured Deployment Controller from the switchboard configuration row
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public bool PollerRecurseSetting { get; set; }

        /// <summary>
        /// The pingable source boolean for this configured Deployment Controller from the switchboard configuration row
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public bool PingableSourceSetting { get; set; }

        /// <summary>
        /// The last poller error if there was one where PreprocessPerformsDiscovery is true, else empty and ignored
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public string PollError { get; set; }

        /// <summary>
        /// Set this to true in your constructor if all backlog polling/discovery is to be performed by ListPreprocess 
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public virtual bool PreprocessPerformsDiscovery { get; set; }

        /// <summary>
        /// Set if an alternate cache of assemblies is to be used for the execution of InstructionSets generated by this Controller
        /// </summary>
        [Category("Sandboxing")]
        [DisplayName("Alternate Assembly Store"), DescriptionAttribute("Should an alternate cache of assemblies be used for the execution of InstructionSets generated by this Controller?")]
        public string AlternateAssemblyStore { get; set; }

        /// <summary>
        /// An internal ID unique to this Controller's sandboxes executing on Branch Managers
        /// </summary>
        [Browsable(false)]
        public string SandboxID { get; set; }
               
        /// <summary>
        /// Set if an app config different from the Branch Manager default should be used when executing sandboxes
        /// </summary>
        [Category("Sandboxing")]
        [DisplayName("Sandbox App Config XML"), DescriptionAttribute("When running in a sandbox, what should the app config look like?")]
        public List<string> SandboxAppConfigXml { get; set; }


        /// <summary>
        /// Should reports be cached after list walk for post mortem evaluation?
        /// </summary>
        [Category("Post Mortem Evaluation")]
        [DisplayName("Cache Post Mortem MetaData"), DescriptionAttribute("Should reports be cached after list walk for post mortem evaluation?")]
        public bool CachePostMortem { get; set; }

        /// <summary>
        /// Allow any given listing to be assigned from, without regard to order, and from multiple managers simultaneously?
        /// </summary>
        [Category("Assignment Profile")]
        [DisplayName("Allow Async Assignment"), DescriptionAttribute("Allow any given listing to be assigned from, without regard to order, and from multiple managers simultaneously?")]
        public bool AllowThreadedAssignment { get; set; }

        /// <summary>
        /// Limit coordination efforts among managers based on subnet proximity?
        /// </summary>
        [Category("Assignment Profile")]
        [DisplayName("Use Subnet Coordination"), DescriptionAttribute("Limit coordination efforts among managers based on subnet proximity?")]
        public bool UseSubnetCoordination { get; set; }


        /// <summary>
        /// Request that the controller order assignments using the Priority Filter set if implemented?
        /// </summary>
        [Category("Assignment Profile")]
        [DisplayName("Honor Priority Filters"), DescriptionAttribute("Request that the controller order assignments using the Priority Filter set if implemented?")]
        public bool HonorPriorityFilters { get; set; }

        /// <summary>
        /// Often Controllers use a partially configured InstructionSet from disk in GenerateDeploymentDetails; this is the reference
        /// </summary>
        [Category("Instruction Set")]
        [DisplayName("InstructionSet Template"), DescriptionAttribute("What is the name of the InstructionSet Template to be used by this Controller?")]
        public string InstructionSetTemplate { get; set; }

        /// <summary>
        /// Have the Surge Deployment Manager set every InstructionSet Process Name to this value before deployment (empty or null results in no change).
        /// 
        /// e.g. If the InstructionSetTemplate is MoveFile then it's process name is MoveFile, but it may be used by many Deployment Controllers so a Deployment Controller
        /// named MoveArchiveFiles may choose to override the MoveFile process name to MoveArchiveFile for clarity
        /// </summary>
        [Category("Instruction Set")]
        [DisplayName("Override InstructionSet Process Name"), DescriptionAttribute("Have the Surge Deployment Manager set every InstructionSet Process Name to this value before deployment (empty or null results in no change).")]
        public string OverrideInstructionSetProcessName { get; set; }

        /// <summary>
        /// A list used to pass into CoordinatedKeyManagers to focus coordination efforts
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public abstract List<string> CoordinateWith { get; }

        /// <summary>
        /// A list optionally used by controllers to order assignments (mostly used by ListPreprocess)
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public abstract List<string> PriorityFilters { get; }

        /// <summary>
        /// This KeyManager can be used to "lock" values across all coordinated Deployment Manager Services 
        /// in order to affect coordination control beyond the default "initiationSource exclusive lock" provided by the Surge platform.
        /// </summary>
        protected abstract CoordinatedKeyManager CoordinatedKeyManager { get; }

        /// <summary>
        /// This is a Key Value Pair dictionary used to replace placeholders in Controllers and InstructionSet Templates  
        /// Preload from constructors with "Reserved" as the value if users aren't to edit because GenerateDeploymentDetails will be setting the value 
        /// </summary>
        [Browsable(false)]
        public STEM.Sys.Serialization.Dictionary<string, string> TemplateKVP { get; set; }

        /// <summary>
        /// A free form dictionary for reporting aditional Post Mortem metadata
        /// e.g. PostMortemMetaData["AverageWriteTime"] = writeTimes.Average().ToString(System.Globalization.CultureInfo.CurrentCulture)
        /// </summary>
        [Browsable(false)]
        [DisplayName("Post Mortem Evaluation MetaData"), DescriptionAttribute("During execution fill this with any metadata that may be useful to the evaluation of the execution post mortem.")]
        public STEM.Sys.Serialization.Dictionary<string, string> PostMortemMetaData { get; set; }
        
        StemStr _ReuseStemStr = null;

        /// <summary>
        /// Customize a target string by applying the TemplateKVP map to the target
        /// </summary>
        /// <param name="target">The string to which the map is to be applied</param>
        /// <param name="map">The TemplateKVP map used to modify the target</param>
        /// <param name="cloneMap">Should the map be cloned as it will be modified in this method</param>
        /// <returns>The modified target</returns>
        public virtual string ApplyKVP(string target, System.Collections.Generic.Dictionary<string, string> map, bool cloneMap = true)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            if (String.IsNullOrEmpty(target))
                throw new ArgumentNullException(nameof(target));

            System.Collections.Generic.Dictionary<string, string> kvp = map;

            if (cloneMap)
                kvp = new System.Collections.Generic.Dictionary<string, string>(map);

            kvp["[NewGuid]"] = "Reserved";

            if (!kvp.ContainsKey("[UtcNow]"))
                kvp["[UtcNow]"] = TemplateKVP["[UtcNow]"];

            kvp["[DeploymentControllerName]"] = DeploymentControllerName;

            kvp["[DeploymentControllerID]"] = DeploymentControllerID;

            kvp["[ControllerVersionNumber]"] = VersionNumber;

            kvp["[SwitchboardRowID]"] = SwitchboardRowID;

            kvp["[DeploymentManagerIP]"] = STEM.Sys.IO.Net.MachineIP();

            if (String.IsNullOrEmpty(OverrideInstructionSetProcessName))
                kvp["[InstructionSetProcessName]"] = STEM.Sys.IO.Path.GetFileNameWithoutExtension(InstructionSetTemplate);
            else
                kvp["[InstructionSetProcessName]"] = OverrideInstructionSetProcessName;

            kvp["[PollerSourceString]"] = PollerSourceString;

            kvp["[PollerFileFilter]"] = PollerFileFilter;

            kvp["[PollerDirectoryFilter]"] = PollerDirectoryFilter;

            kvp["[PollerRecurseSetting]"] = PollerRecurseSetting.ToString(System.Globalization.CultureInfo.CurrentCulture);

            kvp["[PingableSourceSetting]"] = PollerRecurseSetting.ToString(System.Globalization.CultureInfo.CurrentCulture);                      

            if (_ReuseStemStr == null)
                lock (this)
                    if (_ReuseStemStr == null)
                        _ReuseStemStr = new StemStr(target, target.Length * 2);

            string xml = null;

            lock (_ReuseStemStr)
                xml = KVPMapUtils.ApplyKVP(target, kvp, true, _ReuseStemStr);

            return xml;
        }

        /// <summary>
        /// Customize a target string by applying the TemplateKVP map to the target
        /// </summary>
        /// <param name="target">The string to which the map is to be applied</param>
        /// <param name="map">The TemplateKVP map used to modify the target</param>
        /// <param name="branchIP">The branchIP this is related to</param>
        /// <param name="initiationSource">The initiationSource passed in to GenerateDeploymentDetails()</param>
        /// <param name="cloneMap">Should the map be cloned as it will be modified in this method</param>
        /// <returns>The modified target</returns>
        public virtual string ApplyKVP(string target, System.Collections.Generic.Dictionary<string, string> map, string branchIP, string initiationSource, bool cloneMap = true)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            if (String.IsNullOrEmpty(target))
                throw new ArgumentNullException(nameof(target));

            if (String.IsNullOrEmpty(branchIP))
                throw new ArgumentNullException(nameof(branchIP));

            if (String.IsNullOrEmpty(initiationSource))
                throw new ArgumentNullException(nameof(initiationSource));

            System.Collections.Generic.Dictionary<string, string> kvp = map;

            if (cloneMap)
                kvp = new System.Collections.Generic.Dictionary<string, string>(map);
            
            kvp["[BranchIP]"] = branchIP;
            kvp["[BranchName]"] = STEM.Sys.IO.Net.MachineName(branchIP);
            
            kvp["[InitiationSource]"] = initiationSource;

            return ApplyKVP(target, kvp, false);
        }

        /// <summary>
        /// Customize an iSetTemplate by applying the TemplateKVP map to the InstructionsXml
        /// </summary>
        /// <param name="iSetTemplate">A clone of the template to be modified in this method</param>
        /// <param name="map">The TemplateKVP map used to modify the iSetTemplate</param>
        /// <param name="cloneMap">Should the map be cloned as it will be modified in this method</param>
        public virtual void CustomizeInstructionSet(_InstructionSet iSetTemplate, System.Collections.Generic.Dictionary<string, string> map, bool cloneMap = true)
        {
            if (iSetTemplate == null)
                throw new ArgumentNullException(nameof(iSetTemplate));

            if (map == null)
                throw new ArgumentNullException(nameof(map));

            iSetTemplate.SerializationSourceInstructionDocument = ApplyKVP(iSetTemplate.SerializationSourceInstructionDocument, map, cloneMap);
            iSetTemplate.RandomizeInstructionIDs();
        }

        /// <summary>
        /// Customize an iSetTemplate by applying the TemplateKVP map to the InstructionsXml
        /// </summary>
        /// <param name="iSetTemplate">A clone of the template to be modified in this method</param>
        /// <param name="map">The TemplateKVP map used to modify the iSetTemplate</param>
        /// <param name="branchIP">The branchIP this will be assigned to</param>
        /// <param name="initiationSource">The initiationSource passed in to GenerateDeploymentDetails()</param>
        /// <param name="cloneMap">Should the map be cloned as it will be modified in this method</param>
        public virtual void CustomizeInstructionSet(_InstructionSet iSetTemplate, System.Collections.Generic.Dictionary<string, string> map, string branchIP, string initiationSource, bool cloneMap = true)
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

            Guid iSetID = Guid.NewGuid();

            kvp["[BranchIP]"] = branchIP;
            kvp["[BranchName]"] = STEM.Sys.IO.Net.MachineName(branchIP);

            kvp["[ISetID]"] = iSetID.ToString();
            
            kvp["[InitiationSource]"] = initiationSource;
            
            CustomizeInstructionSet(iSetTemplate, kvp, false);

            iSetTemplate.ID = iSetID;
        }

        /// <summary>
        /// InstructionSets executing on a Branch may choose to send Messages back to the DeploymentController that generated the InstructionSet;
        /// these Messages are delivered to the DeploymentController through InstructionMessageReceived
        /// </summary>
        /// <param name="m">The message related to an Instruction</param>
        public virtual void MessageReceived(Message m)
        {
        }

        /// <summary>
        /// InstructionSets executing on a Branch may choose to send Messages back to the DeploymentController that generated the InstructionSet;
        /// these Messages are delivered to the DeploymentController through InstructionMessageReceived
        /// </summary>
        /// <param name="m">The message related to an Instruction</param>
        /// <param name="d">The DeploymentDetails instance related to the InstructionSet that generated the Message</param>
        public virtual void InstructionMessageReceived(InstructionMessage m, DeploymentDetails d)
        {
        }
               
        /// <summary>
        /// Called when the DeploymentController instance is being disposed
        /// </summary>
        public virtual void Disposing()
        {
        }

        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("_DeploymentController.Dispose", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            try
            {
                Disposing();
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("_DeploymentController.Dispose.Disposing", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// RememberDeployment is called with DeploymentDetails that should be known to your Controller.
        /// If no independent bookkeeping is being performed by your custom Controller, simply ignore these calls.
        /// </summary>
        /// <param name="details">The instance of DeploymentDetails that this Controller is being asked to remember</param>
        public virtual void RememberDeployment(DeploymentDetails details)
        {
        }

        /// <summary>
        /// BranchStatusUpdate is called to update the Controller on Branch availability
        /// </summary>
        /// <param name="address">The Branch IP Address whose state is being updated</param>
        /// <param name="state">Latest BranchState</param>
        public virtual void BranchStatusUpdate(string address, BranchState state)
        {
        }

        /// <summary>
        /// ExecutionComplete is called to inform the Controller that execution has completed.
        /// </summary>
        /// <param name="details">The instance of DeploymentDetails that this Controller issued</param>
        /// <param name="exceptions">A list of exceptions if any</param>
        public virtual void ExecutionComplete(DeploymentDetails details, List<Exception> exceptions)
        {
        }

        /// <summary>
        /// Gives the Controller the opportunity to override any other Branch limits
        /// for this single targetSoinitiationSourceurce assignment attempt
        /// </summary>
        /// <param name="initiationSource">The target </param>
        /// <param name="limitedToBranches">The Branch IPs that this assignment is limited to already</param>
        /// <returns>A list of Branch IPs that can be used for this initiationSource for the next assignment request</returns>
        public virtual List<string> LimitBranchesFor(string initiationSource, IReadOnlyList<string> limitedToBranches)
        {
            return new List<string>();
        }

        /// <summary>
        /// This method presents opportunity for the Controller to evaluate the list of sources found in the 
        /// sourceDirectory using the directoryFilter and fileFilter. The Controller can simply
        /// choose to return the list unaltered or perform some work and return a list strings that will
        /// be evaluated one at a time and passed to GenerateDeploymentDetails().
        /// </summary>
        /// <param name="list">The list of sources found</param>
        /// <returns>The list of strings to be walked</returns>
        public virtual List<string> ListPreprocess(IReadOnlyList<string> list)
        {
            return ApplyPriorityFilterOrdering(list);
        }

        /// <summary>
        /// This method presents opportunity for the Controller to evaluate the list of sources found in the 
        /// sourceDirectory using the directoryFilter and fileFilter. The Controller can simply
        /// choose to return the list unaltered or perform some work and return a list strings that will
        /// be evaluated one at a time and passed to GenerateDeploymentDetails().
        /// </summary>
        /// <param name="list">The list of sources found</param>
        /// <returns>The list of strings to be walked</returns>
        public virtual List<string> ApplyPriorityFilterOrdering(IReadOnlyList<string> list)
        {
            if (HonorPriorityFilters)
            {
                if (list.Count == 0)
                    return new List<string>();

                List<string> filters = PriorityFilters;

                if (filters == null || filters.Count == 0)
                    return new List<string>(list);

                Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
                foreach (string filter in filters)
                    results[filter] = new List<string>();

                List<string> working = new List<string>(list);

                foreach (string filter in filters)
                {
                    Regex inFilter = STEM.Sys.IO.Path.BuildInclusiveFilter(filter);
                    Regex exFilter = STEM.Sys.IO.Path.BuildExclusiveFilter(filter);

                    results[filter] = STEM.Sys.IO.Path.WhereStringsMatch(working, inFilter);
                    results[filter] = STEM.Sys.IO.Path.WhereStringsNotMatch(results[filter], exFilter);

                    if (results[filter].Count > 0)
                        working = working.Except(results[filter]).ToList();
                }

                List<string> ret = new List<string>();
                foreach (string filter in filters)
                    if (results[filter].Count > 0)
                        ret.AddRange(results[filter]);

                if (working.Count > 0)
                    ret.AddRange(working);

                return ret;
            }
            else
            {
                return new List<string>(list);
            }
        }

        static Dictionary<string, _InstructionSet> _InstructionSet = new Dictionary<string, _InstructionSet>(StringComparer.InvariantCultureIgnoreCase);
        static Dictionary<string, DateTime> _LastWriteTime = new Dictionary<string, DateTime>(StringComparer.InvariantCultureIgnoreCase);

        public static string GetFileText(string filename)
        {
            return STEM.Sys.IO.TextFileManager.GetFileText(filename);
        }

        protected virtual _InstructionSet GetTemplateInstance(bool cloneTemplate = false)
        {
            return GetTemplateInstance(InstructionSetTemplate, cloneTemplate);
        }

        protected virtual _InstructionSet GetTemplateInstance(string templateName, bool cloneTemplate = false)
        {
            string txt = STEM.Sys.IO.TextFileManager.GetFileText(templateName);

            if (txt != null)
            {
                DateTime lwt = STEM.Sys.IO.TextFileManager.GetFileLastWriteTime(templateName);
                if (!_LastWriteTime.ContainsKey(templateName) || lwt > _LastWriteTime[templateName])
                {
                    try
                    {
                        _InstructionSet[templateName] = STEM.Sys.Serializable.Deserialize(txt) as _InstructionSet;
                        _LastWriteTime[templateName] = lwt;
                    }
                    catch { }
                }
            }

            if (_InstructionSet.ContainsKey(templateName))
            {
                _InstructionSet iSet = _InstructionSet[templateName];

                if (cloneTemplate)
                    return iSet.Clone(iSet);

                return iSet;
            }

            return null;
        }

        /// <summary>
        /// This method generates InstructionSets upon request. When called, GenerateDeploymentDetails is assured that tainitiationSourcergetSource is
        /// exclusively "locked" for assignment at that time.
        /// </summary>
        /// <param name="listPreprocessResult">The list of initiationSources returned from the most recent call to ListPreprocess</param>
        /// <param name="initiationSource">The initiationSource from the listPreprocessResult list for which an InstructionSet is being requested</param>
        /// <param name="recommendedBranchIP">The Branch IP that is preferred for this assignment</param>
        /// <param name="limitedToBranches">The Branch IPs that this assignment is limited to in case you choose to override recommendedBranchIP</param>
        /// <returns>The DeploymentDetails instance containing the InstructionSet to be assigned, or null if no assignment should be made at this time</returns>
        public virtual DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            DeploymentDetails ret = null;
            try
            {
                _InstructionSet clone = GetTemplateInstance(true);

                CustomizeInstructionSet(clone, TemplateKVP, recommendedBranchIP, initiationSource, true);

                bool updated = clone.PopulateAuthenticationDetails(GetAuthenticationStore());

                if (updated)
                    CustomizeInstructionSet(clone, TemplateKVP, recommendedBranchIP, initiationSource, true);

                return new DeploymentDetails(clone, recommendedBranchIP);
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("DeploymentController.GenerateDeploymentDetails", new Exception(InstructionSetTemplate + ": " + initiationSource, ex).ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return ret;
        }

        protected abstract string GetAuthenticationStore();

    }
}






//////[TypeConverter(typeof(ExpandableObjectConverter))]
//////public abstract class DeploymentController : _DeploymentController
//////{
//////    public DeploymentController()
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

//////    internal bool _PreprocessPerformsDiscovery
//////    {
//////        get
//////        {
//////            return PreprocessPerformsDiscovery;
//////        }
//////        set
//////        {
//////            PreprocessPerformsDiscovery = value;
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