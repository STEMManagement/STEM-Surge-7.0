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
using System.Linq;

namespace STEM.Sys.Serialization
{
    public class Dictionary<TKey, TValue> : System.Collections.Generic.Dictionary<TKey, TValue>, IXmlSerializable
    {
        public Dictionary() : base() { }
        public Dictionary(Dictionary<TKey, TValue> source) : base()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (TKey key in source.Keys.ToList())
                this[key] = source[key];
        }

        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            TKey key;
            TValue value;

            XmlSerializer xsKey = STEM.Sys.XmlSerializerCollection.GetXmlSerializer(typeof(TKey));
            XmlSerializer xsValue = STEM.Sys.XmlSerializerCollection.GetXmlSerializer(typeof(TValue));

            reader.ReadStartElement();
            if (reader.NodeType == XmlNodeType.None || reader.Name != "Entry")
                return;

            while (reader.NodeType == XmlNodeType.EndElement)
                return;

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement();
                key = (TKey)xsKey.Deserialize(reader);
                value = (TValue)xsValue.Deserialize(reader);
                if (key != null)
                    this.Add(key, value);
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            try
            {
                XmlSerializer xsKey = STEM.Sys.XmlSerializerCollection.GetXmlSerializer(typeof(TKey));
                XmlSerializer xsValue = STEM.Sys.XmlSerializerCollection.GetXmlSerializer(typeof(TValue));

                System.Collections.Generic.List<TKey> keys = new System.Collections.Generic.List<TKey>(this.Keys);
                foreach (TKey key in keys)
                {
                    if (key == null)
                        continue;

                    if (this.ContainsKey(key))
                        if (this[key] != null)
                        {
                            lock (this[key])
                            {
                                writer.WriteStartElement("Entry");
                                try
                                {
                                    xsKey.Serialize(writer, key);
                                    xsValue.Serialize(writer, this[key]);
                                }
                                finally
                                {
                                    writer.WriteEndElement();
                                }
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Dictionary.WriteXml", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                throw;
            }
        }
    }
}

