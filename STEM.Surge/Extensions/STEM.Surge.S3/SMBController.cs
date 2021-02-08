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

namespace STEM.Surge.S3
{
    public class SMBControllerWithS3Auth : STEM.Surge.SMB.SMBController
    {
        [Category("S3")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public STEM.Surge.S3.Authentication Authentication { get; set; }

        public SMBControllerWithS3Auth() : base()
        {
            Authentication = new STEM.Surge.S3.Authentication();
        }

        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            DeploymentDetails dd = base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);

            if (dd != null)
            {
                foreach (Instruction ins in dd.ISet.Instructions)
                {
                    foreach (PropertyInfo prop in ins.GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(IAuthentication))))
                    {
                        IAuthentication a = prop.GetValue(ins) as IAuthentication;

                        if (a.VersionDescriptor.TypeName == "STEM.Surge.S3.Authentication")
                        {
                            PropertyInfo i = a.GetType().GetProperties().FirstOrDefault(p => p.Name == "AccessKey");
                            if (i != null)
                            {
                                string k = i.GetValue(a) as string;
                                if (String.IsNullOrEmpty(k))
                                {
                                    i.SetValue(a, Authentication.AccessKey);

                                    i = a.GetType().GetProperties().FirstOrDefault(p => p.Name == "SecretKey");
                                    if (i != null)
                                        i.SetValue(a, Authentication.SecretKey);

                                    i = a.GetType().GetProperties().FirstOrDefault(p => p.Name == "Region");
                                    if (i != null)
                                        i.SetValue(a, Authentication.Region);

                                    i = a.GetType().GetProperties().FirstOrDefault(p => p.Name == "ServiceURL");
                                    if (i != null)
                                        i.SetValue(a, Authentication.ServiceURL);
                                }
                            }
                        }
                    }
                }
            }

            return dd;
        }
    }
}
