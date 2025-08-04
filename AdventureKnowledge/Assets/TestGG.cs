using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestGG : MonoBehaviour
{
    public TMP_InputField getter;
    public TMP_InputField sender;
    public TMP_InputField message;
    void Start()
    {
        StartCoroutine(wait());
    }

    public void Onclick()
    {
        string Getter = "zet5t";
        string Sender = "testtuan";
        string Message = message.text;
        // FirebaseRealtimeDB.Instance.SendMessage(Getter, Sender, Message);
    }

    IEnumerator wait()
    {
        while (!FirebaseRealtimeDB.Instance.isInit)
        {
            yield return null;
        }
        FirebaseRealtimeDB.Instance.ListenForNotificationChat("testtuan",
                 (dic) =>
                 {
                     bool isNotHasNoti = false;
                     foreach (var kvp in dic)
                     {
                         foreach (var friendKvp in (Dictionary<string, object>)kvp.Value)
                         {
                             Debug.Log($"Key: {friendKvp.Key}, Value: {friendKvp.Value}");
                             bool noti = (bool)friendKvp.Value; if (noti) { Debug.Log(noti); isNotHasNoti = true; break; }
                         }
                     }
                     if (!isNotHasNoti) { Debug.Log(isNotHasNoti); }
                 });
    }

}
