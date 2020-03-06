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
using System.Xml.Linq;
using System.Xml.Serialization;

namespace STEM.Surge
{
    /// <summary>
    /// This is the opensource base class for all STEM.Surge Instructions
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class Instruction : STEM.Sys.Serializable, IDisposable
    {
        /// <summary>
        /// Unique ID of this instance
        /// </summary>
        [Browsable(false)]
        public Guid ID { get; set; }

        /// <summary>
        /// A label used in flow control to jump to this Instruction
        /// </summary>
        [DisplayName("Flow Control label")]
        [Category("Flow")]
        public string FlowControlLabel { get; set; }

        [DisplayName("Target Label")]
        [Category("Flow")]
        [Description("The label to skip forward to when FailureAction == SkipToLabel")]
        public string FailureActionLabel { get; set; }

        /// <summary>
        /// What to do in the event _Run returns false
        /// </summary>
        [DisplayName("Failure Action")]
        [Category("Flow")]
        public FailureAction FailureAction { get; set; }

        /// <summary>
        /// For internal use only
        /// The last known stage of execution
        /// </summary>
        [DisplayName("Execution Stage")]
        [Category("Flow")]
        public Stage Stage { get; set; }

        /// <summary>
        /// For internal use only
        /// Tracks the history of execution Stage settings
        /// </summary>
        [Browsable(false)]
        public List<Stage> ExecutionStageHistory { get; set; }

        /// <summary>
        /// For internal use only
        /// The free text message string set in _Run
        /// </summary>
        [Browsable(false)]
        public string Message { get; set; }

        /// <summary>
        /// For internal use only
        /// The time _Run was called (MinDate before execution start)
        /// </summary>
        [Browsable(false)]
        public DateTime Start { get; set; }

        /// <summary>
        /// For internal use only
        /// The time _Run finished (MinDate before execution complete)
        /// </summary>
        [Browsable(false)]
        public DateTime Finish { get; set; }

        /// <summary>
        /// Set to true if the execution of this Instruction should be halted
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public bool Stop { get; set; }

        /// <summary>
        /// _Run can add any exceptions that should be reported upon completion
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public List<Exception> Exceptions { get; set; }

        /// <summary>
        /// For serialization use
        /// </summary>
        [Browsable(false)]
        public XElement ExceptionsXml
        {
            get
            {
                string xml = "<Exceptions>";
                foreach (Exception e in Exceptions)
                {
                    string m = e.ToString();
                    xml += "<Exception>" + System.Security.SecurityElement.Escape(m) + "</Exception>";
                }

                xml += "</Exceptions>";

                XDocument doc = XDocument.Parse(xml);

                return doc.Root;
            }

            set
            {
                if (value != null)
                    foreach (XElement n in value.Elements())
                        Exceptions.Add(new Exception(n.Value));
            }
        }

        /// <summary>
        /// A free form dictionary for reporting aditional Post Mortem metadata
        /// e.g. PostMortemMetaData["AverageWriteTime"] = writeTimes.Average().ToString(System.Globalization.CultureInfo.CurrentCulture)
        /// </summary>
        [Browsable(false)]
        [Category("Post Mortem")]
        [DisplayName("Post Mortem Evaluation MetaData"), DescriptionAttribute("During execution fill this with any metadata that may be useful to the evaluation of the execution post mortem.")]
        public STEM.Sys.Serialization.Dictionary<string, string> PostMortemMetaData { get; set; }

        /// <summary>
        /// A boolean indicating if PostMortemMetaData dictionary population is desired
        /// </summary>
        [DisplayName("Populate Post Mortem MetaData"), DescriptionAttribute("Should the Post Mortem Evaluation MetaData be populated during execution?")]
        [Category("Post Mortem")]
        public bool PopulatePostMortemMeta { get; set; }

        /// <summary>
        /// Provides get access to the InstructionSet instance of which this instruction is a member
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public _InstructionSet InstructionSet { get; internal set; }

        /// <summary>
        /// Provides get access to an instructions ordinal position within this InstructionSet instance (0-based)
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public int OrdinalPosition { get; private set; }
        
        internal void SetOrdinalPosition(int op)
        {
            OrdinalPosition = op;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Instruction()
        {
            ID = Guid.NewGuid();
            FlowControlLabel = "";
            FailureActionLabel = "";
            FailureAction = FailureAction.Continue;
            Message = "Unexecuted";
            Stage = Stage.Ready;
            ExecutionStageHistory = new List<Stage>();
            Start = Finish = DateTime.MinValue;
            Stop = false;
            Exceptions = new List<Exception>();
            PostMortemMetaData = new Sys.Serialization.Dictionary<string, string>();
            PopulatePostMortemMeta = true;
            OrdinalPosition = 0;
        }

        /// <summary>
        /// Called from within Instructions executing _Run, this affects a skip of all remaining Instructions within this InstructionSet
        ///
        /// This is the same as returning false from _Run with a FailureAction = FailureAction.SkipRemaining
        /// </summary>
        protected void SkipRemaining()
        {
            try
            {
                if (InstructionSet != null)
                    for (int i = OrdinalPosition + 1; i < InstructionSet.Instructions.Count; i++)
                        if (InstructionSet.Instructions[i].Stage == Stage.Ready)
                        {
                            InstructionSet.Instructions[i].Stage = Stage.Skip;
                            InstructionSet.Instructions[i].ExecutionStageHistory.Add(Stage.Skip);
                        }
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
            }
        }

        /// <summary>
        /// Called from within Instructions executing _Run, this affects a skip of the next Instruction within this InstructionSet
        /// </summary>
        protected void SkipNext()
        {
            if (InstructionSet != null)
            {
                try
                {
                    InstructionSet.Instructions[OrdinalPosition + 1].Stage = Stage.Skip;
                    InstructionSet.Instructions[OrdinalPosition + 1].ExecutionStageHistory.Add(Stage.Skip);
                }
                catch { }
            }
        }

        /// <summary>
        /// Called from within Instructions executing _Rollback, this affects a skip of the previous Instruction within this InstructionSet
        /// </summary>
        protected void SkipPrevious()
        {
            if (InstructionSet != null)
            {
                try
                {
                    InstructionSet.Instructions[OrdinalPosition - 1].Stage = Stage.Skip;
                    InstructionSet.Instructions[OrdinalPosition - 1].ExecutionStageHistory.Add(Stage.Skip);
                }
                catch { }
            }
        }

        /// <summary>
        /// Called from within Instructions executing _Run, this affects a skip of all following Instructions up to, but not including, the Instruction whose FlowControlLabel == label
        /// </summary>
        /// <param name="label">The label to skip to. Specifying a label that does not exist will cause a skip of all remaining Instructions.</param>
        protected void SkipForwardToFlowControlLabel(string label)
        {
            if (String.IsNullOrEmpty(label))
                return;

            if (InstructionSet != null)
            {
                try
                {
                    for (int i = OrdinalPosition + 1; i < InstructionSet.Instructions.Count; i++)
                    {
                        if (!InstructionSet.Instructions[i].FlowControlLabel.Equals(label, StringComparison.InvariantCultureIgnoreCase))
                        {
                            InstructionSet.Instructions[i].Stage = Stage.Skip;
                            InstructionSet.Instructions[i].ExecutionStageHistory.Add(Stage.Skip);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Called from within Instructions executing _Rollback, this affects a skip of all previous Instructions up to, but not including, the Instruction whose FlowControlLabel == label
        /// </summary>
        /// <param name="label">The label to skip to. Specifying a label that does not exist will cause a skip of all previous Instructions.</param>
        protected void SkipBackwardToFlowControlLabel(string label)
        {
            if (String.IsNullOrEmpty(label))
                return;

            if (InstructionSet != null)
            {
                try
                {
                    for (int i = OrdinalPosition - 1; i >= 0; i--)
                    {
                        if (!InstructionSet.Instructions[i].FlowControlLabel.Equals(label, StringComparison.InvariantCultureIgnoreCase))
                        {
                            InstructionSet.Instructions[i].Stage = Stage.Skip;
                            InstructionSet.Instructions[i].ExecutionStageHistory.Add(Stage.Skip);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Called from within Instructions executing _Run, this affects a rollback of prceeding Instructions executed and a 
        /// skip of all remaining Instructions within this InstructionSet without calling _Rollback on this Instruction
        /// </summary>
        protected void RollbackAllPreceedingAndSkipRemaining()
        {
            try
            {
                if (InstructionSet != null)
                {
                    for (int i = OrdinalPosition - 1; i >= 0; i--)
                        try
                        {
                            InstructionSet.Instructions[i].Rollback();
                        }
                        catch (Exception ex)
                        {
                            Exceptions.Add(ex);
                        }

                    for (int i = OrdinalPosition + 1; i < InstructionSet.Instructions.Count; i++)
                        if (InstructionSet.Instructions[i].Stage == Stage.Ready)
                        {
                            InstructionSet.Instructions[i].Stage = Stage.Skip;
                            InstructionSet.Instructions[i].ExecutionStageHistory.Add(Stage.Skip);
                        }
                }
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
            }
        }

        public void Rollback()
        {
            if (Stage == Stage.Completed || Stage == STEM.Surge.Stage.Stopped)
            {
                try
                {
                    _Rollback();
                }
                catch (Exception ex)
                {
                    Exceptions.Add(ex);
                }

                Stage = Stage.RolledBack;
                ExecutionStageHistory.Add(Stage.RolledBack);
            }
        }

        /// <summary>
        /// Manages the execution of the implementation of _Run()
        /// </summary>
        public void Run()
        {
            if (Stop)
                return;

            bool success = true;
            try
            {
                if (Stage != STEM.Surge.Stage.Ready)
                    return;

                InstructionSet.InstructionSetContainer["_Run Called"] = DateTime.UtcNow;

                success = _Run();

                if (Stop)
                {
                    Stage = Stage.Stopped;
                    ExecutionStageHistory.Add(Stage.Stopped);
                }
                else
                {
                    Stage = Stage.Completed;
                    ExecutionStageHistory.Add(Stage.Completed);
                }
            }
            catch (Exception ex)
            {
                Stage = STEM.Surge.Stage.Completed;
                ExecutionStageHistory.Add(Stage.Completed);
                success = false;
                Exceptions.Add(ex);
            }
            finally
            {
                try
                {
                    if (InstructionSet != null)
                    {
                        if (!success)
                        {
                            switch (FailureAction)
                            {
                                case STEM.Surge.FailureAction.Continue:
                                    break;

                                case STEM.Surge.FailureAction.Rollback:
                                    for (int i = OrdinalPosition; i >= 0; i--)
                                    {
                                        try
                                        {
                                            InstructionSet.Instructions[i].Rollback();
                                        }
                                        catch (Exception ex)
                                        {
                                            Exceptions.Add(ex);
                                        }
                                    }

                                    SkipRemaining();
                                    break;

                                case STEM.Surge.FailureAction.SkipRemaining:
                                    SkipRemaining();
                                    break;

                                case Surge.FailureAction.SkipNext:
                                    SkipNext();
                                    break;

                                case Surge.FailureAction.SkipToLabel:

                                    SkipForwardToFlowControlLabel(FailureActionLabel);
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    STEM.Sys.EventLog.WriteEntry(InstructionSet.ProcessName, ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                }
            }

            return;
        }

        /// <summary>
        /// Called from within Instructions to add to the Message related to the execution of this Instruction
        /// </summary>
        /// <param name="message">A message segment</param>
        protected void AppendToMessage(string message)
        {
            try
            {
                if (!string.IsNullOrEmpty(Message))
                    Message += Environment.NewLine;

                Message += message;
            }
            catch { }
        }

        /// <summary>
        /// Called from within Instructions to add to the Message related to the execution of this Instruction
        /// </summary>
        /// <param name="message">A message segment</param>
        /// <param name="args">Used in the following call string.Format(message, args)</param>
        protected void AppendToMessage(string message, params object[] args)
        {
            AppendToMessage(string.Format(System.Globalization.CultureInfo.CurrentCulture, message, args));
        }

        /// <summary>
        /// This is where derived classes define their enterprise specific run functionality
        /// </summary>
        /// <returns>true to continue and false to engage FailureAction</returns>
        protected abstract bool _Run();

        /// <summary>
        /// This is where derived classes define their enterprise specific rollback functionality
        /// </summary>
        protected abstract void _Rollback();

        /// <summary>
        /// Send a message back to the Deployment Controller that generated this InstructionSet
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns>True if successfully delivered</returns>
        public bool ReportMessage(STEM.Sys.Messaging.Message message)
        {
            return InstructionSet.ReportMessage(message);
        }

        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Instruction.Dispose", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
        }
    }
}

