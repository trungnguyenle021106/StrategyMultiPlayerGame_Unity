using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MasterClientInfoV2 : MonoBehaviourPunCallbacks, IClientInfo
{
    #region private Serialize Fỉelds

    #endregion
    #region private Fields
    private DisplayPlayerInfoV2 displayPlayerInfoV2;
    private RoomUIV2 roomUI;
    #endregion

    private void Awake()
    {
        this.displayPlayerInfoV2 = gameObject.GetComponent<DisplayPlayerInfoV2>();
    }

    private void Start()
    {
        this.roomUI = GameObject.Find("Room UI").GetComponent<RoomUIV2>();
        if (photonView.IsMine)
        {
            this.roomUI.SetStartButtonFirstTime();
        }
    }

    #region Pun Callbacks

    #endregion

    #region Pun RPC
    [PunRPC]
    private void SendMessageRPC(string message, string Sender)
    {
        GameObject roomChat = RoomChatPooler.Instance.GetPooledChat();
        DisplayMessageRoom displayMessageRoom = roomChat.GetComponent<DisplayMessageRoom>();
        displayMessageRoom.SetTextNamePlayer(Sender);
        displayMessageRoom.SetTextContent(":" + message);
        displayMessageRoom.SetColorBackGround(photonView.IsMine);
        roomChat.SetActive(true);
        this.roomUI.SetWarnImgRoomChatButton(true);
    }

    [PunRPC]
    private void SelectCharacterRPC(string characterName)
    {
        this.displayPlayerInfoV2.SetImgCharacterPicked(characterName);
    }

    #endregion

    #region public Methods  
    public void SendMessage(string message, string Sender)
    {
        photonView.RPC("SendMessageRPC", RpcTarget.All, message, Sender);
    }

    public GameObject GetPlayerInfo()
    {
        return gameObject;
    }

    public void SelectCharacter(string characterName)
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["characterName"] = characterName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

        photonView.RPC("SelectCharacterRPC", RpcTarget.AllBuffered, characterName);
    }


    public void StartOrReadyGame()
    {
        List<Player> playerList = PhotonNetwork.CurrentRoom.Players.Values.ToList<Player>();
        foreach (Player player in playerList)
        {
            if (!player.CustomProperties.ContainsKey("characterName"))
            {
                return;
            }
            if (!player.IsMasterClient && !(bool)player.CustomProperties["isReady"])
            {
                return;
            }
        }
        this.LoadGame();
    }
    #endregion

    #region private Methods
    private void LoadGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
        PhotonNetwork.LoadLevel("Strategy Game Scene");
    }
    #endregion
}