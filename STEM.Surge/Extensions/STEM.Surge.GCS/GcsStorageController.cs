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
using System.IO;
using System.Reflection;
using System.Linq;
using Google.Cloud.Storage.V1;

namespace STEM.Surge.GCS
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("GCS Storage Controller")]
    [Description("Derived from Basic File Controller... Poll a container in the GCS cloud. The Switchboard configuration for Source should look like: " +
        "'\\\\ContainerName\\folderX' where the containerName can be expandable with '#' " +
        "and 'folderX' translates to the container directory name. The 'File Filter', 'Directory Filter', and 'Recurse' " +
        "settings in the Switchboard are also applied to the poll. " +
        "This controller provides '[ContainerName]' for template use.")]
    public class GcsStorageController : STEM.Surge.BasicControllers.BasicFileController
    {
        public GcsStorageController()
        {
            TemplateKVP["[ContainerName]"] = "Reserved";
        }

        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            try
            {
                TemplateKVP["[ContainerName]"] = STEM.Sys.IO.Path.FirstTokenOfPath(initiationSource).ToLower();

                return base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);
            }
            finally
            {
                TemplateKVP["[ContainerName]"] = "Reserved";
            }
        }
    }
}
