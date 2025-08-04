using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MatchingClientInfo : MonoBehaviourPunCallbacks
{
    #region private Serialize Fỉelds
    #endregion

    #region private Fields
    private DisplayPlayerInfoV2 displayPlayerInfoV2;
    private bool isReady;
    #endregion

    private void Awake()
    {
        this.displayPlayerInfoV2 = gameObject.GetComponent<DisplayPlayerInfoV2>();
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            this.isReady = false;
            this.SetReady();
        }
    }

    #region Pun Callbacks

    #endregion

    #region Pun RPC

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

    #endregion

    #region public Methods
    public GameObject GetPlayerInfo()
    {
        return gameObject;
    }

    public void Ready()
    {
        if (this.isReady) { this.isReady = false; }
        else { this.isReady = true; }

        this.SetReady();
        this.SetTextPlayerDescripton(this.isReady);
    }

    public void SelectCharacter(string characterName)
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["characterName"] = characterName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

        photonView.RPC("SelectCharacterRPC", RpcTarget.AllBuffered, characterName);
    }

    #endregion

    #region private Methods
    private void SetTextPlayerDescripton(bool isReady)
    {
        photonView.RPC("SetTextPlayerDescriptonRPC", RpcTarget.AllBuffered, isReady);
    }

    private void SetReady()
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["isReady"] = this.isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }
    #endregion
}