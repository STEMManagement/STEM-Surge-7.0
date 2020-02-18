﻿/*
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
using RdKafka;

namespace STEM.Surge.Kafka
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("File To Topic")]
    [Description("Write data from a file to the Topic.")]
    public class FileToTopic : STEM.Surge.Instruction
    {
        [Category("Kafka Server")]
        [DisplayName("Server Address"), DescriptionAttribute("What is the Server Address?")]
        public string ServerAddress { get; set; }

        [Category("Kafka Server")]
        [DisplayName("Server Port"), DescriptionAttribute("What is the Server Port?")]
        public string Port { get; set; }

        [Category("Kafka Server")]
        [DisplayName("Topic Name"), Description("The Topic to which the data is to be published.")]
        public string TopicName { get; set; }
        
        [DisplayName("Source File")]
        [Description("The file from which the data is to be obtained.")]
        public string SourceFile { get; set; }

        [Category("Retry")]
        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
        public int Retry { get; set; }

        [Category("Retry")]
        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should we wait between retries?")]
        public int RetryDelaySeconds { get; set; }

        public FileToTopic()
        {
            ServerAddress = "[QueueServerAddress]";
            Port = "[QueueServerPort]";

            TopicName = "[TopicName]";

            SourceFile = @"[TargetPath]\[TargetName]";

            Retry = 1;
            RetryDelaySeconds = 2;
        }

        protected override void _Rollback()
        {
        }

        protected override bool _Run()
        {
            int r = Retry;

            while (r-- >= 0 && !Stop)
            {
                try
                {
                    byte[] bData = System.IO.File.ReadAllBytes(SourceFile);

                    if (bData != null)
                    {
                        using (Producer producer = new Producer(ServerAddress + ":" + Port))
                        {
                            using (Topic topic = producer.Topic(TopicName))
                            {
                                topic.Produce(bData).Wait();
                            }
                        }
                    }

                    break;
                }
                catch (Exception ex)
                {
                    if (r < 0)
                    {
                        AppendToMessage(ex.Message);
                        Exceptions.Add(ex);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(RetryDelaySeconds * 1000);
                    }
                }
            }

            return Exceptions.Count == 0;
        }
    }
}