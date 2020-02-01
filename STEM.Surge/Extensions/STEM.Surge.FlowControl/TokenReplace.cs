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
using STEM.Surge;

namespace STEM.Surge.FlowControl
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Token Replace")]
    [Description("Replace Tokens in follow-on Instructions.")]
    public class TokenReplace : Instruction
    {
        public enum ReplaceIn { Next, All }

        [Category("Flow")]
        [DisplayName("Replace in next or all follow-on Instruction(s)")]
        [Description("Replace in next or all follow-on Instruction(s)")]
        public ReplaceIn ReplaceInInstructions { get; set; }

        [Category("Flow")]
        [DisplayName("Windows Replacement Dictionary")]
        [DescriptionAttribute("Map of X -> Y replacement to perofrm if Operating System is Windows.")]
        public STEM.Sys.Serialization.Dictionary<string, string> WindowsReplacementMap { get; set; }

        [Category("Flow")]
        [DisplayName("Linux Replacement Dictionary")]
        [DescriptionAttribute("Map of X -> Y replacement to perofrm if Operating System is Linux.")]
        public STEM.Sys.Serialization.Dictionary<string, string> LinuxReplacementMap { get; set; }

        public TokenReplace() : base()
        {
            ReplaceInInstructions = ReplaceIn.All;
            WindowsReplacementMap = new Sys.Serialization.Dictionary<string, string>();
            LinuxReplacementMap = new Sys.Serialization.Dictionary<string, string>();
        }

        protected override bool _Run()
        {
            try
            {
                STEM.Sys.Serialization.Dictionary<string, string> map = WindowsReplacementMap;

                if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    map = LinuxReplacementMap;

                for (int x = OrdinalPosition + 1; x < InstructionSet.Instructions.Count; x++)
                {
                    string xml = InstructionSet.Instructions[x].Serialize();

                    foreach (string s in map.Keys)
                        xml = xml.Replace(s, map[s]);

                    InstructionSet.Instructions[x] = Instruction.Deserialize(xml) as Instruction;

                    if (ReplaceInInstructions == ReplaceIn.Next)
                        break;
                }
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.Message);
                Exceptions.Add(ex);
            }

            return Exceptions.Count == 0;
        }

        protected override void _Rollback()
        {
        }
    }
}
