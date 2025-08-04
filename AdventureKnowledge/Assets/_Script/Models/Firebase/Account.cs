using System.Collections.Generic;
using Firebase.Firestore;
using UnityEngine.SocialPlatforms;
public class Account
{
    public string id { get; set; }
    public string Name { get; set; }
    public int Energy { get; set; }
    public int Coins { get; set; }
    public string Characters { get; set; }
    public Timestamp LastLoginTime { get; set; }
    public int ADViewTime { get; set; }
    public int RankPoint { get; set; }
    public List<IUserProfile> friends;
}
