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
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MySql.Data.MySqlClient;

namespace STEM.Surge.MySQL
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("SQLController")]
    [Description("Customize an InstructionSet Template using the column names from a query as implicit [PlaceHolders]. " +
        "(e.g. if 'Sql to be executed' is 'SELECT ID, StreetAddress, PhoneNumber, Email FROM People' and 'Row Key Column Index (0-Based)' is set to '0' then " +
        "for each ID an InstructionSet Template will be customized with possible placeholder replacement for [ID], [StreetAddress], [PhoneNumber], and [Email])")]
    public class MySqlController : STEM.Surge.DeploymentController
    {
        [Category("MySQL Server")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public new Authentication Authentication { get; set; }

        [Category("MySQL Controller")]
        [DisplayName("Sql to be executed"), DescriptionAttribute("This is the Sql that will feed the assignment chain.")]
        public List<string> Sql { get; set; }

        [Category("MySQL Controller")]
        [DisplayName("Row Key Column Index (0-Based)"), DescriptionAttribute("Query result column index to use as the row key.")]
        public int KeyColumn { get; set; }

        [Category("MySQL Controller")]
        [DisplayName("Attempt assignment of every result row before requery"), DescriptionAttribute("")]
        public bool OnlyRequeryWhenResultListHasBeenCompletelyProcessed { get; set; }

        public MySqlController()
        {
            Authentication = new Authentication();
            Sql = new List<string>();
            KeyColumn = 0;
            OnlyRequeryWhenResultListHasBeenCompletelyProcessed = true;
            PreprocessPerformsDiscovery = true;
        }

        Dictionary<string, Dictionary<string, string>> _QueryResults = new Dictionary<string, Dictionary<string, string>>();

        public override List<string> ListPreprocess(IReadOnlyList<string> list)
        {
            List<string> returnList = new List<string>();

            try
            {
                if (_QueryResults.Count > 0 && OnlyRequeryWhenResultListHasBeenCompletelyProcessed)
                {
                    returnList.AddRange(_QueryResults.Keys);
                    return returnList;
                }
            }
            catch { }

            _QueryResults.Clear();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Authentication.ConnectionString))
                {
                    MySqlCommand command = new MySqlCommand(String.Join("\r\n", Sql), connection);

                    connection.Open();

                    MySqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            string key = null;
                            Dictionary<string, string> v = new Dictionary<string, string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string value = reader[i].ToString();

                                if (i == KeyColumn)
                                {
                                    key = value;
                                }

                                v["[" + reader.GetName(i).Replace("[", "").Replace("]", "") + "]"] = value;
                            }

                            _QueryResults[key] = v;
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }

                returnList.AddRange(_QueryResults.Keys);

                PollError = "";
            }
            catch (Exception ex)
            {
                PollError = "(" + STEM.Sys.IO.Net.MachineIP() + ") encountered an error querying: " + ex.Message;
                STEM.Sys.EventLog.WriteEntry("MySQLController.ListPreprocess", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return returnList;
        }
        
        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            try
            {
                Dictionary<string, string> v = _QueryResults[initiationSource];

                if (v != null)
                {
                    _QueryResults.Remove(initiationSource);

                    System.Collections.Generic.Dictionary<string, string> kvp = TemplateKVP.ToDictionary(i => i.Key, i => i.Value);

                    foreach (string k in v.Keys)
                        kvp[k] = v[k];

                    DeploymentDetails ret = base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);

                    CustomizeInstructionSet(ret.ISet, kvp, ret.BranchIP, initiationSource, false);

                    return ret;
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("MySQLController.GenerateDeploymentDetails", new Exception(initiationSource, ex).ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return null;
        }
    }
}
