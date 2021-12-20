using System;
using System.Collections.Generic;
using System.Text;

namespace STEM.Sys.IO.Listing
{
    public class FileInfo
    {
        public DateTime LastWriteTimeUtc { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public DateTime LastAccessTimeUtc { get; set; }
        public long Size { get; set; }

        public FileInfo()
        {
            LastWriteTimeUtc = DateTime.MinValue;
            CreationTimeUtc = DateTime.MinValue;
            LastAccessTimeUtc = DateTime.MinValue;
            Size = -1;
        }
    }
}
