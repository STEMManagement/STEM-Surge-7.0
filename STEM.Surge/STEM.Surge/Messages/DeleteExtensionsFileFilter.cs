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
using STEM.Sys.Messaging;

namespace STEM.Surge.Messages
{
    /// <summary>
    /// Message sent from a SurgeActor to a DeploymentManager requesting an Extension FileFilter Deletion
    /// </summary>
    public class DeleteExtensionsFileFilter : STEM.Sys.Messaging.Message
    {
        public string FileFilter { get; set; }

        public DeleteExtensionsFileFilter()
        {
        }

        public DeleteExtensionsFileFilter(string fileFilter)
        {
            FileFilter = fileFilter;
        }
    }
}
