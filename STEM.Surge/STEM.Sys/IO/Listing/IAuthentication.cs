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
using System.ComponentModel;
using System.Xml.Serialization;

namespace STEM.Sys.IO.Listing
{
    /// <summary>
    /// This is used to identify classes wherein some sort of authentication is being configured so that the UI can present bulk reconfiguration options to users.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [XmlType(TypeName = "STEM.Sys.IO.Listing.IAuthentication")]
    public abstract class IAuthentication : STEM.Sys.Security.IAuthentication
    {
        public IAuthentication()
        {
        }

        public abstract IListingAgent ConstructListingAgent(ListingType listingType, string path, string fileFilter, string subpathFilter, bool recurse);

        public abstract bool FileExists(string file);

        public abstract STEM.Sys.IO.Listing.FileInfo GetFileInfo(string file);

        public abstract bool DirectoryExists(string directory);

        public abstract STEM.Sys.IO.Listing.DirectoryInfo GetDirectoryInfo(string directory);

        public abstract void CreateDirectory(string directory);

        public abstract void DeleteDirectory(string directory, bool recurse, bool deleteFiles);

        public abstract void DeleteFile(string file);

        public virtual string UniqueFilename(string file)
        {
            string unique = file;

            int cnt = 1;

            while (FileExists(unique))
            {
                unique = string.Format("{0}" + System.IO.Path.DirectorySeparatorChar + "{1}_{2}{3}",
                    STEM.Sys.IO.Path.GetDirectoryName(file),
                    STEM.Sys.IO.Path.GetFileNameWithoutExtension(file),
                    (cnt++).ToString("0000"),
                    STEM.Sys.IO.Path.GetExtension(file));
            }

            return unique;
        }
    }
}
