using System;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace YelpJSON {

    class YelpCheckin {

        public string business_id;
        public string date;

        static public void AddCheckins() {
            string json;

            DataTable checkins = new DataTable();
            checkins.Columns.Add("BusinessID", typeof(string));
            checkins.Columns.Add("CheckinDate", typeof(DateTime));

            Console.WriteLine($"{DateTime.Now} : Parsing checkin json");
            using (StreamReader infile = new StreamReader("yelp_checkin.json")) {
                while ((json = infile.ReadLine()) != null) {
                    var checkin = JsonConvert.DeserializeObject<YelpCheckin>(json);
                    var dates = checkin.date.Split(',');
                    foreach (var d in dates) {
                        checkins.Rows.Add(new object[] {
                            checkin.business_id,
                            DateTime.Parse(d)
                        });
                    }
                }
            }

            Console.WriteLine($"{DateTime.Now} : Writing {checkins.Rows.Count,0:n0} checkin records");
            Program.WriteTable(checkins, "Checkins");
        }
    }
}
