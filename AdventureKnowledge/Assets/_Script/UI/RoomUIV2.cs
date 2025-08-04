using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using WebSocketSharp;

public class RoomUIV2 : MonoBehaviour
{
    #region private Serialize Fields
    [SerializeField] private GameObject panelPlayerInfo;
    [SerializeField] private GameObject panelCharacterSelection;
    [SerializeField] private RoomManagerV2 roomManagerV2;

    [SerializeField] private TextMeshProUGUI textStartOrReadyGame;
    [SerializeField] private Button buttonStartOrReadyButton;

    [SerializeField] private List<GameObject> listCharacters;
    [SerializeField] private Sprite redButton;
    [SerializeField] private Sprite greenButton;
    [SerializeField] private GameObject kickButtonGO;
    [SerializeField] private Button kickButton;

    [SerializeField] private GameObject friendScrollView;
    [SerializeField] private GameObject friendChatScrollView;

    [SerializeField] private Sprite offlineSprite;
    [SerializeField] private Sprite onlineSprite;
    [SerializeField] private Image imgStatusFriendChat;
    [SerializeField] private GameObject imgWarningFriend;
    [SerializeField] private GameObject imgWarningChat;
    [SerializeField] private GameObject roomChatPanel;
    [SerializeField] private ScrollRect chatFriendScrollRect;
    [SerializeField] private ScrollRect chatRoomScrollRect;
    [SerializeField] private TMP_Dropdown chatRoomDropDown;
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private TMP_InputField inputFriendSearch;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject confirmPanel;
    #endregion

    #region private Fields
    private InfoAccount infoAccount;
    private bool isSettingCharacter;
    private bool isChatFriendOpen;
    private string friendIsChatting;
    private string namePanelOpen;
    private string idChat;
    #endregion
    private void Awake()
    {
        this.infoAccount = GameObject.Find("InfoAccount").GetComponent<InfoAccount>();
    }

    void Start()
    {
        this.isSettingCharacter = false;
        this.namePanelOpen = "Friend";
        this.isChatFriendOpen = false;
        this.idChat = "";
        this.LoadListFriend();
    }

    void Update()
    {

    }

    #region public Method
    public void SetOnOffKickButton(bool isOn)
    {
        this.kickButtonGO.SetActive(isOn);
    }

    public Button GetKickButton()
    {
        return this.kickButton;
    }
    //SETTING
    public void Onclick_QuitGameButton()
    {
        this.OpenCloseSettingPanel(false);
        this.OpenCloseConfirmPanel(true);
    }

    public void Onclick_CancelConfirm()
    {
        this.OpenCloseConfirmPanel(false);
    }

    public void Onclick_AcceptConfirm()
    {
        this.roomManagerV2.QuitGame();
    }

    public void Onclick_CloseSettingPanel()
    {
        this.OpenCloseSettingPanel(false);
    }

    public void Onclick_OpenSettingPanel()
    {
        this.OpenCloseSettingPanel(true);
    }

    private void OpenCloseSettingPanel(bool isOpen)
    {
        this.settingPanel.SetActive(isOpen);
    }

    private void OpenCloseConfirmPanel(bool isOpen)
    {
        this.confirmPanel.SetActive(isOpen);
    }
    //SETTING
    public void OnValueChangeSearchFriend()
    {
        if (this.inputFriendSearch.text.Equals(""))
        {
            foreach (GameObject friend in FriendRoomPooler.Instance.GetListFriendRoom())
            {
                DisplayFriendRoom displayFriendRoom = friend.GetComponent<DisplayFriendRoom>();
                if (displayFriendRoom.GetName() != "")
                {
                    friend.SetActive(true);
                }
            }
        }
        else
        {
            foreach (GameObject friend in FriendRoomPooler.Instance.GetListFriendRoom())
            {
                DisplayFriendRoom displayFriendRoom = friend.GetComponent<DisplayFriendRoom>();
                if (displayFriendRoom.GetName() == "") continue;
                if (displayFriendRoom.GetName().ToLower().Contains(this.inputFriendSearch.text.ToLower()))
                {
                    friend.SetActive(true);
                }
                else
                {
                    friend.SetActive(false);
                }
            }
        }
    }

    public void Onclick_ChatRoomButton()
    {
        this.OpenCloseMainPanel("RoomChat");
    }

    public void SetWarnImgRoomChatButton(bool isOn)
    {
        this.chatRoomScrollRect.normalizedPosition = new Vector2(0, 0);
        if (!this.namePanelOpen.Equals("RoomChat")) this.imgWarningChat.SetActive(isOn);
    }

    public void Onclick_SendRoomMessage()
    {
        int selectedIndex = chatRoomDropDown.value;
        string selectedValue = chatRoomDropDown.options[selectedIndex].text;
        if (selectedValue.Equals("Chọn tin nhắn gửi")) { return; }
        this.roomManagerV2.SendRoomMessage(selectedValue);
    }

    public void Onclick_SendMessage()
    {
        if (!chatInput.text.Equals(""))
        {
            string Getter = this.friendIsChatting;
            string Sender = this.infoAccount.account.Name;
            string Message = chatInput.text;
            FirebaseRealtimeDB.Instance.SendMessage(Getter, Sender, Message, (response) =>
            {
                this.idChat = response.ToString();

                FirebaseRealtimeDB.Instance.ListenForChatChildChanges(this.idChat, (dic) =>
                {
                    this.LoadChat(dic);
                });

                FirebaseRealtimeDB.Instance.ChatIsRead(this.infoAccount.account.Name, Getter, idChat);
            });
            chatInput.text = "";
        }
    }

    public GameObject GetPanelPlayerInfo()
    {
        return this.panelPlayerInfo;
    }

    public void UpdateReadyButton(bool amReady)
    {
        if (amReady)
        {
            this.SetTextReadyButtonIsReady();
            this.SetReadyButtonIsReady();
            return;
        }
        this.SetTextReadyButtonIsNotReady();
        this.SetReadyButtonIsNotReady();
    }

    public void TurnReadyButtonOnOFF(bool isOn)
    {
        if (isOn)
        {
            this.buttonStartOrReadyButton.interactable = true;
            return;
        }
        this.buttonStartOrReadyButton.interactable = false;
    }

    public void UpdateStartButton(bool isAllReady)
    {
        if (isAllReady)
        {
            this.SetOnStartButton();
            return;
        }
        this.SetOffStartButton();
    }

    public void Onclick_LeaveRoom()
    {
        this.LeaveRoom();
    }

    public void Onclick_StartOrReadyGame()
    {
        this.StartOrReadyGame();
    }

    public void Onclick_OpenPanelCharacterSelection()
    {
        this.OpenPanelCharacterSelection();
        if (!this.isSettingCharacter) { this.ShowCharacterIAmOwner(); this.isSettingCharacter = true; }
    }

    public void Onclick_ClosePanelCharacterSelection()
    {
        this.ClosePanelCharacterSelection();
    }

    public void SetStartButtonFirstTime()
    {
        this.SetTextStartButton();
    }

    public void SetReadyButtonFirstTime()
    {
        this.SetTextReadyButtonIsNotReady();
    }

    public void Onclick_SelectCharacter(string characterName)
    {
        this.SelectCharacter(characterName);
        this.ClosePanelCharacterSelection();
    }

    public void SetStatusFriendList(Dictionary<string, Dictionary<string, bool>> friendStatusList)
    {
        foreach (GameObject friendRoom in FriendRoomPooler.Instance.GetListFriendRoom())
        {
            if (friendRoom.activeInHierarchy)
            {
                DisplayFriendRoom displayFriendRoom = friendRoom.GetComponent<DisplayFriendRoom>();
                foreach (KeyValuePair<string, Dictionary<string, bool>> kvp in friendStatusList)
                {
                    if (kvp.Key.Equals(displayFriendRoom.GetName()))
                    {
                        bool isInRoom = false;
                        bool isOnline = false;
                        foreach (KeyValuePair<string, bool> entry in kvp.Value)
                        {
                            if (entry.Key.Equals("isOnline"))
                            {
                                if (entry.Value)
                                {
                                    displayFriendRoom.SetStatus(this.onlineSprite);
                                    isOnline = true;
                                }
                                else if (!entry.Value)
                                {
                                    displayFriendRoom.SetStatus(this.offlineSprite);
                                }
                            }
                            else if (entry.Key.Equals("isInRoom") && entry.Value)
                            {
                                isInRoom = true;
                            }
                        }

                        if (isOnline && !isInRoom) { displayFriendRoom.SetInviteBtnFollowStatus(true); }
                        else if (isOnline && isInRoom) { displayFriendRoom.SetInviteBtnFollowStatus(false); }
                        else if (!isOnline) { displayFriendRoom.SetInviteBtnFollowStatus(false); }
                        break;
                    }
                }
            }
        }

        if (this.isChatFriendOpen)
        {
            foreach (GameObject friendRoom in FriendRoomPooler.Instance.GetListFriendRoom())
            {
                if (friendRoom.activeInHierarchy)
                {
                    DisplayFriendRoom displayFriendRoom = friendRoom.GetComponent<DisplayFriendRoom>();
                    if (displayFriendRoom.GetName().Equals(this.friendIsChatting))
                    {
                        this.imgStatusFriendChat.sprite = displayFriendRoom.GetStatus();
                        return;
                    }
                }
            }
        }

    }
    public void Onclick_FriendButton()
    {
        this.OpenCloseMainPanel("Friend");
    }
    #endregion

    #region private Methods
    private void SetNotification(Dictionary<string, object> dic)
    {
        bool hasNoti = false;
        foreach (var kvp in dic)
        {
            foreach (var friendKvp in (Dictionary<string, object>)kvp.Value)
            {
                Debug.Log($"Key: {friendKvp.Key}, Value: {friendKvp.Value}");
                bool noti = (bool)friendKvp.Value;
                if (noti)
                {
                    hasNoti = true;
                    break;
                }
            }
        }
        this.imgWarningFriend.SetActive(hasNoti);

        foreach (var kvp in dic)
        {
            foreach (var friendKvp in (Dictionary<string, object>)kvp.Value)
            {
                bool noti = (bool)friendKvp.Value;
                foreach (GameObject friend in FriendRoomPooler.Instance.GetListFriendRoom())
                {
                    DisplayFriendRoom displayFriendRoom = friend.GetComponent<DisplayFriendRoom>();
                    if (!string.IsNullOrEmpty(displayFriendRoom.GetName()) && kvp.Key.Equals(displayFriendRoom.GetName()))
                    {
                        displayFriendRoom.SetOnOffWarn(noti);
                        break;
                    }
                }
            }
        }

        if (idChat == "" && isChatFriendOpen)
        {
            foreach (var kvp in dic)
            {
                if (kvp.Key.Equals(this.friendIsChatting))
                {
                    foreach (var friendKvp in (Dictionary<string, object>)kvp.Value)
                    {
                        bool noti = (bool)friendKvp.Value;
                        if (noti)
                        {
                            this.idChat = friendKvp.Key;
                            FirebaseRealtimeDB.Instance.ListenForChatChildChanges(this.idChat, (dic) =>
                            {
                                this.LoadChat(dic);
                            });
                            FirebaseRealtimeDB.Instance.ChatIsRead(this.infoAccount.account.Name, this.friendIsChatting, idChat);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void OpenCloseMainPanel(string namePanel)
    {
        switch (namePanel)
        {
            case "Friend":
                this.OpenCloseFriendScrollView(true);
                this.OpenCloseFriendChatScrollView(false);
                this.OpenCloseChatRoom(false);
                break;
            case "FriendChat":
                Debug.Log("FriendChat" + true);
                this.OpenCloseFriendChatScrollView(true);
                this.OpenCloseChatRoom(false);
                this.OpenCloseFriendScrollView(false);
                break;
            case "RoomChat":
                this.OpenCloseChatRoom(true);
                this.OpenCloseFriendScrollView(false);
                this.OpenCloseFriendChatScrollView(false);
                break;
        }
        this.namePanelOpen = namePanel;
    }

    private void OpenCloseChatRoom(bool isOpen)
    {
        if (isOpen)
        {
            this.imgWarningChat.SetActive(false);
            this.roomChatPanel.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        }
        else
        {
            this.roomChatPanel.GetComponent<RectTransform>().localPosition = new Vector2(1200, 0);
        }
    }

    private void OpenCloseFriendScrollView(bool isOpen)
    {
        if (isOpen)
        {
            this.friendScrollView.GetComponent<RectTransform>().localPosition = new Vector2(0, -35.3f);
        }
        else
        {
            this.friendScrollView.GetComponent<RectTransform>().localPosition = new Vector2(1200, -35.3f);
        }
    }

    private void OpenCloseFriendChatScrollView(bool isOpen)
    {
        Debug.Log("OpenCloseFriendChatScrollView:" + isOpen);
        if (!isOpen)
        {
            this.isChatFriendOpen = false;
            if (this.idChat != "") { FirebaseRealtimeDB.Instance.ChatIsRead(this.infoAccount.account.Name, this.friendIsChatting, this.idChat); }
            FirebaseRealtimeDB.Instance.UnListenForChatChildChanges();
            this.idChat = "";

            foreach (GameObject chat in ChatPooler.Instance.GetListChat())
            {
                if (chat.activeInHierarchy)
                {
                    ChatPooler.Instance.ReturnChatToPooler(chat);
                }
            }
        }
        foreach (GameObject friendRoom in FriendRoomPooler.Instance.GetListFriendRoom())
        {
            if (!friendRoom.activeInHierarchy) continue;
            DisplayFriendRoom displayFriendRoom = friendRoom.GetComponent<DisplayFriendRoom>();
            if (displayFriendRoom.GetName().Equals(this.friendIsChatting))
            {
                this.imgStatusFriendChat.sprite = displayFriendRoom.GetStatus();
                break;
            }
        }
        this.friendChatScrollView.SetActive(isOpen);
    }

    private void Onclick_InviteButton(string playerName, Action callback)
    {
        FirebaseRealtimeDB.Instance.InviteFriend(playerName, this.infoAccount.account.Name,
         this.roomManagerV2.GetRoomName(), callback);
    }

    private void Onclick_ChatButton(string playerName)
    {
        this.isChatFriendOpen = true;
        this.friendIsChatting = playerName;

        this.OpenCloseMainPanel("FriendChat");

        FirebaseRealtimeDB.Instance.CheckExistChat(playerName, this.infoAccount.account.Name, (response) =>
        {
            Debug.Log("Onclick_ChatButton :" + response);
            if (!response.Equals("False"))
            {
                this.idChat = response.ToString();

                FirebaseRealtimeDB.Instance.ListenForChatChildChanges(this.idChat, (dic) =>
                {
                    this.LoadChat(dic);
                });

                FirebaseRealtimeDB.Instance.ChatIsRead(this.infoAccount.account.Name, playerName, idChat);
            }
        });
    }

    private void LoadChat(Dictionary<string, object> dic)
    {
        foreach (var kvp in dic)
        {
            foreach (var content in (Dictionary<string, object>)kvp.Value)
            {
                Debug.Log($"Key: {content.Key}, Value: {content.Value}");
                GameObject chat = ChatPooler.Instance.GetPooledChat();
                DisplayMessage displayMessage = chat.GetComponent<DisplayMessage>();
                if (content.Key.Equals(this.infoAccount.account.Name))
                {
                    displayMessage.SetTextMeOrFriend(true);
                    displayMessage.SetColorBackGround(true);
                }
                else
                {
                    displayMessage.SetTextMeOrFriend(false);
                    displayMessage.SetColorBackGround(false);
                }
                displayMessage.SetTextContent(content.Value.ToString());
                chat.SetActive(true);
            }
        }
        this.chatFriendScrollRect.normalizedPosition = new Vector2(0, 0);
    }

    private void LoadListFriend()
    {
        foreach (IUserProfile userProfile in this.infoAccount.account.friends)
        {
            GameObject friendRoom = FriendRoomPooler.Instance.GetPooledFriendRoom();
            DisplayFriendRoom displayFriendRoom = friendRoom.GetComponent<DisplayFriendRoom>();
            string playerName = userProfile.userName;
            displayFriendRoom.SetAvatar(userProfile.image);
            displayFriendRoom.SetName(userProfile.userName);
            displayFriendRoom.GetChatBtn().onClick.AddListener(() =>
            {
                this.Onclick_ChatButton(playerName);
            });
            displayFriendRoom.GetInviteBtn().onClick.AddListener(() =>
            {
                this.Onclick_InviteButton(playerName, displayFriendRoom.SetTime);
            });
            friendRoom.SetActive(true);
        }
        FirebaseRealtimeDB.Instance.ListenStatusFriend((dic) =>
        {
            this.SetStatusFriendList(dic);
        });
        FirebaseRealtimeDB.Instance.ListenForNotificationChat(this.infoAccount.account.Name, (dic) =>
       {
           this.SetNotification(dic);
       });
    }
    private void ShowCharacterIAmOwner()
    {
        string[] myCharacters = this.infoAccount.account.Characters.Split(',');
        for (int j = 0; j < listCharacters.Count; j++)
        {
            for (int i = 0; i < myCharacters.Length; i++)
            {
                if (listCharacters[j].name.Equals(myCharacters[i]))
                {
                    listCharacters[j].SetActive(true);
                }
            }
        }
    }
    private void SelectCharacter(string chacracterName)
    {
        this.roomManagerV2.CharacterSelected(chacracterName);
    }

    private void SetTextReadyButtonIsReady()
    {
        this.textStartOrReadyGame.text = "Đã sẵn sàng";
    }

    private void SetTextReadyButtonIsNotReady()
    {
        this.textStartOrReadyGame.text = "Sẵn sàng";
    }

    private void SetTextStartButton()
    {
        this.textStartOrReadyGame.text = "Bắt đầu";
    }

    private void SetOnStartButton()
    {
        this.buttonStartOrReadyButton.interactable = true;
    }

    private void SetOffStartButton()
    {
        this.buttonStartOrReadyButton.interactable = false;
    }
    private void SetReadyButtonIsReady()
    {
        this.buttonStartOrReadyButton.GetComponent<Image>().sprite = this.redButton;
    }

    private void SetReadyButtonIsNotReady()
    {
        this.buttonStartOrReadyButton.GetComponent<Image>().sprite = this.greenButton;
    }

    private void LeaveRoom()
    {
        this.roomManagerV2.LeaveRoom();
    }

    private void OpenPanelCharacterSelection()
    {
        this.panelCharacterSelection.SetActive(true);
    }

    private void ClosePanelCharacterSelection()
    {
        this.panelCharacterSelection.SetActive(false);
    }

    private void StartOrReadyGame()
    {
        this.roomManagerV2.StartOrReadyGame();
    }

    #endregion
}
