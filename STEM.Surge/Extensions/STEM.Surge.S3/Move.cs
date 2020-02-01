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
using System.ComponentModel;

namespace STEM.Surge.S3
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Move")]
    [Description("Move a file or folder from 'Source Path' to 'Destination Path' where either can be expandable paths if it's not the S3 target.")]
    public class Move : CopyBase
    {
        public Move()
            : base()
        {
            Action = ActionType.Move;
        }
    }
}