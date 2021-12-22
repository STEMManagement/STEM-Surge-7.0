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

namespace STEM.Surge
{
    // STUB for backward compatibility
    // PLEASE USE STEM.Sys.Security.IAuthentication instead
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [XmlType(TypeName = "STEM.Surge.IAuthentication")]
    [Obsolete("STUB for backward compatibility. Please use STEM.Sys.Security.IAuthentication instead.", true)]
    public class IAuthentication : STEM.Sys.Security.IAuthentication
    {
        public override void PopulateFrom(Sys.Security.IAuthentication source)
        {
        }
    }
}
