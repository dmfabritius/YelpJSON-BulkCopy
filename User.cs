using System;
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

        class UserFriend {
            public string UserID;
            public string FriendID;
        }

        class YelpFriends {
            public string user_id;
            public string[] friends;
        }

        static public void Parse() {
            Table<User> users = new Table<User>();
            Table<UserFriend> friends = new Table<UserFriend>();

            Console.WriteLine($"{DateTime.Now} : Parsing user json");
            using (StreamReader infile = new StreamReader("yelp_user.json")) {
                string json;
                while ((json = infile.ReadLine()) != null) {
                    users.AddRow(JsonConvert.DeserializeObject<User>(json));
                    var user = JsonConvert.DeserializeObject<YelpFriends>(json);
                    foreach (var friend_id in user.friends) {
                        friends.Rows.Add(new object[] {
                            user.user_id,
                            friend_id
                        });
                    }
                }
            }
            Console.WriteLine($"{DateTime.Now} : Writing {users.Rows.Count,0:n0} user records");
            users.WriteTable("Users");
            Console.WriteLine($"{DateTime.Now} : Writing {friends.Rows.Count,0:n0} friends records");
            friends.WriteTable("UserFriends");
        }
    }
}
