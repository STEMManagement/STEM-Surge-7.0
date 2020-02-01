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
using STEM.Sys.Threading;
using STEM.Sys.State;

namespace STEM.Sys
{
    /// <summary>
    /// Globally available convenience objects
    /// </summary>
    public static class Global
    {
        static Global()
        {
            ThreadPool = new ThreadPool(Int32.MaxValue, true);
            Session = new Session();
            Cache = new Cache();
        }
        
        /// <summary>
        /// Session
        /// </summary>
        static public Session Session { get; private set; }
        /// <summary>
        /// Cache
        /// </summary>
        static public Cache Cache { get; private set; }

        /// <summary>
        /// Shared pool
        /// </summary>
        public static ThreadPool ThreadPool { get; private set; }
    }
}
