using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YelpJSON {

    class YelpBusiness {

        public string business_id;
        public string name;
        public string address;
        public string city;
        public string state;
        public string postal_code;
        public float latitude;
        public float longitude;
        public float stars;
        public int review_count;
        public int is_open;
        public string categories;
        public JObject attributes;

        static YelpBusiness business;
        readonly static HashSet<string> catHash = new HashSet<string>();
        readonly static HashSet<string> attHash = new HashSet<string>();
        readonly static List<BizCategory> bizcats = new List<BizCategory>();
        readonly static List<BizAttribute> bizatts = new List<BizAttribute>();

        readonly static DataTable bizTable = new DataTable();
        readonly static DataTable catTable = new DataTable();
        readonly static DataTable attTable = new DataTable();
        readonly static DataTable bizcatTable = new DataTable();
        readonly static DataTable bizattTable = new DataTable();

        static public void AddBusinesses() {
            string json;

            DefineTables();
            Console.WriteLine($"{DateTime.Now} : Parsing business json");
            using (StreamReader infile = new StreamReader("yelp_business.json")) {
                while ((json = infile.ReadLine()) != null) AddBusiness(json);
            }
            Console.WriteLine($"{DateTime.Now} : Writing {bizTable.Rows.Count,0:n0} business records");
            Program.WriteTable(bizTable, "Businesses");

            AddCategories();
            AddAttributes();
            AddBusinessCategories();
            AddBusinessAttributes();
        }

        static void DefineTables() {
            bizTable.Columns.Add("BusinessID", typeof(string));
            bizTable.Columns.Add("Name", typeof(string));
            bizTable.Columns.Add("Address", typeof(string));
            bizTable.Columns.Add("City", typeof(string));
            bizTable.Columns.Add("State", typeof(string));
            bizTable.Columns.Add("PostalCode", typeof(string));
            bizTable.Columns.Add("Latitude", typeof(float));
            bizTable.Columns.Add("Longitude", typeof(float));
            bizTable.Columns.Add("Stars", typeof(float));
            bizTable.Columns.Add("ReviewCount", typeof(int));
            bizTable.Columns.Add("IsOpen", typeof(int));

            //catTable.Columns.Add("CategoryID", typeof(int)); // identity column
            catTable.Columns.Add("Name", typeof(string));

            //attTable.Columns.Add("AttributeID", typeof(int)); // identity column
            attTable.Columns.Add("Name", typeof(string));

            bizcatTable.Columns.Add("BusinessID", typeof(string));
            bizcatTable.Columns.Add("CategoryID", typeof(int));

            bizattTable.Columns.Add("BusinessID", typeof(string));
            bizattTable.Columns.Add("AttributeID", typeof(int));
            bizattTable.Columns.Add("Value", typeof(string));
        }

        static void AddBusiness(string json) {
            business = JsonConvert.DeserializeObject<YelpBusiness>(json);
            bizTable.Rows.Add(new Object[] {
                business.business_id,
                business.name,
                business.address,
                business.city,
                business.state,
                business.postal_code,
                business.latitude,
                business.longitude,
                business.stars,
                business.review_count,
                business.is_open
            });
            ParseCategories();
            ParseAttributes("", business.attributes.ToString());
        }

        static void ParseCategories() {
            var cats = business.categories.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var cat in cats) {
                catHash.Add(cat.Trim());
                bizcats.Add(new BizCategory {
                    business_id = business.business_id,
                    category = cat.Trim()
                });
            }
        }

        static void ParseAttributes(string root, string json) {
            if (json == "{}") return;
            if (root != "") root += ".";
            var attribs = JsonConvert.DeserializeObject<Attrib>(json);
            foreach (var attrib in attribs.data) {
                if (attrib.Value.Type == JTokenType.Object) {
                    ParseAttributes(attrib.Key, attrib.Value.ToString());
                }
                else {
                    attHash.Add(root + attrib.Key);
                    bizatts.Add(new BizAttribute {
                        business_id = business.business_id,
                        attribute = root + attrib.Key,
                        value = attrib.Value.ToString()
                    });
                }
            }
        }

        static void AddCategories() {
            foreach (var cat in catHash) {
                catTable.Rows.Add(new Object[] { cat });
            }
            Console.WriteLine($"{DateTime.Now} : Writing {catTable.Rows.Count,0:n0} category records");
            Program.WriteTable(catTable, "Categories");
        }

        static void AddAttributes() {
            foreach (var att in attHash) {
                attTable.Rows.Add(new Object[] { att });
            }
            Console.WriteLine($"{DateTime.Now} : Writing {attTable.Rows.Count,0:n0} attribute records");
            Program.WriteTable(attTable, "Attributes");
        }

        static void AddBusinessCategories() {
            Console.WriteLine($"{DateTime.Now} : Loading category records");
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Categories", Program.sqlConnection);
            DataTable categories = new DataTable();
            da.Fill(categories);
            var catdict = categories.AsEnumerable().ToDictionary(
                    row => row["Name"].ToString(),
                    row => Int32.Parse(row["CategoryID"].ToString())
                );

            foreach (var bizcat in bizcats) {
                bizcatTable.Rows.Add(new Object[] {
                    bizcat.business_id,
                    catdict[bizcat.category]
                });
            }
            Console.WriteLine($"{DateTime.Now} : Writing {bizcatTable.Rows.Count,0:n0} business category records");
            Program.WriteTable(bizcatTable, "BusinessCategories");
        }

        static void AddBusinessAttributes() {
            Console.WriteLine($"{DateTime.Now} : Loading attribute records");
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Attributes", Program.sqlConnection);
            DataTable attributes = new DataTable();
            da.Fill(attributes);
            var attdict = attributes.AsEnumerable().ToDictionary(
                    row => row["Name"].ToString(),
                    row => Int32.Parse(row["AttributeID"].ToString())
                );

            foreach (var bizatt in bizatts) {
                bizattTable.Rows.Add(new Object[] {
                    bizatt.business_id,
                    attdict[bizatt.attribute],
                    bizatt.value
                });
            }
            Console.WriteLine($"{DateTime.Now} : Writing {bizattTable.Rows.Count,0:n0} business attribute records");
            Program.WriteTable(bizattTable, "BusinessAttributes");
        }
    }

    class Attrib {
        [JsonExtensionData]
        public Dictionary<string, JToken> data;
    }

    class BizCategory {
        public string business_id;
        public string category;
    }

    class BizAttribute {
        public string business_id;
        public string attribute;
        public string value;
    }
}
