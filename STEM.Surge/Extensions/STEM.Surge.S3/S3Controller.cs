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
using System.Reflection;
using System.Linq;
using STEM.Sys.IO.Listing;
using Amazon.S3;
using Amazon.S3.Model;

namespace STEM.Surge.S3
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("S3Controller")]
    [Description("Derived from Basic File Controller... Poll a bucket in the Amazon S3 cloud. The Switchboard configuration for Source should look like: " +
        "'\\\\BucketName\\folder1\\folder2' where the bucket can be expandable with '#' " +
        "and 'folderX' translates to the S3 prefix. The 'File Filter', 'Directory Filter', and 'Recurse' " +
        "settings in the Switchboard are also applied to the poll. " +
        "This controller provides '[BucketName]' and '[KeyPrefix]' for template use.")]
    public class S3Controller : STEM.Surge.BasicControllers.BasicFileController
    {        
        public S3Controller()
        {
            TemplateKVP["[BucketName]"] = "Reserved";
            TemplateKVP["[KeyPrefix]"] = "Reserved";
        }
        
        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            try
            {
                string prefix = STEM.Sys.IO.Path.GetDirectoryName(initiationSource);

                if (prefix == null)
                {
                    prefix = "";
                }
                else
                {
                    prefix = prefix.Trim(System.IO.Path.DirectorySeparatorChar);

                    prefix = prefix.Substring(STEM.Sys.IO.Path.FirstTokenOfPath(initiationSource).Length);

                    prefix = prefix.Trim(System.IO.Path.DirectorySeparatorChar);
                }

                TemplateKVP["[BucketName]"] = STEM.Sys.IO.Path.FirstTokenOfPath(initiationSource).ToLower();
                TemplateKVP["[KeyPrefix]"] = prefix;

                return base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);
            }
            finally
            {
                TemplateKVP["[BucketName]"] = "Reserved";
                TemplateKVP["[KeyPrefix]"] = "Reserved";
            }
        }
    }
}
