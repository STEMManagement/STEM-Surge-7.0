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
using RabbitMQ.Client;

namespace STEM.Surge.RabbitMQ
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("File To Queue")]
    [Description("Write data from a file to the Queue.")]
    public class FileToQueue : STEM.Surge.Instruction
    {
        [Category("Queue Server")]
        [DisplayName("Queue Server Address"), DescriptionAttribute("What is the Queue Server Address?")]
        public string ServerAddress { get; set; }

        [Category("Queue Server")]
        [DisplayName("Queue Port"), DescriptionAttribute("What is the Queue Port?")]
        public string Port { get; set; }

        [DisplayName("Destination Queue")]
        [Description("The Queue to which the data is to be saved.")]
        public string QueueName { get; set; }
        
        [DisplayName("Source File")]
        [Description("The file from which the data is to be obtained.")]
        public string SourceFile { get; set; }

        [Category("Retry")]
        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
        public int Retry { get; set; }

        [Category("Retry")]
        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should we wait between retries?")]
        public int RetryDelaySeconds { get; set; }

        public FileToQueue()
        {
            ServerAddress = "[QueueServerAddress]";
            Port = "[QueueServerPort]";

            QueueName = "[QueueName]";

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
                        ConnectionFactory factory = new ConnectionFactory() { HostName = ServerAddress, Port = Int32.Parse(this.Port) };
                        using (IConnection connection = factory.CreateConnection())
                        {
                            using (IModel channel = connection.CreateModel())
                            {
                                channel.QueueDeclare(queue: QueueName,
                                                     durable: false,
                                                     exclusive: false,
                                                     autoDelete: false,
                                                     arguments: null);

                                channel.BasicPublish(exchange: "",
                                                     routingKey: QueueName,
                                                     basicProperties: null,
                                                     body: bData);
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
