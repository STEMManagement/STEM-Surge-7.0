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

namespace STEM.Sys.IO
{
    public enum FileExistsAction { Skip, Throw, Overwrite, MakeUnique }

    public static class File
    {
        public static bool FileInUse(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            filename = STEM.Sys.IO.Path.ChangeIpToMachineName(STEM.Sys.IO.Path.AdjustPath(filename));

            try
            {
                if (!System.IO.File.Exists(filename))
                    return false;

                //System.IO.File.SetAttributes(filename, System.IO.FileAttributes.Normal);

                using (new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None)) { }
                return false;
            }
            catch
            {
                return true;
            }
        }

        public static string UniqueFilename(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            filename = STEM.Sys.IO.Path.ChangeIpToMachineName(STEM.Sys.IO.Path.AdjustPath(filename));

            string unique = filename;

            if (filename.ToUpper(System.Globalization.CultureInfo.CurrentCulture).Contains("/DEV/NULL") ||
                filename.ToUpper(System.Globalization.CultureInfo.CurrentCulture).Contains("\\DEV\\NULL"))
                return unique;

            int cnt = 1;

            while (System.IO.File.Exists(unique))
            {
                unique = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}" + System.IO.Path.DirectorySeparatorChar + "{1}_{2}{3}",
                    STEM.Sys.IO.Path.GetDirectoryName(filename),
                    STEM.Sys.IO.Path.GetFileNameWithoutExtension(filename),
                    (cnt++).ToString("0000", System.Globalization.CultureInfo.CurrentCulture),
                    STEM.Sys.IO.Path.GetExtension(filename));
            }

            return unique;
        }

        public static void STEM_Delete(string file, bool expand, int retryCount = 0, int retryDelaySeconds = 3)
        {
            if (String.IsNullOrEmpty(file))
                throw new ArgumentNullException(nameof(file));

            List<string> files = new List<string>();
            if (expand)
                files = Path.ExpandRangedPath(file);
            else
                files.Add(file);

            foreach (string f in files)
            {
                string adjustedFilename = STEM.Sys.IO.Path.ChangeIpToMachineName(STEM.Sys.IO.Path.AdjustPath(f));

                int r = retryCount;

                while (r >= 0)
                {
                    try
                    {
                        if (System.IO.File.Exists(adjustedFilename))
                        {
                            System.IO.File.SetAttributes(adjustedFilename, System.IO.FileAttributes.Normal);
                            System.IO.File.Delete(adjustedFilename);
                        }

                        break;
                    }
                    catch
                    {
                    }

                    if (r > 0)
                        System.Threading.Thread.Sleep(retryDelaySeconds * 1000);

                    r--;
                }
            }
        }

        public static void STEM_Move(string source, string destination, FileExistsAction ifExists)
        {
            if (String.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            if (String.IsNullOrEmpty(destination))
                throw new ArgumentNullException(nameof(destination));

            string s = "";
            STEM_Move(source, destination, ifExists, out s);
        }

        public static void STEM_Copy(string source, string destination, FileExistsAction ifExists)
        {
            if (String.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            if (String.IsNullOrEmpty(destination))
                throw new ArgumentNullException(nameof(destination));

            string s = "";
            STEM_Copy(source, destination, ifExists, out s);
        }

        public static void STEM_Move(string source, string destination, FileExistsAction ifExists, out string destinationFilename, int retryCount = 0, int retryDelaySeconds = 3)
        {
            if (String.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            if (String.IsNullOrEmpty(destination))
                throw new ArgumentNullException(nameof(destination));

            STEM_Move(source, destination, ifExists, out destinationFilename, retryCount, retryDelaySeconds, true);
        }

        public static void STEM_Copy(string source, string destination, FileExistsAction ifExists, out string destinationFilename, int retryCount = 0, int retryDelaySeconds = 3)
        {
            if (String.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            if (String.IsNullOrEmpty(destination))
                throw new ArgumentNullException(nameof(destination));

            STEM_Copy(source, destination, ifExists, out destinationFilename, retryCount, retryDelaySeconds, true);
        }

        public static void STEM_Move(string source, string destination, FileExistsAction ifExists, out string destinationFilename, int retryCount, int retryDelaySeconds, bool useTempHop)
        {
            if (String.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            if (String.IsNullOrEmpty(destination))
                throw new ArgumentNullException(nameof(destination));

            source = STEM.Sys.IO.Path.ChangeIpToMachineName(STEM.Sys.IO.Path.AdjustPath(source));
            destination = STEM.Sys.IO.Path.ChangeIpToMachineName(STEM.Sys.IO.Path.AdjustPath(destination));

            if (source.Equals(destination, StringComparison.InvariantCultureIgnoreCase))
            {
                destinationFilename = destination;
                return;
            }

            destinationFilename = "";

            try
            {
                STEM_Copy(source, destination, ifExists, out destinationFilename, retryCount, retryDelaySeconds, useTempHop);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("File copy failed") && String.IsNullOrEmpty(destinationFilename))
                    throw new Exception("File move failed: " + source);

                throw new Exception("File.STEM_Move", ex);
            }

            if (!String.IsNullOrEmpty(destinationFilename) && !source.Equals(destinationFilename, StringComparison.InvariantCultureIgnoreCase))
                System.IO.File.Delete(source);
        }

        public static void STEM_Copy(string source, string destination, FileExistsAction ifExists, out string destinationFilename, int retryCount, int retryDelaySeconds, bool useTempHop)
        {
            if (String.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            if (String.IsNullOrEmpty(destination))
                throw new ArgumentNullException(nameof(destination));

            source = STEM.Sys.IO.Path.ChangeIpToMachineName(STEM.Sys.IO.Path.AdjustPath(source));
            destination = STEM.Sys.IO.Path.ChangeIpToMachineName(STEM.Sys.IO.Path.AdjustPath(destination));

            if (source.Equals(destination, StringComparison.InvariantCultureIgnoreCase))
            {
                destinationFilename = destination;
                return;
            }

            destinationFilename = "";

            while (retryCount >= 0)
            {
                if (!System.IO.File.Exists(source))
                    throw new System.IO.FileNotFoundException("Could not find file " + source);

                string d = destination;

                if (useTempHop)
                {
                    string dTemp = System.IO.Path.Combine(STEM.Sys.IO.Path.GetDirectoryName(d), "Temp");

                    if (!System.IO.Directory.Exists(dTemp))
                        System.IO.Directory.CreateDirectory(dTemp);
                }
                else
                {
                    if (!System.IO.Directory.Exists(STEM.Sys.IO.Path.GetDirectoryName(d)))
                        System.IO.Directory.CreateDirectory(STEM.Sys.IO.Path.GetDirectoryName(d));
                }

                try
                {                    
                    if (useTempHop)
                    {
                        string dTemp = System.IO.Path.Combine(STEM.Sys.IO.Path.GetDirectoryName(d), "Temp");

                        if (!System.IO.Directory.Exists(dTemp))
                            System.IO.Directory.CreateDirectory(dTemp);

                        try
                        {
                            if (System.IO.File.Exists(d))
                            {
                                switch (ifExists)
                                {
                                    case FileExistsAction.Throw:
                                        throw new System.IO.IOException("Destination file already exists: " + destination);

                                    case FileExistsAction.Skip:
                                        return;

                                    case FileExistsAction.Overwrite:

                                        System.IO.File.Copy(source, System.IO.Path.Combine(dTemp, STEM.Sys.IO.Path.GetFileName(source)), true);

                                        try
                                        {
                                            System.IO.File.SetAttributes(d, System.IO.FileAttributes.Normal);
                                            System.IO.File.Copy(System.IO.Path.Combine(dTemp, STEM.Sys.IO.Path.GetFileName(source)), d, true);
                                        }
                                        catch
                                        {
                                            throw new System.IO.IOException("Destination file could not be overwritten: " + d);
                                        }

                                        break;

                                    case FileExistsAction.MakeUnique:
                                        d = File.UniqueFilename(destination);

                                        System.IO.File.Copy(source, System.IO.Path.Combine(dTemp, STEM.Sys.IO.Path.GetFileName(source)), true);
                                        System.IO.File.Move(System.IO.Path.Combine(dTemp, STEM.Sys.IO.Path.GetFileName(source)), d);

                                        break;
                                }
                            }
                            else
                            {
                                System.IO.File.Copy(source, System.IO.Path.Combine(dTemp, STEM.Sys.IO.Path.GetFileName(source)), true);
                                System.IO.File.Move(System.IO.Path.Combine(dTemp, STEM.Sys.IO.Path.GetFileName(source)), d);
                            }

                            destinationFilename = d;
                        }
                        finally
                        {
                            try
                            {
                                if (System.IO.File.Exists(System.IO.Path.Combine(dTemp, STEM.Sys.IO.Path.GetFileName(source))))
                                    System.IO.File.Delete(System.IO.Path.Combine(dTemp, STEM.Sys.IO.Path.GetFileName(source)));
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        if (System.IO.File.Exists(d))
                        {
                            switch (ifExists)
                            {
                                case FileExistsAction.Throw:
                                    throw new System.IO.IOException("Destination file already exists: " + destination);

                                case FileExistsAction.Skip:
                                    return;

                                case FileExistsAction.Overwrite:

                                    System.IO.File.SetAttributes(d, System.IO.FileAttributes.Normal);
                                    System.IO.File.Copy(source, d, true);

                                    break;

                                case FileExistsAction.MakeUnique:

                                    d = File.UniqueFilename(destination);
                                    System.IO.File.Copy(source, d);

                                    break;
                            }
                        }
                        else
                        {
                            System.IO.File.Copy(source, d);
                        }

                        destinationFilename = d;
                    }

                    if (destinationFilename == d)
                        break;
                }
                catch
                {
                    if (retryCount == 0)
                        throw;
                }

                if (retryCount > 0)
                    System.Threading.Thread.Sleep(retryDelaySeconds * 1000);

                retryCount--;
            }

            if (String.IsNullOrEmpty(destinationFilename))
                throw new Exception("File copy failed: " + source);
        }
    }
}