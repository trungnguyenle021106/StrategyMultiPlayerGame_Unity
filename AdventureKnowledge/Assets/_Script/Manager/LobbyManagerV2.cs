using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
public class LobbyManagerV2 : MonoBehaviourPunCallbacks
{
    #region private Seriazlie Fields
    [SerializeField] private LobbyUIV2 lobbyUIV2;
    #endregion
    #region private Fields
    private bool isMatchMaking;
    private InfoAccount infoAccount;
    #endregion
    void Awake()
    {
        if (!PhotonNetwork.InLobby) { this.JoinLobby(); }
        // if (PhotonNetwork.IsConnectedAndReady) { this.JoinLobby(); }
        // else
        // {
        //     PhotonNetwork.NetworkingClient.StateChanged += OnStateChanged;
        // }
    }
    private void Start()
    {
        this.isMatchMaking = false;
        this.infoAccount = GameObject.Find("InfoAccount").GetComponent<InfoAccount>();
    }

    #region Pun Callbacks
    public override void OnLeftRoom()
    {
        this.isMatchMaking = false;
        FirebaseRealtimeDB.Instance.AmIJoinRoom(this.infoAccount.account.Name, false);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        this.CreateRoomMatchMaking();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        this.lobbyUIV2.OpenCloseJoinRoomFail(true);
        this.lobbyUIV2.SetOffWrongPasswordLabel();
        this.lobbyUIV2.SetLoadingPanel(false);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        FirebaseRealtimeDB.Instance.DenyAllInvite(this.infoAccount.account.Name);
        FirebaseRealtimeDB.Instance.AmIJoinRoom(this.infoAccount.account.Name, true);
        this.UnListenEventFBRealtime();
        if (!isMatchMaking) { this.LoadRoom(); return; }
        if (isMatchMaking && !PhotonNetwork.IsMasterClient) { this.lobbyUIV2.SetLoadingPanel(true); }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo == null || roomInfo.CustomProperties.ContainsKey("Matching") || roomInfo.CustomProperties.ContainsKey("currentQuestion")) continue;
            this.lobbyUIV2.UpdateListRoom(roomInfo);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnCreateRoomFailed error :" + message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        this.LoadMatching();
    }

    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InLobby) { this.JoinLobby(); }
    }

    #endregion

    #region public Methods
    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenRank()
    {
        if (Social.localUser.authenticated)
        {
            GooglePlayGames.PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
        else
        {
            Debug.LogError("Not signed in to Google Play Games.");
        }
    }
    public void BuyCharacter(string Character, int Coins, Action<AccountFB> callback)
    {
        AccountFB accountFB = new AccountFB();
        accountFB.Characters = this.infoAccount.account.Characters + $",{Character}";
        accountFB.Energy = this.infoAccount.account.Energy;
        accountFB.Coins = this.infoAccount.account.Coins - Coins;
        accountFB.Name = this.infoAccount.account.Name;
        accountFB.ADViewTime = this.infoAccount.account.ADViewTime;
        accountFB.LastLoginTime = this.infoAccount.account.LastLoginTime;
        accountFB.RankPoint = this.infoAccount.account.RankPoint;

        FirestoreDatabase.Instance.SetNameCollection("Account");
        FirestoreDatabase.Instance.UpdateAsync<AccountFB>(this.infoAccount.account.id, accountFB, callback);
    }
    public void CancelMatching()
    {
        if (PhotonNetwork.InRoom) this.LeaveRoom();
        StopAllCoroutines();
        this.isMatchMaking = false;
    }

    public void JoinMatchingRoom()
    {
        Debug.Log("JoinMatchingRoom");

        // Kiểm tra trạng thái trước khi gọi JoinRandomRoom
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogError("Photon chưa sẵn sàng kết nối! Đợi kết nối trước khi tham gia phòng.");
            return;
        }

        if (!this.isMatchMaking)
        {
            this.isMatchMaking = true;
            StartCoroutine(this.StartCountDownFindGame());
        }
        PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable { { "Matching", true } }, 2);
    }

    public void CreateRoom(string roomName, string roomType, int maxPlayers, string password)
    {
        if (roomName.Equals(""))
        {
            this.lobbyUIV2.SetOnOffLabelRoomNameError(true);
            return;
        }

        foreach (GameObject room in RoomPooler.Instance.GetListRoom())
        {
            DisplayRoomInfo displayRoomInfo = room.GetComponent<DisplayRoomInfo>();
            if (displayRoomInfo.roomInfo != null && roomName.Equals(displayRoomInfo.roomInfo.Name))
            {
                this.lobbyUIV2.SetOnOffLabelRoomNameError(true);
                return;
            }
        }


        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["RoomType"] = roomType;
        if (password != null)
        {
            customProperties["Password"] = password;
            PhotonNetwork.CreateRoom(
            roomName,
            new RoomOptions
            {
                MaxPlayers = maxPlayers,
                CustomRoomProperties = customProperties,
                CustomRoomPropertiesForLobby = new string[] { "RoomType", "Password" },
            });
        }
        else
        {
            PhotonNetwork.CreateRoom(
           roomName,
           new RoomOptions
           {
               MaxPlayers = maxPlayers,
               CustomRoomProperties = customProperties,
               CustomRoomPropertiesForLobby = new string[] { "RoomType" },
           });
        }
    }

    public void JoinRoom(string roomName)
    {
        this.lobbyUIV2.SetLoadingPanel(true);
        PhotonNetwork.JoinRoom(roomName);
    }



    #endregion

    #region private Methods 


    private void CreateRoomMatchMaking()
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["Matching"] = true;
        PhotonNetwork.CreateRoom(
           PhotonNetwork.NickName + "room",
           new RoomOptions
           {
               MaxPlayers = 2,
               CustomRoomProperties = customProperties,
               CustomRoomPropertiesForLobby = new string[] { "Matching" },
           });
    }

    private void LoadRoom()
    {
        this.lobbyUIV2.SetLoadingPanel(true);
        PhotonNetwork.LoadLevel("MyRoomSceneV2");
    }

    private void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    private void OnStateChanged(Photon.Realtime.ClientState previousState, Photon.Realtime.ClientState state)
    {
        if (state == Photon.Realtime.ClientState.ConnectedToMasterServer)
        {
            this.JoinLobby();
            PhotonNetwork.NetworkingClient.StateChanged -= OnStateChanged;
        }
    }

    private void LoadMatching()
    {
        this.lobbyUIV2.SetLoadingPanel(true);
        PhotonNetwork.LoadLevel("MyRoomMatchingScene");
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private IEnumerator StartCountDownFindGame()
    {
        int time = 1;
        while (time <= 30)
        {
            this.lobbyUIV2.SetTextTime(time);
            yield return new WaitForSeconds(1);
            time++;
        }
        this.isMatchMaking = false;
        this.LeaveRoom();
        this.lobbyUIV2.OpenCloseMatchingPanel(false);
    }

    private void UnListenEventFBRealtime()
    {
        FirebaseRealtimeDB.Instance.UnListenForInviteRemoveChanges(); // Hủy lắng nghe kiểm tra mất node
        FirebaseRealtimeDB.Instance.UnListenForInviteValueChildChanges(); // Hủy lắng nghe lời mời
        FirebaseRealtimeDB.Instance.UnListenStatusFriend(); // Hủy lắng nghe trạng thái
        FirebaseRealtimeDB.Instance.UnListenForNotificationChat(); // Hủy lắng nghe thông báo
    }
    #endregion
}