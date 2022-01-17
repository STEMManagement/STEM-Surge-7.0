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
            DateTime ret = DateTime.MinValue;

            foreach (string t in STEM.Sys.IO.Path.OrderPathsWithSubnet(filename, STEM.Sys.IO.Net.MachineIP()))
                try
                {
                    if (ret < _LastWriteTime[t])
                        ret = _LastWriteTime[t];
                }
                catch { }

            return ret;
        }

        public static string GetFileText(string filename)
        {
            foreach (string t in STEM.Sys.IO.Path.OrderPathsWithSubnet(filename, STEM.Sys.IO.Net.MachineIP()))
                try
                {
                    string content = null;

                    while (true)
                        try
                        {
                            if (_FileContent.ContainsKey(t))
                            {
                                content = _FileContent[t];
                            }

                            break;
                        }
                        catch { }

                    if (content != null)
                    {
                        if (_LastWriteTimeCheck.ContainsKey(t))
                        {
                            if ((DateTime.UtcNow - _LastWriteTimeCheck[t]).TotalSeconds > 30)
                            {
                                lock (_LastWriteTime)
                                    if ((DateTime.UtcNow - _LastWriteTimeCheck[t]).TotalSeconds > 30)
                                    {
                                        DateTime lwt = System.IO.File.GetLastWriteTimeUtc(t);

                                        if (_LastWriteTime[t] != lwt)
                                        {
                                            try
                                            {
                                                content = System.IO.File.ReadAllText(t);

                                                _FileContent[t] = content;
                                                _LastWriteTime[t] = lwt;
                                                _LastWriteTimeCheck[t] = DateTime.UtcNow;
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                            }
                        }

                        return content;
                    }
                    else
                    {
                        lock (_LastWriteTime)
                            if (System.IO.File.Exists(t))
                            {
                                DateTime lwt = System.IO.File.GetLastWriteTimeUtc(t);

                                try
                                {
                                    content = System.IO.File.ReadAllText(t);

                                    _FileContent[t] = content;
                                    _LastWriteTime[t] = lwt;
                                    _LastWriteTimeCheck[t] = DateTime.UtcNow;
                                }
                                catch
                                {
                                }

                                return content;
                            }
                    }
                }
                catch { }

            return null;
        }
    }
}
