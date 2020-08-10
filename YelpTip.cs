using System;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace YelpJSON {

    class YelpTip {

        public string business_id;
        public string user_id;
        public DateTime date;
        public int likes;
        public string text;

        static public void AddTips() {
            string json;

            DataTable tips = new DataTable();
            tips.Columns.Add("BusinessID", typeof(string));
            tips.Columns.Add("UserID", typeof(string));
            tips.Columns.Add("TipDate", typeof(DateTime));
            tips.Columns.Add("Likes", typeof(int));
            tips.Columns.Add("Text", typeof(string));

            Console.WriteLine($"{DateTime.Now} : Parsing tip json");
            using (StreamReader infile = new StreamReader("yelp_tip.json")) {
                while ((json = infile.ReadLine()) != null) {
                    var tip = JsonConvert.DeserializeObject<YelpTip>(json);
                    tips.Rows.Add(new Object[] {
                        tip.business_id,
                        tip.user_id,
                        tip.date,
                        tip.likes,
                        tip.text
                    });
                }
            }
            Console.WriteLine($"{DateTime.Now} : Writing {tips.Rows.Count,0:n0} tip records");
            Program.WriteTable(tips, "Tips");
        }
    }
}
