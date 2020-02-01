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
using System.Text.RegularExpressions;

namespace STEM.Sys.IO
{
    public static class Directory
    {
        public static List<string> STEM_GetFiles(string path, bool expand)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            return Get(path, "*", "*", System.IO.SearchOption.TopDirectoryOnly, RequestType.Files, expand);
        }

        public static List<string> STEM_GetFiles(string path, string fileSearchPattern, bool expand)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (String.IsNullOrEmpty(fileSearchPattern))
                throw new ArgumentNullException(nameof(fileSearchPattern));

            return Get(path, fileSearchPattern, "*", System.IO.SearchOption.TopDirectoryOnly, RequestType.Files, expand);
        }

        public static List<string> STEM_GetFiles(string path, string fileSearchPattern, string directorySearchPattern, System.IO.SearchOption searchOption, bool expand)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (String.IsNullOrEmpty(fileSearchPattern))
                throw new ArgumentNullException(nameof(fileSearchPattern));

            if (String.IsNullOrEmpty(directorySearchPattern))
                throw new ArgumentNullException(nameof(directorySearchPattern));

            return Get(path, fileSearchPattern, directorySearchPattern, searchOption, RequestType.Files, expand);
        }

        public static List<string> STEM_GetDirectories(string path, bool expand)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            return Get(path, "*", "*", System.IO.SearchOption.TopDirectoryOnly, RequestType.Directories, expand);
        }

        public static List<string> STEM_GetDirectories(string path, string directorySearchPattern, bool expand)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (String.IsNullOrEmpty(directorySearchPattern))
                throw new ArgumentNullException(nameof(directorySearchPattern));

            return Get(path, "*", directorySearchPattern, System.IO.SearchOption.TopDirectoryOnly, RequestType.Directories, expand);
        }

        public static List<string> STEM_GetDirectories(string path, string directorySearchPattern, System.IO.SearchOption searchOption, bool expand)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (String.IsNullOrEmpty(directorySearchPattern))
                throw new ArgumentNullException(nameof(directorySearchPattern));

            return Get(path, "*", directorySearchPattern, searchOption, RequestType.Directories, expand);
        }

        public static void STEM_Delete(string directory, bool expand)
        {
            if (String.IsNullOrEmpty(directory))
                throw new ArgumentNullException(nameof(directory));

            List<string> dirs = new List<string>();

            if (expand)
                dirs = STEM.Sys.IO.Path.ExpandRangedPath(directory);
            else
                dirs.Add(directory);

            foreach (string d in dirs)
            {
                string dir = STEM.Sys.IO.Path.AdjustPath(d);


                if (System.IO.Directory.Exists(dir))
                {
                    List<string> files = STEM_GetFiles(dir, "*", "*", System.IO.SearchOption.AllDirectories, false);

                    foreach (string f in files)
                        try
                        {
                            System.IO.File.Delete(f);
                        }
                        catch { }

                    System.IO.Directory.Delete(dir, true);
                }
            }
        }

        public static void STEM_Move(string sourceDirName, string destDirName, string fileSearchPattern, string directorySearchPattern, FileExistsAction ifExists, bool expandSource)
        {
            if (String.IsNullOrEmpty(sourceDirName))
                throw new ArgumentNullException(nameof(sourceDirName));

            if (String.IsNullOrEmpty(destDirName))
                throw new ArgumentNullException(nameof(destDirName));

            if (String.IsNullOrEmpty(fileSearchPattern))
                throw new ArgumentNullException(nameof(fileSearchPattern));

            if (String.IsNullOrEmpty(directorySearchPattern))
                throw new ArgumentNullException(nameof(directorySearchPattern));

            STEM_Move(sourceDirName, destDirName, fileSearchPattern, directorySearchPattern, ifExists, false, expandSource);
        }

        public static void STEM_Move(string sourceDirName, string destDirName, string fileSearchPattern, string directorySearchPattern, FileExistsAction ifExists, bool expandSource, bool moveFilesOnly)
        {
            if (String.IsNullOrEmpty(sourceDirName))
                throw new ArgumentNullException(nameof(sourceDirName));

            if (String.IsNullOrEmpty(destDirName))
                throw new ArgumentNullException(nameof(destDirName));

            if (String.IsNullOrEmpty(fileSearchPattern))
                throw new ArgumentNullException(nameof(fileSearchPattern));

            if (String.IsNullOrEmpty(directorySearchPattern))
                throw new ArgumentNullException(nameof(directorySearchPattern));

            sourceDirName = STEM.Sys.IO.Path.AdjustPath(sourceDirName);
            destDirName = STEM.Sys.IO.Path.AdjustPath(destDirName);

            List<string> files = STEM_GetFiles(sourceDirName, fileSearchPattern, directorySearchPattern, System.IO.SearchOption.AllDirectories, expandSource);

            foreach (string f in files)
            {
                File.STEM_Copy(f, System.IO.Path.Combine(destDirName, f.Substring(sourceDirName.TrimEnd('\\', '/').Length + 1)), ifExists);

                try
                {
                    System.IO.File.Delete(f);
                }
                catch { }
            }

            if (!moveFilesOnly)
                STEM_Delete(sourceDirName, expandSource);
        }

        public static void STEM_Copy(string sourceDirName, string destDirName, string fileSearchPattern, string directorySearchPattern, FileExistsAction ifExists, bool expandSource)
        {
            if (String.IsNullOrEmpty(sourceDirName))
                throw new ArgumentNullException(nameof(sourceDirName));

            if (String.IsNullOrEmpty(destDirName))
                throw new ArgumentNullException(nameof(destDirName));

            if (String.IsNullOrEmpty(fileSearchPattern))
                throw new ArgumentNullException(nameof(fileSearchPattern));

            if (String.IsNullOrEmpty(directorySearchPattern))
                throw new ArgumentNullException(nameof(directorySearchPattern));

            List<string> files = STEM_GetFiles(sourceDirName, fileSearchPattern, directorySearchPattern, System.IO.SearchOption.AllDirectories, expandSource);

            foreach (string f in files)
                File.STEM_Copy(f, System.IO.Path.Combine(destDirName, f.Substring(sourceDirName.TrimEnd('\\', '/').Length + 1)), ifExists);
        }

        enum RequestType { Files, Directories }
        static List<string> Get(string path, string fileSearchPattern, string directorySearchPattern, System.IO.SearchOption searchOption, RequestType getType, bool expand)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            
            if (String.IsNullOrEmpty(fileSearchPattern))
                throw new ArgumentNullException(nameof(fileSearchPattern));

            if (String.IsNullOrEmpty(directorySearchPattern))
                throw new ArgumentNullException(nameof(directorySearchPattern));

            path = STEM.Sys.IO.Path.AdjustPath(path);

            List<string> inclusiveDirectoryFilters = new List<string>();
            List<string> inclusiveFileFilters = new List<string>();
            Regex exclusiveFileFilterRegex = null;
            Regex exclusiveDirectoryFilterRegex = null;
            Regex inclusiveDirectoryFilterRegex = null;

            foreach (string pattern in directorySearchPattern.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                if (!pattern.Trim().StartsWith("<>", StringComparison.InvariantCultureIgnoreCase) && !pattern.Trim().StartsWith("!", StringComparison.InvariantCultureIgnoreCase))
                    inclusiveDirectoryFilters.Add(pattern.Trim());

            exclusiveDirectoryFilterRegex = Path.BuildExclusiveFilter(directorySearchPattern);

            if (fileSearchPattern != null)
                foreach (string pattern in fileSearchPattern.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    if (!pattern.Trim().StartsWith("<>", StringComparison.InvariantCultureIgnoreCase) && !pattern.Trim().StartsWith("!", StringComparison.InvariantCultureIgnoreCase))
                        inclusiveFileFilters.Add(pattern.Trim());

            if (inclusiveFileFilters.Count < 1)
                inclusiveFileFilters.Add("*");

            exclusiveFileFilterRegex = Path.BuildExclusiveFilter(fileSearchPattern);

            if (inclusiveDirectoryFilters.Count < 1)
                inclusiveDirectoryFilters.Add("*");

            inclusiveDirectoryFilterRegex = Path.BuildInclusiveFilter(String.Join("|", inclusiveDirectoryFilters));

            List<string> expanded = null;

            if (expand)
                expanded = new List<string>(STEM.Sys.IO.Path.ExpandRangedPath(path));
            else
                expanded = new List<string>(new string[] { path });

            List<string> dirs = new List<string>();                        
            List<string> result = new List<string>();

            if (getType == RequestType.Directories)
            {
                foreach (string d in expanded)
                    dirs.AddRange(Directories(STEM.Sys.IO.Path.ChangeIpToMachineName(d), searchOption, inclusiveDirectoryFilters, inclusiveDirectoryFilterRegex, exclusiveDirectoryFilterRegex, false));

                return dirs.OrderByDescending(i => i.Count(j => j == System.IO.Path.DirectorySeparatorChar)).Distinct().ToList();
            }
            else
            {
                if (inclusiveDirectoryFilters.Count == 1 && inclusiveDirectoryFilters[0] == "*" || searchOption == System.IO.SearchOption.TopDirectoryOnly)
                {
                    foreach (string d in expanded)
                        foreach (string fileFilter in inclusiveFileFilters)
                        {
                            try
                            {
                                result.AddRange(Path.WhereFilesNotMatch(new List<string>(System.IO.Directory.GetFiles(STEM.Sys.IO.Path.ChangeIpToMachineName(d), fileFilter)), exclusiveFileFilterRegex));
                            }
                            catch { }
                        }
                }

                if (searchOption == System.IO.SearchOption.AllDirectories)
                {
                    foreach (string d in expanded)
                        dirs.AddRange(Directories(STEM.Sys.IO.Path.ChangeIpToMachineName(d), searchOption, inclusiveDirectoryFilters, inclusiveDirectoryFilterRegex, exclusiveDirectoryFilterRegex, false));

                    foreach (string d in dirs)
                        foreach (string fileFilter in inclusiveFileFilters)
                        {
                            try
                            {
                                result.AddRange(Path.WhereFilesNotMatch(new List<string>(System.IO.Directory.GetFiles(STEM.Sys.IO.Path.ChangeIpToMachineName(d), fileFilter)), exclusiveFileFilterRegex));
                            }
                            catch { }
                        }
                }

                return result.OrderBy(i => STEM.Sys.IO.Path.GetFileName(i)).Distinct().ToList();
            }
        }

        static List<string> Directories(string path, System.IO.SearchOption searchOption, List<string> inclusiveDirectoryFilters, Regex inclusiveDirectoryFilterRegex, Regex exclusiveDirectoryFilters, bool adding)
        {
            List<string> ret = new List<string>();

            if (path.ToUpper(System.Globalization.CultureInfo.CurrentCulture).Contains("/DEV/NULL") ||
                path.ToUpper(System.Globalization.CultureInfo.CurrentCulture).Contains("\\DEV\\NULL"))
                return ret;

            if (!System.IO.Directory.Exists(path))
                return ret;

            path = STEM.Sys.IO.Path.AdjustPath(path);

            try
            {
                List<string> allDirs = new List<string>(System.IO.Directory.GetDirectories(path, "*"));

                foreach (string d in allDirs)
                {
                    if (exclusiveDirectoryFilters != null && exclusiveDirectoryFilters.IsMatch(STEM.Sys.IO.Path.GetFileName(d).ToUpper(System.Globalization.CultureInfo.CurrentCulture)))
                        continue;

                    if (adding)
                    {
                        ret.Add(d);

                        if (searchOption == System.IO.SearchOption.AllDirectories)
                            ret.AddRange(Directories(d, searchOption, inclusiveDirectoryFilters, inclusiveDirectoryFilterRegex, exclusiveDirectoryFilters, true));
                    }
                    else if (inclusiveDirectoryFilterRegex.IsMatch(STEM.Sys.IO.Path.GetFileName(d).ToUpper(System.Globalization.CultureInfo.CurrentCulture)))
                    {
                        ret.Add(d);

                        if (searchOption == System.IO.SearchOption.AllDirectories)
                            ret.AddRange(Directories(d, searchOption, inclusiveDirectoryFilters, inclusiveDirectoryFilterRegex, exclusiveDirectoryFilters, true));
                    }
                    else
                    {
                        if (searchOption == System.IO.SearchOption.AllDirectories)
                            ret.AddRange(Directories(d, searchOption, inclusiveDirectoryFilters, inclusiveDirectoryFilterRegex, exclusiveDirectoryFilters, false));
                    }
                }
            }
            catch { return new List<string>(); }

            return ret.Distinct().ToList();
        }
    }
}
