using System;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace YelpJSON {

    class User {
        [JsonProperty("user_id")] public string UserID;
        [JsonProperty("name")] public string Name;
        [JsonProperty("yelping_since")] public DateTime YelpingSince;
        [JsonProperty("average_stars")] public float AverageStars;
        [JsonProperty("fans")] public int Fans;
        [JsonProperty("cool")] public int Cool;
        [JsonProperty("funny")] public int Funny;
        [JsonProperty("useful")] public int Useful;
        [JsonProperty("tipcount")] public int TipCount;
    }

    class UserFriend {
        public string UserID;
        public string FriendID;
    }

    class YelpFriends {
        public string user_id;
        public string[] friends;
    }

    class UserParser {

        static public void AddUsers() {
            string json;
            DataTable users = Program.CreateTable<User>();
            DataTable friends = Program.CreateTable<UserFriend>();

            Console.WriteLine($"{DateTime.Now} : Parsing user json");
            using (StreamReader infile = new StreamReader("yelp_user.json")) {
                while ((json = infile.ReadLine()) != null) {
                    Program.AddRow(users, JsonConvert.DeserializeObject<User>(json));
                    var yelpFriends = JsonConvert.DeserializeObject<YelpFriends>(json);
                    foreach (var friendID in yelpFriends.friends) {
                        friends.Rows.Add(new object[] {
                            yelpFriends.user_id,
                            friendID
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
