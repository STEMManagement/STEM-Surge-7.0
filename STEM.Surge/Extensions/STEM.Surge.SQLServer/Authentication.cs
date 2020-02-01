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
using System.Xml.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data.SqlClient;
using STEM.Sys.Security;

namespace STEM.Surge.SQLServer
{
    public class Authentication : IAuthentication
    {
        [Category("SQL Server")]
        [DisplayName("SQL Server Address"), DescriptionAttribute("What is the database address?")]
        public string SqlDatabaseAddress { get; set; }

        [Category("SQL Server")]
        [DisplayName("SQL Server Database Name"), DescriptionAttribute("What is the database name?")]
        public string SqlDatabaseName { get; set; }

        [Category("SQL Server")]
        [DisplayName("Use Integrated Security"), DescriptionAttribute("Should a user name and password be used or (in Windows only) should Integrated Security be used?")]
        public bool UseIntegratedSecurity { get; set; }

        [Category("SQL Server")]
        [DisplayName("SQL Server User"), DescriptionAttribute("What is the database user?")]
        public string SqlUser { get; set; }

        [Category("SQL Server")]
        [DisplayName("SQL Server Password"), DescriptionAttribute("What is the database password?")]
        [XmlIgnore]
        [PasswordPropertyText(true)]
        public string SqlPassword { get; set; }
        [Browsable(false)]
        public string SqlPasswordEncoded
        {
            get
            {
                return this.Entangle(SqlPassword);
            }

            set
            {
                SqlPassword = this.Detangle(value);
            }
        }

        public Authentication()
        {
            SqlDatabaseAddress = "localhost";
            SqlDatabaseName = "MyDatabase";
            UseIntegratedSecurity = false;
            SqlUser = "";
            SqlPassword = "";
        }

        [XmlIgnore]
        [Browsable(false)]
        internal string ConnectionString
        {
            get
            {
                SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder();

                connectionString.InitialCatalog = SqlDatabaseName;
                connectionString.DataSource = SqlDatabaseAddress;

                if (UseIntegratedSecurity)
                {
                    connectionString.IntegratedSecurity = true;
                    connectionString.PersistSecurityInfo = true;
                }
                else
                {
                    connectionString.UserID = SqlUser;
                    connectionString.Password = SqlPassword;
                }

                return connectionString.ConnectionString;
            }
        }
    }
}
