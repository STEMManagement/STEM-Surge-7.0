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
using System.Xml.Serialization;
using System.ComponentModel;
using System.Linq;
using System.Data;
using STEM.Sys;
using STEM.Sys.Security;
using Npgsql;
using NpgsqlTypes;

namespace STEM.Surge.PostGreSQL
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("PostGresBaseInstruction")]
    public abstract class PostGresBaseInstruction : Instruction, IDisposable
    {
        [Category("PostGres Server")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }

        static PostGresBaseInstruction()
        {
            SqlTypeToNpgsqlDbType["boolean"] = NpgsqlDbType.Boolean;
            SqlTypeToNpgsqlDbType["smallint"] = NpgsqlDbType.Smallint;
            SqlTypeToNpgsqlDbType["integer"] = NpgsqlDbType.Integer;
            SqlTypeToNpgsqlDbType["bigint"] = NpgsqlDbType.Bigint;
            SqlTypeToNpgsqlDbType["real"] = NpgsqlDbType.Real;
            SqlTypeToNpgsqlDbType["precision"] = NpgsqlDbType.Double;
            SqlTypeToNpgsqlDbType["numeric"] = NpgsqlDbType.Numeric;
            SqlTypeToNpgsqlDbType["money"] = NpgsqlDbType.Money;
            SqlTypeToNpgsqlDbType["text"] = NpgsqlDbType.Text;
            SqlTypeToNpgsqlDbType["character varying"] = NpgsqlDbType.Varchar;
            SqlTypeToNpgsqlDbType["character"] = NpgsqlDbType.Char;
            SqlTypeToNpgsqlDbType["citext"] = NpgsqlDbType.Citext;
            SqlTypeToNpgsqlDbType["json"] = NpgsqlDbType.Json;
            SqlTypeToNpgsqlDbType["jsonb"] = NpgsqlDbType.Jsonb;
            SqlTypeToNpgsqlDbType["xml"] = NpgsqlDbType.Xml;
            SqlTypeToNpgsqlDbType["point"] = NpgsqlDbType.Point;
            SqlTypeToNpgsqlDbType["lseg"] = NpgsqlDbType.LSeg;
            SqlTypeToNpgsqlDbType["path"] = NpgsqlDbType.Path;
            SqlTypeToNpgsqlDbType["polygon"] = NpgsqlDbType.Polygon;
            SqlTypeToNpgsqlDbType["line"] = NpgsqlDbType.Line;
            SqlTypeToNpgsqlDbType["circle"] = NpgsqlDbType.Circle;
            SqlTypeToNpgsqlDbType["box"] = NpgsqlDbType.Box;
            SqlTypeToNpgsqlDbType["bit"] = NpgsqlDbType.Bit;
            SqlTypeToNpgsqlDbType["bit varying"] = NpgsqlDbType.Varbit;
            SqlTypeToNpgsqlDbType["hstore"] = NpgsqlDbType.Hstore;
            SqlTypeToNpgsqlDbType["uuid"] = NpgsqlDbType.Uuid;
            SqlTypeToNpgsqlDbType["cidr"] = NpgsqlDbType.Cidr;
            SqlTypeToNpgsqlDbType["inet"] = NpgsqlDbType.Inet;
            SqlTypeToNpgsqlDbType["macaddr"] = NpgsqlDbType.MacAddr;
            SqlTypeToNpgsqlDbType["tsquery"] = NpgsqlDbType.TsQuery;
            SqlTypeToNpgsqlDbType["tsvector"] = NpgsqlDbType.TsVector;
            SqlTypeToNpgsqlDbType["date"] = NpgsqlDbType.Date;
            SqlTypeToNpgsqlDbType["interval"] = NpgsqlDbType.Interval;
            SqlTypeToNpgsqlDbType["timestamp"] = NpgsqlDbType.Timestamp;
            SqlTypeToNpgsqlDbType["timestamp with time zone"] = NpgsqlDbType.TimestampTz;
            SqlTypeToNpgsqlDbType["timestamp without time zone"] = NpgsqlDbType.Timestamp;
            SqlTypeToNpgsqlDbType["time"] = NpgsqlDbType.Time;
            SqlTypeToNpgsqlDbType["time with time zone"] = NpgsqlDbType.TimeTz;
            SqlTypeToNpgsqlDbType["time without time zone"] = NpgsqlDbType.Time;
            SqlTypeToNpgsqlDbType["bytea"] = NpgsqlDbType.Bytea;
            SqlTypeToNpgsqlDbType["oid"] = NpgsqlDbType.Oid;
            SqlTypeToNpgsqlDbType["xid"] = NpgsqlDbType.Xid;
            SqlTypeToNpgsqlDbType["cid"] = NpgsqlDbType.Cid;
            SqlTypeToNpgsqlDbType["oidvector"] = NpgsqlDbType.Oidvector;
            SqlTypeToNpgsqlDbType["name"] = NpgsqlDbType.Name;
            SqlTypeToNpgsqlDbType["(internal) char"] = NpgsqlDbType.InternalChar;
        }

        public PostGresBaseInstruction()
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
                    using (NpgsqlConnection connection = new NpgsqlConnection(Authentication.ConnectionString))
                    {
                        connection.Open();
                        NpgsqlCommand command = new NpgsqlCommand(sql, connection);
                        command.ExecuteNonQuery();
                        connection.Close();
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
                    using (NpgsqlConnection connection = new NpgsqlConnection(Authentication.ConnectionString))
                    {
                        connection.Open();

                        NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, connection);

                        DataSet ds = new DataSet();

                        da.Fill(ds);

                        connection.Close();

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

        static Dictionary<string, NpgsqlDbType> SqlTypeToNpgsqlDbType = new Dictionary<string, NpgsqlDbType>();
        
        protected void ImportDataTable(DataTable dt, string tableName)
        {
            DataSet ds = null;

            string[] tableParts = tableName.Split('.');

            if (tableParts.Count() == 2)
                ds = ExecuteQuery("SELECT column_name, data_type FROM information_schema.columns WHERE table_schema = '" + tableParts[0] + "' AND table_name = '" + tableParts[1] + "'", 3);
            else
                ds = ExecuteQuery("SELECT column_name, data_type FROM information_schema.columns WHERE table_name = '" + tableParts[0] + "'", 3);

            Dictionary<string, string> typeMap = ds.Tables[0].AsEnumerable().ToDictionary(i => i[0].ToString(), j => j[1].ToString());

            foreach (string t in typeMap.Values)
                if (!SqlTypeToNpgsqlDbType.ContainsKey(t))
                    throw new Exception("SqlType not mapped tp NpgsqlDbType: " + t);

            using (NpgsqlConnection connection = new NpgsqlConnection(Authentication.ConnectionString))
            {
                connection.Open();
                using (var writer = connection.BeginBinaryImport("copy " + tableName + " FROM STDIN (FORMAT BINARY)"))
                {
                    foreach (DataRow rw in dt.Rows)
                    {
                        writer.StartRow();

                        for (int col = 0; col < dt.Columns.Count; col++)
                        {
                            writer.Write(rw[col], SqlTypeToNpgsqlDbType[typeMap[dt.Columns[col].ColumnName]]);
                        }
                    }

                    writer.Complete();
                }
                connection.Close();
            }
        }

        NpgsqlConnection _NpgsqlConnection = null;
        protected NpgsqlConnection OpenConnection()
        {
            if (_NpgsqlConnection == null)
                lock (this)
                    if (_NpgsqlConnection == null)
                    {
                        _NpgsqlConnection = new NpgsqlConnection(Authentication.ConnectionString);
                        _NpgsqlConnection.Open();
                    }

            return _NpgsqlConnection;
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);

            if (_NpgsqlConnection != null)
                lock (this)
                    if (_NpgsqlConnection != null)
                    {
                        try
                        {
                            _NpgsqlConnection.Close();
                        }
                        catch { }

                        _NpgsqlConnection = null;
                    }
        }
    }
}