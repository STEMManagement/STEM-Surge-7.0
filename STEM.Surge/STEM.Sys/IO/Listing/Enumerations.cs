using System;
using System.Collections.Generic;
using System.Text;

namespace STEM.Sys.IO.Listing
{
    public enum ItemType { Directory, File }

    public enum ListingType { Directory, File, All }

    public enum ListingAgentStatus { Waiting, Listing, Error }

    [Flags]
    public enum ListingElements
    {
        None = 0,
        CreationTimeUtc = 1 << 1,
        LastWriteTimeUtc = 1 << 2,
        LastAccessTimeUtc = 1 << 3,
        Size = 1 << 4,
        AllTimes = CreationTimeUtc | LastWriteTimeUtc | LastAccessTimeUtc,
        All = CreationTimeUtc | LastWriteTimeUtc | LastAccessTimeUtc | Size
    }
}
