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
using System.Data;
using System.Linq;
using System.ComponentModel;
using STEM.Sys.Threading;

namespace STEM.Surge.PostGreSQL
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("ImportPostMortemGroup")]
    [Description("Surge uses a PostGres database for PostMortem statistics. This instruction can be configured to " +
        "import a group of PostMortem InstructionSets into the Surge PostMortem database. It is intended to be used with a GroupingController.")]
    public class ImportPostMortemGroup : ImportPostMortem
    {
        [DisplayName("File List (Property Name: FileList)"), DescriptionAttribute("List<string> Property for the GroupingController to populate.")]
        public List<string> FileList { get; set; }

        public ImportPostMortemGroup()
        {
            FileList = new List<string>();
        }

        protected override void _Rollback()
        {
        }

        List<string> _Files = new List<string>();
        DataTable _ISet = null;
        DataTable _Instructions = null;
        static STEM.Sys.Threading.ThreadPool _LoaderPool = new ThreadPool(Int32.MaxValue);

        protected override bool _Run()
        {
            try
            {
                _ISet = Build_ISetTable();
                _Instructions = Build_InstructionTable();

                Random rnd = new Random();

                List<LoadFile> loaders = new List<LoadFile>();
                foreach (string file in FileList)
                {
                    LoadFile i = new LoadFile(this, file, loaders);

                    if (loaders.Count < FileList.Count - 1)
                        lock (loaders)
                        {
                            _LoaderPool.RunOnce(i);
                            loaders.Add(i);
                        }
                }

                LoadFile me = new LoadFile(this, FileList[FileList.Count - 1], loaders);

                lock (loaders)
                    loaders.Add(me);

                me.CallExecute();

                while (loaders.Count > 0)
                    System.Threading.Thread.Sleep(10);

                if (Exceptions.Count == 0)
                {
                    ImportDataTable(_ISet, _ISet.TableName);
                    ImportDataTable(_Instructions, _Instructions.TableName);

                    foreach (string file in _Files)
                        System.IO.File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.Message);
                Exceptions.Add(ex);
            }

            return Exceptions.Count == 0;
        }

        class LoadFile : STEM.Sys.Threading.IThreadable
        {
            ImportPostMortemGroup _Ins = null;
            string _File = null;
            List<LoadFile> _Loaders = null;

            public LoadFile(ImportPostMortemGroup ins, string file, List<LoadFile> loaders)
            {
                _Ins = ins;
                _File = file;
                _Loaders = loaders;
            }

            public void CallExecute()
            {
                Execute(null);
            }

            protected override void Execute(ThreadPool owner)
            {
                _File = STEM.Sys.IO.Path.AdjustPath(_File);

                try
                {
                    if (System.IO.File.Exists(_File))
                    {
                        string xml = System.IO.File.ReadAllText(_File);

                        if (_File.EndsWith(".is", StringComparison.InvariantCultureIgnoreCase))
                            _Ins.IngestInstructionSet(xml, _Ins._ISet, _Ins._Instructions);

                        _Ins._Files.Add(_File);
                    }
                }
                catch (Exception ex)
                {
                    lock (_Ins.Exceptions)
                        _Ins.Exceptions.Add(ex);
                }

                lock (_Loaders)
                    _Loaders.Remove(this);
            }
        }
    }
}
