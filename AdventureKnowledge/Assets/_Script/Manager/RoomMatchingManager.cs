using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomMatchingManager : MonoBehaviourPunCallbacks
{
    #region private Seriazlie Fields
    [SerializeField] private RoomMatchingUI roomMatchingUI;
    #endregion

    #region public Fields
    #endregion

    #region private Fields
    private MatchingClientInfo matchingClientInfo;
    private bool allAreSetting;
    private InfoAccount infoAccount;
    #endregion
    // Start is called before the first frame update

    void Awake()
    {

    }

    void Start()
    {
        this.infoAccount = GameObject.Find("InfoAccount").GetComponent<InfoAccount>();
        if (PhotonNetwork.IsMasterClient) this.allAreSetting = false;
        StartCoroutine(WaitForJoinRoomAndInstantiate());
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Pun Callbacks

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        FirebaseRealtimeDB.Instance.AmIJoinRoom(this.infoAccount.account.Name, false);
        this.LoadLobby();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("timeMatchingEnd")) { StartCoroutine(this.StartCountDown()); }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (PhotonNetwork.IsMasterClient && changedProps.ContainsKey("isReady") && !this.allAreSetting && this.CheckClientsAreSetting()) this.SetTimeEndMatching();
        if (targetPlayer.IsLocal && changedProps.ContainsKey("isReady")) { this.roomMatchingUI.UpdateReadyButton((bool)changedProps["isReady"]); }
        if (PhotonNetwork.IsMasterClient && changedProps.ContainsKey("isReady")) { this.CheckClientsAreReady(); }
        if (targetPlayer.IsLocal && changedProps.ContainsKey("characterName")) { this.roomMatchingUI.TurnReadyButtonOnOFF(true); return; }
    }
    #endregion

    #region public Methods

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CharacterSelected(string characterName)
    {
        this.matchingClientInfo.SelectCharacter(characterName);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void Ready()
    {
        this.matchingClientInfo.Ready();
    }

    #endregion

    #region private Methods
    private void InstantieInfoPlayer()
    {
        GameObject playerInfoInstance = PhotonNetwork.Instantiate("PlayerInfoV2", new Vector3(0, 0, 0), Quaternion.identity);
        this.SetMatchingClientInfo(playerInfoInstance);
    }


    private void SetMatchingClientInfo(GameObject playerInfoInstance)
    {
        this.matchingClientInfo = playerInfoInstance.GetComponent<MatchingClientInfo>();
    }

    private IEnumerator WaitForJoinRoomAndInstantiate()
    {
        while (!PhotonNetwork.InRoom)
        {
            yield return null;
        }
        this.InstantieInfoPlayer();
    }

    private bool CheckClientsAreSetting()
    {
        List<Player> playerList = PhotonNetwork.CurrentRoom.Players.Values.ToList<Player>();
        if (playerList.Count == 1)
        {
            return false;
        }

        foreach (Player player in playerList)
        {
            if (player.CustomProperties["isReady"] == null)
            {
                return false;
            }
        }
        return true;
    }

    private void SetTimeEndMatching()
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["timeMatchingEnd"] = PhotonNetwork.ServerTimestamp + 20000;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    private void CheckClientsAreReady()
    {
        List<Player> playerList = PhotonNetwork.CurrentRoom.Players.Values.ToList<Player>();
        if (playerList.Count == 1)
        {
            return;
        }

        foreach (Player player in playerList)
        {
            if (player.CustomProperties["isReady"] == null || (bool)player.CustomProperties["isReady"] == false)
            {
                return;
            }
        }
        this.LoadGame();
    }

    private void LoadLobby()
    {
        SceneManager.LoadScene("MyLobbyScene");
    }

    private void LoadGame()
    {
        PhotonNetwork.LoadLevel("Strategy Game Scene");
    }

    private IEnumerator StartCountDown()
    {
        int timeEnd = (int)PhotonNetwork.CurrentRoom.CustomProperties["timeMatchingEnd"];
        while (PhotonNetwork.ServerTimestamp <= timeEnd)
        {
            int time = timeEnd - PhotonNetwork.ServerTimestamp;
            this.roomMatchingUI.SetTextTime(time / 1000);
            yield return null;
        }
        if (PhotonNetwork.IsMasterClient) { this.LeaveRoom(); }
    }
    #endregion
}

