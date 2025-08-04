using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomMatchingUI : MonoBehaviour
{
    #region private Serialize Fields
    [SerializeField] private RoomMatchingManager roomMatchingManager;
    [SerializeField] private GameObject panelPlayerInfo;
    [SerializeField] private GameObject panelCharacterSelection;

    [SerializeField] private TextMeshProUGUI textReadyGame;
    [SerializeField] private Button readyButton;

    [SerializeField] private TextMeshProUGUI textCountDown;

    [SerializeField] private List<GameObject> listCharacters;
    [SerializeField] private Sprite redButton;
    [SerializeField] private Sprite greenButton;

    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject confirmPanel;
    #endregion

    #region private Fields
    private InfoAccount infoAccount;
    private bool isSettingCharacter;
    #endregion
    void Awake()
    {
        this.infoAccount = GameObject.Find("InfoAccount").GetComponent<InfoAccount>();
    }

    void Start()
    {
        this.isSettingCharacter = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region public Methods

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
        this.roomMatchingManager.QuitGame();
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

    public void Onclick_Ready()
    {
        this.roomMatchingManager.Ready();
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
            this.readyButton.interactable = true;
            return;
        }
        this.readyButton.interactable = false;
    }

    public void Onclick_LeaveRoom()
    {
        this.LeaveRoom();
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



    public void SetTextTime(int time)
    {
        this.textCountDown.text = time.ToString();
    }

    public void Onclick_SelectCharacter(string characterName)
    {
        this.SelectCharacter(characterName);
        this.ClosePanelCharacterSelection();
    }
    #endregion

    #region private Methods

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
        this.roomMatchingManager.CharacterSelected(chacracterName);
    }


    private void SetTextReadyButtonIsReady()
    {
        this.textReadyGame.text = "Đã sẵn sàng";
    }

    private void SetTextReadyButtonIsNotReady()
    {
        this.textReadyGame.text = "Sẵn sàng";
    }

    private void SetReadyButtonIsReady()
    {
        this.readyButton.GetComponent<Image>().sprite = this.redButton;
    }

    private void SetReadyButtonIsNotReady()
    {
        this.readyButton.GetComponent<Image>().sprite = this.greenButton;
    }

    private void LeaveRoom()
    {
        this.roomMatchingManager.LeaveRoom();
    }

    private void OpenPanelCharacterSelection()
    {
        this.panelCharacterSelection.SetActive(true);
    }

    private void ClosePanelCharacterSelection()
    {
        this.panelCharacterSelection.SetActive(false);
    }

    #endregion
}
