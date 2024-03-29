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
using RabbitMQ.Client;

namespace STEM.Surge.RabbitMQ
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Container To Queue")]
    [Description("Write data from a container to the Queue.")]
    public class ContainerToQueue : STEM.Surge.Instruction
    {
        [Category("Queue Server")]
        [DisplayName("Queue Server Address"), DescriptionAttribute("What is the Queue Server Address?")]
        public string ServerAddress { get; set; }

        [Category("Queue Server")]
        [DisplayName("Queue Port"), DescriptionAttribute("What is the Queue Port?")]
        public string Port { get; set; }

        [Category("Queue Server")]
        [DisplayName("Destination Queue"), Description("The Queue to which the data is to be saved.")]
        public string QueueName { get; set; }

        [DisplayName("Container Data Key")]
        [Description("The key in the container where the data is loaded.")]
        public string ContainerDataKey { get; set; }

        [DisplayName("Target Container")]
        [Description("The container with the data to be queued.")]
        public ContainerType TargetContainer { get; set; }
        
        [Category("Retry")]
        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
        public int Retry { get; set; }

        [Category("Retry")]
        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should we wait between retries?")]
        public int RetryDelaySeconds { get; set; }

        public ContainerToQueue()
        {
            ServerAddress = "[QueueServerAddress]";
            Port = "[QueueServerPort]";

            QueueName = "[QueueName]";

            ContainerDataKey = "[TargetNameWithoutExt]";
            TargetContainer = ContainerType.InstructionSetContainer;

            Retry = 1;
            RetryDelaySeconds = 2;
        }

        protected override void _Rollback()
        {            
        }
        
        protected override bool _Run()
        {
            return ExecuteContainerToQueue();
        }

        public bool ExecuteContainerToQueue()
        {
            int r = Retry;

            while (r-- >= 0 && !Stop)
            {
                try
                {
                    string sData = null;
                    byte[] bData = null;

                    switch (TargetContainer)
                    {
                        case ContainerType.InstructionSetContainer:

                            if (!InstructionSet.InstructionSetContainer.ContainsKey(ContainerDataKey))
                                throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                            sData = InstructionSet.InstructionSetContainer[ContainerDataKey] as string;
                            bData = InstructionSet.InstructionSetContainer[ContainerDataKey] as byte[];

                            break;

                        case ContainerType.Session:

                            if (!STEM.Sys.State.Containers.Session.ContainsKey(ContainerDataKey))
                                throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                            sData = STEM.Sys.State.Containers.Session[ContainerDataKey] as string;
                            bData = STEM.Sys.State.Containers.Session[ContainerDataKey] as byte[];

                            break;

                        case ContainerType.Cache:

                            if (!STEM.Sys.State.Containers.Cache.ContainsKey(ContainerDataKey))
                                throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                            sData = STEM.Sys.State.Containers.Cache[ContainerDataKey] as string;
                            bData = STEM.Sys.State.Containers.Cache[ContainerDataKey] as byte[];

                            break;
                    }

                    byte[] data = null;

                    if (bData != null && bData.Length > 0)
                        data = bData;

                    if (data == null)
                        if (sData != null && sData.Length > 0)
                            data = System.Text.Encoding.UTF8.GetBytes(sData);
                    
                    if (data != null)
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
                                                     body: data);
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
