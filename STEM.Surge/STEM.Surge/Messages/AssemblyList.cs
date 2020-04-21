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
using STEM.Sys.Messaging;

namespace STEM.Surge.Messages
{
    public class AssemblyList : STEM.Sys.Messaging.FileList
    {
        public bool IsWindows { get; set; }
        public bool IsX64 { get; set; }

        public AssemblyList()
        {
            IsWindows = STEM.Sys.Control.IsWindows;
            IsX64 = STEM.Sys.Control.IsX64;            
        }

        public AssemblyList(string directory, bool recurse)
        {
            IsWindows = STEM.Sys.Control.IsWindows;
            IsX64 = STEM.Sys.Control.IsX64;

            base.Path = directory;

            directory = System.IO.Path.GetFullPath(directory);

            foreach (string s in STEM.Sys.IO.Directory.STEM_GetFiles(directory, "*.dll|*.so|*.a|*.lib", "!.Archive|!TEMP", recurse ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly, false))
            {
                if (!Descriptions.Exists(i => i.Filename == s.Substring(directory.Length).Trim(System.IO.Path.DirectorySeparatorChar)))
                    try
                    {
                        Descriptions.Add(new Sys.IO.FileDescription(directory, s.Substring(directory.Length).Trim(System.IO.Path.DirectorySeparatorChar), false));
                    }
                    catch { }
            }
        }
    }
}
