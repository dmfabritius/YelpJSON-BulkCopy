using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace YelpJSON {

    class YelpUser {

        public string user_id;
        public string name;
        public DateTime yelping_since;
        public float average_stars;
        public int fans;
        public int cool;
        public int funny;
        public int useful;
        public int tipcount;
        public List<string> friends;

        static public void AddUsers() {
            string json;

            DataTable users = new DataTable();
            users.Columns.Add("UserID", typeof(string));
            users.Columns.Add("Name", typeof(string));
            users.Columns.Add("YelpingSince", typeof(DateTime));
            users.Columns.Add("AverageStars", typeof(float));
            users.Columns.Add("Fans", typeof(int));
            users.Columns.Add("Cool", typeof(int));
            users.Columns.Add("Funny", typeof(int));
            users.Columns.Add("Useful", typeof(int));
            users.Columns.Add("TipCount", typeof(int));

            DataTable friends = new DataTable();
            friends.Columns.Add("UserID", typeof(string));
            friends.Columns.Add("FriendID", typeof(string));

            Console.WriteLine($"{DateTime.Now} : Parsing user json");
            using (StreamReader infile = new StreamReader("yelp_user.json")) {
                while ((json = infile.ReadLine()) != null) {
                    var user = JsonConvert.DeserializeObject<YelpUser>(json);
                    users.Rows.Add(new Object[] {
                            user.user_id,
                            user.name,
                            user.yelping_since,
                            user.average_stars,
                            user.fans,
                            user.cool,
                            user.funny,
                            user.useful,
                            user.tipcount
                        });

                    foreach (var friend in user.friends) {
                        friends.Rows.Add(new Object[] {
                            user.user_id,
                            friend
                        });
                    }
                }
            }
            Console.WriteLine($"{DateTime.Now} : Writing {users.Rows.Count,0:n0} user records");
            Program.WriteTable(users, "Users");
            Console.WriteLine($"{DateTime.Now} : Writing {friends.Rows.Count,0:n0} friends records");
            Program.WriteTable(friends, "UserFriends");
        }
    }
}
