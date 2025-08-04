using Firebase.Firestore;

[FirestoreData]
public class QAPairFB
{
    [FirestoreProperty]
    public string Question { get; set; }
    [FirestoreProperty]
    public string Answer { get; set; }
    [FirestoreProperty]
    public string Type { get; set; }
    [FirestoreProperty]
    public string Category { get; set; }
    [FirestoreProperty]
    public int HardPoint { get; set; }
}