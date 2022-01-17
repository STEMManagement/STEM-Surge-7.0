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
using FluentFTP;
using STEM.Listing.FTP;

namespace STEM.Surge.FTP
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("FTPController")]
    [Description("Derived from Basic File Controller... Poll a source on an FTP server. The Switchboard configuration for Source should look like: " +
        "'\\\\10.0.0.80\\FtpRoot\\FtpSubdir' where the address can be expandable and identifies the FTP server(s) to poll, " +
        "'FtpRoot' is the name of the root folder, and 'FtpSubdir(s)' can be specified. The 'File Filter', 'Directory Filter', and 'Recurse' " +
        "settings in the Switchboard are also applied to the poll, and 'Pingable Source' is honored such that a successful ping would " +
        "be required before a connect. This controller provides '[FtpServerAddress]' and '[FtpServerPort]' for template use.")]
    public class FTPController : STEM.Surge.BasicControllers.BasicFileController
    {        
        public FTPController()
        {
            TemplateKVP["[FtpServerAddress]"] = "Reserved";
            TemplateKVP["[FtpServerPort]"] = "Reserved";
        }

        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            try
            {
                TemplateKVP["[FtpServerAddress]"] = STEM.Sys.IO.Path.IPFromPath(initiationSource);
                TemplateKVP["[FtpServerPort]"] = ((Authentication)SourceAuthentication()).Port;

                return base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);
            }
            finally
            {
                TemplateKVP["[FtpServerAddress]"] = "Reserved";
                TemplateKVP["[FtpServerPort]"] = "Reserved";
            }
        }
    }
}
