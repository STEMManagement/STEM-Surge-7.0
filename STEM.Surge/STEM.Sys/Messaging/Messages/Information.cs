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
using System.Linq;
using System.Xml.Serialization;

namespace STEM.Sys.Messaging
{
    public class Information : Message
    {
        public enum Tier { Information, NonCriticalError, CriticalError }

        public Tier InformationTier { get; set; }
        public string Details { get; set; }

        public Information()
        {
        }

        public Information(string details, Tier tier)
        {
            Details = details;
            InformationTier = tier;
        }

        public Information(Exception ex, Tier tier)
        {
            if (ex != null)
            {
                Details = ex.ToString();
                InformationTier = tier;
            }
        }
    }
}
