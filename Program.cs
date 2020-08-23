using System;
using System.Configuration;
using System.Data.SqlClient;
using Npgsql; // Nuget https://www.nuget.org/packages/Npgsql 

namespace YelpJSON {

    class Program {

        static readonly public SqlConnection sqlConnection =
            new SqlConnection(ConfigurationManager.ConnectionStrings["sqlYelpDB"].ConnectionString);

        static readonly public NpgsqlConnection pgConnection =
            new NpgsqlConnection(ConfigurationManager.ConnectionStrings["pgYelpDB"].ConnectionString);


        static void Main() {
            sqlConnection.Open();
            pgConnection.Open();

            //UserParser.AddUsers();
            BusinessParser.AddBusinesses();
            //TipParser.AddTips();
            //CheckinParser.AddCheckins();

            sqlConnection.Dispose();
            pgConnection.Dispose();

            Console.WriteLine($"{DateTime.Now} : * Import complete *");
            Console.Write("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
