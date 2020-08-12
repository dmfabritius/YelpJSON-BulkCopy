using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace YelpJSON {

    class Program {

        static readonly public SqlConnection sqlConnection =
            new SqlConnection(ConfigurationManager.ConnectionStrings["YelpDB"].ConnectionString);

        static void Main() {
            sqlConnection.Open();

            YelpUser.AddUsers();
            YelpBusiness.AddBusinesses();
            YelpTip.AddTips();
            YelpCheckin.AddCheckins();

            sqlConnection.Dispose();

            Console.WriteLine($"{DateTime.Now} : Import complete.");
            Console.ReadLine();
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
