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
using STEM.Sys.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Reflection;
using System.IO;

namespace STEM.Sys.State
{
    
    [XmlType(TypeName = "STEM.Sys.State.SessionObject")]
    public class SessionObject : IDisposable
    {
        public STEM.Sys.VersionDescriptor VersionDescriptor { get { return new STEM.Sys.VersionDescriptor(_Value.GetType()); } set { } }

        public SessionObject() { }

        public SessionObject(string key, object value)
        {
            Key = key;
            _Value = value;
            LastAccess = DateTime.UtcNow;
            LastRead = DateTime.UtcNow;
            LastWrite = DateTime.UtcNow;
        }

        public string Key { get; set; }

        object _Value;

        [XmlIgnore]
        public object Value
        {
            get
            {
                LastAccess = DateTime.UtcNow;
                LastRead = DateTime.UtcNow;
                return _Value;
            }
        }

        public DateTime LastAccess { get; set; }
        public DateTime LastRead { get; set; }
        public DateTime LastWrite { get; set; }

        public string Serialize()
        {
            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    using (XmlTextWriter xtw = new XmlTextWriter(sw))
                    {
                        xtw.Formatting = Formatting.Indented;
                        xtw.Indentation = 3;

                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(STEM.Sys.Serializable.Serialize(this));

                        if (doc.DocumentElement.PreviousSibling != null)
                            doc.RemoveChild(doc.DocumentElement.PreviousSibling);

                        XmlDocument doc2 = new XmlDocument();
                        doc2.LoadXml(STEM.Sys.Serializable.Serialize(_Value));

                        if (doc2.DocumentElement.PreviousSibling != null)
                            doc2.RemoveChild(doc2.DocumentElement.PreviousSibling);

                        doc.DocumentElement.InnerXml += "<Value>" + doc2.DocumentElement.OuterXml + "</Value>";

                        doc.WriteContentTo(xtw);

                        return sw.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("SessionObject.Serialize", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                return null;
            }
        }

        public static SessionObject Deserialize(string xml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                if (doc.DocumentElement.SelectNodes("//VersionDescriptor/AssemblyName").Count == 0 ||
                    doc.DocumentElement.SelectNodes("//VersionDescriptor/TypeName").Count == 0)
                    throw new Exception("The VersionDescriptor tag is missing or malformed.");

                Assembly asm = AppDomain.CurrentDomain.Load(new AssemblyName(doc.DocumentElement.SelectNodes("//VersionDescriptor/AssemblyName")[0].InnerText));

                if (asm == null)
                    throw new Exception("The Assembly:" + doc.DocumentElement.SelectNodes("//VersionDescriptor/AssemblyName")[0].InnerText + " could not be located.");

                Type tgt = asm.GetType(doc.DocumentElement.SelectNodes("//VersionDescriptor/TypeName")[0].InnerText);

                if (tgt == null)
                    throw new Exception("The Type:" + doc.DocumentElement.SelectNodes("//VersionDescriptor/TypeName")[0].InnerText + " could not be located.");

                string valueXml = doc.DocumentElement.SelectSingleNode("Value").InnerXml;

                doc.DocumentElement.RemoveChild(doc.DocumentElement.SelectSingleNode("Value"));

                SessionObject so = STEM.Sys.Serializable.Deserialize(doc.DocumentElement.OuterXml, typeof(SessionObject)) as SessionObject;

                so._Value = STEM.Sys.Serializable.Deserialize(valueXml, tgt);

                return so;
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("SessionObject.Deserialize", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                return null;
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
            if(_Value is IDisposable)
                try
                {
                    ((IDisposable)_Value).Dispose();
                }
                catch { }

            _Value = null;
        }
    }
}
