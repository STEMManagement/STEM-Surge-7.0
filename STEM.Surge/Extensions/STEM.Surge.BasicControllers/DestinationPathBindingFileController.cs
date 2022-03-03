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
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using STEM.Sys.State;

namespace STEM.Surge.BasicControllers
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("DestinationPathBindingFileController")]
    [Description("Customize an InstructionSet Template using placeholders related to the file properties for each file discovered. (e.g. [TargetPath], [TargetName], [LastWriteTimeUtc]...) " +
           "Note that with this controller [DestinationPath] can be an expandable path where the controller will bind the source to the best path based on network proximity and destination directory existence (if 'Check for directory existence' is enabled). " +
           "Files from this controller are addressed in alphabetical order. " +
           "This controller seeks to issue instruction sets based on the source path of each file being bound to a consistent destination directory.")]
    public class DestinationPathBindingFileController : SwitchboardRowBasicFileController
    {
        public DestinationPathBindingFileController()
        {
            AllowThreadedAssignment = false;
        }

        Dictionary<string, string> _DestinationMap = new Dictionary<string, string>();

        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            string dp = TemplateKVP.Keys.ToList().FirstOrDefault(i => i.Equals("[DestinationPath]", StringComparison.InvariantCultureIgnoreCase));
            if (dp == null)
                throw new Exception("No macro [DestinationPath] exists in this DeploymentController.");

            TemplateKVP["[DestinationPath]"] = TemplateKVP[dp];

            string origDest = TemplateKVP["[DestinationPath]"];

            string path = System.IO.Path.GetDirectoryName(initiationSource).ToUpper();

            if (string.IsNullOrEmpty(path))
                return base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);

            lock (_DestinationMap)
            {
                try
                {
                    string dest = null;
                    if (_DestinationMap.ContainsKey(path))
                        dest = _DestinationMap[path];

                    if (!string.IsNullOrEmpty(dest))
                        if (CheckDirectoryExists)
                            if (!DirectoryExists(dest))
                                dest = null;

                    if (string.IsNullOrEmpty(dest))
                    {
                        DeploymentDetails ret = base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);
                        _DestinationMap[path] = LastDestinationSelected;
                        return ret;
                    }

                    TemplateKVP[dp] = TemplateKVP["[DestinationPath]"] = dest;

                    return base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);
                }
                catch
                {
                    _DestinationMap.Remove(path);

                    throw;
                }
                finally
                {
                    TemplateKVP[dp] = TemplateKVP["[DestinationPath]"] = origDest;
                }
            }
        }
    }
}
