using System;
using System.Collections.Generic;
using System.Text;

namespace STEM.Sys.IO.Listing
{
    public class DirectoryInfo
    {
        public DateTime LastWriteTimeUtc { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public DateTime LastAccessTimeUtc { get; set; }

        public DirectoryInfo()
        {
            LastWriteTimeUtc = DateTime.MinValue;
            CreationTimeUtc = DateTime.MinValue;
            LastAccessTimeUtc = DateTime.MinValue;
        }
    }
}
