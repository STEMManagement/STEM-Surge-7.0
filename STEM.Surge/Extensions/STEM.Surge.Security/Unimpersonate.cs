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

using System.Xml.Serialization;
using System.ComponentModel;
using STEM.Sys.Security;
using STEM.Surge;

namespace STEM.Surge.Security
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Unimpersonate")]
    public class Unimpersonate : Instruction
    {
        public string Domain { get; set; }
        public string User { get; set; }

        [XmlIgnore]
        [PasswordPropertyText(true)]
        public string Password { get; set; }
        [Browsable(false)]
        public string PasswordEncoded
        {
            get
            {
                return this.Entangle(Password);
            }

            set
            {
                Password = this.Detangle(value);
            }
        }

        public bool IsLocalUser { get; set; }

        public Unimpersonate()
            : base()
        {
            IsLocalUser = true;
            Domain = ".";
            User = "Username";
            Password = "Password";
        }

        protected override void _Rollback()
        {
            STEM.Sys.Security.Impersonation wu = new STEM.Sys.Security.Impersonation();
            string userkey = "WINDOWSUSER_" + Domain + ":" + User;
            if (InstructionSet.InstructionSetContainer.ContainsKey(userkey))
                return;

            if (!string.IsNullOrEmpty(Domain))
                wu.Impersonate(Domain, User, Password, !IsLocalUser);
            else
                wu.Impersonate(User, Password, !IsLocalUser);

            InstructionSet.InstructionSetContainer.Add(userkey, wu);
        }

        protected override bool _Run()
        {
            string userkey = "WINDOWSUSER_" + Domain + ":" + User;
            object wu;
            if (InstructionSet.InstructionSetContainer.TryGetValue(userkey, out wu))
            {
                (wu as STEM.Sys.Security.Impersonation).UnImpersonate();
                InstructionSet.InstructionSetContainer.Remove(userkey);
            }
            return true;
        }
    }
}
