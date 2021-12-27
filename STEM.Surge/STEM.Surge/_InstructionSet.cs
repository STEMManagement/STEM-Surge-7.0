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
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using STEM.Sys.IO.TCP;
using STEM.Sys.Messaging;
using STEM.Sys.State;
using STEM.Surge.Messages;
using System.Runtime.InteropServices;

namespace STEM.Surge
{
    /// <summary>
    /// This is the opensource base class for STEM.Surge.Internal InstructionSet
    /// See the bottom of this file for the Internal class implementation
    /// The InstructionSet class shouldn't need to be derived from and is therefore sealed
    /// 
    /// 
    /// virtual methods may be added in the future as opportunities for opensource developers to further customize
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class _InstructionSet : STEM.Sys.Serializable, IDisposable
    {
        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern void GetSystemTimePreciseAsFileTime(out long filetime);


        /// <summary>
        /// Unique ID of this instance
        /// </summary>
        [Browsable(false)]
        public Guid ID { get; set; }

        /// <summary>
        /// The IPV4 address of the branch executing this InstructionSet
        /// </summary>
        [Browsable(false)]
        public string BranchIP { get; set; }

        /// <summary>
        /// The time on the Deployment Manager that this InstructionSet was sent to the branch
        /// </summary>
        [Browsable(false)]
        public DateTime Assigned { get; set; }

        /// <summary>
        /// The time on the Branch Manager that this InstructionSet was received  
        /// </summary>
        [Browsable(false)]
        public DateTime Received { get; set; }

        /// <summary>
        /// The time on the Branch Manager that the execution was started
        /// </summary>
        [Browsable(false)]
        public DateTime Started { get; set; }

        /// <summary>
        /// The time on the Branch Manager that the execution was completed
        /// </summary>
        [Browsable(false)]
        public DateTime Completed { get; set; }
        
        /// <summary>
        /// An internal ID used to trace back to the Deployment Controller that generated this InstructionSet
        /// </summary>
        [Browsable(false)]
        public string DeploymentControllerID { get; set; }

        /// <summary>
        /// The IPV4 address of the Deployment Manager that generated this InstructionSet
        /// </summary>
        [Browsable(false)]
        public string DeploymentManagerIP { get; set; }

        /// <summary>
        /// The name of the Deployment Controller that generated this InstructionSet
        /// </summary>
        [Browsable(false)]
        public string DeploymentController { get; set; }

        /// <summary>
        /// The name of the InstructionSetTemplate used by the Deployment Controller to generate this InstructionSet
        /// </summary>
        [Browsable(false)]
        public string InstructionSetTemplate { get; set; }

        /// <summary>
        /// The value used by GenerateDeploymentDetails to customize an instance of the InstructionSetTemplate for this assignment 
        /// </summary>
        [Browsable(false)]
        public string InitiationSource { get; set; }

        /// <summary>
        /// The name used as a process name for the purpose of logging (this is equal to InstructionSetTemplate)
        /// </summary>
        [DisplayName("Process Name")]
        public string ProcessName { get; set; }

        /// <summary>
        /// If this is a static InstructionSet, should it be run continuously on an interval?
        /// </summary>
        [DisplayName("Run this Static InstructionSet continuously")]
        public bool ContinuousExecution { get; set; }

        /// <summary>
        /// If this is a static InstructionSet running on a continuous interval, this is the interval in seconds
        /// </summary>
        [DisplayName("When Static and ContinuousExecution, what's the interval (seconds)")]
        public int ContinuousExecutionInterval { get; set; }

        /// <summary>
        /// If this is a static InstructionSet, should it be run in sandboxes as well as the main branch service? 
        /// </summary>
        [Browsable(false)]
        [DisplayName("If this is a static InstructionSet, should it be run in sandboxes as well as the main branch service?")]
        public bool ExecuteStaticInSandboxes { get; set; }

        /// <summary>
        /// Should executed InstructionSets be cached after execution for post mortem evaluation?
        /// </summary>
        [DisplayName("Cache Post Mortem MetaData"), DescriptionAttribute("Should executed InstructionSets be cached after execution for post mortem evaluation?")]
        [Category("Post Mortem")]
        public bool CachePostMortem { get; set; }

        /// <summary>
        /// At configuration, or during execution, fill this with any metadata that may be useful to the evaluation of the execution post mortem
        /// </summary>
        [DisplayName("Post Mortem Evaluation MetaData"), DescriptionAttribute("During execution fill this with any metadata that may be useful to the evaluation of the execution post mortem.")]
        [Category("Post Mortem")]
        public STEM.Sys.Serialization.Dictionary<string, string> PostMortemMetaData { get; set; }

        /// <summary>
        /// A key used to verify that this InstructionSet was issued from a valid Deployment Manager
        /// </summary>
        [Browsable(false)]
        public string DeploymentManagerRelease { get; set; }

        MessageConnection Connection { get; set; }
        SurgeBranchManager BranchManager { get; set; }
               
        static Queue<StemStr> _ReuseQueue = new Queue<StemStr>();

        public override string Serialize()
        {
            StemStr str = null;

            if (_ReuseQueue.Count > 0)
                lock (_ReuseQueue)
                    if (_ReuseQueue.Count > 0)
                        str = _ReuseQueue.Dequeue();

            if (str == null)
                str = new StemStr("", 10000);

            try
            {
                str.Reset("<InstructionSet xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>");

                str.Append(VersionDescriptor.Serialize());
                str.Append("\r\n   <ID>" + ID.ToString() + "</ID>");
                str.Append("\r\n   <BranchIP>" + System.Security.SecurityElement.Escape(BranchIP) + "</BranchIP>");
                str.Append("\r\n   <Assigned>" + XmlConvert.ToString(Assigned, XmlDateTimeSerializationMode.Utc) + "</Assigned>");
                str.Append("\r\n   <Received>" + XmlConvert.ToString(Received, XmlDateTimeSerializationMode.Utc) + "</Received>");
                str.Append("\r\n   <Started>" + XmlConvert.ToString(Started, XmlDateTimeSerializationMode.Utc) + "</Started>");
                str.Append("\r\n   <Completed>" + XmlConvert.ToString(Completed, XmlDateTimeSerializationMode.Utc) + "</Completed>");
                str.Append("\r\n   <DeploymentControllerID>" + DeploymentControllerID + "</DeploymentControllerID>");
                str.Append("\r\n   <DeploymentManagerIP>" + DeploymentManagerIP + "</DeploymentManagerIP>");
                str.Append("\r\n   <DeploymentController>" + DeploymentController + "</DeploymentController>");
                str.Append("\r\n   <InstructionSetTemplate>" + System.Security.SecurityElement.Escape(InstructionSetTemplate) + "</InstructionSetTemplate>");
                str.Append("\r\n   <InitiationSource>" + System.Security.SecurityElement.Escape(InitiationSource) + "</InitiationSource>");
                str.Append("\r\n   <ProcessName>" + System.Security.SecurityElement.Escape(ProcessName) + "</ProcessName>");
                str.Append("\r\n   <ContinuousExecution>" + XmlConvert.ToString(ContinuousExecution) + "</ContinuousExecution>");
                str.Append("\r\n   <ContinuousExecutionInterval>" + XmlConvert.ToString(ContinuousExecutionInterval) + "</ContinuousExecutionInterval>");
                str.Append("\r\n   <ExecuteStaticInSandboxes>" + XmlConvert.ToString(ExecuteStaticInSandboxes) + "</ExecuteStaticInSandboxes>");
                str.Append("\r\n   <CachePostMortem>" + XmlConvert.ToString(CachePostMortem) + "</CachePostMortem>");
                str.Append("\r\n   <DeploymentManagerRelease>" + System.Security.SecurityElement.Escape(DeploymentManagerRelease) + "</DeploymentManagerRelease>");

                if (PostMortemMetaData != null && PostMortemMetaData.Count > 0)
                {
                    XDocument d = XDocument.Load(new System.Xml.XmlTextReader(new System.IO.StringReader(STEM.Sys.Serializable.Serialize(PostMortemMetaData))));
                    d.Root.Name = "PostMortemMetaData";
                    str.Append("\r\n   " + d.ToString());
                }
                else
                {
                    str.Append("\r\n   <PostMortemMetaData />");
                }

                str.Append("\r\n   <InstructionsXml>" + SerializationSourceInstructionDocument + "</InstructionsXml>");
                
                str.Append("\r\n</InstructionSet>");

                return str.ToString();
            }
            finally
            {
                if (_ReuseQueue.Count < 1000)
                    lock (_ReuseQueue)
                        _ReuseQueue.Enqueue(str);
            }
        }

        XDocument _InstructionDocument = null;
        string _SerializationSourceInstructionDocument = null;

        [Browsable(false)]
        [XmlIgnore]
        public string SerializationSourceInstructionDocument
        {
            get
            {
                lock (this)
                {
                    if (_SerializationSourceInstructionDocument == null)
                    {
                        XElement e = InstructionsXml;
                        _SerializationSourceInstructionDocument = e.ToString();
                    }

                    return _SerializationSourceInstructionDocument;
                }
            }

            set
            {
                lock (this)
                {
                    _SerializationSourceInstructionDocument = value;
                    _InstructionDocument = null;
                    _Instructions = null;
                }
            }
        }

        /// <summary>
        /// This is used for the actual serialized InstructionSet
        /// </summary>
        [Browsable(false)]
        public XElement InstructionsXml
        {
            get
            {
                lock (this)
                {
                    if (_InstructionDocument != null)
                    {
                        return _InstructionDocument.Root;
                    }

                    if (_SerializationSourceInstructionDocument == null)
                    {
                        string xml = "<Instructions>";
                        if (_Instructions != null)
                            foreach (Instruction i in _Instructions)
                                try
                                {
                                    xml += i.Serialize();
                                }
                                catch (Exception ex)
                                {
                                    STEM.Sys.EventLog.WriteEntry("InstructionSet.InstructionsXml_get", "Instruction serialization failed for: \r\n" + i.VersionDescriptor.TypeName + ", " + i.VersionDescriptor.AssemblyName + "\r\n in InstructionSet " + ProcessName + " " + VersionDescriptor.TypeName + ", " + VersionDescriptor.AssemblyName + "\r\n\r\n" + ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                                    throw new Exception("InstructionSet.InstructionsXml_get", ex);
                                }

                        xml += "</Instructions>";

                        using (System.IO.StringReader sr = new System.IO.StringReader(xml.ToString(System.Globalization.CultureInfo.CurrentCulture)))
                        {
                            using (System.Xml.XmlTextReader tr = new System.Xml.XmlTextReader(sr))
                            {
                                _InstructionDocument = XDocument.Load(tr);
                            }
                        }

                        return _InstructionDocument.Root;
                    }
                    else
                    {
                        using (System.IO.StringReader sr = new System.IO.StringReader(_SerializationSourceInstructionDocument))
                        {
                            using (System.Xml.XmlTextReader tr = new System.Xml.XmlTextReader(sr))
                            {
                                _InstructionDocument = XDocument.Load(tr);
                            }
                        }

                        return _InstructionDocument.Root;
                    }
                }
            }

            set
            {
                if (value != null)
                    lock (this)
                    {
                        _InstructionDocument = XDocument.Load(value.CreateReader());
                        _SerializationSourceInstructionDocument = _InstructionDocument.Root.ToString();
                        _Instructions = null;
                    }
            }
        }

        List<Instruction> _Instructions = null;

        /// <summary>
        /// This is the list of Instructions to be executed in order
        /// Instructions can modify this list during execution to add, modify, or delete Instructions
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public List<Instruction> Instructions
        {
            get
            {
                lock (this)
                {
                    if (_Instructions == null)
                        if (InstructionsXml != null)
                        {
                            _Instructions = new List<Instruction>();

                            foreach (XElement n in InstructionsXml.Elements())
                            {
                                try
                                {
                                    _Instructions.Add(Instruction.Deserialize(n.ToString()) as Instruction);
                                }
                                catch (Exception ex)
                                {
                                    STEM.Sys.EventLog.WriteEntry("InstructionSet.Instructions_get", "Instruction deserialization failed for: \r\n" + n.ToString() + "\r\n in InstructionSet " + ProcessName + " " + VersionDescriptor.TypeName + ", " + VersionDescriptor.AssemblyName + "\r\n\r\n" + ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                                    throw new Exception("InstructionSet.Instructions_get", ex);
                                }
                            }
                        }
                        else
                        {
                            _Instructions = new List<Instruction>();
                        }

                    _InstructionDocument = null;
                    _SerializationSourceInstructionDocument = null;

                    return _Instructions;
                }
            }
        }


        const string VDClose = "</VersionDescriptor>";
        const string IDOpen = "<ID>";

        /// <summary>
        /// A method to randomize Instruction Guid IDs without Deserializing the collection if possible
        /// </summary>
        public void RandomizeInstructionIDs()
        {
            lock (this)
            {
                List<Guid> ids = new List<Guid>();

                if (_Instructions != null)
                {
                    foreach (Instruction i in _Instructions)
                    {
                        i.ID = Guid.NewGuid();
                        ids.Add(i.ID);
                    }
                }

                if (_InstructionDocument != null)
                {
                    int x = 0;
                    foreach (XElement n in _InstructionDocument.Root.Elements())
                    {
                        if (ids.Count < x+1)
                            ids.Add(Guid.NewGuid());

                        n.Elements().FirstOrDefault(i => i.Name.LocalName == "ID").Value = ids[x].ToString();

                        x++;
                    }
                }

                if (_SerializationSourceInstructionDocument != null)
                {
                    _InstructionDocument = null;

                    StemStr str = new StemStr(_SerializationSourceInstructionDocument, _SerializationSourceInstructionDocument.Length);

                    int index = 0;
                    int x = 0;
                    while ((index = str.IndexOf(VDClose, index)) > -1)
                    {
                        index += VDClose.Length;
                        
                        int i = str.IndexOf(IDOpen, index);
                        if (i == -1)
                            continue;

                        index = i;
                        
                        index += IDOpen.Length;
                        
                        if (ids.Count < x + 1)
                            ids.Add(Guid.NewGuid());

                        str.Overwrite(ids[x].ToString(), index);

                        x++;
                    }

                    _SerializationSourceInstructionDocument = str.ToString();
                }
            }
        }

        /// <summary>
        /// A method to set Instruction Guid IDs to Empty without Deserializing the collection if possible
        /// </summary>
        public void ClearInstructionIDs()
        {
            lock (this)
            {
                if (_Instructions != null)
                    foreach (Instruction i in _Instructions)
                        i.ID = Guid.Empty;

                if (_InstructionDocument != null)
                {
                    foreach (XElement n in _InstructionDocument.Root.Elements())
                    {
                        n.Elements().FirstOrDefault(i => i.Name.LocalName == "ID").Value = Guid.Empty.ToString();
                    }
                }

                if (_SerializationSourceInstructionDocument != null)
                {
                    _InstructionDocument = null;

                    StemStr str = new StemStr(_SerializationSourceInstructionDocument, _SerializationSourceInstructionDocument.Length);

                    int index = 0;
                    while ((index = str.IndexOf(VDClose, index)) > -1)
                    {
                        index += VDClose.Length;

                        int i = str.IndexOf(IDOpen, index);
                        if (i == -1)
                            continue;

                        index = i;

                        index += IDOpen.Length;

                        str.Overwrite(Guid.Empty.ToString(), index);
                    }

                    _SerializationSourceInstructionDocument = str.ToString();
                }
            }
        }

        /// <summary>
        /// This method minimizes re-serialization by updating only Instruction xml nodes specified in the instructionOrdinals list
        /// </summary>
        /// <param name="instructionOrdinals">Instructions that need to be re-serialized because they have been modified</param>
        /// <returns></returns>
        public string SerializeWithLimitedRefresh(params int[] instructionOrdinals)
        {
            if (instructionOrdinals == null)
                throw new ArgumentNullException(nameof(instructionOrdinals));

            lock (this)
            {
                foreach (int instructionOrdinal in instructionOrdinals)
                {
                    if (instructionOrdinal < 0)
                        continue;

                    if (_Instructions != null)
                    {
                        if (_Instructions.Count <= instructionOrdinal)
                            continue;
                    }
                    else
                    {
                        if (Instructions.Count <= instructionOrdinal)
                            continue;
                    }

                    try
                    {
                        Instruction i = null;

                        _SerializationSourceInstructionDocument = null;
                        
                        if (_Instructions != null)
                            i = _Instructions[instructionOrdinal];
                        else
                            i = Instructions[instructionOrdinal];
                        
                        XElement e = InstructionsXml.Descendants().Where(x => x.Name.LocalName == "ID" && x.Value.Equals(i.ID.ToString(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                        e.Parent.ReplaceWith(XElement.Parse(i.Serialize()));
                    }
                    catch (Exception ex)
                    {
                        STEM.Sys.EventLog.WriteEntry("InstructionSet.SerializeWithLimitedRefresh", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                        _InstructionDocument = null;
                    }
                }

                return Serialize();
            }
        }
        
        /// <summary>
        /// This is a non serialized container used to pass objects by key between Instructions within this InstructionSet instance during execution
        /// This reaches end of life at the end of execution
        /// 
        /// Session should be used for objects to be shared among multiple InstructionSets within a Branch Manager service OR a Sandbox
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public Dictionary<string, object> InstructionSetContainer { get; private set; }

        /// <summary>
        /// Used to obtain exclusive locks within a Branch Manager service OR a Sandbox 
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public KeyManager KeyManager { get; private set; }

        public _InstructionSet()
        {
            Connection = null;

            DeploymentManagerRelease = "Empty";

            ID = Guid.NewGuid();
            BranchIP = "";
            Assigned = DateTime.MinValue;
            Received = DateTime.MinValue;
            Started = DateTime.MinValue;
            Completed = DateTime.MinValue;
            DeploymentControllerID = "Empty";
            DeploymentManagerIP = "Empty";
            DeploymentController = "Empty";
            InstructionSetTemplate = "Empty";
            InstructionSetContainer = new Dictionary<string, object>();
            ProcessName = "Unknown";
            InitiationSource = "Empty";
            ContinuousExecution = false;
            ContinuousExecutionInterval = 60;
            ExecuteStaticInSandboxes = false;
            CachePostMortem = false;
            PostMortemMetaData = new Sys.Serialization.Dictionary<string, string>();
        }


        public _InstructionSet(_InstructionSet iSet)
        {
            if (iSet == null)
                throw new ArgumentNullException(nameof(iSet));

            Connection = iSet.Connection;
            DeploymentManagerRelease = iSet.DeploymentManagerRelease;
            ID = iSet.ID;            
            BranchIP = iSet.BranchIP;            
            Assigned = iSet.Assigned;           
            Received = iSet.Received;            
            Started = iSet.Started;            
            Completed = iSet.Completed;
            DeploymentManagerIP = iSet.DeploymentManagerIP;
            DeploymentControllerID = iSet.DeploymentControllerID;
            DeploymentController = iSet.DeploymentController;      
            InstructionSetTemplate = iSet.InstructionSetTemplate;      
            InitiationSource = iSet.InitiationSource;            
            ProcessName = iSet.ProcessName;         
            ContinuousExecution = iSet.ContinuousExecution;         
            ContinuousExecutionInterval = iSet.ContinuousExecutionInterval;
            ExecuteStaticInSandboxes = iSet.ExecuteStaticInSandboxes;
            CachePostMortem = iSet.CachePostMortem;         
            ProcessName = iSet.ProcessName;
            PostMortemMetaData = new Sys.Serialization.Dictionary<string, string>(iSet.PostMortemMetaData);

            _Instructions = iSet._Instructions;
            _InstructionDocument = iSet._InstructionDocument;
            _SerializationSourceInstructionDocument = iSet._SerializationSourceInstructionDocument;
        }

        public abstract _InstructionSet Clone(_InstructionSet iSet);

        bool _Stop = false;

        /// <summary>
        /// Called to halt execution
        /// </summary>
        public void Stop()
        {
            if (_Stop)
                return;

            _Stop = true;

            foreach (Instruction i in Instructions)
            {
                if (i.Stage == STEM.Surge.Stage.Ready)
                {
                    i.Stop = true;
                    i.Stage = STEM.Surge.Stage.Stopped;
                }
            }
        }
               
        void PrepareSet()
        {
            if (_Instructions == null)
                _Instructions = Instructions;

            for (int xx = 0; xx < _Instructions.Count; xx++)
            {
                _Instructions[xx].SetOrdinalPosition(xx);
                _Instructions[xx].InstructionSet = this;

                if (_Instructions[xx].ExecutionStageHistory.Count == 0)
                    _Instructions[xx].ExecutionStageHistory.Add(_Instructions[xx].Stage);
            }
        }

        public abstract void Run(SurgeBranchManager branchManager, MessageConnection connection, string cacheDirectory, KeyManager keyManager, string clientAddress);

        static DateTime PreciseTime()
        {
            DateTime utcNow = DateTime.UtcNow;

            try
            {
                long filetime;
                GetSystemTimePreciseAsFileTime(out filetime);
                utcNow = DateTime.FromFileTimeUtc(filetime);
            }
            catch
            {
            }

            return utcNow;
        }

        protected void Run(SurgeBranchManager branchManager, MessageConnection connection, string cacheDirectory, KeyManager keyManager)
        {
            BranchManager = branchManager;
            Connection = connection;

            try
            {
                KeyManager = keyManager;

                Started = PreciseTime();
                
                if (cacheDirectory != null)
                    ReportMessage(new ExecutionStarted(this));

                this.PostMortemMetaData["ManagedThreadId"] = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString(System.Globalization.CultureInfo.CurrentCulture);

                PrepareSet();

                int iNum = 0;
                while (true)
                {
                    if (_Instructions.Count <= iNum)
                        break;

                    if (_Stop)
                        return;

                    PrepareSet();

                    _Instructions[iNum].Message = "";
                    _Instructions[iNum].Start = PreciseTime();

                    if (_Instructions[iNum].Stage == STEM.Surge.Stage.Skip || _Instructions[iNum].Stage == STEM.Surge.Stage.Completed)
                    {
                        string xml = SerializeWithLimitedRefresh(iNum - 1);

                        if (cacheDirectory != null)
                            File.WriteAllText(Path.Combine(cacheDirectory, ID.ToString() + ".is"), xml);

                        _Instructions[iNum].Finish = PreciseTime();
                        iNum++;
                        continue;
                    }
                    else
                    {
                        string xml = SerializeWithLimitedRefresh(iNum - 1, iNum);

                        if (cacheDirectory != null)
                            File.WriteAllText(Path.Combine(cacheDirectory, ID.ToString() + ".is"), xml);
                    }

                    try
                    {
                        _Instructions[iNum].PopulatePostMortemMeta = _Instructions[iNum].PopulatePostMortemMeta && CachePostMortem;
                        _Instructions[iNum].Run();
                    }
                    catch (Exception ex)
                    {
                        _Instructions[iNum].Exceptions.Add(ex);
                        STEM.Sys.EventLog.WriteEntry(ProcessName, ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                    }

                    _Instructions[iNum].Finish = PreciseTime();
                    iNum++;
                }

                SerializeWithLimitedRefresh(iNum - 1, iNum);
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("_InstructionSet.Run", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }
            finally
            {
                Completed = PreciseTime();

                _Stop = true;

                if (cacheDirectory != null)
                    try
                    {
                        ExecutionCompleted ec = new ExecutionCompleted(this);
                        foreach (Instruction i in _Instructions)
                        {
                            foreach (Exception e in i.Exceptions)
                            {
                                STEM.Sys.EventLog.WriteEntry(ProcessName, e.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                            }
                        }

                        ReportMessage(ec);
                    }
                    catch (Exception ex)
                    {
                        STEM.Sys.EventLog.WriteEntry("_InstructionSet.Run", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                    }

                bool error = false;
                try
                {
                    foreach (Instruction i in _Instructions)
                        if (i.Exceptions.Count > 0)
                        {
                            error = true;
                            break;
                        }
                }
                catch (Exception ex)
                {
                    STEM.Sys.EventLog.WriteEntry("InstructionSet.Run", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                }

                if (branchManager != null)
                    try
                    {
                        branchManager.EndRun(this, error);
                    }
                    catch (Exception ex)
                    {
                        STEM.Sys.EventLog.WriteEntry("InstructionSet.Run", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                    }

                Dispose();
            }
        }

        /// <summary>
        /// Send a message back to the Deployment Controller that issued this InstructionSet
        /// </summary>
        /// <param name="message"></param>
        /// <returns>True if delivered</returns>
        public bool ReportMessage(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (message is InstructionMessage)
            {
                InstructionMessage m = (InstructionMessage)message;

                m.DeploymentControllerID = DeploymentControllerID;
                m.InstructionSetID = ID;
            }

            try
            {
                return SurgeBranchManager.SendMessage(message, Connection, BranchManager);
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("_InstructionSet.Report", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return false;
        }

        protected enum FailureAction { ReturnInput, ThrowException, ReturnNull };

        [Flags]
        protected enum SearchSpace
        {
            InstructionSetContainer = 1,
            Session = 2,
            Cache = 3,
        }

        static Regex _Regex = new Regex(@"\[[^\]]*\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Used to replace tokens in a string from the searchSpace of interest
        /// 
        /// e.g. 
        /// 
        /// Session["[CompanyX DBAddress]"] = "192.168.100.100"
        /// Session["[CompanyX DBUser]"] = "sa"
        /// 
        /// string sqlConn = Server=[CompanyX DBAddress];Database=myDataBase;User Id=[CompanyX DBUser];
        /// sqlConn = ReplaceTokens(sqlConn, FailureAction.ThrowException, SearchSpace.Session)
        /// 
        /// </summary>
        /// <param name="tokenizedString"></param>
        /// <param name="failureAction"></param>
        /// <param name="searchSpace"></param>
        /// <returns>String with tokens replaced</returns>
        protected string ReplaceTokens(string tokenizedString, FailureAction failureAction, SearchSpace searchSpace = SearchSpace.InstructionSetContainer | SearchSpace.Session | SearchSpace.Cache)
        {
            if (String.IsNullOrEmpty(tokenizedString))
                throw new ArgumentNullException(nameof(tokenizedString));

            string xform = tokenizedString;            
            List<string> tokens = new List<string>();

            foreach (Match m in _Regex.Matches(tokenizedString))
                tokens.Add(m.Value);

            tokens = tokens.Distinct().ToList();

            foreach (string t in tokens.ToList())
            {
                if ((searchSpace & SearchSpace.InstructionSetContainer) != 0)
                {
                    if (InstructionSetContainer.ContainsKey(t))
                    {
                        xform = xform.Replace(t, (string)InstructionSetContainer[t]);
                        tokens.Remove(t);
                        continue;
                    }

                    string x = t.Replace("[", "");
                    x = x.Replace("]", "");

                    if (InstructionSetContainer.ContainsKey(x))
                    {
                        xform = xform.Replace(t, (string)InstructionSetContainer[x]);
                        tokens.Remove(t);
                        continue;
                    }
                }

                if ((searchSpace & SearchSpace.Session) != 0)
                {
                    if (STEM.Sys.Global.Session.ContainsKey(t))
                    {
                        xform = xform.Replace(t, (string)STEM.Sys.Global.Session[t]);
                        tokens.Remove(t);
                        continue;
                    }

                    string x = t.Replace("[", "");
                    x = x.Replace("]", "");

                    if (STEM.Sys.Global.Session.ContainsKey(x))
                    {
                        xform = xform.Replace(t, (string)STEM.Sys.Global.Session[x]);
                        tokens.Remove(t);
                        continue;
                    }
                }

                if ((searchSpace & SearchSpace.Cache) != 0)
                {
                    if (STEM.Sys.Global.Cache.ContainsKey(t))
                    {
                        xform = xform.Replace(t, (string)STEM.Sys.Global.Cache[t]);
                        tokens.Remove(t);
                        continue;
                    }

                    string x = t.Replace("[", "");
                    x = x.Replace("]", "");

                    if (STEM.Sys.Global.Cache.ContainsKey(x))
                    {
                        xform = xform.Replace(t, (string)STEM.Sys.Global.Cache[x]);
                        tokens.Remove(t);
                        continue;
                    }
                }
            }

            if (tokens.Count > 0)
            {
                switch (failureAction)
                {
                    case FailureAction.ReturnInput:
                        return tokenizedString;

                    case FailureAction.ReturnNull:
                        return null;

                    case FailureAction.ThrowException:
                        Exception e = new Exception("Tokens not found: " + String.Join(",", tokens));
                        STEM.Sys.EventLog.WriteEntry("InstructionSet.ReplaceTokens", e.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                        throw e;
                }
            }

            return xform;
        }
        
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("_InstructionSet.Dispose", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (_Instructions != null)
            {
                foreach (Instruction i in _Instructions)
                    i.Exceptions.Clear();

                foreach (Instruction i in _Instructions)
                    try
                    {
                        i.Dispose();
                    }
                    catch { }

                _Instructions.Clear();
            }

            InstructionSetContainer.Clear();
        }

        public abstract bool PopulateAuthenticationDetails(string authStore);
    }
}



//////[TypeConverter(typeof(ExpandableObjectConverter))]
//////public class InstructionSet : _InstructionSet
//////{
//////    public InstructionSet() : base()
//////    {
//////    }

//////    public InstructionSet(InstructionSet iSet) : base(iSet)
//////    {
//////    }

//////    public sealed override void Run(SurgeBranchManager branchManager, MessageConnection connection, string cacheDirectory, KeyManager keyManager, string clientAddress)
//////    {
//////        try
//////        {
//////            Started = DateTime.UtcNow;

//////            if (!SHA256.VerifyHash(######, DeploymentManagerRelease))
//////                throw new Exception("Invalid DeploymentManager Release.");

//////            Run(branchManager, connection, cacheDirectory, keyManager);
//////        }
//////        catch (Exception ex)
//////        {
//////            STEM.Sys.EventLog.WriteEntry("InstructionSet.Run", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
//////        }
//////    }
//////}




