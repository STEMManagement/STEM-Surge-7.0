﻿/*
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
using System.IO;
using System.Reflection;
using System.Linq;

namespace STEM.Surge.SMB
{
    public enum SMBListType { File, Directory, All }

    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("SMBController")]
    [Description("Customize an InstructionSet Template using placeholders related to the properties for each file or directory discovered. (e.g. [TargetPath], [TargetName], [LastWriteTimeUtc]...) " +
        "Note that with this controller [DestinationPath] can be an expandable path where the controller will choose the best path based on network proximity and destination directory existence (if 'Check for directory existence' is enabled). " +
        "Files from this controller are addressed in alphabetical order unless 'Randomize List' is set to true.")]
    public class SMBController : STEM.Surge.BasicControllers.BasicFileController
    {
        [Category("SMB")]
        [DisplayName("List Type"), DescriptionAttribute("Are you assigning files or folders?")]
        public SMBListType ListType { get; set; }

        public SMBController()
        {
            ListType = SMBListType.File;
        }

        public override List<string> ListPreprocess(IReadOnlyList<string> list)
        {
            List<string> returnList = new List<string>();
            try
            {
                if (ListType == SMBListType.Directory || ListType == SMBListType.All)
                    returnList = list.Select(i => STEM.Sys.IO.Path.GetDirectoryName(i)).Distinct().ToList();

                if (ListType != SMBListType.Directory)
                    returnList.AddRange(list);

                if (RandomizeList)
                {
                    Random rnd = new Random();
                    returnList = returnList.OrderBy(i => rnd.Next()).ToList();
                }
                else
                {
                    returnList.Sort();
                }

                PollError = "";
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("SMBController.ListPreprocess", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return returnList;
        }
    }
}
