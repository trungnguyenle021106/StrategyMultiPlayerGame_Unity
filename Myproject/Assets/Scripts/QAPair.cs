using Firebase.Firestore;
using UnityEngine;

public class QAPair
{
    public string id { get; set; }
    public string Question { get; set; }
    public string Answer { get; set; }
    public string Type { get; set; }
    public string Category { get; set; }
    public int HardPoint { get; set; }
}