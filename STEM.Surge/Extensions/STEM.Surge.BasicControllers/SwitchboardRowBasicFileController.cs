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
using System.Collections.Generic;
using System.ComponentModel;

namespace STEM.Surge.BasicControllers
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("SwitchboardRow Basic File Controller")]
    [Description("All files from a spannable Switchboard Row will be processed in a single listing by a single instance of this controller. " +
        "Customize an InstructionSet Template using placeholders related to the properties for each file or directory discovered. (e.g. [TargetPath], [TargetName], [LastWriteTimeUtc]...) " +
        "Note that with this controller [DestinationPath] can be an expandable path where the controller will choose the best path based on network proximity and destination directory existence (if 'Check for directory existence' is enabled). " +
        "Files from this controller are addressed in alphabetical order unless 'Randomize List' is set to true.")]
    public class SwitchboardRowBasicFileController : BasicFileController
    {
        static Dictionary<string, SwitchboardRowBasicFileController> _RootControllers = new Dictionary<string, SwitchboardRowBasicFileController>(StringComparer.InvariantCultureIgnoreCase);
        static Dictionary<string, List<SwitchboardRowBasicFileController>> _RegisteredControllers = new Dictionary<string, List<SwitchboardRowBasicFileController>>(StringComparer.InvariantCultureIgnoreCase);

        public SwitchboardRowBasicFileController()
        {
        }

        Dictionary<string, List<string>> _Listings = new Dictionary<string, List<string>>();
        internal void SetListing(IReadOnlyList<string> list, string deploymentManagerID)
        {
            lock (_Listings)
            {
                if (list == null)
                {
                    if (_Listings.ContainsKey(deploymentManagerID))
                        _Listings.Remove(deploymentManagerID);

                    return;
                }

                _Listings[deploymentManagerID] = list.ToList();
            }
        }

        internal List<string> GetListing()
        {
            List<string> ret = new List<string>();

            lock (_Listings)
            {
                foreach (string k in _Listings.Keys.ToList())
                {
                    ret.AddRange(_Listings[k]);
                    _Listings.Remove(k);
                }
            }

            ret.Sort();
            return ret;
        }

        protected virtual List<string> SwitchboardRowListPreprocess(IReadOnlyList<string> list)
        {
            return base.ListPreprocess(list);
        }

        public sealed override List<string> ListPreprocess(IReadOnlyList<string> list)
        {
            if (!_RegisteredControllers.ContainsKey(SwitchboardRowID))
                lock (_RegisteredControllers)
                    if (!_RegisteredControllers.ContainsKey(SwitchboardRowID))
                        _RegisteredControllers[SwitchboardRowID] = new List<SwitchboardRowBasicFileController>();

            lock (_RegisteredControllers[SwitchboardRowID])
                if (!_RegisteredControllers[SwitchboardRowID].Contains(this))
                    _RegisteredControllers[SwitchboardRowID].Add(this);

            if (!_RootControllers.ContainsKey(SwitchboardRowID))
                lock (_RootControllers)
                    if (!_RootControllers.ContainsKey(SwitchboardRowID))
                        _RootControllers[SwitchboardRowID] = this;

            _RootControllers[SwitchboardRowID].SetListing(list, DeploymentControllerID);

            if (_RootControllers[SwitchboardRowID] != this)
                return new List<string>();

            return SwitchboardRowListPreprocess(GetListing());
        }

        protected override void Dispose(bool dispose)
        {
            try
            {
                lock (_RegisteredControllers[SwitchboardRowID])
                {
                    if (_RegisteredControllers[SwitchboardRowID].Contains(this))
                        _RegisteredControllers[SwitchboardRowID].Remove(this);

                    lock (_RootControllers)
                        if (_RootControllers[SwitchboardRowID] == this)
                        {
                            SwitchboardRowBasicFileController i = _RegisteredControllers[SwitchboardRowID].FirstOrDefault();

                            if (i == null)
                                _RootControllers.Remove(SwitchboardRowID);
                            else
                                _RootControllers[SwitchboardRowID] = i;
                        }
                }
            }
            catch { }

            base.Dispose(dispose);
        }
    }
}
