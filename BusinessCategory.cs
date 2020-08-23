using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;
using Npgsql;

namespace YelpJSON {

    class BusinessCategory {
        public string BusinessID;
        public int CategoryID;
    }

    class Category {
        // CategoryID is auto-generated
        public string Name;
    }

    class YelpBusinessCategories {
        public string business_id;
        public string categories; // string with multiple comma-separated categories
    }

    class BizCategory {
        public string business_id;
        public string category;
    }

    class BusinessCategories {
        readonly static HashSet<string> catHash = new HashSet<string>(); // list of unique categories
        readonly static List<BizCategory> bizcats = new List<BizCategory>(); // temporarily store non-normalized business categories

        public static void Parse(string json) {
            var business = JsonConvert.DeserializeObject<YelpBusinessCategories>(json);
            var cats = business.categories.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var cat in cats) {
                catHash.Add(cat.Trim());
                bizcats.Add(new BizCategory {
                    business_id = business.business_id,
                    category = cat.Trim()
                });
            }
        }

        public static void Add() {
            Table<Category> cats = new Table<Category>();
            foreach (var cat in catHash) cats.Rows.Add(new object[] { cat });
            Console.WriteLine($"{DateTime.Now} : Writing {cats.Rows.Count,0:n0} category records");
            cats.WriteTable("Categories");

            DataTable categories = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Categories", Program.sqlConnection))
                da.Fill(categories);
            //using (NpgsqlDataAdapter da = new NpgsqlDataAdapter("SELECT * FROM Categories", Program.pgConnection))
            //    da.Fill(categories);

            var catdict = categories.AsEnumerable().ToDictionary(
                    row => row["Name"].ToString(),
                    row => Int32.Parse(row["CategoryID"].ToString())
                );

            Table<BusinessCategory> businessCategories = new Table<BusinessCategory>();
            foreach (var bizcat in bizcats) {
                businessCategories.Rows.Add(new object[] {
                    bizcat.business_id,
                    catdict[bizcat.category]
                });
            }
            Console.WriteLine($"{DateTime.Now} : Writing {businessCategories.Rows.Count,0:n0} business category records");
            businessCategories.WriteTable("BusinessCategories");
        }
    }
}