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
using System.Linq;
using STEM.Surge;
using STEM.Sys.IO;

namespace STEM.Surge.Messages
{
    /// <summary>
    /// Sent as a request from a SurgeActor to a DeploymentManager for 'CurrentFile' to obtain historical FileDescriptions
    /// Sent as a response from a DeploymentManager to a SurgeActor populated with historical FileDescriptions
    /// </summary>
    public class FileHistory : STEM.Sys.Messaging.Message
    {
        public string CurrentFile { get; set; }

        public List<FileDescription> Descriptions { get; set; }

        public FileHistory()
        {
            Descriptions = new List<FileDescription>();
        }

        public FileHistory(string currentFile)
        {
            Descriptions = new List<FileDescription>();
            CurrentFile = currentFile;
        }

        internal void Populate()
        {
            if (CurrentFile.EndsWith(".DC", StringComparison.InvariantCultureIgnoreCase))
            {
            }
            else if (CurrentFile.EndsWith(".IS", StringComparison.InvariantCultureIgnoreCase))
            {
            }
        }
    }
}
