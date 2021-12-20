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
using System.Linq;
using STEM.Sys.Threading;

namespace STEM.Sys.IO.Listing
{
    public abstract class IListingAgent
    {
        static STEM.Sys.Threading.ThreadPool _UtilPool = new STEM.Sys.Threading.ThreadPool(Int32.MaxValue, false);
        object _AccessMutex = new object();

        public IAuthentication Authentication { get; private set; }
        public ListingType ListingType { get; private set; }
        public string Path { get; private set; }
        public string FileFilter { get; private set; }
        public string SubpathFilter { get; private set; }
        public bool Recurse { get; private set; }

        public IListingAgent(IAuthentication authentication, ListingType listingType, string path, string fileFilter, string subpathFilter, bool recurse)
        {
            Authentication = authentication;
            ListingType = listingType;
            Path = path;
            FileFilter = fileFilter;
            SubpathFilter = subpathFilter;
            Recurse = recurse;
        }

        public ListResult GetListResult(ListingElements listingElements)
        {
            ListResult ret = new ListResult(this);

            string msg;

            ret.Entries = GetListingEntries(listingElements, out msg);

            ret.Status = Status;
            ret.ListingAgentMessage = msg;

            return ret;
        }

        protected abstract List<ListingEntry> GetListingEntries(ListingElements elements, out string message);

        ListingAgentStatus _Status = ListingAgentStatus.Waiting;
        protected ListingAgentStatus Status
        {
            get
            {
                return _Status;
            }

            set
            {
                _Status = value;

                lock (_AccessMutex)
                    if (_StatusChanged != null)
                        _UtilPool.RunOnce(new System.Threading.ThreadStart(StatusUpdate));
            }
        }

        void StatusUpdate()
        {
            List<Delegate> il = null;

            lock (_AccessMutex)
                if (_StatusChanged != null)
                    il = _StatusChanged.GetInvocationList().ToList();


            if (il != null)
            {
                foreach (var h in il)
                    try
                    {
                        h.DynamicInvoke(this, _Status);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            STEM.Sys.EventLog.WriteEntry("IListingAgent.StatusChanged", h.Target.GetType().FullName + " - " + ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                        }
                        catch
                        {
                            STEM.Sys.EventLog.WriteEntry("IListingAgent.StatusChanged", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                        }
                    }
            }
        }

        EventHandler<ListingAgentStatus> _StatusChanged;
        public event EventHandler<ListingAgentStatus> StatusChanged
        {
            add
            {
                if (value == null)
                    return;

                lock (_AccessMutex)
                    if (_StatusChanged == null || !_StatusChanged.GetInvocationList().ToList().Contains(value))
                        _StatusChanged += value;
            }

            remove
            {
                if (value == null)
                    return;

                lock (_AccessMutex)
                    if (_StatusChanged != null && _StatusChanged.GetInvocationList().ToList().Contains(value))
                        _StatusChanged -= value;
            }
        }
    }
}
