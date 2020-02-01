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
using System.Globalization;
using System.Xml;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace STEM.Sys.Serialization
{
    
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TimeSpan : IXmlSerializable
    {
        public System.TimeSpan Value { get; set; }

        public TimeSpan()
        {
            Value = System.TimeSpan.FromMilliseconds(0);
        }

        public TimeSpan(System.TimeSpan t)
        {
            Value = System.TimeSpan.FromMilliseconds(t.TotalMilliseconds);
        }

        public TimeSpan(double TotalMilliseconds)
        {
            Value = System.TimeSpan.FromMilliseconds(TotalMilliseconds);
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            XmlDocument doc = new XmlDocument();

            doc.LoadXml(reader.ReadInnerXml());

            Value = System.TimeSpan.FromMilliseconds(0);

            try
            {
                double tms = Double.Parse(doc.DocumentElement.SelectSingleNode("//TotalMilliseconds").InnerText, System.Globalization.CultureInfo.CurrentCulture);
                Value = System.TimeSpan.FromMilliseconds(tms);
            }
            catch { }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteRaw("<TimeSpan><TotalMilliseconds>" + Value.TotalMilliseconds + "</TotalMilliseconds></TimeSpan>");
        }
    }
}
