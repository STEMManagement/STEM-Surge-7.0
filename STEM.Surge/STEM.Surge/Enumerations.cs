﻿/*
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

namespace STEM.Surge
{
    public enum ExecuteOn { ForwardExecution, Rollback }
    public enum FailureAction { Continue, SkipRemaining, SkipNext, Rollback, SkipToLabel }
    public enum Stage { Ready, Skip, Stopped, RolledBack, Completed }
    public enum BranchState { RegisteredSpare, Online, Offline, Silent }
    public enum AgeOrigin { LastWriteTime, LastAccessTime, CreationTime }
    public enum OSType { Windows, Linux }

    public enum ContainerType
    {
        InstructionSetContainer,
        Session,
        Cache,
    }

    public enum DataType
    {
        String,
        Binary
    }

    public enum CommonPostMortemKeys
    {
        AbnormalExecution,
        DestinationIP,
        LastOperation,
        ProcessingEnd,
        ProcessingStart,
        SourceIP
    }
}
