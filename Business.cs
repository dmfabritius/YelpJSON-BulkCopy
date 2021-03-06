﻿using System;
using System.IO;
using Newtonsoft.Json;

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

        static public void Parse() {
            Table<Business> businesses = new Table<Business>();

            Console.WriteLine($"{DateTime.Now} : Parsing business json");
            using (StreamReader infile = new StreamReader("yelp_business.json")) {
                string json;
                while ((json = infile.ReadLine()) != null) {
                    businesses.AddRow(JsonConvert.DeserializeObject<Business>(json));
                    BusinessAttributes.Parse(json);
                    BusinessCategories.Parse(json);
                }
            }
            Console.WriteLine($"{DateTime.Now} : Writing {businesses.Rows.Count,0:n0} business records");
            businesses.WriteTable("Businesses");

            // after parsing all businesses to determine the full list of attributes and categories,
            // add those to the database
            BusinessAttributes.Add();
            BusinessCategories.Add();
        }
    }
}
