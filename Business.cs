using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YelpJSON {

    class Business {
        [JsonProperty("business_id")] public string BusinessID;
        [JsonProperty("name")] public string Name;
        [JsonProperty("address")] public string Address;
        [JsonProperty("city")] public string City;
        [JsonProperty("state")] public string State;
        [JsonProperty("postal_code")] public string PostalCode;
        [JsonProperty("latitude")] public float Latitude;
        [JsonProperty("longitude")] public float Longitude;
        [JsonProperty("stars")] public float Stars;
        [JsonProperty("review_count")] public int ReviewCount;
        [JsonProperty("is_open")] public int IsOpen;
    }

    class YelpBusinessAttributes {
        public JObject attributes;  // dynamic json object heirarchy
    }

    class YelpBusinessCategories {
        public string categories; // string with multiple comma-separated categories
    }

    class YelpAttributes {
        [JsonExtensionData]
        public Dictionary<string, JToken> data;
    }

    class BizAttribute {
        public string business_id;
        public string attribute;
        public string value;
    }

    class BizCategory {
        public string business_id;
        public string category;
    }

    class BusinessParser {
        static Business business;
        readonly static HashSet<string> attHash = new HashSet<string>();
        readonly static HashSet<string> catHash = new HashSet<string>();
        readonly static List<BizAttribute> bizatts = new List<BizAttribute>();
        readonly static List<BizCategory> bizcats = new List<BizCategory>();

        static public void AddBusinesses() {
            string json;
            DataTable businesses = Program.CreateTable<Business>();

            Console.WriteLine($"{DateTime.Now} : Parsing business json");
            using (StreamReader infile = new StreamReader("yelp_business.json")) {
                while ((json = infile.ReadLine()) != null) {
                    business = JsonConvert.DeserializeObject<Business>(json);
                    Program.AddRow(businesses, business);
                    ParseAttributes("", JsonConvert.DeserializeObject<YelpBusinessAttributes>(json).attributes.ToString());
                    ParseCategories(JsonConvert.DeserializeObject<YelpBusinessCategories>(json).categories);
                }
            }
            Console.WriteLine($"{DateTime.Now} : Writing {businesses.Rows.Count,0:n0} business records");
            Program.WriteTable(businesses, "Businesses");

            AddAttributes();
            AddCategories();
            AddBusinessAttributes();
            AddBusinessCategories();
        }

        static void ParseAttributes(string root, string json) {
            if (json == "{}") return;
            if (root != "") root += ".";
            var attributes = JsonConvert.DeserializeObject<YelpAttributes>(json).data;
            foreach (var a in attributes) {
                string attribute = root + a.Key;
                if (a.Value.Type == JTokenType.Object) {
                    ParseAttributes(attribute, a.Value.ToString());
                }
                else {
                    attHash.Add(attribute);
                    bizatts.Add(new BizAttribute {
                        business_id = business.BusinessID,
                        attribute = attribute,
                        value = a.Value.ToString()
                    });
                }
            }
        }

        static void ParseCategories(string categories) {
            var cats = categories.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var cat in cats) {
                catHash.Add(cat.Trim());
                bizcats.Add(new BizCategory {
                    business_id = business.BusinessID,
                    category = cat.Trim()
                });
            }
        }

        static void AddCategories() {
            DataTable categories = new DataTable();
            categories.Columns.Add("Name", typeof(string));
            foreach (var cat in catHash) categories.Rows.Add(new object[] { cat });
            Console.WriteLine($"{DateTime.Now} : Writing {categories.Rows.Count,0:n0} category records");
            Program.WriteTable(categories, "Categories");
        }

        static void AddAttributes() {
            DataTable attributes = new DataTable();
            attributes.Columns.Add("Name", typeof(string));
            foreach (var att in attHash) attributes.Rows.Add(new object[] { att });
            Console.WriteLine($"{DateTime.Now} : Writing {attributes.Rows.Count,0:n0} attribute records");
            Program.WriteTable(attributes, "Attributes");
        }

        static void AddBusinessCategories() {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Categories", Program.sqlConnection);
            DataTable categories = new DataTable();
            da.Fill(categories);
            var catdict = categories.AsEnumerable().ToDictionary(
                    row => row["Name"].ToString(),
                    row => Int32.Parse(row["CategoryID"].ToString())
                );

            DataTable businessCategories = new DataTable();
            businessCategories.Columns.Add("BusinessID", typeof(string));
            businessCategories.Columns.Add("CategoryID", typeof(int));

            foreach (var bizcat in bizcats) {
                businessCategories.Rows.Add(new object[] {
                    bizcat.business_id,
                    catdict[bizcat.category]
                });
            }
            Console.WriteLine($"{DateTime.Now} : Writing {businessCategories.Rows.Count,0:n0} business category records");
            Program.WriteTable(businessCategories, "BusinessCategories");
        }

        static void AddBusinessAttributes() {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Attributes", Program.sqlConnection);
            DataTable attributes = new DataTable();
            da.Fill(attributes);
            var attdict = attributes.AsEnumerable().ToDictionary(
                    row => row["Name"].ToString(),
                    row => Int32.Parse(row["AttributeID"].ToString())
                );

            DataTable bizattTable = new DataTable();
            bizattTable.Columns.Add("BusinessID", typeof(string));
            bizattTable.Columns.Add("AttributeID", typeof(int));
            bizattTable.Columns.Add("Value", typeof(string));

            foreach (var bizatt in bizatts) {
                bizattTable.Rows.Add(new object[] {
                    bizatt.business_id,
                    attdict[bizatt.attribute],
                    bizatt.value
                });
            }
            Console.WriteLine($"{DateTime.Now} : Writing {bizattTable.Rows.Count,0:n0} business attribute records");
            Program.WriteTable(bizattTable, "BusinessAttributes");
        }
    }
}
