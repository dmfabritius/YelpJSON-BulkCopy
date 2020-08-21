using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace YelpJSON {

    class Program {

        static readonly public SqlConnection sqlConnection =
            new SqlConnection(ConfigurationManager.ConnectionStrings["YelpDB"].ConnectionString);

        static void Main() {
            sqlConnection.Open();

            UserParser.AddUsers();
            BusinessParser.AddBusinesses();
            TipParser.AddTips();
            CheckinParser.AddCheckins();

            sqlConnection.Dispose();

            Console.WriteLine($"{DateTime.Now} : Import complete.");
            Console.ReadLine();
        }

        public static DataTable CreateTable<T>() {
            DataTable table = new DataTable();
            foreach (FieldInfo info in typeof(T).GetFields()) {
                table.Columns.Add(new DataColumn(info.Name, info.FieldType));
            }
            return table;
        }

        public static void AddRow(DataTable table, object item) {
            DataRow row = table.NewRow();
            foreach (FieldInfo info in item.GetType().GetFields()) {
                row[info.Name] = info.GetValue(item);
            }
            table.Rows.Add(row);
        }

        public static void WriteTable(DataTable table, string tableName) {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection)) {
                bulkCopy.DestinationTableName = tableName;
                foreach (DataColumn col in table.Columns) {
                    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                try {
                    bulkCopy.WriteToServer(table);
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error bulk loading {tableName}: {ex.Message}");
                    while (Console.KeyAvailable) Console.ReadKey(true);
                    Console.ReadLine();
                }
            }
        }
    }
}
