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
using System.Data.SqlClient;
using STEM.Sys;
using STEM.Sys.Security;
using STEM.Surge;

namespace STEM.Surge.SQLServer
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("SQLBaseInstruction")]
    public abstract class SQLBaseInstruction : Instruction, IDisposable
    {
        [Category("SQL Server")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }

        public SQLBaseInstruction()
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
                    using (SqlConnection connection = new SqlConnection(Authentication.ConnectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sql, connection);
                        command.ExecuteNonQuery();
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
        }

        protected DataSet ExecuteQuery(string sql, int timeoutAttempts)
        {
            int retry = timeoutAttempts;
            while (retry-- >= 0)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(Authentication.ConnectionString))
                    {
                        connection.Open();

                        SqlDataAdapter da = new SqlDataAdapter(sql, connection);

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
            using (SqlConnection connection = new SqlConnection(Authentication.ConnectionString))
            {
                connection.Open();
                using (SqlBulkCopy s = new SqlBulkCopy(connection))
                {
                    s.DestinationTableName = tableName;

                    foreach (var column in dt.Columns)
                        s.ColumnMappings.Add(column.ToString(), column.ToString());

                    int retry = timeoutAttempts;
                    while (retry-- >= 0)
                    {
                        try
                        {
                            s.WriteToServer(dt);
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
        }

        SqlConnection _SqlConnection = null;
        protected SqlConnection OpenConnection()
        {
            if (_SqlConnection == null)
                lock (this)
                    if (_SqlConnection == null)
                    {
                        _SqlConnection = new SqlConnection(Authentication.ConnectionString);
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