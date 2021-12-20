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
using System.Linq;
using System.Reflection;
using STEM.Sys.Security;
using STEM.Sys.IO.Listing;

namespace STEM.Listing.SMB
{
    public class Authentication : STEM.Sys.IO.Listing.IAuthentication
    {
        [Category("SMB")]
        [DisplayName("Impersonation User"), DescriptionAttribute("Domain\\Username?")]
        public string ImpersonationUser { get; set; }

        [Category("SMB")]
        [DisplayName("Impersonation Password"), DescriptionAttribute("Password?")]
        [XmlIgnore]
        [PasswordPropertyText(true)]
        public string ImpersonationPassword { get; set; }
        [Browsable(false)]
        public string ImpersonationPasswordEncoded
        {
            get
            {
                return this.Entangle(ImpersonationPassword);
            }

            set
            {
                ImpersonationPassword = this.Detangle(value);
            }
        }


        [Category("SMB")]
        [DisplayName("Local User Impersonation"), DescriptionAttribute("Is the user account local to the machine?")]
        public bool LocalUserImpersonation { get; set; }

        public Authentication()
        {
            ImpersonationUser = "";
            ImpersonationPassword = "";
            LocalUserImpersonation = false;
        }

        public override void PopulateFrom(Sys.Security.IAuthentication source)
        {
            if (source.VersionDescriptor.TypeName == VersionDescriptor.TypeName)
            {
                PropertyInfo i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "ImpersonationUser");
                if (i != null)
                {
                    string k = i.GetValue(source) as string;
                    if (!String.IsNullOrEmpty(k) && String.IsNullOrEmpty(ImpersonationUser))
                        ImpersonationUser = k;
                }

                i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "ImpersonationPassword");
                if (i != null)
                {
                    string k = i.GetValue(source) as string;
                    if (!String.IsNullOrEmpty(k) && String.IsNullOrEmpty(ImpersonationPassword))
                    {
                        ImpersonationUser = k;

                        i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "LocalUserImpersonation");
                        if (i != null)
                        {
                            LocalUserImpersonation = (bool)i.GetValue(source);
                        }
                    }
                }
            }
            else
            {
                throw new Exception("IAuthentication Type mismatch.");
            }
        }

        public override IListingAgent ConstructListingAgent(ListingType listingType, string path, string fileFilter, string subpathFilter, bool recurse)
        {
            return new ListingAgent(this, listingType, path, fileFilter, subpathFilter, recurse);
        }

        public override void CreateDirectory(string directory)
        {
            Impersonation token = null;

            try
            {
                Impersonate(out token);

                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Unimpersonate(token);
            }
        }

        public override void DeleteDirectory(string directory, bool recurse, bool deleteFiles)
        {
            Impersonation token = null;

            try
            {
                Impersonate(out token);

                if (System.IO.Directory.Exists(directory))
                {
                    if (!deleteFiles)
                    {
                        if (STEM.Sys.IO.Directory.STEM_GetFiles(directory, "*", "*", recurse ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly, false).Count > 0)
                            throw new System.IO.IOException(directory + " contains files and deleteFiles is false.");
                    }

                    System.IO.Directory.Delete(directory, recurse);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Unimpersonate(token);
            }
        }

        public override void DeleteFile(string file)
        {
            Impersonation token = null;

            try
            {
                Impersonate(out token);

                if (System.IO.File.Exists(file))
                    System.IO.File.Delete(file);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Unimpersonate(token);
            }
        }

        public override bool DirectoryExists(string directory)
        {
            Impersonation token = null;

            try
            {
                Impersonate(out token);

                return System.IO.Directory.Exists(directory);
            }
            catch
            {
            }
            finally
            {
                Unimpersonate(token);
            }

            return false;
        }

        public override bool FileExists(string file)
        {
            Impersonation token = null;

            try
            {
                Impersonate(out token);

                return System.IO.File.Exists(file);
            }
            catch
            {
            }
            finally
            {
                Unimpersonate(token);
            }

            return false;
        }

        public override DirectoryInfo GetDirectoryInfo(string directory)
        {
            Impersonation token = null;

            try
            {
                Impersonate(out token);

                if (System.IO.Directory.Exists(directory))
                {
                    System.IO.DirectoryInfo i = new System.IO.DirectoryInfo(directory);
                    return new DirectoryInfo { CreationTimeUtc = i.CreationTimeUtc, LastAccessTimeUtc = i.LastAccessTimeUtc, LastWriteTimeUtc = i.LastWriteTimeUtc };
                }
            }
            catch
            { 
            }
            finally
            {
                Unimpersonate(token);
            }

            return null;
        }

        public override FileInfo GetFileInfo(string file)
        {
            Impersonation token = null;

            try
            {
                Impersonate(out token);

                if (System.IO.File.Exists(file))
                {
                    System.IO.FileInfo i = new System.IO.FileInfo(file);
                    return new FileInfo { CreationTimeUtc = i.CreationTimeUtc, LastAccessTimeUtc = i.LastAccessTimeUtc, LastWriteTimeUtc = i.LastWriteTimeUtc, Size = i.Length };
                }
            }
            catch
            {
            }
            finally
            {
                Unimpersonate(token);
            }

            return null;
        }

        internal void Impersonate(out Impersonation token)
        {
            try
            {
                if (!String.IsNullOrEmpty(ImpersonationUser) && !String.IsNullOrEmpty(ImpersonationPassword))
                {
                    Impersonation i = new Impersonation();
                    token = i;

                    i.Impersonate(ImpersonationUser, ImpersonationPassword, LocalUserImpersonation);
                }
                else
                {
                    token = null;
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("SMB.Authentication.Impersonate", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                throw ex;
            }
        }

        internal void Unimpersonate(Impersonation token)
        {
            try
            {
                if (token != null)
                    token.UnImpersonate();
            }
            catch { }
        }
    }
}
