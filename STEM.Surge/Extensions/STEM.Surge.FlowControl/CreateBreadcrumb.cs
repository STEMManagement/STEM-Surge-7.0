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
using System.ComponentModel;
using STEM.Surge;

namespace STEM.Surge.FlowControl
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("CreateBreadcrumb")]
    [Description("Create an empty file as a breadcrumb to trigger task assignment from a switchboard row.")]
    public class CreateBreadcrumb : Instruction
    {
        [Category("Flow")]
        [DisplayName("Filename")]
        [Description("The full path of the breadcrumb file")]
        public string FileName { get; set; }

        public CreateBreadcrumb() : base()
        {
            FileName = "[DestinationPath]\\[NewGuid].myBreadcrumb";
        }

        protected override bool _Run()
        {
            try
            {
                using (FileStream fs = File.Open(FileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                {

                }
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.Message);
                Exceptions.Add(ex);
            }

            return Exceptions.Count == 0;
        }

        protected override void _Rollback()
        {
            try
            {
                if (File.Exists(FileName))
                    File.Delete(FileName);
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.Message);
                Exceptions.Add(ex);
            }
        }
    }
}
