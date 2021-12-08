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
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Azure Storage Controller")]
    [Description("Derived from Basic File Controller... Poll a container in the Azure cloud. The Switchboard configuration for Source should look like: " +
        "'\\\\ContainerName\\folderX' where the containerName can be expandable with '#' " +
        "and 'folderX' translates to the container directory name. The 'File Filter', 'Directory Filter', and 'Recurse' " +
        "settings in the Switchboard are also applied to the poll. " +
        "This controller provides '[ContainerName]' for template use.")]
    public class AzureStorageController : STEM.Surge.BasicControllers.BasicFileController
    {
        [Category("Azure")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }

        [Category("Azure")]
        [DisplayName("List Type"), DescriptionAttribute("Are you assigning files or folders?")]
        public AzureListType ListType { get; set; }

        public AzureStorageController()
        {
            Authentication = new Authentication();

            PreprocessPerformsDiscovery = true;

            ListType = AzureListType.File;

            TemplateKVP["[ContainerName]"] = "Reserved";
        }

        public override List<string> ListPreprocess(IReadOnlyList<string> list)
        {
            string container = "";
            List<string> returnList = new List<string>();
            try
            {
                container = Authentication.ContainerFromPath(PollerSourceString);
                string directory = Authentication.PrefixFromPath(PollerSourceString);
                
                List<IListBlobItem> azureList = Authentication.ListObjects(container, directory, ListType, PollerRecurseSetting, PollerDirectoryFilter, PollerFileFilter);

                returnList = azureList.Select(i => Authentication.ToString(i)).ToList();
                
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
            catch (AggregateException ex)
            {
                PollError = "";
                foreach (Exception e in ex.InnerExceptions)
                {
                    if (PollError != "")
                        PollError += "\r\n";

                    PollError += "(" + STEM.Sys.IO.Net.MachineIP() + ") encountered an error connecting to " + container + ": " + e.Message;
                }
            }
            catch (Exception ex)
            {
                PollError = "(" + STEM.Sys.IO.Net.MachineIP() + ") encountered an error connecting to " + container + ": " + ex.Message;
                STEM.Sys.EventLog.WriteEntry("AzureController.ListPreprocess", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            if (HonorPriorityFilters)
                returnList = ApplyPriorityFilterOrdering(returnList);

            return returnList;
        }

        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            try
            {
                TemplateKVP["[ContainerName]"] = STEM.Sys.IO.Path.FirstTokenOfPath(initiationSource).ToLower();

                DeploymentDetails dd = base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);

                if (dd != null)
                {
                    foreach (Instruction ins in dd.ISet.Instructions)
                    {
                        foreach (PropertyInfo prop in ins.GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(IAuthentication))))
                        {
                            IAuthentication a = prop.GetValue(ins) as IAuthentication;

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
            finally
            {
                TemplateKVP["[ContainerName]"] = "Reserved";
            }
        }

        public override bool FileExists(string file, string user, string password, bool isLocal)
        {
            return FileExists(file);
        }

        public override bool FileExists(string file)
        {
            return Authentication.FileExists(file);
        }

        public override DateTime GetAgeBasis(string initiationSource)
        {
            FDCFileInfo fi = GetFileInfo(initiationSource);

            if (fi != null)
                switch (SelectedOrigin)
                {
                    case STEM.Surge.AgeOrigin.LastWriteTime:
                        return fi.LastWriteTimeUtc;

                    case STEM.Surge.AgeOrigin.LastAccessTime:
                        return fi.LastAccessTimeUtc;

                    case STEM.Surge.AgeOrigin.CreationTime:
                        return fi.CreationTimeUtc;
                }

            FDCDirectoryInfo di = GetDirectoryInfo(initiationSource);

            if (di != null)
                switch (SelectedOrigin)
                {
                    case STEM.Surge.AgeOrigin.LastWriteTime:
                        return di.LastWriteTimeUtc;

                    case STEM.Surge.AgeOrigin.LastAccessTime:
                        return di.LastAccessTimeUtc;

                    case STEM.Surge.AgeOrigin.CreationTime:
                        return di.CreationTimeUtc;
                }

            return DateTime.MinValue;
        }

        public override FDCFileInfo GetFileInfo(string file, string user, string password, bool isLocal)
        {
            return GetFileInfo(file);
        }

        public override FDCFileInfo GetFileInfo(string file)
        {
            return Authentication.GetFileInfo(file);
        }

        public override bool DirectoryExists(string directory, string user, string password, bool isLocal)
        {
            return DirectoryExists(directory);
        }

        public override bool DirectoryExists(string directory)
        {
            return Authentication.DirectoryExists(directory);
        }

        public override FDCDirectoryInfo GetDirectoryInfo(string directory, string user, string password, bool isLocal)
        {
            return GetDirectoryInfo(directory);
        }

        public override FDCDirectoryInfo GetDirectoryInfo(string directory)
        {
            return Authentication.GetDirectoryInfo(directory);
        }

        public override void CreateDirectory(string directory, string user, string password, bool isLocal)
        {
            CreateDirectory(directory);
        }

        public override void CreateDirectory(string directory)
        {
            Authentication.CreateDirectory(directory);
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);
            Authentication.Dispose();
        }
    }
}
