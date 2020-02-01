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

using System.ComponentModel;
using STEM.Surge;

namespace STEM.Surge.FlowControl
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Sleep")]
    [Description("Sleep for a configurable time.")]
    public class Sleep : Instruction
    {
        [Category("Flow")]
        [DisplayName("Seconds To Sleep")]
        public int Seconds { get; set; }

        public Sleep() : base()
        {
            Seconds = 0;
        }
        
        protected override bool _Run()
        {
            int s = Seconds;
            while (s-- > 0 && !Stop)
                System.Threading.Thread.Sleep(1000);
            return true;
        }

        protected override void _Rollback()
        {
        }
    }
}
