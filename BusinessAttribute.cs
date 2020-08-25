using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YelpJSON {

    class BusinessAttribute {
        public string BusinessID;
        public int AttributeID;
        public string Value;
    }

    class Attribute {
        public Identity AttributeID;
        public string Name;
    }

    class YelpBusinessAttributes {
        public string business_id;
        public JObject attributes;  // dynamic json object heirarchy
    }

    class AttributeData {
        [JsonExtensionData]
        public Dictionary<string, JToken> data;
    }

    class BizAttribute {
        public string business_id;
        public string attribute;
        public string value;
    }

    class BusinessAttributes {
        static YelpBusinessAttributes business;
        readonly static HashSet<string> attHash = new HashSet<string>(); // list of unique attributes
        readonly static List<BizAttribute> bizatts = new List<BizAttribute>(); // temporarily store non-normalized business attributes

        public static void Parse(string json) {
            business = JsonConvert.DeserializeObject<YelpBusinessAttributes>(json);
            ParseAttributes("", business.attributes.ToString());
        }

        static void ParseAttributes(string root, string json) {
            if (json == "{}") return;
            if (root != "") root += ".";
            var attributes = JsonConvert.DeserializeObject<AttributeData>(json).data;
            foreach (var a in attributes) {
                string attribute = root + a.Key;
                if (a.Value.Type == JTokenType.Object) {
                    ParseAttributes(attribute, a.Value.ToString());
                }
                else {
                    attHash.Add(attribute);
                    bizatts.Add(new BizAttribute {
                        business_id = business.business_id,
                        attribute = attribute,
                        value = a.Value.ToString()
                    });
                }
            }
        }

        public static void Add() {
            Table<Attribute> attributes = new Table<Attribute>();
            foreach (var att in attHash) attributes.Rows.Add(new object[] { 0, att });
            Console.WriteLine($"{DateTime.Now} : Writing {attributes.Rows.Count,0:n0} attribute records");
            attributes.WriteTable("Attributes"); // insert into database and populate AttributeID field

            attributes.Clear();
            attributes.Fill("SELECT * FROM Attributes");
            var attdict = attributes.AsEnumerable().ToDictionary(
                row => row["Name"].ToString(),
                row => Int32.Parse(row["AttributeID"].ToString())
            );

            Table<BusinessAttribute> businessAttributes = new Table<BusinessAttribute>();
            foreach (var bizatt in bizatts) {
                businessAttributes.Rows.Add(new object[] {
                    bizatt.business_id,
                    attdict[bizatt.attribute], // use attribute name to lookup AttributeID
                    bizatt.value
                });
            }
            Console.WriteLine($"{DateTime.Now} : Writing {businessAttributes.Rows.Count,0:n0} business attribute records");
            businessAttributes.WriteTable("BusinessAttributes");
        }
    }
}
