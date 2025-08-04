using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;


public class LobbyUIV2 : MonoBehaviour
{
    #region private Seriazlie Fields
    [SerializeField] private LobbyManagerV2 lobbyManagerV2;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject panelJoinRoomFail;
    // TOP
    [SerializeField] private Image userImage;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerCoinsText;
    [SerializeField] private TextMeshProUGUI playerEnergyText;
    [SerializeField] private TextMeshProUGUI playerAdsViewText;
    [SerializeField] private Button RewardButton;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private TextMeshProUGUI rankPoint;
    // TOP

    // MAIN PLAYGAME
    [SerializeField] private GameObject labelRoomNameError;
    [SerializeField] private TextMeshProUGUI textCountDown;
    [SerializeField] private GameObject playGamePanel;
    [SerializeField] private GameObject roomScrollView;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private GameObject goRoomHasPassword;
    [SerializeField] private TMP_InputField roomPasswordInputField;
    [SerializeField] private TMP_Dropdown typeRoomDropDown;
    [SerializeField] private GameObject passwordRoomPanel;
    [SerializeField] private TMP_InputField roomConfirmPasswordInputField;
    [SerializeField] private GameObject wrongPasswordLabel;
    [SerializeField] private GameObject matchMakingPanel;
    [SerializeField] private Button MatchGameButton;
    [SerializeField] private Button CreateRoomButton;
    // MAIN PLAYGAME

    // MAIN UNLOCK
    [SerializeField] private List<GameObject> listCharacters;
    [SerializeField] private GameObject unlockPanel;
    [SerializeField] private GameObject confirmUnlockPanel;
    // MAIN UNLOCK


    // MAIN FRIEND
    [SerializeField] private TMP_InputField inputRoomSearch;
    [SerializeField] private TMP_InputField inputCharacterSearch;
    [SerializeField] private TMP_InputField inputFriendSearch;
    [SerializeField] private GameObject friendPanel;
    [SerializeField] private GameObject chatPanel;
    [SerializeField] private GameObject infoFriendChat;
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private Sprite onlineSprite;
    [SerializeField] private Sprite offlineSprite;
    [SerializeField] private GameObject warningIcon;
    [SerializeField] private ScrollRect chatScrollRect;
    // MAIN FRIEND

    // BOT
    [SerializeField] private Button playBtn;
    [SerializeField] private Button unlockBtn;
    [SerializeField] private Button friendBtn;
    // BOT
    #endregion

    #region private Fields
    private InfoAccount infoAccount;
    private string roomNamePassword;
    private string roomPassword;
    private string namePanelIsOpened;
    private bool isChatFriendOpen;
    private string idChat;
    private string nameFriendIsChatting;
    private bool isChatNoti;
    private bool isInviteNoti;
    private string NameCharacterBuying;
    private int CoinsNeedTobuy;
    private bool isSearchingRoom;
    private bool isSearchingFriend;
    private bool isSearchingCharacter;
    #endregion

    private void Awake()
    {
        this.infoAccount = GameObject.Find("InfoAccount").GetComponent<InfoAccount>();
        this.userImage.sprite = this.infoAccount.userAvatarSprite;
        this.playerNameText.text = this.infoAccount.account.Name;
        this.playerEnergyText.text = this.infoAccount.account.Energy.ToString();
        this.playerCoinsText.text = this.infoAccount.account.Coins.ToString();
        this.namePanelIsOpened = "Play";
    }

    // Start is called before the first frame update
    private void Start()
    {
        this.isChatFriendOpen = false;
        this.userImage.sprite = this.infoAccount.userAvatarSprite;
        this.playerNameText.text = this.infoAccount.account.Name;
        this.isInviteNoti = false;
        this.isChatNoti = false;
        this.idChat = "";

        this.SetCoins();
        this.SetEnergy();
        this.SetAdsView();
        this.SetRankPoint();
        this.SetOnOffRewardButton();
        this.CheckEnergy(false);
        this.ChangeColorBottomButtons(true, this.playBtn);
        this.LoadCharacterUnlock();
        this.LoadFriendFirstTime();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    #region public Methods
    public void Onclick_CloseJoinRoomFailed()
    {
        this.OpenCloseJoinRoomFail(false);
    }

    public void OpenCloseJoinRoomFail(bool isOn)
    {
        this.panelJoinRoomFail.SetActive(isOn);
    }
    public void OnValueChangeCharacter()
    {
        if (this.inputCharacterSearch.text.Equals(""))
        {
            foreach (GameObject character in this.listCharacters)
            {
                character.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject character in this.listCharacters)
            {
                if (character.name.ToLower().Contains(this.inputCharacterSearch.text.ToLower()))
                {
                    character.SetActive(true);
                }
                else
                {
                    character.SetActive(false);
                }
            }
        }
    }

    public void OnValueChangeSearchFriend()
    {
        if (this.inputFriendSearch.text.Equals(""))
        {
            foreach (GameObject friend in FriendPooler.Instance.GetListFriend())
            {
                DisplayFriendInfo displayFriendInfo = friend.GetComponent<DisplayFriendInfo>();
                if (displayFriendInfo.userProfile != null)
                {
                    friend.SetActive(true);
                }
            }
        }
        else
        {
            foreach (GameObject friend in FriendPooler.Instance.GetListFriend())
            {
                DisplayFriendInfo displayFriendInfo = friend.GetComponent<DisplayFriendInfo>();
                if (displayFriendInfo.userProfile == null) continue;
                if (displayFriendInfo.userProfile.userName.ToLower().Contains(this.inputFriendSearch.text.ToLower()))
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

    public void OnValueChangeSearchRoom()
    {
        if (this.inputRoomSearch.text.Equals(""))
        {
            this.isSearchingRoom = false;
            foreach (GameObject room in RoomPooler.Instance.GetListRoom())
            {
                if (room.GetComponent<DisplayRoomInfo>().roomInfo == null) continue;
                room.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject room in RoomPooler.Instance.GetListRoom())
            {
                if (room.GetComponent<DisplayRoomInfo>().roomInfo == null) continue;
                string roomName = room.GetComponent<DisplayRoomInfo>().roomInfo.Name;
                if (roomName.ToLower().Contains(this.inputRoomSearch.text.ToLower()))
                {
                    room.SetActive(true);
                }
                else
                {
                    room.SetActive(false);
                }
            }
        }
    }


    public void SetRankPoint()
    {
        this.rankPoint.text = "Điểm rank hiện tại : " + this.infoAccount.account.RankPoint.ToString();
    }

    public void SetOnOffRewardButton()
    {
        if (this.infoAccount.account.ADViewTime > 0)
        {
            this.RewardButton.interactable = true;
        }
        else
        {
            this.RewardButton.interactable = false;
        }
    }
    public void SetLoadingPanel(bool isOpen)
    {
        this.loadingPanel.SetActive(isOpen);
    }

    public void Onclick_Avatar()
    {
        this.OpenCloseSettingPanel(true);
    }

    public void Onclick_QuitGame()
    {
        this.OpenCloseSettingPanel(false);
        this.OpenCloseConfirmPanel(true);
    }

    public void Accept_ConfirmPanel()
    {
        this.lobbyManagerV2.QuitGame();
    }

    public void Cancel_ConfirmPanel()
    {
        this.OpenCloseConfirmPanel(false);
    }

    public void Onclick_CloseSetting()
    {
        this.OpenCloseSettingPanel(false);
    }
    //PLAY GAME 
    public void Onclick_CancelMatchingGame()
    {
        this.lobbyManagerV2.CancelMatching();
        this.OpenCloseMatchingPanel(false);
    }

    public void SetTextTime(int time)
    {
        this.textCountDown.text = time.ToString();
    }

    public void Onclick_MatchingGame()
    {
        this.OpenCloseMatchingPanel(true);
        this.lobbyManagerV2.JoinMatchingRoom();
    }

    public void OpenCloseMatchingPanel(bool isOpen)
    {
        if (isOpen) { this.matchMakingPanel.SetActive(true); return; }
        this.matchMakingPanel.SetActive(false); return;
    }

    public void OnchangeValueTypeRoom()
    {
        switch (this.typeRoomDropDown.value)
        {
            case 0: this.goRoomHasPassword.SetActive(false); break;
            case 1: this.goRoomHasPassword.SetActive(true); break;
        }
    }

    public void Onclick_CreateNewRoomButton()
    {
        string typeRoom = null;
        switch (this.typeRoomDropDown.value)
        {
            case 0: typeRoom = "Công khai"; break;
            case 1: typeRoom = "Riêng tư"; break;
        }
        string roomPassword = null;
        if (typeRoom.Equals("Công khai"))
        {
            this.lobbyManagerV2.CreateRoom(this.roomNameInputField.text, typeRoom, 2, roomPassword);
        }
        else
        {
            roomPassword = this.roomPasswordInputField.text;
            this.lobbyManagerV2.CreateRoom(this.roomNameInputField.text, typeRoom, 2, roomPassword);
        }
    }

    public void Onclick_CancelCreateRoom()
    {
        this.OpenCloseCreateRoomPanel(false);
    }

    public void Onclick_PlayGameButton()
    {
        this.OpenCloseMainPanel("Play");
        this.OpenCloseRoomScrollView(true);
    }

    public void Onclick_CreateRoomButton()
    {
        this.OpenCloseCreateRoomPanel(true);
    }

    public void Onclick_CancelCreateRoomPanel()
    {
        this.OpenCloseCreateRoomPanel(false);
        this.SetOnOffLabelRoomNameError(false);
    }

    public void UpdateListRoom(RoomInfo roomInfo)
    {
        this.UpdateRoomInfo(roomInfo);
    }


    //PLAY GAME

    // UNLOCK
    public void Onclick_UnlockButton()
    {
        this.OpenCloseMainPanel("Unlock");
    }
    // UNLOCK

    // FRIEND
    public void Onclick_FriendButton()
    {
        this.OpenCloseMainPanel("Friend");
    }

    public void Onclick_SendMessage()
    {
        if (!chatInput.text.Equals(""))
        {
            string Getter = this.infoFriendChat.GetComponent<DisplayFriendChat>().userProfile.userName;
            string Sender = this.infoAccount.account.Name;
            string Message = chatInput.text;
            FirebaseRealtimeDB.Instance.SendMessage(Getter, Sender, Message, (response) =>
            {
                Debug.Log($"Getter {Getter}, Sender {Sender}, and idChat {idChat} must not be null or empty.");
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

    private void SetInviteButton(Dictionary<string, string> friendInvite)
    {
        foreach (GameObject friend in FriendPooler.Instance.GetListFriend())
        {
            DisplayFriendInfo displayFriendInfo = friend.GetComponent<DisplayFriendInfo>();
            if (displayFriendInfo.userProfile == null) continue;

            foreach (KeyValuePair<string, string> entry in friendInvite)
            {
                if (entry.Key.Equals(displayFriendInfo.userProfile.userName))
                {
                    displayFriendInfo.GetAcceptInvitedButton().onClick.RemoveAllListeners();
                    displayFriendInfo.GetAcceptInvitedButton().onClick.AddListener(() =>
                    {
                        FirebaseRealtimeDB.Instance.DenyInvite(displayFriendInfo.userProfile.userName, this.infoAccount.account.Name, null);
                        this.JoinRoom(entry.Value);
                    });
                    displayFriendInfo.GetInvite(this.infoAccount.account.Name);
                    break;
                }
            }
        }
    }


    private void SetOnOffWarnImgButton()
    {
        if (!isChatNoti && !isInviteNoti) { this.warningIcon.SetActive(false); }
        else
        {
            this.warningIcon.SetActive(true);
        }
    }


    private void SetInviteNotificationIcon()
    {
        this.isInviteNoti = false;
        this.SetOnOffWarnImgButton();
    }

    private void SetChatNotificationIcon(Dictionary<string, object> dic)
    {
        //Xử lý hiển thị ở button
        bool hasChatNoti = false;
        foreach (var kvp in dic)
        {
            foreach (var friendKvp in (Dictionary<string, object>)kvp.Value)
            {
                Debug.Log($"Key: {friendKvp.Key}, Value: {friendKvp.Value}");
                bool noti = (bool)friendKvp.Value;
                if (noti)
                {
                    hasChatNoti = true;
                    break;
                }
            }
        }

        this.isChatNoti = hasChatNoti;
        this.SetOnOffWarnImgButton();

        //Xử lý hiển thị ở button

        foreach (var kvp in dic)
        {
            foreach (var friendKvp in (Dictionary<string, object>)kvp.Value)
            {
                Debug.Log($"Key: {friendKvp.Key}, Value: {friendKvp.Value}");
                bool noti = (bool)friendKvp.Value;
                foreach (GameObject friend in FriendPooler.Instance.GetListFriend())
                {
                    DisplayFriendInfo displayFriendInfo = friend.GetComponent<DisplayFriendInfo>();
                    if (displayFriendInfo.userProfile != null
                    && kvp.Key.Equals(displayFriendInfo.userProfile.userName))
                    {
                        displayFriendInfo.SetWarningImg(noti);
                        break;
                    }
                }
            }
        }

        if (idChat == "" && isChatFriendOpen)
        {
            foreach (var kvp in dic)
            {
                if (kvp.Key.Equals(nameFriendIsChatting))
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
                            FirebaseRealtimeDB.Instance.ChatIsRead(this.infoAccount.account.Name, nameFriendIsChatting, idChat);
                            break;
                        }
                    }
                }
            }
        }
    }
    // FRIEND
    public void Onclick_JoinRoom(string roomName)
    {
        bool roomHasPassword = false;
        foreach (GameObject room in RoomPooler.Instance.GetListRoom())
        {
            if (room.GetComponent<DisplayRoomInfo>().roomInfo != null
            && room.GetComponent<DisplayRoomInfo>().roomInfo.Name.Equals(roomName)
            && room.GetComponent<DisplayRoomInfo>().roomInfo.CustomProperties.ContainsKey("Password"))
            {
                this.roomNamePassword = roomName;
                this.roomPassword = room.GetComponent<DisplayRoomInfo>().roomInfo.CustomProperties["Password"].ToString();
                this.OpenClosePasswordRoom(true);
                return;
            }
        }
        if (!roomHasPassword) this.lobbyManagerV2.JoinRoom(roomName);
    }

    public void Onclick_ConfirmRoomPassword()
    {
        if (this.roomConfirmPasswordInputField.text.Equals(this.roomPassword))
        {
            this.JoinRoom(roomNamePassword);
        }
        else { this.wrongPasswordLabel.SetActive(true); }
    }

    public void SetOffWrongPasswordLabel()
    {
        if (this.wrongPasswordLabel.activeInHierarchy) this.wrongPasswordLabel.SetActive(false);
    }

    public void Onclick_CancelRoomPassword()
    {
        this.wrongPasswordLabel.SetActive(false);
        this.OpenClosePasswordRoom(false);
    }

    public void SetOnOffLabelRoomNameError(bool isOn)
    {
        this.labelRoomNameError.SetActive(isOn);
    }

    public void LoadStatusFriendUI(Dictionary<string, Dictionary<string, bool>> friendStatus)
    {
        foreach (GameObject friend in FriendPooler.Instance.GetListFriend())
        {
            DisplayFriendInfo displayFriendInfo = friend.GetComponent<DisplayFriendInfo>();
            if (displayFriendInfo.userProfile == null) continue;

            foreach (KeyValuePair<string, Dictionary<string, bool>> kvp in friendStatus)
            {
                if (kvp.Key.Equals(displayFriendInfo.userProfile.userName))
                {
                    foreach (KeyValuePair<string, bool> entry in kvp.Value)
                    {
                        Debug.Log($"Name: {kvp.Value}, null: {entry.Value}");
                        if (entry.Key.Equals("isOnline"))
                        {
                            if (entry.Value)
                            {
                                displayFriendInfo.SetStatus(this.onlineSprite);
                                break;
                            }
                            if (!entry.Value)
                            {
                                displayFriendInfo.SetStatus(this.offlineSprite);
                                break;
                            }
                        }
                    }
                }

            }
        }

        if (isChatFriendOpen)
        {
            DisplayFriendChat displayChatFriendInfo = this.infoFriendChat.GetComponent<DisplayFriendChat>();
            foreach (GameObject friend in FriendPooler.Instance.GetListFriend())
            {
                DisplayFriendInfo displayFriendInfo = friend.GetComponent<DisplayFriendInfo>();
                if (displayFriendInfo.userProfile == null) continue;

                if (displayChatFriendInfo.userProfile.userName.Equals(displayFriendInfo.userProfile.userName))
                {
                    displayChatFriendInfo.SetStatus(displayFriendInfo.GetStatus());
                    return;
                }
            }
        }

    }


    public void Onclick_CloseChat()
    {
        foreach (GameObject chat in ChatPooler.Instance.GetListChat())
        {
            if (chat.activeInHierarchy)
            {
                ChatPooler.Instance.ReturnChatToPooler(chat);
            }
        }

        this.OpenCloseChatPanel(false);
        this.isChatFriendOpen = false;
        FirebaseRealtimeDB.Instance.ChatIsRead(this.infoAccount.account.Name, this.nameFriendIsChatting, this.idChat);
        FirebaseRealtimeDB.Instance.UnListenForChatChildChanges();
        this.idChat = "";
    }
    public void AcceptUnlockCharacter()
    {
        this.SetLoadingPanel(true);
        this.lobbyManagerV2.BuyCharacter(this.NameCharacterBuying, this.CoinsNeedTobuy, (accountFB) =>
        {
            this.LoadInfoMation(accountFB);
            this.SetLoadingPanel(false);
            this.OpenCloseUnlockConfirmPanel(false);
        });
    }

    public void Onclick_CancelUnlockCharacter()
    {
        this.OpenCloseUnlockConfirmPanel(false);
    }

    public void Onclick_Rank()
    {
        this.lobbyManagerV2.OpenRank();
    }
    #endregion

    #region private Methods
    private void OpenCloseConfirmPanel(bool isOpen)
    {
        this.confirmPanel.SetActive(isOpen);
    }
    private void LoadInfoMation(AccountFB accountFB)
    {
        this.infoAccount.account.Coins = accountFB.Coins;
        this.infoAccount.account.Characters = accountFB.Characters;
        this.SetCoins();
        this.LoadCharacterUnlock();
    }

    private void Onclick_BuyCharacter(string nameCharacter, int Coins)
    {
        Debug.Log("Onclick_BuyCharacter");
        this.CoinsNeedTobuy = Coins;
        this.NameCharacterBuying = nameCharacter;
        this.OpenCloseUnlockConfirmPanel(true);
    }

    private void OpenCloseUnlockConfirmPanel(bool isOpen)
    {
        this.confirmUnlockPanel.SetActive(isOpen);
    }

    public void LoadCharacterUnlock()
    {    //case 1 đủ tiền, case 2 không đủ tiền, case 3 đã mua
        string[] myCharacter = this.infoAccount.account.Characters.Split(',');
        int myCoins = this.infoAccount.account.Coins;
        foreach (GameObject character in this.listCharacters)
        {
            bool isMine = false;
            DisplayCharacterUnlock displayCharacterUnlock = character.GetComponent<DisplayCharacterUnlock>();
            for (int i = 0; i < myCharacter.Length; i++)
            {
                if (myCharacter[i].Equals(character.name))
                {
                    Debug.Log(myCharacter[i]);
                    displayCharacterUnlock.SetStatusUnlockCharacter(3);
                    isMine = true;
                    break;
                }
            }
            if (!isMine)
            {
                if (displayCharacterUnlock.GetCoins() > myCoins)
                {
                    displayCharacterUnlock.SetStatusUnlockCharacter(2);
                }
                else if (displayCharacterUnlock.GetCoins() <= myCoins)
                {
                    string nameCharacter = character.name;
                    int coins = displayCharacterUnlock.GetCoins();
                    displayCharacterUnlock.SetStatusUnlockCharacter(1);
                    displayCharacterUnlock.GetBuyButton().onClick.RemoveAllListeners();
                    displayCharacterUnlock.GetBuyButton().onClick.AddListener(() =>
                    {
                        this.Onclick_BuyCharacter(nameCharacter, coins);
                    });
                }
            }
        }
    }
    private void OpenCloseChatPanel(bool isOpen)
    {
        if (isOpen)
        {
            DisplayFriendChat displayChatFriendInfo = this.infoFriendChat.GetComponent<DisplayFriendChat>();
            foreach (GameObject friend in FriendPooler.Instance.GetListFriend())
            {
                DisplayFriendInfo displayFriendInfo = friend.GetComponent<DisplayFriendInfo>();
                if (displayFriendInfo.userProfile == null) continue;

                if (displayChatFriendInfo.userProfile.userName.Equals(displayFriendInfo.userProfile.userName))
                {
                    displayChatFriendInfo.SetStatus(displayFriendInfo.GetStatus());
                    break;
                }
            }
        }
        this.chatPanel.SetActive(isOpen);
    }


    private void Onclick_ChatButton(IUserProfile userProfile)
    {
        this.infoFriendChat.GetComponent<DisplayFriendChat>().SetUserProfile(userProfile);
        this.OpenCloseChatPanel(true);
        this.isChatFriendOpen = true;
        this.chatScrollRect.normalizedPosition = new Vector2(0, 0);
        this.nameFriendIsChatting = userProfile.userName;
        FirebaseRealtimeDB.Instance.CheckExistChat(userProfile.userName, this.infoAccount.account.Name, (response) =>
        {
            if (!response.Equals("False"))
            {
                this.idChat = response.ToString();

                FirebaseRealtimeDB.Instance.ListenForChatChildChanges(this.idChat, (dic) =>
                {
                    this.LoadChat(dic);
                });

                FirebaseRealtimeDB.Instance.ChatIsRead(this.infoAccount.account.Name, userProfile.userName, idChat);
            }
        });

    }

    private void LoadChat(Dictionary<string, object> dic)
    {
        Debug.Log(dic.Count);
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
        this.chatScrollRect.normalizedPosition = new Vector2(0, 0);
    }

    private void LoadFriendFirstTime()
    {
        foreach (IUserProfile friendUP in this.infoAccount.account.friends)
        {
            GameObject friend = FriendPooler.Instance.GetPooledFriend();
            DisplayFriendInfo displayFriendInfo = friend.GetComponent<DisplayFriendInfo>();
            displayFriendInfo.SetUserProfile(friendUP);
            displayFriendInfo.GetChatButton().onClick.AddListener(() =>
            {
                this.Onclick_ChatButton(displayFriendInfo.userProfile);
            });
            friend.SetActive(true);
        }

        FirebaseRealtimeDB.Instance.ListenStatusFriend((dic) =>
        {
            this.LoadStatusFriendUI(dic);
        });

        FirebaseRealtimeDB.Instance.ListenForNotificationChat(this.infoAccount.account.Name, (dic) =>
        {
            this.SetChatNotificationIcon(dic);
        });

        FirebaseRealtimeDB.Instance.ListenForInviteValueChildChanges(this.infoAccount.account.Name, (dic) =>
        {
            this.isInviteNoti = true;
            this.SetOnOffWarnImgButton();
            this.SetInviteButton(dic);
        });

        FirebaseRealtimeDB.Instance.ListenForInviteRemoveChanges(this.infoAccount.account.Name, () =>
        {
            this.SetInviteNotificationIcon();
        });
    }


    private void JoinRoom(string roomName)
    {
        this.lobbyManagerV2.JoinRoom(roomName);
    }
    private void OpenClosePasswordRoom(bool isOpen)
    {
        this.passwordRoomPanel.SetActive(isOpen);
    }
    public void SetCoins()
    {
        this.playerCoinsText.text = this.infoAccount.account.Coins.ToString();
    }

    public void SetEnergy()
    {
        this.playerEnergyText.text = this.infoAccount.account.Energy.ToString() + "/20";
    }

    public void SetAdsView()
    {
        this.playerAdsViewText.text = this.infoAccount.account.ADViewTime.ToString() + "/5";
    }

    public void CheckEnergy(bool isHaveRoom)
    {
        bool canPlay;
        if (this.infoAccount.account.Energy > 0) { canPlay = true; }
        else { canPlay = false; }

        this.MatchGameButton.interactable = canPlay;
        this.CreateRoomButton.interactable = canPlay;

        if (isHaveRoom)
        {
            foreach (GameObject room in RoomPooler.Instance.GetListRoom())
            {
                if (room.GetComponent<DisplayRoomInfo>().roomInfo == null) continue;
                room.GetComponent<DisplayRoomInfo>().GetJoinButton().interactable = canPlay;
            }
        }
    }

    private void UpdateRoomInfo(RoomInfo roomInfo)
    {
        bool canPlay;
        if (this.infoAccount.account.Energy > 0) { canPlay = true; }
        else { canPlay = false; }

        bool isNotExist = true;
        foreach (GameObject room in RoomPooler.Instance.GetListRoom())
        {
            RoomInfo roomInfoPooled = room.GetComponent<DisplayRoomInfo>().roomInfo;
            if (roomInfoPooled != null && roomInfo.Name.Equals(roomInfoPooled.Name))
            {
                isNotExist = false;
                break;
            }
        }

        if (isNotExist)
        {
            Debug.Log("currentQuestion" + roomInfo.Name);
            GameObject room = RoomPooler.Instance.GetPooledRoom();
            room.GetComponent<DisplayRoomInfo>().roomInfo = roomInfo;
            room.GetComponent<DisplayRoomInfo>().SetInfomation();
            string roomName = room.GetComponent<DisplayRoomInfo>().roomInfo.Name;
            room.GetComponent<DisplayRoomInfo>().GetJoinButton().onClick.AddListener(() => this.Onclick_JoinRoom(roomName));
            room.GetComponent<DisplayRoomInfo>().GetJoinButton().interactable = canPlay;
            if (!this.isSearchingRoom) room.SetActive(true);
            return;
        }
        
        foreach (GameObject room in RoomPooler.Instance.GetListRoom())
        {
            if (room.GetComponent<DisplayRoomInfo>().roomInfo == null) continue;
            string roomName = room.GetComponent<DisplayRoomInfo>().roomInfo.Name;
            if (roomInfo.Name.Equals(roomName) && (roomInfo.PlayerCount == 2
            || roomInfo.CustomProperties.ContainsKey("currentQuestion")
            || roomInfo.RemovedFromList || roomInfo.IsOpen == false || roomInfo.IsVisible == false))
            {
                room.GetComponent<DisplayRoomInfo>().roomInfo = null;
                RoomPooler.Instance.ReturnRoomToPooler(room);
                break;
            }
            else if (roomInfo.Name.Equals(roomName) && !roomInfo.RemovedFromList)
            {
                room.GetComponent<DisplayRoomInfo>().roomInfo = roomInfo;
                room.GetComponent<DisplayRoomInfo>().SetInfomation();
                room.GetComponent<DisplayRoomInfo>().GetJoinButton().interactable = canPlay;
                break;
            }
        }
    }
    private void OpenCloseMainPanel(string namePanel)
    {
        if (namePanel.Equals(this.namePanelIsOpened)) return;
        switch (this.namePanelIsOpened)
        {
            case "Play":
                this.OpenClosePlayGamePanel(false);
                this.ChangeColorBottomButtons(false, this.playBtn);
                break;
            case "Unlock":
                this.OpenCloseUnlockPanel(false);
                this.ChangeColorBottomButtons(false, this.unlockBtn);
                break;
            case "Friend":
                this.OpenCloseFriendPanel(false);
                this.ChangeColorBottomButtons(false, this.friendBtn);
                break;
        }

        switch (namePanel)
        {
            case "Play":
                this.OpenClosePlayGamePanel(true);
                this.ChangeColorBottomButtons(true, this.playBtn);
                break;
            case "Unlock":
                this.OpenCloseUnlockPanel(true);
                this.ChangeColorBottomButtons(true, this.unlockBtn);
                break;
            case "Friend":
                this.OpenCloseFriendPanel(true);
                this.ChangeColorBottomButtons(true, this.friendBtn);
                break;
        }
        this.namePanelIsOpened = namePanel;
    }

    private void ChangeColorBottomButtons(bool isSelected, Button btn)
    {
        Color32 color32 = btn.image.color;
        if (isSelected)
        {
            btn.image.color = new Color32(color32.r, color32.g, color32.b, 100);
            return;
        }
        btn.image.color = new Color32(color32.r, color32.g, color32.b, 255);
        return;
    }

    private void OpenCloseSettingPanel(bool isOpen)
    {
        this.settingPanel.SetActive(isOpen);
    }

    private void OpenClosePlayGamePanel(bool isOpen)
    {
        this.playGamePanel.SetActive(isOpen);
    }

    private void OpenCloseRoomScrollView(bool isOpen)
    {
        this.roomScrollView.SetActive(isOpen);
    }

    private void OpenCloseCreateRoomPanel(bool isOpen)
    {
        this.createRoomPanel.SetActive(isOpen);
    }

    private void OpenCloseUnlockPanel(bool isOpen)
    {
        this.unlockPanel.SetActive(isOpen);
    }

    private void OpenCloseFriendPanel(bool isOpen)
    {
        if (isOpen) { this.friendPanel.GetComponent<RectTransform>().localPosition = new Vector2(0, 0); }
        else
        {
            this.friendPanel.GetComponent<RectTransform>().localPosition = new Vector2(1200, 0);
        }
    }
    #endregion
}
