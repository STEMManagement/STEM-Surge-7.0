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
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace STEM.Surge.BasicControllers
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("DestinationPathElectionController")]
    [Description("Customize an InstructionSet Template using placeholders related to the file properties for each file discovered. (e.g. [TargetPath], [TargetName], [LastWriteTimeUtc]...) " +
        "Note that with this controller [DestinationPath] can be an expandable path where the controller will choose the best path based on network proximity, destination directory existence, and a configured 'Destination Load Limit'. " +
        "Files from this controller are addressed in alphabetical order. ")]
    public class DestinationPathElectionController : BasicFileController
    {        
        [DisplayName("Destination Load Limit"), DescriptionAttribute("How many files can exist in any destination before it should stop receiving files?")]
        public int DestinationLoadLimit { get; set; }
                      
        public DestinationPathElectionController()
        {
            DestinationLoadLimit = 0;
            AllowThreadedAssignment = false;
        }
        
        class MapEntry
        {
            public string FirstThree
            {
                get
                {
                    return STEM.Sys.IO.Path.GetFileNameWithoutExtension(STEM.Sys.IO.Path.IPFromPath(Path));
                }
            }

            public string FirstTwo
            {
                get
                {
                    return STEM.Sys.IO.Path.GetFileNameWithoutExtension(FirstThree);
                }
            }

            public int LastFileCount { get; set; }

            public string Path { get; set; }

            public DateTime LastAssignment = DateTime.MinValue;
            public DateTime LastRealPoll = DateTime.MinValue;
        }
        
        List<MapEntry> _MapEntries = new List<MapEntry>();

        protected string FindBestDestination(string initiationSource, string recommendedBranchIP)
        {
            return FindBestDestination(initiationSource, recommendedBranchIP, null);
        }

        protected string FindBestDestination(string initiationSource, string recommendedBranchIP, List<string> limitedDestinations)
        {
            try
            {
                string subDir = null;

                if (RecreateSubFromRootOf != null && RecreateSubFromRootOf.Trim().Length > 0)
                {
                    if (initiationSource.ToUpper().IndexOf(RecreateSubFromRootOf.ToUpper()) >= 0)
                    {
                        int startIndex = initiationSource.ToUpper().IndexOf(RecreateSubFromRootOf.ToUpper()) + RecreateSubFromRootOf.Length + 1;

                        if (startIndex < STEM.Sys.IO.Path.GetDirectoryName(initiationSource).Length)
                            subDir = initiationSource.Substring(startIndex, STEM.Sys.IO.Path.GetDirectoryName(initiationSource).Length - startIndex);
                    }
                }

                string fileFirst3 = STEM.Sys.IO.Path.GetFileNameWithoutExtension(STEM.Sys.IO.Path.IPFromPath(initiationSource));
                string branchFirst3 = STEM.Sys.IO.Path.GetFileNameWithoutExtension(recommendedBranchIP);
                string fileFirst2 = STEM.Sys.IO.Path.GetFileNameWithoutExtension(fileFirst3);
                string branchFirst2 = STEM.Sys.IO.Path.GetFileNameWithoutExtension(branchFirst3);

                string destinationPath = SelectDestination(branchFirst3, subDir, limitedDestinations);
                if (destinationPath == null && (branchFirst3 != fileFirst3))
                    destinationPath = SelectDestination(fileFirst3, subDir, limitedDestinations);
                if (destinationPath == null)
                    destinationPath = SelectDestination(branchFirst2, subDir, limitedDestinations);
                if (destinationPath == null && (branchFirst2 != fileFirst2))
                    destinationPath = SelectDestination(fileFirst2, subDir, limitedDestinations);
                if (destinationPath == null)
                    destinationPath = SelectDestination(null, subDir, limitedDestinations);

                return destinationPath;
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("DestinationPathElectionController.FindBestDestination", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return null;
        }
        
        string _DestinationPath = null;
        protected string SelectDestination(string ipKey, string subDir, List<string> limitedDestinations)
        {
            try
            {
                if (DestinationLoadLimit == Int32.MaxValue)
                    DestinationLoadLimit = 0;

                List<string> paths = STEM.Sys.IO.Path.ExpandRangedPath(_DestinationPath);
                foreach (string path in paths)
                {
                    if (_MapEntries.FirstOrDefault(i => i.Path == path) == null)
                        lock (_MapEntries)
                            if (_MapEntries.FirstOrDefault(i => i.Path == path) == null)
                                _MapEntries.Add(new MapEntry { Path = path, LastFileCount = 0, LastAssignment = DateTime.MinValue });
                }

                List<MapEntry> delete = new List<MapEntry>();
                foreach (MapEntry me in _MapEntries)
                    if (!paths.Contains(me.Path))
                        delete.Add(me);

                foreach (MapEntry me in delete)
                    _MapEntries.Remove(me);

                List<MapEntry> useThese = new List<MapEntry>();
                if (limitedDestinations != null && limitedDestinations.Count > 0)
                {
                    List<string> allLimits = new List<string>();
                    limitedDestinations.ForEach(i => allLimits.AddRange(STEM.Sys.IO.Path.ExpandRangedPath(i)));

                    _MapEntries.ForEach(i =>
                        {
                            if (allLimits.Contains(i.Path))
                                useThese.Add(i);
                        });
                }
                else
                {
                    useThese.AddRange(_MapEntries);
                }

                if (ipKey != null)
                {
                    if (useThese.FirstOrDefault(i => i.FirstThree == ipKey) != null)
                    {
                        useThese = useThese.Where(i => i.FirstThree == ipKey).ToList();
                    }
                    else if (useThese.FirstOrDefault(i => i.FirstTwo == ipKey) != null)
                    {
                        useThese = useThese.Where(i => i.FirstTwo == ipKey).ToList();
                    }
                    else
                    {
                        return null;
                    }
                }

                foreach (MapEntry me in useThese.OrderBy(i => i.LastAssignment))
                {
                    try
                    {
                        if (!GoodDirectory(me.Path))
                            continue;

                        if (DestinationLoadLimit > 0 && (DateTime.UtcNow - me.LastRealPoll).TotalSeconds > 5)
                        {
                            string absDest = me.Path;

                            if (subDir != null)
                                absDest = Path.Combine(absDest, subDir);

                            if (!GoodDirectory(absDest))
                            {
                                if (!String.IsNullOrEmpty(DestinationPathUser) && !String.IsNullOrEmpty(DestinationPathPassword))
                                {
                                    STEM.Sys.Security.Impersonation impersonation = new STEM.Sys.Security.Impersonation();
                                    try
                                    {
                                        impersonation.Impersonate(DestinationPathUser, DestinationPathPassword, DestinationPathImpersonationIsLocal);
                                        CreateDirectory(absDest);
                                    }
                                    finally { impersonation.UnImpersonate(); }
                                }
                                else
                                {
                                    CreateDirectory(absDest);
                                }
                            }

                            if (!String.IsNullOrEmpty(DestinationPathUser) && !String.IsNullOrEmpty(DestinationPathPassword))
                            {
                                STEM.Sys.Security.Impersonation impersonation = new STEM.Sys.Security.Impersonation();
                                try
                                {
                                    impersonation.Impersonate(DestinationPathUser, DestinationPathPassword, DestinationPathImpersonationIsLocal);
                                    me.LastFileCount = STEM.Sys.IO.Directory.STEM_GetFiles(absDest, "*", "*", SearchOption.TopDirectoryOnly, false).Count;
                                }
                                finally { impersonation.UnImpersonate(); }
                            }
                            else
                            {
                                me.LastFileCount = STEM.Sys.IO.Directory.STEM_GetFiles(absDest, "*", "*", SearchOption.TopDirectoryOnly, false).Count;
                            }

                            me.LastRealPoll = DateTime.UtcNow;
                        }

                        if (DestinationLoadLimit > 0)
                        {
                            if (DestinationLoadLimit < me.LastFileCount)
                                continue;

                            me.LastFileCount++;
                        }
                    
                        me.LastAssignment = DateTime.UtcNow;
                        return me.Path;
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("DestinationPathElectionController.SelectDestination", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return null;
        }

        Random _Random = new Random();
        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            string orig = "";
            string dp = null;

            try
            {
                dp = TemplateKVP.Keys.ToList().FirstOrDefault(i => i.Equals("[DestinationPath]", StringComparison.InvariantCultureIgnoreCase));

                if (_DestinationPath == null)
                {
                    if (dp == null)
                        throw new Exception("No macro [DestinationPath] exists in this DeploymentController.");

                    orig = TemplateKVP["[DestinationPath]"] = TemplateKVP[dp];

                    _DestinationPath = TemplateKVP["[DestinationPath]"];

                    _DestinationPath = ApplyKVP(_DestinationPath, TemplateKVP, true);
                }

                string destinationPath = FindBestDestination(initiationSource, recommendedBranchIP);

                if (destinationPath == null)
                    return null;

                TemplateKVP["[DestinationPath]"] = TemplateKVP[dp] = destinationPath;

                return base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("DestinationPathElectionController.GenerateDeploymentDetails", new Exception(InstructionSetTemplate + ": " + initiationSource, ex).ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }
            finally
            {
                if (orig != "")
                {
                    TemplateKVP["[DestinationPath]"] = TemplateKVP[dp] = orig;
                }

                _DestinationPath = null;
            }

            return null;
        }
    }
}
