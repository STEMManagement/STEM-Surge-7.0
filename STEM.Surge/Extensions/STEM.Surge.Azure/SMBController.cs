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
using Microsoft.Azure.Storage.Blob;

namespace STEM.Surge.Azure
{
    public class SMBControllerWithAzureAuth : STEM.Surge.SMB.SMBController
    {
        [Category("Azure")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public STEM.Surge.Azure.Authentication Authentication { get; set; }

        public SMBControllerWithAzureAuth() : base()
        {
            Authentication = new STEM.Surge.Azure.Authentication();
        }

        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            DeploymentDetails dd = base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);

            if (dd != null)
            {
                foreach (Instruction ins in dd.ISet.Instructions)
                {
                    foreach (PropertyInfo prop in ins.GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(STEM.Surge.Azure.IAuthentication))))
                    {
                        STEM.Surge.Azure.IAuthentication a = prop.GetValue(ins) as STEM.Surge.Azure.IAuthentication;

                        PropertyInfo i = a.GetType().GetProperties().FirstOrDefault(p => p.Name == "StorageConnectionString");
                        if (i != null)
                        {
                            string k = i.GetValue(a) as string;
                            if (String.IsNullOrEmpty(k))
                            {
                                i.SetValue(a, Authentication.StorageConnectionString);
                            }
                        }
                    }
                }
            }

            return dd;
        }
    }
}
