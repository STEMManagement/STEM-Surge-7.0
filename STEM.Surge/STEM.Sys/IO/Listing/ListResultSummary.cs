using System;
using System.Collections.Generic;
using System.Text;

namespace STEM.Sys.IO.Listing
{
    public class ListResultSummary
    {
        public string ListingAgentType { get; set; }

        public ListingAgentStatus Status { get; set; }

        public string ListingAgentMessage { get; set; }

        public ListingType ListingType { get; private set; }

        public string Path { get; set; }

        public string FileFilter { get; set; }

        public string SubpathFilter { get; set; }

        public bool Recurse { get; set; }

        public int Count { get; set; }

        public ListResultSummary()
        {
        }

        public ListResultSummary(ListResult result)
        {
            ListingAgentType = result.ListingAgentType;
            Status = result.Status;
            ListingAgentMessage = result.ListingAgentMessage;
            ListingType = result.ListingType;
            Path = result.Path;
            FileFilter = result.FileFilter;
            SubpathFilter = result.SubpathFilter;
            Recurse = result.Recurse;
            Count = result.Entries.Count;
        }
    }
}
