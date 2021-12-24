using System;
using System.Collections.Generic;
using System.Text;

namespace STEM.Sys.IO
{
    public class TextFileManager
    {
        static Dictionary<string, string> _FileContent = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        static Dictionary<string, DateTime> _LastWriteTime = new Dictionary<string, DateTime>(StringComparer.InvariantCultureIgnoreCase);
        static Dictionary<string, DateTime> _LastWriteTimeCheck = new Dictionary<string, DateTime>(StringComparer.InvariantCultureIgnoreCase);

        public static DateTime GetFileLastWriteTime(string filename)
        {
            lock (_FileContent)
            {
                DateTime ret = DateTime.MinValue;

                foreach (string t in STEM.Sys.IO.Path.OrderPathsWithSubnet(filename, STEM.Sys.IO.Net.MachineIP()))
                {
                    string txt = GetFileText(t);

                    if (txt != null)
                        if (ret < _LastWriteTime[t])
                            ret = _LastWriteTime[t];
                }

                return ret;
            }
        }
        public static string GetFileText(string filename)
        {
            lock (_FileContent)
                foreach (string t in STEM.Sys.IO.Path.OrderPathsWithSubnet(filename, STEM.Sys.IO.Net.MachineIP()))
                {
                    string content = null;
                    if (_FileContent.ContainsKey(t))
                    {
                        content = _FileContent[t];
                    }

                    if (!System.IO.File.Exists(t) && content == null)
                    {
                        STEM.Sys.EventLog.WriteEntry("STEM.Sys.IO.TextFileManager.GetFileText", "File does not exist: " + t, STEM.Sys.EventLog.EventLogEntryType.Error);
                        continue;
                    }
                    else if (System.IO.File.Exists(t))
                    {
                        if (_LastWriteTimeCheck.ContainsKey(t))
                        {
                            if (content == null || (DateTime.UtcNow - _LastWriteTimeCheck[t]).TotalSeconds > 15)
                            {
                                DateTime lwt = System.IO.File.GetLastWriteTimeUtc(t);

                                if (_LastWriteTime[t] != lwt || content == null)
                                {
                                    content = System.IO.File.ReadAllText(t);

                                    _FileContent[t] = content;
                                    _LastWriteTime[t] = lwt;
                                    _LastWriteTimeCheck[t] = DateTime.UtcNow;
                                }
                            }
                        }
                        else
                        {
                            DateTime lwt = System.IO.File.GetLastWriteTimeUtc(t);

                            content = System.IO.File.ReadAllText(t);

                            _FileContent[t] = content;
                            _LastWriteTime[t] = lwt;
                            _LastWriteTimeCheck[t] = DateTime.UtcNow;
                        }
                    }

                    if (content != null)
                        return content;
                }

            return null;
        }
    }
}
