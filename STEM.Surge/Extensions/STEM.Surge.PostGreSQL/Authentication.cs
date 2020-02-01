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
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Npgsql;
using STEM.Sys.Security;

namespace STEM.Surge.PostGreSQL
{
    public class Authentication : IAuthentication
    {
        [DisplayName("PostGres Server Address"), DescriptionAttribute("What is the database address?"), Category("PostGres Server")]
        public string PostGresDatabaseAddress { get; set; }

        [DisplayName("PostGres Server Port"), DescriptionAttribute("What is the database port?"), Category("PostGres Server")]
        public int PostGresDatabasePort { get; set; }

        [DisplayName("PostGres Server Database Name"), DescriptionAttribute("What is the database name?"), Category("PostGres Server")]
        public string PostGresDatabaseName { get; set; }

        [DisplayName("Use Integrated Security"), DescriptionAttribute("Should a user name and password be used or (in Windows only) should Integrated Security be used?"), Category("PostGres Server")]
        public bool UseIntegratedSecurity { get; set; }

        [DisplayName("PostGres Server user"), DescriptionAttribute("What is the database user?"), Category("PostGres Server")]
        public string PostGresUser { get; set; }

        [DisplayName("PostGres Server Password"), DescriptionAttribute("What is the database password?"), Category("PostGres Server")]
        [XmlIgnore]
        [PasswordPropertyText(true)]
        public string PostGresPassword { get; set; }
        [Browsable(false)]
        public string PostGresPasswordEncoded
        {
            get
            {
                return this.Entangle(PostGresPassword);
            }

            set
            {
                PostGresPassword = this.Detangle(value);
            }
        }

        public Authentication()
        {
            PostGresDatabaseAddress = "localhost";
            PostGresDatabasePort = 5432;
            PostGresDatabaseName = "MyDatabase";

            UseIntegratedSecurity = false;
            PostGresUser = "";
            PostGresPassword = "";
        }

        [XmlIgnore]
        [Browsable(false)]
        internal string ConnectionString
        {
            get
            {
                NpgsqlConnectionStringBuilder connectionString = new NpgsqlConnectionStringBuilder();

                connectionString.Database = PostGresDatabaseName;
                connectionString.Host = PostGresDatabaseAddress;
                connectionString.Port = PostGresDatabasePort;

                if (UseIntegratedSecurity)
                {
                    connectionString.IntegratedSecurity = true;
                    connectionString.PersistSecurityInfo = true;
                }
                else
                {
                    connectionString.Username = PostGresUser;
                    connectionString.Password = PostGresPassword;
                }

                return connectionString.ConnectionString;
            }
        }
    }
}
