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
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;
using System.IO;

namespace STEM.Sys.Security
{
    public class Impersonation : IDisposable
    {
        object _ObjectLock = new object();

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        extern static bool CloseHandle(IntPtr handle);

        private static object _StaticLock = new object();

        object _ImpersonatedUser = null;
        IntPtr _UserHandle = IntPtr.Zero;

        public void UnImpersonate()
        {
            lock (_ObjectLock)
            {
                try
                {
                    if (_ImpersonatedUser != null)
                    {
#if NETFRAMEWORK
                        ((WindowsImpersonationContext)_ImpersonatedUser).Undo();
                        ((WindowsImpersonationContext)_ImpersonatedUser).Dispose();
#endif
                    }
                }
                catch { }
                finally
                {
                    _ImpersonatedUser = null;
                }

                try
                {
                    if (_UserHandle != IntPtr.Zero)
                    {
                        CloseHandle(_UserHandle);
                    }
                }
                catch { }
                finally
                {
                    _UserHandle = IntPtr.Zero;
                }
            }
        }
        
        public void Impersonate(string userName, string password, bool remote)
        {
            string domainName = STEM.Sys.IO.Path.GetDirectoryName(userName);
            string Username = STEM.Sys.IO.Path.GetFileName(userName);
            if (domainName == ".")
                domainName = Environment.MachineName;

            Impersonate(domainName, Username, password, remote);
        }

#if NETFRAMEWORK
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
#endif
        public void Impersonate(string domainName, string userName, string password, bool remote)
        {
            lock (_StaticLock)
            {
                if (_UserHandle != IntPtr.Zero)
                    throw new Exception("Impersonation object already in use.");

                _UserHandle = new IntPtr(0);

                bool returnValue = false;

                //enum LogonType 
                //LOGON32_LOGON_INTERACTIVE = 2,
                //LOGON32_LOGON_NETWORK = 3,
                //LOGON32_LOGON_BATCH = 4,
                //LOGON32_LOGON_SERVICE = 5,
                //LOGON32_LOGON_UNLOCK = 7,
                //LOGON32_LOGON_NETWORK_CLEARTEXT = 8,
                //LOGON32_LOGON_NEW_CREDENTIALS = 9,    

                //enum LogonProvider 
                //LOGON32_PROVIDER_DEFAULT = 0,
                //LOGON32_PROVIDER_WINNT50 = 3,

                try
                {
                    if (!remote)
                        returnValue = LogonUser(userName, domainName, password, 2, 0, out _UserHandle);
                    else
                        returnValue = LogonUser(userName, domainName, password, 9, 3, out _UserHandle);

                    if (false == returnValue)
                    {
                        int ret = Marshal.GetLastWin32Error();
                        throw new System.ComponentModel.Win32Exception(ret);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("LogonUser threw an exception.", ex);
                }

                try
                {
#if NETFRAMEWORK
                    WindowsIdentity id = new WindowsIdentity(_UserHandle);
                    _ImpersonatedUser = id.Impersonate();
#endif
                }
                catch (Exception ex)
                {
                    throw new Exception("WindowsIdentity.Impersonate threw an exception.", ex);
                }
            }
        }

        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch { }

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            UnImpersonate();
        }
    }
}
