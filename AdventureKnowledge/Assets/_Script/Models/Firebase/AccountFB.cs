using Firebase.Firestore;

[FirestoreData]
public class AccountFB
{
    [FirestoreProperty]
    public string Name { get; set; }
    [FirestoreProperty]
    public int Energy { get; set; }
    [FirestoreProperty]
    public int Coins { get; set; }
    [FirestoreProperty]
    public string Characters { get; set; }
    [FirestoreProperty]
    public int ADViewTime { get; set; }
    [FirestoreProperty]
    public Timestamp LastLoginTime { get; set; }
    [FirestoreProperty]
    public int RankPoint { get; set; }
}
