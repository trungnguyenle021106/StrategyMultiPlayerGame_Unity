using Firebase.Firestore;

[FirestoreData]
public class QAPair
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
