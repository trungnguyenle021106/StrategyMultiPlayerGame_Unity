using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DisplayPlayerInfoV2 : MonoBehaviour, IPunInstantiateMagicCallback
{
    #region private Serialize Fields
    [SerializeField] private List<GameObject> listCharacterImg;
    [SerializeField] private Image imgBackground;
    [SerializeField] private GameObject labelCharacterSelection;
    [SerializeField] private TextMeshProUGUI textPlayerName;
    [SerializeField] private TextMeshProUGUI textPlayerDescription;
    [SerializeField] private MyColor32 myColor32;
    #endregion

    #region private Fields
    private bool isSelectCharacter;
    #endregion

    #region Pun Callbacks
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        this.isSelectCharacter = false;
        this.SetParent();
        this.SetTextPlayerName(info.Sender.NickName);
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Matching")) { this.SettingMatchingClient(); }
        else
        {
            if (info.Sender.IsMasterClient) { this.SettingMasterClient(); }
            else { this.SettingClient(); }

            if (!info.photonView.IsMine && PhotonNetwork.IsMasterClient)
            {
                RoomManagerV2 roomManagerV2 = GameObject.Find("Room Manager").GetComponent<RoomManagerV2>();
                roomManagerV2.SetOtherPhotonView(info.photonView);

                RoomUIV2 roomUIV2 = GameObject.Find("Room UI").GetComponent<RoomUIV2>();
                roomUIV2.SetOnOffKickButton(true);
                roomUIV2.GetKickButton().onClick.RemoveAllListeners();
                roomUIV2.GetKickButton().onClick.AddListener(() =>
                {
                    roomManagerV2.KickPlayer(info.photonView);
                });
            }
        }
        this.SetColorBackground(info.photonView.AmOwner);
        this.SetOnclick(info.photonView.IsMine);
    }
    #endregion

    #region public Methods
    public void SetColorTextPlayerDescription(bool isReady)
    {
        if (isReady) { this.textPlayerDescription.color = this.myColor32.GetColor32("Xanh lá"); }
        else { this.textPlayerDescription.color = this.myColor32.GetColor32("Đỏ"); }
    }

    public void SetImgCharacterPicked(string nameCharacter)
    {
        if (!this.isSelectCharacter) { this.SetOffLabelCharacterSelection(); this.isSelectCharacter = true; }
        foreach (GameObject imgCharacter in listCharacterImg)
        {
            imgCharacter.SetActive(false);
        }

        foreach (GameObject imgCharacter in listCharacterImg)
        {
            if (imgCharacter.name.Contains(nameCharacter))
            {
                imgCharacter.SetActive(true);
                return;
            }
        }
    }

    public void SetTextPlayerName(string playerName)
    {
        this.textPlayerName.text = playerName;
    }

    public void SetTexPlayerDescription(string textPlayerDescription)
    {
        this.textPlayerDescription.text = textPlayerDescription;
    }

    public void SetTexPlayerDescription(bool isReady)
    {
        if (isReady)
        {
            this.textPlayerDescription.text = "Sẵn sàng";
            return;
        }
        this.textPlayerDescription.text = "Chưa \nsẵn sàng";
    }

    #endregion

    #region private Methods

    private void SettingMatchingClient()
    {
        gameObject.AddComponent<MatchingClientInfo>();
    }

    private void SettingMasterClient()
    {
        gameObject.AddComponent<MasterClientInfoV2>();
        this.textPlayerDescription.color = this.myColor32.GetColor32("Xanh lá");
        this.SetTexPlayerDescription("Chủ phòng");
    }

    private void SettingClient()
    {
        gameObject.AddComponent<ClientInfoV2>();
    }

    private void SetParent()
    {
        GameObject parent;
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Matching"))
        {
            parent = GameObject.Find("Room Matching UI").GetComponent<RoomMatchingUI>().GetPanelPlayerInfo();
            gameObject.transform.SetParent(parent.transform, false);
            return;
        }
        parent = GameObject.Find("Room UI").GetComponent<RoomUIV2>().GetPanelPlayerInfo();
        gameObject.transform.SetParent(parent.transform, false);
    }

    private void SetColorBackground(bool isMine)
    {
        if (isMine) { this.imgBackground.color = this.myColor32.GetColor32("Cam"); }
        else { this.imgBackground.color = this.myColor32.GetColor32("Xanh dương"); }
    }

    private void SetOnclick(bool isMine)
    {
        if (isMine && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Matching"))
        {
            RoomMatchingUI roomUI = GameObject.Find("Room Matching UI").GetComponent<RoomMatchingUI>();
            gameObject.AddComponent<Button>().onClick.AddListener(roomUI.Onclick_OpenPanelCharacterSelection);
            return;
        }

        if (isMine && !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Matching"))
        {
            RoomUIV2 roomUI = GameObject.Find("Room UI").GetComponent<RoomUIV2>();
            gameObject.AddComponent<Button>().onClick.AddListener(roomUI.Onclick_OpenPanelCharacterSelection);
            return;
        }
    }

    private void SetOffLabelCharacterSelection()
    {
        this.labelCharacterSelection.SetActive(false);
    }
    #endregion
}