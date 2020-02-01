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

namespace STEM.Surge
{
    /// <summary>
    /// This is used to identify classes wherein some sort of authentication is being configured so that the UI can present bulk reconfiguration options to users.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class IAuthentication : STEM.Sys.Serializable, IDisposable
    {
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("IAuthentication.Dispose", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
        }
    }
}
