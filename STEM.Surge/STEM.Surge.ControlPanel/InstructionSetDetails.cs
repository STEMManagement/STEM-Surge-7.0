using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using System.Linq;
using System.Windows.Forms;

namespace STEM.Surge.ControlPanel
{
    public partial class InstructionSetDetails : Form
    {
        public InstructionSetDetails(Surge._InstructionSet iSet)
        {
            InitializeComponent();

            try
            {
                Text = iSet.ProcessName;
                
                XmlTextReader reader = new XmlTextReader(new StringReader(iSet.Serialize()));
                int depth = -1;
                bool inValue = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:

                            depth++;

                            inValue = false;

                            xmlText.AppendText("\n");

                            for (int i = 0; i < depth; i++)
                                xmlText.AppendText("   ");

                            xmlText.SelectionColor = System.Drawing.Color.Blue;

                            xmlText.AppendText("<");

                            xmlText.SelectionColor = System.Drawing.Color.Brown;

                            xmlText.AppendText(reader.Name);

                            if (reader.HasAttributes)
                            {
                                xmlText.SelectionColor = System.Drawing.Color.Red;

                                reader.MoveToFirstAttribute();

                                do
                                {
                                    xmlText.AppendText(" " + reader.Name + "='" + reader.Value + "'");
                                } while (reader.MoveToNextAttribute());
                            }

                            xmlText.SelectionColor = System.Drawing.Color.Blue;

                            if (!reader.IsEmptyElement)
                            {
                                xmlText.AppendText(">");
                            }
                            else
                            {
                                depth--;
                                xmlText.AppendText(" />");
                            }

                            break;

                        case XmlNodeType.Text:

                            inValue = true;

                            xmlText.SelectionColor = System.Drawing.Color.Black;

                            xmlText.AppendText(reader.Value);

                            break;

                        case XmlNodeType.EndElement:
                            
                            if (!inValue)
                            {
                                xmlText.AppendText("\n");

                                for (int i = 0; i < depth; i++)
                                    xmlText.AppendText("   ");
                            }

                            depth--;
                            inValue = false;

                            xmlText.SelectionColor = System.Drawing.Color.Blue;

                            xmlText.AppendText("</");

                            xmlText.SelectionColor = System.Drawing.Color.Brown;

                            xmlText.AppendText(reader.Name);

                            xmlText.SelectionColor = System.Drawing.Color.Blue;

                            xmlText.AppendText(">");

                            break;

                    }

                }
                
                List<string> lines = new List<string>();

                lines.Add("Instruction Set Details");
                lines.Add("Initiation Source: " + iSet.InitiationSource);
                lines.Add("Instructions: " + iSet.Instructions.Count);
                lines.Add("Executable Instructions: " + iSet.Instructions.Where(i => i.Stage != Stage.Skip).Count());
                lines.Add("Executed Instructions: " + iSet.Instructions.Where(i => i.Stage != Stage.Skip && i.Stage != Stage.Ready).Count());
                lines.Add("Assigned From: " + iSet.DeploymentManagerIP);
                lines.Add("Assigned: " + iSet.Assigned.ToString("yyyy/MM/dd HH:mm:ss.fffffff"));
                lines.Add("Executed On: " + iSet.BranchIP);
                lines.Add("Receved: " + iSet.Received.ToString("yyyy/MM/dd HH:mm:ss.fffffff"));

                if (iSet.Started > DateTime.MinValue)
                {
                    TimeSpan delay = (iSet.Started - iSet.Received);

                    if ((int)delay.TotalMinutes > 0)
                        lines.Add("Delay To Start: " + delay.TotalMinutes.ToString("0.0000") + "m");
                    else
                        lines.Add("Delay To Start: " + delay.TotalSeconds.ToString("0.0000") + "s");
                }

                if (iSet.Completed > DateTime.MinValue)
                {
                    TimeSpan delay = (iSet.Completed - iSet.Started);

                    if ((int)delay.TotalMinutes > 0)
                        lines.Add("Total Execution Time: " + delay.TotalMinutes.ToString("0.0000") + "m");
                    else
                        lines.Add("Total Execution Time: " + delay.TotalSeconds.ToString("0.0000") + "s");
                }

                lines.Add("\r\n\r\n");

                foreach (STEM.Surge.Instruction i in iSet.Instructions)
                {
                    lines.Add(i.VersionDescriptor.TypeName);
                    _SummaryLines[lines.Count] = i.ID;
                    lines.Add("\tFailure Action - \t" + i.FailureAction.ToString());
                    _SummaryLines[lines.Count] = i.ID;
                    lines.Add("\tLast Stage - \t" + i.Stage.ToString());
                    _SummaryLines[lines.Count] = i.ID;
                    lines.Add("\tStart Time - \t" + i.Start.ToString("yyyy/MM/dd HH:mm:ss.fffffff"));
                    _SummaryLines[lines.Count] = i.ID;
                    lines.Add("\tEnd Time - \t" + i.Finish.ToString("yyyy/MM/dd HH:mm:ss.fffffff"));
                    _SummaryLines[lines.Count] = i.ID;

                    if (i.Finish > DateTime.MinValue)
                    {
                        TimeSpan delay = (i.Finish - i.Start);

                        if ((int)delay.TotalMinutes > 0)
                            lines.Add("\tExecution Time: " + delay.TotalMinutes.ToString("0.0000") + "m");
                        else
                            lines.Add("\tExecution Time: " + delay.TotalSeconds.ToString("0.0000") + "s");

                        _SummaryLines[lines.Count] = i.ID;
                    }

                    if (i.Exceptions.Count > 0)
                    {
                        lines.Add("");
                        _SummaryLines[lines.Count] = i.ID;
                        lines.Add("\tInstruction Exceptions (" + i.Exceptions.Count + ") :");
                        _SummaryLines[lines.Count] = i.ID;

                        foreach (Exception ex in i.Exceptions)
                        {
                            lines.Add("");
                            _SummaryLines[lines.Count] = i.ID;

                            foreach (string el in ex.Message.Split(new char[] { '\r', '\n' }))
                            {
                                lines.Add("\t" + el);
                                _SummaryLines[lines.Count] = i.ID;
                            }
                        }
                    }

                    lines.Add("");
                }
                
                summaryText.Lines = lines.ToArray();
                summaryText.SelectionChanged += SummaryText_SelectionChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "There was an error reading the InstructionSet.");
                throw ex;
            }
        }

        Dictionary<int, Guid> _SummaryLines = new Dictionary<int, Guid>();

        private void SummaryText_SelectionChanged(object sender, EventArgs e)
        {
            int ci = summaryText.GetFirstCharIndexOfCurrentLine();
            int ln = summaryText.GetLineFromCharIndex(ci) - 1;

            if (_SummaryLines.ContainsKey(ln))
            {
                string id = _SummaryLines[ln].ToString();

                int a = xmlText.Text.IndexOf(id);
                if (a >= 0)
                    ln = xmlText.GetLineFromCharIndex(a) - 5;
                else
                    return;

                xmlText.SelectionStart = xmlText.GetFirstCharIndexFromLine(ln);
                xmlText.SelectionLength = 0;
                xmlText.ScrollToCaret();
            }
        }
    }
}
