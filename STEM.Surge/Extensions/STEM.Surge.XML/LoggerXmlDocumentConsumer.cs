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
using System.ComponentModel;
using System.Xml.Linq;
using System.Linq;
using STEM.Surge.Logging;

namespace STEM.Surge.XML
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Logger XmlDocument Consumer")]
    [Description("Read in Xml Logger Documents and feed them into a different Logger.")]
    public class LoggerXmlDocumentConsumer : Instruction
    {
        [Category("Logger")]
        [DisplayName("Logger Name"), DescriptionAttribute("The logger into which the Xml Logs should be ingested.")]
        public string LoggerName { get; set; }

        [Category("Logger")]
        [DisplayName("File List (Property Name: FileList)"), DescriptionAttribute("List<string> Property for the GroupingController to populate.")]
        public List<string> FileList { get; set; }

        public LoggerXmlDocumentConsumer()
        {
            FileList = new List<string>();
        }

        protected override void _Rollback()
        {
        }

        protected override bool _Run()
        {
            List<Exception> exceptions = new List<Exception>();

            try
            {
                List<ILogger.EventData> events = new List<ILogger.EventData>();
                List<ILogger.ObjectData> objects = new List<ILogger.ObjectData>();
                List<ILogger.EventMetadata> meta = new List<ILogger.EventMetadata>();

                List<string> filesConsumed = new List<string>();

                foreach (string file in FileList)
                {
                    try
                    {
                        XDocument doc = XDocument.Load(new System.Xml.XmlTextReader(new System.IO.StringReader(System.IO.File.ReadAllText(STEM.Sys.IO.Path.AdjustPath(file)))));

                        foreach (XElement e in doc.Descendants().Where(i => i.Name.LocalName == "ObjectData").ToList())
                        {
                            ILogger.ObjectData d = new ILogger.ObjectData
                            {
                                ID = Guid.Parse(e.Descendants().FirstOrDefault(i => i.Name.LocalName == "ID").Value),
                                Name = e.Descendants().FirstOrDefault(i => i.Name.LocalName == "Name").Value,
                                CreationTime = DateTime.Parse(e.Descendants().FirstOrDefault(i => i.Name.LocalName == "CreationTime").Value)
                            };

                            objects.Add(d);
                        }

                        foreach (XElement e in doc.Descendants().Where(i => i.Name.LocalName == "EventData").ToList())
                        {
                            ILogger.EventData d = new ILogger.EventData
                            {
                                EventID = Guid.Parse(e.Descendants().FirstOrDefault(i => i.Name.LocalName == "EventID").Value),
                                ObjectID = Guid.Parse(e.Descendants().FirstOrDefault(i => i.Name.LocalName == "ObjectID").Value),
                                ObjectName = e.Descendants().FirstOrDefault(i => i.Name.LocalName == "ObjectName").Value,
                                EventName = e.Descendants().FirstOrDefault(i => i.Name.LocalName == "EventName").Value,
                                ProcessName = e.Descendants().FirstOrDefault(i => i.Name.LocalName == "ProcessName").Value,
                                MachineName = e.Descendants().FirstOrDefault(i => i.Name.LocalName == "MachineName").Value,
                                EventTime = DateTime.Parse(e.Descendants().FirstOrDefault(i => i.Name.LocalName == "EventTime").Value)
                            };

                            events.Add(d);
                        }

                        foreach (XElement e in doc.Descendants().Where(i => i.Name.LocalName == "EventMetadata").ToList())
                        {
                            ILogger.EventMetadata d = new ILogger.EventMetadata
                            {
                                EventID = Guid.Parse(e.Descendants().FirstOrDefault(i => i.Name.LocalName == "EventID").Value),
                                Metadata = e.Descendants().FirstOrDefault(i => i.Name.LocalName == "Metadata").Value
                            };

                            meta.Add(d);
                        }

                        filesConsumed.Add(STEM.Sys.IO.Path.AdjustPath(file));
                    }
                    catch { }
                }

                ILogger logger = STEM.Surge.Logging.ILogger.GetLogger(LoggerName);

                foreach (Guid id in objects.Select(i => i.ID).ToList())
                {
                    List<ILogger.ObjectData> objs = objects.Where(i => i.ID == id).ToList();

                    if (objs.Count == 1)
                        continue;

                    ILogger.ObjectData keep = objs[0];
                    foreach (ILogger.ObjectData o in objs)
                    {
                        if (o == keep)
                            continue;

                        if (keep.CreationTime < o.CreationTime)
                        {
                            keep.CreationTime = o.CreationTime;

                            if (String.IsNullOrEmpty(keep.Name))
                                keep.Name = o.Name;
                        }
                    }

                    foreach (ILogger.ObjectData o in objs)
                    {
                        if (o == keep)
                            continue;

                        objects.Remove(o);
                    }
                }

                if (objects.Count > 0)
                    if (!logger.BulkLoad(objects, out exceptions))
                        throw new Exception("Error loading Objects");

                if (events.Count > 0)
                    if (!logger.BulkLoad(events, out exceptions))
                        throw new Exception("Error loading Events");

                if (meta.Count > 0)
                    if (!logger.BulkLoad(meta, out exceptions))
                        throw new Exception("Error loading EventMetadata");

                foreach (string file in filesConsumed)
                {
                    try
                    {
                        System.IO.File.Delete(file);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
                foreach (Exception e in exceptions)
                    Exceptions.Add(e);
            }

            return Exceptions.Count() == 0;
        }
    }
}
