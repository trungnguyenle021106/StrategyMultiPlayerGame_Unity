using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayRoomInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textRoomName;
    [SerializeField] private TextMeshProUGUI textRoomType;
    [SerializeField] private TextMeshProUGUI textMaxAndCurrentPlayers;
    [SerializeField] private Button buttonJoinRoom;
    public RoomInfo roomInfo;

    private void Awake()
    {
        roomInfo = null;
    }

    public void SetInfomation()
    {
        this.textRoomName.text = roomInfo.Name;
        this.textRoomType.text = roomInfo.CustomProperties["RoomType"].ToString();
        this.textMaxAndCurrentPlayers.text = roomInfo.MaxPlayers.ToString() + "/" + roomInfo.PlayerCount.ToString();
    }

    public Button GetJoinButton()
    {
        return this.buttonJoinRoom;
    }

}
