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
using Confluent.Kafka;

namespace STEM.Surge.Kafka
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Topic to Container")]
    [Description("Write data from a Topic to a container.")]
    public class TopicToContainer : STEM.Surge.Instruction
    {
        [Category("Kafka Server")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }

        [DisplayName("Container Data Key")]
        [Description("The key in the container where the data is to be loaded.")]
        public string ContainerDataKey { get; set; }

        [DisplayName("Target Container")]
        [Description("The container where the data is to be loaded.")]
        public ContainerType TargetContainer { get; set; }

        [DisplayName("Content Type")]
        [Description("Whether the data is a string or a byte array.")]
        public DataType ContentType { get; set; }

        [Category("Retry")]
        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
        public int Retry { get; set; }

        [Category("Retry")]
        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should we wait between retries?")]
        public int RetryDelaySeconds { get; set; }

        [Category("Flow")]
        [DisplayName("Zero Items Action"), Description("What flow action should be taken if no items are found?")]
        public FailureAction ZeroItemsAction { get; set; }

        public TopicToContainer()
        {
            Authentication = new Authentication();

            ContentType = DataType.String;

            ContainerDataKey = "[TargetNameWithoutExt]";
            TargetContainer = ContainerType.InstructionSetContainer;

            Retry = 1;
            RetryDelaySeconds = 2;
            ZeroItemsAction = FailureAction.SkipRemaining;
        }

        protected override void _Rollback()
        {
            if (_Data != null)
            {
                string key = Guid.NewGuid().ToString();

                InstructionSet.InstructionSetContainer[key] = _Data;

                ContainerToTopic r = new ContainerToTopic();
                r.Authentication = Authentication;
                r.ContainerDataKey = key;
                r.TargetContainer = ContainerType.InstructionSetContainer;
                r.Retry = 0;
                r.RetryDelaySeconds = 0;
            }
        }

        byte[] _Data = null;

        protected override bool _Run()
        {
            int r = Retry;

            while (r-- >= 0 && !Stop)
            {
                try
                {
                    string sData = null;
                    byte[] bData = null;


                    if (ContentType == DataType.String)
                    {
                        using (IConsumer<Ignore, string> c = new ConsumerBuilder<Ignore, string>(Authentication.ConsumerConfig(Guid.NewGuid().ToString(), AutoOffsetReset.Earliest)).Build())
                        {
                            c.Subscribe(Authentication.TopicName);


                            ConsumeResult<Ignore, string> msg = c.Consume(RetryDelaySeconds * 1000);

                            if (msg != null && msg.Message != null)
                                sData = msg.Message.Value;
                        }
                    }
                    else
                    {
                        using (IConsumer<Ignore, byte[]> c = new ConsumerBuilder<Ignore, byte[]>(Authentication.ConsumerConfig(Guid.NewGuid().ToString(), AutoOffsetReset.Earliest)).Build())
                        {
                            c.Subscribe(Authentication.TopicName);


                            ConsumeResult<Ignore, byte[]> msg = c.Consume(RetryDelaySeconds * 1000);

                            if (msg != null && msg.Message != null)
                                bData = msg.Message.Value;
                        }
                    }

                    if (sData == null && bData == null)
                    {
                        // No message available at this time.
                        if (r < 0)
                        {
                            switch (ZeroItemsAction)
                            {
                                case FailureAction.SkipRemaining:
                                    Exceptions.Clear();
                                    SkipRemaining();
                                    break;

                                case FailureAction.SkipNext:
                                    Exceptions.Clear();
                                    SkipNext();
                                    break;

                                case FailureAction.SkipToLabel:
                                    Exceptions.Clear();
                                    SkipForwardToFlowControlLabel(FailureActionLabel);
                                    break;

                                case FailureAction.Rollback:
                                    RollbackAllPreceedingAndSkipRemaining();
                                    break;

                                case FailureAction.Continue:
                                    Exceptions.Clear();
                                    break;
                            }

                            Message = "0 Items Actioned\r\n" + Message;

                            return Exceptions.Count == 0;
                        }
                        else
                        {
                            throw new Exception("No items in queue.");
                        }
                    }
                    
                    switch (TargetContainer)
                    {
                        case ContainerType.InstructionSetContainer:

                            if (bData != null)
                                InstructionSet.InstructionSetContainer[ContainerDataKey] = bData;
                            else
                                InstructionSet.InstructionSetContainer[ContainerDataKey] = sData;

                            break;

                        case ContainerType.Session:

                            if (bData != null)
                                STEM.Sys.State.Containers.Session[ContainerDataKey] = bData;
                            else
                                STEM.Sys.State.Containers.Session[ContainerDataKey] = sData;

                            break;

                        case ContainerType.Cache:

                            if (bData != null)
                                STEM.Sys.State.Containers.Cache[ContainerDataKey] = bData;
                            else
                                STEM.Sys.State.Containers.Cache[ContainerDataKey] = sData;

                            break;
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
