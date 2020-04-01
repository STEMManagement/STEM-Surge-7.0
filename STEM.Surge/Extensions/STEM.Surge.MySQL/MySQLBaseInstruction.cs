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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data;
using STEM.Sys;
using MySql.Data.MySqlClient;

namespace STEM.Surge.MySQL
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("SQLBaseInstruction")]
    public abstract class MySqlBaseInstruction : Instruction, IDisposable
    {
        [Category("MySQL Server")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }

        public MySqlBaseInstruction()
        {
            Authentication = new Authentication();
        }

        protected void ExecuteNonQuery(string sql, int timeoutAttempts)
        {
            int retry = timeoutAttempts;
            while (retry-- >= 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Authentication.ConnectionString))
                    {
                        connection.Open();
                        MySqlCommand command = new MySqlCommand(sql, connection);
                        command.ExecuteNonQuery();
                    }

                    break;
                }
                catch (Exception ex)
                {
                    if (retry <= 0)
                        throw ex;

                    if (ex.ToString().ToLower().Contains("deadlocked") || ex.ToString().ToLower().Contains("timeout"))
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }
        }

        protected DataSet ExecuteQuery(string sql, int timeoutAttempts)
        {
            int retry = timeoutAttempts;
            while (retry-- >= 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(Authentication.ConnectionString))
                    {
                        connection.Open();

                        MySqlDataAdapter da = new MySqlDataAdapter(sql, connection);

                        DataSet ds = new DataSet();

                        da.Fill(ds);

                        return ds;
                    }
                }
                catch (Exception ex)
                {
                    if (retry <= 0)
                        throw ex;

                    if (ex.ToString().ToLower().Contains("deadlocked") || ex.ToString().ToLower().Contains("timeout"))
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }

            throw new Exception("Query was not executed.");
        }

        protected void ImportDataTable(DataTable dt, string tableName, int timeoutAttempts)
        {
            string tmpFile = System.IO.Path.GetTempFileName();

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                foreach (DataRow row in dt.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(i => i.GetType() == typeof(DateTime) ? ((DateTime)i).ToString("yyyy-MM-dd HH:mm:ss") : i.ToString());
                    sb.AppendLine(string.Join("\",\"", fields));
                }

                System.IO.File.WriteAllText(tmpFile, sb.ToString());

                using (MySqlConnection connection = new MySqlConnection(Authentication.ConnectionString))
                {
                    connection.Open();
                    
                    int retry = timeoutAttempts;
                    while (retry-- >= 0)
                    {
                        try
                        {
                            MySqlBulkLoader s = new MySqlBulkLoader(connection);

                            s.TableName = tableName;
                            s.FileName = tmpFile;
                            s.FieldTerminator = ",";
                            s.FieldQuotationCharacter = '"';
                            s.Local = true;
                            s.Load();

                            break;
                        }
                        catch (Exception ex)
                        {
                            if (retry <= 0)
                                throw ex;

                            if (ex.ToString().ToLower().Contains("deadlocked") || ex.ToString().ToLower().Contains("timeout"))
                            {
                                System.Threading.Thread.Sleep(1000);
                            }
                        }
                    }
                }
            }
            finally
            {
                try
                {
                    if (File.Exists(tmpFile))
                        File.Delete(tmpFile);
                }
                catch { }
            }
        }

        MySqlConnection _SqlConnection = null;
        protected MySqlConnection OpenConnection()
        {
            if (_SqlConnection == null)
                lock (this)
                    if (_SqlConnection == null)
                    {
                        _SqlConnection = new MySqlConnection(Authentication.ConnectionString);
                        _SqlConnection.Open();
                    }

            return _SqlConnection;
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);
       
            if (_SqlConnection != null)
                lock (this)
                    if (_SqlConnection != null)
                    {
                        try
                        {
                            _SqlConnection.Close();
                        }
                        catch { }

                        _SqlConnection = null;
                    }
        }
    }
}