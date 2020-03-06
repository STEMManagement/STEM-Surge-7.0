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
using STEM.Sys.Security;
using MySql.Data;

namespace STEM.Surge.MySQL
{
    public class Authentication : IAuthentication
    {
        [Category("MySQL Connection String")]
        [DisplayName("MySQL Server Connection String"), DescriptionAttribute("What is the database Connection String (Leave out Password)?")]
        public string SqlConnectionString { get; set; }

        [Category("MySQL Server")]
        [DisplayName("MySQL Server Password"), DescriptionAttribute("What is the database password (if not using Integrated Security)?")]
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
            SqlConnectionString = "Server=localhost;Database=MySQL80;Uid=root;sqlservermode=True;Pooling=True;maximumpoolsize=5;default command timeout=20;";
            SqlPassword = "";
        }

        [XmlIgnore]
        [Browsable(false)]
        internal string ConnectionString
        {
            get
            {
                if (!String.IsNullOrEmpty(SqlPassword))
                {
                    return "Pwd=" + SqlPassword + "; " + SqlConnectionString;
                }
                else
                {
                    return SqlConnectionString;
                }
            }
        }
    }
}
