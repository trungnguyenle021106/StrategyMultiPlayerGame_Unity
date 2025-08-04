using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManagerV2 : MonoBehaviourPunCallbacks
{
    #region private Seriazlie Fields
    [SerializeField] private RoomUIV2 roomUIV2;
    #endregion

    #region public Fields
    #endregion

    #region public Static Fields
    #endregion

    #region private Fields
    private IClientInfo iClientInfo;
    private InfoAccount infoAccount;
    private PhotonView otherPhotonView;
    #endregion
    // Start is called before the first frame update

    void Awake()
    {
        this.UnListenEventFBRealtime();
    }

    void Start()
    {
        this.infoAccount = GameObject.Find("InfoAccount").GetComponent<InfoAccount>();

        StartCoroutine(WaitForJoinRoomAndInstantiate());
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Pun Callbacks
    public void SetOtherPhotonView(PhotonView otherPV)
    {
        this.otherPhotonView = otherPV;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (!newMasterClient.IsLocal) return;
        PhotonNetwork.Destroy(iClientInfo.GetPlayerInfo());
        this.InstantieInfoPlayer();
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("characterName"))
            this.iClientInfo.SelectCharacter(PhotonNetwork.LocalPlayer.CustomProperties["characterName"].ToString());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        FirebaseRealtimeDB.Instance.AmIJoinRoom(newPlayer.NickName, true);
        FirebaseRealtimeDB.Instance.DenyAllInvite(newPlayer.NickName);
        if (PhotonNetwork.IsMasterClient)
        {
            this.roomUIV2.UpdateStartButton(false);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.roomUIV2.SetOnOffKickButton(false);
            this.CheckClientsAreReady();
        }
    }

    public override void OnLeftRoom()
    {
        FirebaseRealtimeDB.Instance.AmIJoinRoom(this.infoAccount.account.Name, false);
        this.UnListenEventFBRealtime();
        this.LoadLobby();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (PhotonNetwork.IsMasterClient)
        {

            if (this.CheckClientsAreReady() && this.CheckAllSelectCharacter())
            {
                this.roomUIV2.UpdateStartButton(true);
                return;
            }
            this.roomUIV2.UpdateStartButton(false);
            return;
        }
        if (targetPlayer.IsLocal && !PhotonNetwork.IsMasterClient && this.CheckAmISelectCharacter(targetPlayer))
        {
            bool isMyReadyStateChange = changedProps.ContainsKey("isReady");
            this.CheckAmIReady(isMyReadyStateChange, changedProps);
        }
    }
    #endregion

    #region public Methods
    public void KickPlayer(PhotonView photonView)
    {
        photonView.RPC("GetKicked", RpcTarget.Others);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public string GetRoomName()
    {
        return PhotonNetwork.CurrentRoom.Name;
    }
    public void CharacterSelected(string characterName)
    {
        iClientInfo.SelectCharacter(characterName);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void StartOrReadyGame()
    {
        this.iClientInfo.StartOrReadyGame();
    }

    public void SendRoomMessage(string Message)
    {
        this.iClientInfo.SendMessage(Message, this.infoAccount.account.Name);
    }

    #endregion

    #region private Methods



    private void InstantieInfoPlayer()
    {
        GameObject playerInfoInstance = PhotonNetwork.Instantiate("PlayerInfoV2", new Vector3(0, 0, 0), Quaternion.identity);
        this.SetIClientInfo(playerInfoInstance);
    }


    private void SetIClientInfo(GameObject playerInfoInstance)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.iClientInfo = playerInfoInstance.GetComponent<MasterClientInfoV2>();
            return;
        }
        this.iClientInfo = playerInfoInstance.GetComponent<ClientInfoV2>();
    }

    private IEnumerator WaitForJoinRoomAndInstantiate()
    {
        while (!PhotonNetwork.InRoom)
        {
            yield return null;
        }
        this.InstantieInfoPlayer();
    }

    private bool CheckAllSelectCharacter()
    {
        List<Player> playerList = PhotonNetwork.CurrentRoom.Players.Values.ToList<Player>();
        foreach (Player player in playerList)
        {
            if (player.CustomProperties["characterName"] == null)
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckClientsAreReady()
    {
        List<Player> playerList = PhotonNetwork.CurrentRoom.Players.Values.ToList<Player>();
        if (playerList.Count == 1)
        {
            return false;
        }

        foreach (Player player in playerList)
        {
            if (player.IsMasterClient == false &&
             (player.CustomProperties["isReady"] == null || (bool)player.CustomProperties["isReady"] == false))
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckAmISelectCharacter(Player player)
    {
        if (player.CustomProperties["characterName"] != null)
        {
            this.roomUIV2.TurnReadyButtonOnOFF(true);
            return true;
        }
        return false;
    }

    private void CheckAmIReady(bool isMyReadyStateChange, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (isMyReadyStateChange)
        {
            bool isReady = (bool)changedProps["isReady"];
            if (isReady)
            {
                this.roomUIV2.UpdateReadyButton(true);
                return;
            }
            this.roomUIV2.UpdateReadyButton(false);
            return;
        }
    }

    private void LoadLobby()
    {
        SceneManager.LoadScene("MyLobbyScene");
    }

    private void UnListenEventFBRealtime()
    {
        FirebaseRealtimeDB.Instance.UnListenStatusFriend(); // Hủy lắng nghe trạng thái
        FirebaseRealtimeDB.Instance.UnListenForNotificationChat(); // Hủy lắng nghe thông báo
    }
    #endregion
}

