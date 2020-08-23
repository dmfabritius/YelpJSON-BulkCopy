using System;
using System.IO;
using Newtonsoft.Json;

namespace YelpJSON {

    class Checkin {
        public string BusinessID;
        public DateTime CheckinDate;
    }

    class YelpCheckin {
        public string business_id;
        public string date; // single string with multiple comma-separated dates
    }

    class CheckinParser {

        static public void AddCheckins() {
            string json;
            Table<Checkin> checkins = new Table<Checkin>();

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
            checkins.WriteTable("Checkins");
        }
    }
}
