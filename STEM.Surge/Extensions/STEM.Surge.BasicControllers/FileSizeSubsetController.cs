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

namespace STEM.Surge.BasicControllers
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("FileSizeSubsetController")]
    [Description("Customize an InstructionSet Template using placeholders related to the file properties for each file discovered. (e.g. [TargetPath], [TargetName], [LastWriteTimeUtc]...) " +
        "Note that with this controller [DestinationPath] can be an expandable path where the controller will choose the best path based on network proximity and destination directory existence (if 'Check for directory existence' is enabled). " +
        "This controller differs from the BasicFileController in that files from this controller are addressed in file size order ascending and partial listings can be returned from FileListPreprocess. " +
        "This controller seeks to limit the maximum file load (in bytes) assigned from this source on a per Branch basis.")]
    public class FileSizeSubsetController : BasicFileController
    {
        public class SizeRange
        {
            public SizeRange()
            {
                MaxLoadPerBranch = 1;
                MinMB = 0;
                MaxMB = 100000;
            }

            public int MaxLoadPerBranch { get; set; }
            public int MinMB { get; set; }
            public int MaxMB { get; set; }

            public SizeRange Clone()
            {
                SizeRange r = new SizeRange();
                r.MaxLoadPerBranch = MaxLoadPerBranch;
                r.MinMB = MinMB;
                r.MaxMB = MaxMB;

                return r;
            }

            internal class Load
            {
                public Load(string ip)
                {
                    IP = ip;
                    Loaded = new List<string>();
                    LastAssignment = DateTime.MinValue;
                }

                public string IP { get; set; }
                public DateTime LastAssignment { get; set; }
                public List<string> Loaded { get; set; }
            }

            internal List<Load> ActiveLoads = new List<Load>();
        }

        public List<SizeRange> MaxLoadByFileSize { get; set; }

        static Dictionary<string, List<SizeRange>> _Ranges = new Dictionary<string, List<SizeRange>>();
        static Dictionary<string, Dictionary<string, long>> _KnownSizes = new Dictionary<string, Dictionary<string, long>>();

        public FileSizeSubsetController()
        {
            MaxLoadByFileSize = new List<SizeRange>();
        }
        
        public override List<string> ListPreprocess(IReadOnlyList<string> list)
        {
            List<string> newList = base.ListPreprocess(list);

            string key = PollerSourceString + ":" + PollerDirectoryFilter + ":" + PollerFileFilter + ":" + PollerRecurseSetting;
            key = key.ToUpper();

            Dictionary<string, long> col;

            lock (_KnownSizes)
            {
                if (!_KnownSizes.ContainsKey(key))
                    _KnownSizes[key] = new Dictionary<string, long>();

                col = _KnownSizes[key];
            }

            lock (col)
            {
                int got = 0;
                foreach (string s in newList.Except(col.Keys))
                    try
                    {
                        col[s] = GetFileInfo(s).Size;
                        got++;
                        if (got > 1000)
                            break;
                    }
                    catch { }

                foreach (string s in col.Keys.ToList().Except(newList))
                    col.Remove(s);

                if (RandomizeList)
                {
                    Random rnd = new Random();
                    return col.OrderBy(i => rnd.Next()).Select(i => i.Key).ToList();
                }

                return col.OrderBy(i => i.Value).Select(i => i.Key).ToList();
            }
        }

        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            try
            {
                string key = PollerSourceString + ":" + PollerDirectoryFilter + ":" + PollerFileFilter + ":" + PollerRecurseSetting;
                key = key.ToUpper();

                Dictionary<string, long> col;

                lock (_KnownSizes)
                {
                    if (!_KnownSizes.ContainsKey(key))
                        _KnownSizes[key] = new Dictionary<string, long>();

                    col = _KnownSizes[key];
                }

                long sz = 0;
                lock (col)
                    sz = col[initiationSource];

                sz = sz / 1048576;
                
                if (!_Ranges.ContainsKey(key))
                    lock (_Ranges)
                        if (!_Ranges.ContainsKey(key))
                            _Ranges[key] = new List<SizeRange>();

                SizeRange range = null;
                lock (_Ranges[key])    
                    range = _Ranges[key].FirstOrDefault(i => i.MinMB <= sz && i.MaxMB >= sz);

                if (range == null)
                    return null;

                lock (range)
                {
                    SizeRange.Load load = range.ActiveLoads.Where(i => i.IP == recommendedBranchIP).FirstOrDefault();

                    if (load != null && load.Loaded.Count < range.MaxLoadPerBranch)
                    {
                        DeploymentDetails ret = base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, load.IP, limitedToBranches);

                        if (ret != null)
                        {
                            load.Loaded.Add(initiationSource);
                            load.LastAssignment = DateTime.UtcNow;
                        }

                        return ret;
                    }
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("FileSizeSubsetController.GenerateDeploymentDetails", new Exception(InstructionSetTemplate + ": " + initiationSource, ex).ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return null;
        }

        public override void ExecutionComplete(DeploymentDetails details, List<Exception> exceptions)
        {
            try
            {
                base.ExecutionComplete(details, exceptions);
            }
            catch { }

            string key = PollerSourceString + ":" + PollerDirectoryFilter + ":" + PollerFileFilter + ":" + PollerRecurseSetting;
            key = key.ToUpper();
            
            if (!_Ranges.ContainsKey(key))
                lock (_Ranges)
                    if (!_Ranges.ContainsKey(key))
                        _Ranges[key] = new List<SizeRange>();

            lock (_Ranges[key])
            {                
                SizeRange range = _Ranges[key].FirstOrDefault(i => i.ActiveLoads.Exists(x => x.Loaded.Contains(details.InitiationSource)));
                if (range != null)
                    lock (range)
                    {
                        SizeRange.Load rem = range.ActiveLoads.FirstOrDefault(z => z.Loaded.Contains(details.InitiationSource));
                        if (rem != null)
                            rem.Loaded.Remove(details.InitiationSource);
                    }
            }
        }

        public override void BranchStatusUpdate(string address, BranchState state)
        {
            try
            {
                base.BranchStatusUpdate(address, state);
            }
            catch { }

            string key = PollerSourceString + ":" + PollerDirectoryFilter + ":" + PollerFileFilter + ":" + PollerRecurseSetting;
            key = key.ToUpper();

            if (!_Ranges.ContainsKey(key))
                lock (_Ranges)
                    if (!_Ranges.ContainsKey(key))
                        _Ranges[key] = new List<SizeRange>();

            lock (_Ranges[key])
            {
                if (_Ranges[key].Count != MaxLoadByFileSize.Count)
                {
                    _Ranges[key].Clear();

                    foreach (SizeRange range in MaxLoadByFileSize)
                    {
                        _Ranges[key].Add(range.Clone());
                    }
                }

                foreach (SizeRange range in _Ranges[key])
                {
                    lock (range)
                    {
                        if (state == BranchState.Online && !range.ActiveLoads.Exists(i => i.IP == address))
                            range.ActiveLoads.Add(new SizeRange.Load(address));

                        if (state != BranchState.Online)
                        {
                            SizeRange.Load rem = range.ActiveLoads.FirstOrDefault(i => i.IP == address);

                            if (rem != null)
                                range.ActiveLoads.Remove(rem);
                        }
                    }
                }
            }
        }
    }
}
