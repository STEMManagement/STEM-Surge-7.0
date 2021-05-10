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
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Xml.Schema;
using System.Text.RegularExpressions;

namespace STEM.Sys
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    
    public class VersionDescriptor
    {
        public VersionDescriptor()
            : this(typeof(VersionDescriptor))
        {
        }

        public VersionDescriptor(Type type)
        {
            if (type != null)
            {
                TypeName = type.FullName;
                AssemblyName = type.Assembly.FullName;
            }
        }

        [XmlIgnore]
        [Category("Version")]
        public string VersionNumber { get; set; }

        string _AssemblyName = null;
        public string AssemblyName
        {
            get
            {
                int start = _AssemblyName.IndexOf("Version=", StringComparison.InvariantCultureIgnoreCase);
                int end = _AssemblyName.IndexOf(',', start) - start;

                string replace = _AssemblyName.Substring(start, _AssemblyName.IndexOf(',', start) - start);

                _AssemblyName = _AssemblyName.Replace(replace, "Version=" + VersionNumber);

                return _AssemblyName;
            }

            set
            {
                _AssemblyName = value;

                if (_AssemblyName != null)
                {
                    if (string.IsNullOrEmpty(VersionNumber))
                    {
                        int start = _AssemblyName.IndexOf("Version=", StringComparison.InvariantCultureIgnoreCase) + 8;
                        VersionNumber = _AssemblyName.Substring(start, _AssemblyName.IndexOf(',', start) - start);
                    }
                }
            }
        }

        public string TypeName { get; set; }

        public string Serialize()
        {
            return

"\r\n   <VersionDescriptor>" +
"\r\n      <AssemblyName>" + AssemblyName + "</AssemblyName>" +
"\r\n      <TypeName>" + TypeName + "</TypeName>" +
"\r\n   </VersionDescriptor>";
        }
    }

    public static class XmlSerializerCollection
    {
        static Dictionary<string, XmlSerializer> XmlSerializers = new Dictionary<string, XmlSerializer>();

        static public XmlSerializer GetXmlSerializer(Type t)
        {
            if (t == null)
                throw new ArgumentNullException(nameof(t));

            if (!XmlSerializerCollection.XmlSerializers.ContainsKey(t.AssemblyQualifiedName))
                lock (XmlSerializerCollection.XmlSerializers)
                    if (!XmlSerializerCollection.XmlSerializers.ContainsKey(t.AssemblyQualifiedName))
                        XmlSerializerCollection.XmlSerializers[t.AssemblyQualifiedName] = new XmlSerializer(t);

            return XmlSerializerCollection.XmlSerializers[t.AssemblyQualifiedName];
        }
    }

    internal partial class XmlWrappingReader : XmlReader, IXmlLineInfo
    {
        //
        // Fields
        //
        protected XmlReader reader;
        protected IXmlLineInfo readerAsIXmlLineInfo;

        // 
        // Constructor
        //
        internal XmlWrappingReader(XmlReader baseReader)
        {
            this.reader = baseReader;
            this.readerAsIXmlLineInfo = baseReader as IXmlLineInfo;
        }

        //
        // XmlReader implementation
        //
        public override XmlReaderSettings Settings { get { return reader.Settings; } }
        public override XmlNodeType NodeType { get { return reader.NodeType; } }
        public override string Name { get { return reader.Name; } }
        public override string LocalName { get { return reader.LocalName; } }
        public override string NamespaceURI { get { return reader.NamespaceURI; } }
        public override string Prefix { get { return reader.Prefix; } }
        public override bool HasValue { get { return reader.HasValue; } }
        public override string Value { get { try { return reader.Value; } catch { } return ""; } }
        public override int Depth { get { return reader.Depth; } }
        public override string BaseURI { get { return reader.BaseURI; } }
        public override bool IsEmptyElement { get { return reader.IsEmptyElement; } }
        public override bool IsDefault { get { return reader.IsDefault; } }
        public override XmlSpace XmlSpace { get { return reader.XmlSpace; } }
        public override string XmlLang { get { return reader.XmlLang; } }
        public override System.Type ValueType { get { return reader.ValueType; } }
        public override int AttributeCount { get { return reader.AttributeCount; } }
        public override bool EOF { get { return reader.EOF; } }
        public override ReadState ReadState { get { return reader.ReadState; } }
        public override bool HasAttributes { get { return reader.HasAttributes; } }
        public override XmlNameTable NameTable { get { return reader.NameTable; } }
        public override bool CanResolveEntity { get { return reader.CanResolveEntity; } }

        public override string GetAttribute(string name)
        {
            return reader.GetAttribute(name);
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            return reader.GetAttribute(name, namespaceURI);
        }

        public override string GetAttribute(int i)
        {
            return reader.GetAttribute(i);
        }

        public override bool MoveToAttribute(string name)
        {
            return reader.MoveToAttribute(name);
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            return reader.MoveToAttribute(name, ns);
        }

        public override void MoveToAttribute(int i)
        {
            reader.MoveToAttribute(i);
        }

        public override bool MoveToFirstAttribute()
        {
            return reader.MoveToFirstAttribute();
        }

        public override bool MoveToNextAttribute()
        {
            return reader.MoveToNextAttribute();
        }

        public override bool MoveToElement()
        {
            return reader.MoveToElement();
        }

        public override bool Read()
        {
            return reader.Read();
        }

        public override void Close()
        {
            reader.Close();
        }

        public override void Skip()
        {
            reader.Skip();
        }

        public override string LookupNamespace(string prefix)
        {
            return reader.LookupNamespace(prefix);
        }

        public override void ResolveEntity()
        {
            reader.ResolveEntity();
        }

        public override bool ReadAttributeValue()
        {
            return reader.ReadAttributeValue();
        }

        //
        // IXmlLineInfo members
        //
        public virtual bool HasLineInfo()
        {
            return (readerAsIXmlLineInfo == null) ? false : readerAsIXmlLineInfo.HasLineInfo();
        }

        public virtual int LineNumber
        {
            get
            {
                return (readerAsIXmlLineInfo == null) ? 0 : readerAsIXmlLineInfo.LineNumber;
            }
        }

        public virtual int LinePosition
        {
            get
            {
                return (readerAsIXmlLineInfo == null) ? 0 : readerAsIXmlLineInfo.LinePosition;
            }
        }        
    }
    internal class XmlExtendableReader : XmlWrappingReader
    {
        private bool _ignoreNamespace { get; set; }

        public XmlExtendableReader(TextReader input, XmlReaderSettings settings, bool ignoreNamespace = false)
        : base(XmlReader.Create(input, settings))
        {
            _ignoreNamespace = ignoreNamespace;
        }
        public XmlExtendableReader(Stream input, XmlReaderSettings settings, bool ignoreNamespace = false)
        : base(XmlReader.Create(input, settings))
        {
            _ignoreNamespace = ignoreNamespace;
        }

        public override string NamespaceURI
        {
            get
            {
                return _ignoreNamespace ? String.Empty : base.NamespaceURI;
            }
        }
    }


    [TypeConverter(typeof(ExpandableObjectConverter))]
    
    public class Serializable
    {
        public Serializable()
        {
            _VersionDescriptor = new VersionDescriptor(GetType());
            VersionNumber = _VersionDescriptor.VersionNumber;
        }

        VersionDescriptor _VersionDescriptor;
        [Browsable(false)]
        public VersionDescriptor VersionDescriptor { get { return _VersionDescriptor; } set { } }

        [XmlIgnore]
        [Category("Version")]
        public string VersionNumber { get; set; }

        public static string Serialize(object o)
        {
            if (o == null)
                return "";

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;
            xws.IndentChars = "   ";

            using (StringWriter sw = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(sw, xws))
                {
                    XmlSerializerCollection.GetXmlSerializer(o.GetType()).Serialize(xmlWriter, o);
                    return sw.ToString();
                }
            }
        }
        
        public virtual string Serialize()
        {
            VersionDescriptor.VersionNumber = VersionNumber;
            return Serialize(this);
        }

        static Dictionary<Type, XmlReaderSettings> _NameTables = new Dictionary<Type, XmlReaderSettings>();

        public static object Deserialize(string xml, Type t)
        {
            if (t == null)
                return null;

            XmlReaderSettings settings = null;

            if (_NameTables.ContainsKey(t))
                settings = _NameTables[t];

            using (TextReader textReader = new StringReader(xml))
            {
                if (settings != null)
                {                    
                    using (XmlExtendableReader reader = new XmlExtendableReader(textReader, settings, true))
                    {
                        return XmlSerializerCollection.GetXmlSerializer(t).Deserialize(reader);
                    }
                }
                else
                {
                    settings = new XmlReaderSettings()
                    {
                        CheckCharacters = false,
                        ConformanceLevel = ConformanceLevel.Document,
                        DtdProcessing = DtdProcessing.Ignore,
                        IgnoreComments = true,
                        IgnoreProcessingInstructions = true,
                        IgnoreWhitespace = true,
                        ValidationType = ValidationType.None,
                        NameTable = null
                    };

                    using (XmlExtendableReader reader = new XmlExtendableReader(textReader, settings, true))
                    {
                        object o = XmlSerializerCollection.GetXmlSerializer(t).Deserialize(reader);

                        settings.NameTable = reader.NameTable;
                        lock (_NameTables)
                            _NameTables[t] = settings;

                        return o;
                    }
                }
            }
        }

        public static object Deserialize(Stream xml, Type t)
        {
            if (t == null)
                return null;

            XmlReaderSettings settings = null;

            if (_NameTables.ContainsKey(t))
                settings = _NameTables[t];

            if (settings != null)
            {
                using (XmlExtendableReader reader = new XmlExtendableReader(xml, settings, true))
                {
                    return XmlSerializerCollection.GetXmlSerializer(t).Deserialize(reader);
                }
            }
            else
            {
                settings = new XmlReaderSettings()
                {
                    CheckCharacters = false,
                    ConformanceLevel = ConformanceLevel.Document,
                    DtdProcessing = DtdProcessing.Ignore,
                    IgnoreComments = true,
                    IgnoreProcessingInstructions = true,
                    IgnoreWhitespace = true,
                    ValidationType = ValidationType.None,
                    NameTable = null
                };

                using (XmlExtendableReader reader = new XmlExtendableReader(xml, settings, true))
                {
                    object o = XmlSerializerCollection.GetXmlSerializer(t).Deserialize(reader);

                    settings.NameTable = reader.NameTable;
                    lock (_NameTables)
                        _NameTables[t] = settings;

                    return o;
                }
            }
        }

        //const string _AssemblyNameOpen = "<AssemblyName>";
        //const string _AssemblyNameClose = "</AssemblyName>";
        //const string _TypeNameOpen = "<TypeName>";
        //const string _TypeNameClose = "</TypeName>";

        static Regex _AssemblyNameRegex = new Regex("(?<=<AssemblyName>)(?:(?!</AssemblyName>).)*", System.Text.RegularExpressions.RegexOptions.Compiled);
        static Regex _TypeNameRegex = new Regex("(?<=<TypeName>)(?:(?!</TypeName>).)*", System.Text.RegularExpressions.RegexOptions.Compiled);

        public static object Deserialize(string xml)
        {
            if (String.IsNullOrEmpty(xml))
                return null;

            string aName = null;
            Match m = _AssemblyNameRegex.Match(xml);
            if (m.Success)
                aName = m.Value;

            string tName = null;
            m = _TypeNameRegex.Match(xml);
            if (m.Success)
                tName = m.Value;

            //int index = xml.IndexOf(_AssemblyNameOpen, StringComparison.Ordinal);

            //string aName = xml.Substring(index + _AssemblyNameOpen.Length, xml.IndexOf(_AssemblyNameClose, StringComparison.Ordinal) - (index + _AssemblyNameOpen.Length));

            //index = xml.IndexOf(_TypeNameOpen, StringComparison.Ordinal);

            //string tName = xml.Substring(index + _TypeNameOpen.Length, xml.IndexOf(_TypeNameClose, StringComparison.Ordinal) - (index + _TypeNameOpen.Length));

            if (aName == null || tName == null)
                throw new Exception("The VersionDescriptor tag is missing or malformed.");

            Assembly asm = STEM.Sys.Serialization.VersionManager.LoadedAssembly(aName);

            if (asm == null)
                throw new Exception("The Assembly:" + aName + " could not be located.");

            Type tgt = asm.GetType(tName);

            if (tgt == null)
                throw new Exception("The Type:" + tName + " could not be located.");
            
            object ds = Deserialize(xml, tgt);

            return ds;
        }
    }
}
