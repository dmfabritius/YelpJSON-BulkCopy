using System;
using System.Data;
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
        static public void AddTips() {
            string json;
            DataTable tips = Program.CreateTable<Tip>();

            Console.WriteLine($"{DateTime.Now} : Parsing tip json");
            using (StreamReader infile = new StreamReader("yelp_tip.json")) {
                while ((json = infile.ReadLine()) != null) {
                    Program.AddRow(tips, JsonConvert.DeserializeObject<Tip>(json));
                }
            }
            Console.WriteLine($"{DateTime.Now} : Writing {tips.Rows.Count,0:n0} tip records");
            Program.WriteTable(tips, "Tips");
        }
    }
}
