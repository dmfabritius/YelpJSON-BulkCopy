using System;
using System.IO;
using Newtonsoft.Json;

namespace YelpJSON {

    class Tip {
        [JsonProperty("business_id")] public string BusinessID;
        [JsonProperty("user_id")] public string UserID;
        [JsonProperty("date")] public DateTime TipDate;
        [JsonProperty("likes")] public int Likes;
        [JsonProperty("text")] public string Text;
    }

    class TipParser {

        static public void Parse() {
            string json;
            Table<Tip> tips = new Table<Tip>();

            Console.WriteLine($"{DateTime.Now} : Parsing tip json");
            using (StreamReader infile = new StreamReader("yelp_tip.json")) {
                while ((json = infile.ReadLine()) != null) {
                    tips.AddRow(JsonConvert.DeserializeObject<Tip>(json));
                }
            }
            Console.WriteLine($"{DateTime.Now} : Writing {tips.Rows.Count,0:n0} tip records");
            tips.WriteTable("Tips");
        }
    }
}
