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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Net;

namespace STEM.Sys.Serialization
{
    
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class IPAddress : IXmlSerializable
    {
        public IPAddress()
            : this(null)
        {
        }

        public IPAddress(System.Net.IPAddress value)
        {
            Value = value;
        }

        public System.Net.IPAddress Value { get; set; }
                
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            string ip = reader.ReadElementContentAsString();
            System.Net.IPAddress address;

            if (System.Net.IPAddress.TryParse(ip, out address))
                Value = address;
            else
                Value = System.Net.IPAddress.Any;
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteValue(Value == null ? String.Empty : Value.ToString());
        }
    }
}