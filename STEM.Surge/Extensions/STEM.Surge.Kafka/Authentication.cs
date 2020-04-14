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
using System.Reflection;
using System.ComponentModel;
using System.Xml.Serialization;
using STEM.Sys.Security;
using Confluent.Kafka;

namespace STEM.Surge.Kafka
{
    public class Authentication : IAuthentication
    {
        [Category("Kafka Server")]
        [DisplayName("Server Address"), DescriptionAttribute("What is the Server Address?")]
        public string ServerAddress { get; set; }

        [Category("Kafka Server")]
        [DisplayName("Server Port"), DescriptionAttribute("What is the Server Port?")]
        public string Port { get; set; }

        [Category("Kafka Server")]
        [DisplayName("Topic Name"), Description("The Topic being addressed.")]
        public string TopicName { get; set; }

        [Category("Kafka Server")]
        [DisplayName("Ssl Certificate Location"), Description("The location of the PEM file.")]
        public string SslCaLocation { get; set; }

        [Category("Kafka Server")]
        [DisplayName("Security Protocol"), Description("The Security Protocol to be used.")]
        public SecurityProtocol SecurityProtocol { get; set; }

        [Category("Kafka Server")]
        [DisplayName("Sasl Mechanism"), Description("The Sasl Mechanism to be used.")]
        public SaslMechanism SaslMechanism { get; set; }

        [Category("Kafka Server")]
        [DisplayName("Sasl Username"), Description("The Sasl Username to be used.")]
        public string SaslUsername { get; set; }
               
        [Category("Kafka Server")]
        [DisplayName("Sasl Password"), DescriptionAttribute("The Sasl Password to be used?")]
        [XmlIgnore]
        [PasswordPropertyText(true)]
        public string SaslPassword { get; set; }
        [Browsable(false)]
        public string SaslPasswordEncoded
        {
            get
            {
                return this.Entangle(SaslPassword);
            }

            set
            {
                SaslPassword = this.Detangle(value);
            }
        }

        public Authentication()
        {            
            string platform = "";
            string lib = "";
            
            if (STEM.Sys.Control.IsWindows)
            {
                lib = "librdkafka.dll";
                platform = "win-x86";
                if (STEM.Sys.Control.IsX64)
                    platform = "win-x64";
            }
            else
            {
                lib = "librdkafka.so";
                platform = "linux-x86";
                if (STEM.Sys.Control.IsX64)
                    platform = "linux-x64";
            }

            foreach (string f in STEM.Sys.IO.Directory.STEM_GetFiles(STEM.Sys.Serialization.VersionManager.VersionCache, lib, platform, System.IO.SearchOption.AllDirectories, false))
                lib = f;
            
            Confluent.Kafka.Library.Load(lib);

            ServerAddress = "[QueueServerAddress]";
            Port = "[QueueServerPort]";

            TopicName = "[TopicName]";
            SslCaLocation = "";
            SecurityProtocol = SecurityProtocol.SaslSsl;
            SaslMechanism = SaslMechanism.ScramSha256;
            SaslUsername = "";
            SaslPassword = "";
        }

        [XmlIgnore]
        [Browsable(false)]
        public ProducerConfig ProducerConfig
        {
            get
            {
                switch (SecurityProtocol)
                {
                    case SecurityProtocol.Ssl:
                        return new ProducerConfig
                        {
                            BootstrapServers = ServerAddress + ":" + Port,
                            SslCaLocation = this.SslCaLocation,
                            SecurityProtocol = this.SecurityProtocol
                        };

                    case SecurityProtocol.SaslSsl:
                    case SecurityProtocol.SaslPlaintext:
                        return new ProducerConfig
                        {
                            BootstrapServers = ServerAddress + ":" + Port,
                            SecurityProtocol = this.SecurityProtocol,
                            SaslMechanism = this.SaslMechanism,
                            SaslUsername = this.SaslUsername,
                            SaslPassword = this.SaslPassword
                        };

                    default:
                        return new ProducerConfig
                        {
                            BootstrapServers = ServerAddress + ":" + Port,
                            SecurityProtocol = this.SecurityProtocol
                        };
                }
            }
        }

        public ConsumerConfig ConsumerConfig(string groupID, AutoOffsetReset autoOffsetReset = AutoOffsetReset.Earliest)
        {
            switch (SecurityProtocol)
            {
                case SecurityProtocol.Ssl:
                    return new ConsumerConfig
                    {
                        GroupId = groupID,
                        BootstrapServers = ServerAddress + ":" + Port,
                        SslCaLocation = this.SslCaLocation,
                        SecurityProtocol = this.SecurityProtocol,
                        AutoOffsetReset = autoOffsetReset
                    };

                case SecurityProtocol.SaslSsl:
                case SecurityProtocol.SaslPlaintext:
                    return new ConsumerConfig
                    {
                        GroupId = groupID,
                        BootstrapServers = ServerAddress + ":" + Port,
                        SecurityProtocol = this.SecurityProtocol,
                        SaslMechanism = this.SaslMechanism,
                        SaslUsername = this.SaslUsername,
                        SaslPassword = this.SaslPassword,
                        AutoOffsetReset = autoOffsetReset
                    };

                default:
                    return new ConsumerConfig
                    {
                        GroupId = groupID,
                        BootstrapServers = ServerAddress + ":" + Port,
                        SecurityProtocol = this.SecurityProtocol,
                        AutoOffsetReset = autoOffsetReset
                    };
            }
        }
    }
}
