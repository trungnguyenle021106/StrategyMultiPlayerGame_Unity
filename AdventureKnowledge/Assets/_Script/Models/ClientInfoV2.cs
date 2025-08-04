using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ClientInfoV2 : MonoBehaviourPunCallbacks, IClientInfo
{
    #region private Serialize Fỉelds

    #endregion

    #region private Fields
    private DisplayPlayerInfoV2 displayPlayerInfoV2;
    private bool isReady;
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
            this.isReady = false;
            this.SetReady();
            this.roomUI.SetReadyButtonFirstTime();
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

    [PunRPC]
    private void SetTextPlayerDescriptonRPC(bool isReady)
    {
        DisplayPlayerInfoV2 playerInfo = this.displayPlayerInfoV2;
        playerInfo.SetTexPlayerDescription(isReady);
        playerInfo.SetColorTextPlayerDescription(isReady);
    }

    [PunRPC]
    private void GetKicked()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region public Methods
    public GameObject GetPlayerInfo()
    {
        return gameObject;
    }
    public void StartOrReadyGame()
    {
        this.Ready();
    }

    public void SelectCharacter(string characterName)
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["characterName"] = characterName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

        photonView.RPC("SelectCharacterRPC", RpcTarget.AllBuffered, characterName);
    }

    public void SendMessage(string message, string Sender)
    {
        photonView.RPC("SendMessageRPC", RpcTarget.All, message, Sender);
    }
    #endregion

    #region private Methods
    private void SetTextPlayerDescripton(bool isReady)
    {
        photonView.RPC("SetTextPlayerDescriptonRPC", RpcTarget.AllBuffered, isReady);
    }
    private void Ready()
    {
        if (this.isReady)
        {
            this.isReady = false;
        }
        else { this.isReady = true; }

        this.SetReady();
        this.SetTextPlayerDescripton(this.isReady);
    }

    private void SetReady()
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["isReady"] = this.isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }
    #endregion
}