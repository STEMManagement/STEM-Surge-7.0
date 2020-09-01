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
    [DisplayName("Queue to file")]
    [Description("Write data from a queue to a file.")]
    public class TopicToFile : STEM.Surge.Instruction
    {
        [Category("Kafka Server")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }

        [DisplayName("Destination File")]
        [Description("The file to which the data is to be saved.")]
        public string DestinationFile { get; set; }

        [DisplayName("File Exists Action")]
        [Description("What action should be taken if the Destination File already exists?")]
        public STEM.Sys.IO.FileExistsAction FileExistsAction { get; set; }

        [Category("Retry")]
        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
        public int Retry { get; set; }

        [Category("Retry")]
        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should we wait between retries?")]
        public int RetryDelaySeconds { get; set; }

        [Category("Flow")]
        [DisplayName("Zero Items Action"), Description("What flow action should be taken if no items are found?")]
        public FailureAction ZeroItemsAction { get; set; }

        public TopicToFile()
        {
            Authentication = new Authentication();

            DestinationFile = "[DestinationPath]\\[TargetName]";
            FileExistsAction = STEM.Sys.IO.FileExistsAction.MakeUnique;

            Retry = 1;
            RetryDelaySeconds = 2;
            ZeroItemsAction = FailureAction.SkipRemaining;
        }

        protected override void _Rollback()
        {
            if (_Data != null)
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

            if (_File != null)
                try
                {
                    System.IO.File.Delete(_File);
                }
                catch { }
        }

        byte[] _Data = null;

        string _File = null;

        protected override bool _Run()
        {
            int r = Retry;

            while (r-- >= 0 && !Stop)
            {
                try
                {
                    using (IConsumer<Ignore, byte[]> c = new ConsumerBuilder<Ignore, byte[]>(Authentication.ConsumerConfig(Guid.NewGuid().ToString(), AutoOffsetReset.Earliest)).Build())
                    {
                        c.Subscribe(Authentication.TopicName);


                        ConsumeResult<Ignore, byte[]> msg = c.Consume(RetryDelaySeconds * 1000);

                        if (msg != null && msg.Message != null)
                            _Data = msg.Message.Value;
                    }
                    
                    if (_Data == null)
                    {
                        // No data available at this time.
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
                            throw new Exception("No items in topic.");
                        }
                    }

                    string file = DestinationFile;

                    if (System.IO.File.Exists(DestinationFile))
                        switch (FileExistsAction)
                        {
                            case STEM.Sys.IO.FileExistsAction.Skip:
                                return true;

                            case STEM.Sys.IO.FileExistsAction.Throw:
                                throw new System.IO.IOException("File already exists.");

                            case STEM.Sys.IO.FileExistsAction.Overwrite:
                            case STEM.Sys.IO.FileExistsAction.OverwriteIfNewer:
                                System.IO.File.Delete(file);
                                break;

                            case STEM.Sys.IO.FileExistsAction.MakeUnique:
                                file = STEM.Sys.IO.File.UniqueFilename(file);
                                break;
                        }

                    using (System.IO.FileStream s = System.IO.File.Open(file, System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None))
                    {
                        s.Write(_Data, 0, _Data.Length);
                        _File = file;
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
