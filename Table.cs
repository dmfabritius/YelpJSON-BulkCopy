using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Npgsql;

namespace YelpJSON {

    class Table<T> : DataTable {

        public Table() : base() {
            foreach (FieldInfo info in typeof(T).GetFields()) {
                Columns.Add(new DataColumn(info.Name, info.FieldType));
            }
        }

        public void AddRow(T item) {
            DataRow row = NewRow();
            foreach (FieldInfo info in item.GetType().GetFields()) {
                row[info.Name] = info.GetValue(item);
            }
            Rows.Add(row);
        }

        public void WriteTable(string tableName) {
            WriteTableSql(tableName);
            //WriteTablePostgres(tableName);
        }

        public void WriteTableSql(string tableName) {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(Program.sqlConnection)) {
                bulkCopy.DestinationTableName = tableName;
                foreach (DataColumn col in Columns) {
                    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                try {
                    bulkCopy.WriteToServer(this);
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error bulk copying {tableName}: {ex.Message}");
                    while (Console.KeyAvailable) Console.ReadKey(true);
                    Console.Write("Press any key to terminate program.");
                    Console.ReadKey();
                }
            }
        }

        static readonly Dictionary<Type, NpgsqlTypes.NpgsqlDbType> types =
            new Dictionary<Type, NpgsqlTypes.NpgsqlDbType>() {
                { typeof(int), NpgsqlTypes.NpgsqlDbType.Integer },
                { typeof(float), NpgsqlTypes.NpgsqlDbType.Real },
                { typeof(string), NpgsqlTypes.NpgsqlDbType.Varchar },
                { typeof(DateTime), NpgsqlTypes.NpgsqlDbType.Timestamp }
            };

        public void WriteTablePostgres(string tableName) {
            string cols = "";
            foreach (DataColumn col in Columns) cols += $"{col.ColumnName.ToLower()},";
            string sql = $"COPY {tableName.ToLower()} ({cols.TrimEnd(',')}) FROM STDIN (FORMAT BINARY)";

            using (var bulkCopy = Program.pgConnection.BeginBinaryImport(sql)) {
                foreach (DataRow row in Rows) {
                    bulkCopy.StartRow();
                    foreach (DataColumn col in Columns) {
                        Type type = col.DataType;
                        if (row.IsNull(col) || (col.DataType == typeof(string) && string.IsNullOrEmpty(row.Field<string>(col)))) {
                            bulkCopy.WriteNull();
                        }
                        else {
                            bulkCopy.Write(row[col], types[col.DataType]);
                        }
                    }
                }
                bulkCopy.Complete();
            }
        }
    }
}
