using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Npgsql;

namespace YelpJSON {

    enum DatabaseEngines { MSSQL, Postgres }
    enum Identity : int { } // work-around for Postgres so it can detect serial/identity columns

    class Table<T> : DataTable {

        static readonly DatabaseEngines engine = DatabaseEngines.MSSQL;
        //static readonly DatabaseEngines engine = DatabaseEngines.Postgres;
        static bool connected = false;
        static SqlConnection sqlConnection;
        static NpgsqlConnection pgConnection;
        static readonly Dictionary<Type, NpgsqlTypes.NpgsqlDbType> types =
            new Dictionary<Type, NpgsqlTypes.NpgsqlDbType>() {
                { typeof(int), NpgsqlTypes.NpgsqlDbType.Integer },
                { typeof(float), NpgsqlTypes.NpgsqlDbType.Real },
                { typeof(string), NpgsqlTypes.NpgsqlDbType.Varchar },
                { typeof(DateTime), NpgsqlTypes.NpgsqlDbType.Timestamp }
            };

        public Table() : base() {
            Connect();
            foreach (FieldInfo info in typeof(T).GetFields()) {
                Columns.Add(new DataColumn(info.Name, info.FieldType));
            }
        }

        void Connect() {
            if (!connected) {
                connected = true;
                if (engine == DatabaseEngines.MSSQL) {
                    sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["sqlYelpDB"].ConnectionString);
                    sqlConnection.Open();
                }
                else {
                    pgConnection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["pgYelpDB"].ConnectionString);
                    pgConnection.Open();
                }
            }
        }

        public void AddRow(T item) {
            DataRow row = NewRow();
            foreach (FieldInfo info in item.GetType().GetFields()) {
                row[info.Name] = info.GetValue(item);
            }
            Rows.Add(row);
        }

        public void Fill(string sql) {
            if (engine == DatabaseEngines.MSSQL)
                FillSql(sql);
            else
                FillPostgres(sql);
        }

        public void FillSql(string sql) {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, sqlConnection))
                da.Fill(this);
        }

        public void FillPostgres(string sql) {
            using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, pgConnection))
                da.Fill(this);
        }

        public void WriteTable(string tableName) {
            if (engine == DatabaseEngines.MSSQL)
                WriteTableSql(tableName);
            else
                WriteTablePostgres(tableName);
        }

        public void WriteTableSql(string tableName) {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection)) {
                bulkCopy.DestinationTableName = tableName;
                foreach (DataColumn col in Columns) {
                    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                bulkCopy.WriteToServer(this);
            }
        }

        public void WriteTablePostgres(string tableName) {
            string cols = "";
            foreach (DataColumn col in Columns)
                if (col.DataType != typeof(Identity))
                    cols += $"{col.ColumnName.ToLower()},";
            string sql = $"COPY {tableName.ToLower()} ({cols.TrimEnd(',')}) FROM STDIN (FORMAT BINARY)";

            using (var bulkCopy = pgConnection.BeginBinaryImport(sql)) {
                foreach (DataRow row in Rows) {
                    bulkCopy.StartRow();
                    foreach (DataColumn col in Columns) {
                        if (col.DataType != typeof(Identity))
                            if (row.IsNull(col))
                                bulkCopy.WriteNull();
                            else
                                bulkCopy.Write(row[col], types[col.DataType]);
                    }
                }
                bulkCopy.Complete();
            }
        }
    }
}
