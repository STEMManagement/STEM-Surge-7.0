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
using System.ComponentModel;
using System.Collections.Generic;

namespace STEM.Surge.BasicControllers
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("FileEnumeratingWithSizeLimits")]
    [Description("Customize an InstructionSet Template using placeholders related to the file properties for each file discovered. (e.g. [TargetPath], [TargetName], [LastWriteTimeUtc]...) " +
           "Note that with this controller [DestinationPath] can be an expandable path where the controller will choose the best path based on network proximity and destination directory existence (if 'Check for directory existence' is enabled)." +
           "This controller differs from the BasicFileController in that partial file listings are obtained in the background and returned from FileListPreprocess possibly incomplete. This " +
           "Controller can be used when the file source is on a slow network connection or contains millions of files. " +
        "Files returned from FileListPreprocess will be limited in size between 'Lower FileSize Limit' and 'Upper FileSize Limit'.")]
    public class FileEnumeratingWithSizeLimits : FileEnumeratingBasicFileController
    {        
        public long LowerFileSizeLimit { get; set; }
        public long UpperFileSizeLimit { get; set; }

        public FileEnumeratingWithSizeLimits()
            : base()
        {
            LowerFileSizeLimit = 0;
            UpperFileSizeLimit = long.MaxValue;
        }

        public override List<string> ListPreprocess(IReadOnlyList<string> list)
        {
            return ListPreprocess(PollerSourceString, PollerDirectoryFilter, PollerFileFilter, UpperFileSizeLimit, LowerFileSizeLimit);
        }
    }
}
