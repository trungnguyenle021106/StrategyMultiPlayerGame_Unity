using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StrategyGameUI : MonoBehaviour, IStrategyGameUI
{
    #region private Serialize Fields
    [SerializeField] private Transform transformMyPostion;
    [SerializeField] private Transform transformEnemyPosition;

    [SerializeField] private TextMeshProUGUI textTimeCounting;
    [SerializeField] private GameObject panelActions;
    [SerializeField] private TextMeshProUGUI hardPointText;
    [SerializeField] private TextMeshProUGUI fakeValueAtk;
    [SerializeField] private TextMeshProUGUI fakeValueDef;
    [SerializeField] private TextMeshProUGUI fakeValueHeal;
    [SerializeField] private TextMeshProUGUI fakeValueBuff;

    [SerializeField] private GameObject panelQuestionAnswer;
    [SerializeField] private Button btnQA;
    [SerializeField] private Button btnAction;

    [SerializeField] private GameObject GOStrategyGameManager;

    [SerializeField] private GameObject resultGamePanel;
    [SerializeField] private Image topImage;
    [SerializeField] private TextMeshProUGUI textResult;
    [SerializeField] private TextMeshProUGUI otherPlayerNameText;
    [SerializeField] private TextMeshProUGUI timeOutRoomText;
    [SerializeField] private TextMeshProUGUI cointText;
    [SerializeField] private Button copyBtn;
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseDrawSprite;
    [SerializeField] private GameObject rankReward;
    [SerializeField] private TextMeshProUGUI pointRankPlus;


    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject confirmPanel;
    #endregion
    #region 
    private StrategyGameManager strategyGameManager;
    private bool isLeaveGame;
    #endregion

    private void Start()
    {
        this.isLeaveGame = false;
        this.strategyGameManager = this.GOStrategyGameManager.GetComponent<StrategyGameManager>();
    }

    #region public Methods
    public void Onclick_SettingButton()
    {
        this.OpenCloseSettingPanel(true);
    }

    public void Onclick_CloseSettingPanel()
    {
        this.OpenCloseSettingPanel(false);
    }

    public void Onclick_LeaveOrQuitGame()
    {
        if (!this.isLeaveGame) this.strategyGameManager.QuitGame();
        if (this.isLeaveGame) this.strategyGameManager.LeaveGame();
    }

    public void Onclick_Cancel()
    {
        this.OpenCloseConfirmPanel(false);
    }

    public void Onclick_LeaveGameButton()
    {
        this.isLeaveGame = true;
        this.OpenCloseSettingPanel(false);
        this.OpenCloseConfirmPanel(true);
    }

    public void Onclick_QuitGameButton()
    {
        this.isLeaveGame = false;
        this.OpenCloseSettingPanel(false);
        this.OpenCloseConfirmPanel(true);
    }

    public void SetInfoForNextQuestion(int hardPoint, string category)
    {
        this.hardPointText.text = $"Câu hỏi kế tiếp có độ khó {hardPoint} sao chủ đề {category}";
    }

    public void SetValueIfAnswerTrue(int atk, int def, int buff, int heal, int buffQA)
    {
        this.fakeValueAtk.text = (atk * buff * buffQA).ToString();
        Debug.Log($"{def}, {buff}, {buffQA}");
        this.fakeValueDef.text = (75 * buff * buffQA).ToString(); // 75 => def
        this.fakeValueHeal.text = (heal * buff * buffQA).ToString();
        this.fakeValueBuff.text = "X" + (buff + buffQA).ToString();
    }

    public void Onclick_Answer(bool isTrueAnswer)
    {
        this.strategyGameManager.SelectAnswer(isTrueAnswer);
    }

    public void SetTextTimeCounting(int time)
    {
        if (time == 5)
        {
            textTimeCounting.color = Color.red;
        }
        // thời gian đếm ngược = 0 thì không hiển thị số 0
        if (time <= 0)
        {
            textTimeCounting.text = "";
            textTimeCounting.color = Color.white;
            return;
        }
        textTimeCounting.text = time.ToString();
    }

    public void Onclick_ButtonAction()
    {
        if (this.panelQuestionAnswer.activeInHierarchy) { this.ClosePanelQuestionAnswer(); }
        if (this.panelActions.activeInHierarchy) { this.CloseActions(); return; }
        this.OpenActions();
    }


    public void Onclick_ButtonQA()
    {
        if (this.panelActions.activeInHierarchy) { this.CloseActions(); }
        if (this.panelQuestionAnswer.activeInHierarchy) { this.ClosePanelQuestionAnswer(); return; }
        this.OpenPanelQuestionAnswer();
    }

    public void Onclick_AttackAction()
    {
        this.strategyGameManager.SelectAction("atk");
        this.CloseActions();
    }

    public void Onclick_DefendAction()
    {
        this.strategyGameManager.SelectAction("def");
        this.CloseActions();
    }

    public void Onclick_HealthAction()
    {
        this.strategyGameManager.SelectAction("heal");
        this.CloseActions();
    }

    public void Onclick_BuffAction()
    {
        this.strategyGameManager.SelectAction("buff");
        this.CloseActions();
    }

    public void ShowPanelAction()
    {
        this.OpenActions();
        this.ClosePanelQuestionAnswer();
    }

    public Transform GetMyPostion(bool isMine)
    {
        if (isMine) return this.transformMyPostion;
        else return this.transformEnemyPosition;
    }

    public void SetOffActions()
    {
        this.btnAction.interactable = false;
    }

    public void SetOnActions()
    {
        this.btnAction.interactable = true;
    }


    public void SetOffQA()
    {
        this.btnQA.interactable = false;
    }

    public void SetOnQA()
    {
        this.btnQA.interactable = true;
    }


    public void CloseActions()
    {
        this.panelActions.SetActive(false);
    }

    public void OpenPanelQuestionAnswer()
    {
        this.panelQuestionAnswer.SetActive(true);
    }

    public void ShowQuestion()
    {
        this.CloseActions();
        this.SetOffActions();
        this.SetOnQA();
        this.OpenPanelQuestionAnswer();
        this.CloseActions();
    }

    public void ShowAction()
    {
        this.SetOffQA();
        this.SetOnActions();
        this.ShowPanelAction();
        this.ClosePanelQuestionAnswer();
    }

    public void ClosePanelQuestionAnswer()
    {
        this.panelQuestionAnswer.SetActive(false);
    }

    public void OpenResultPanel(string otherPlayerName, int result, int cointText, bool isRank, int rankPoint)
    {
        this.resultGamePanel.SetActive(true);
        this.SetTextResult(result);
        this.SetTopImage(result);
        this.otherPlayerNameText.text = otherPlayerName;
        this.cointText.text = "+" + cointText.ToString();
        this.strategyGameManager.UpdateInfoAccount(cointText, rankPoint);
        if (isRank)
        {
            this.rankReward.SetActive(true);
            this.pointRankPlus.text = "+" + rankPoint.ToString();
        }
    }

    public void SetTextOut(int time)
    {
        this.timeOutRoomText.text = time.ToString() + "s";
    }

    public void Onclick_Copy()
    {
        GUIUtility.systemCopyBuffer = this.otherPlayerNameText.text;
        this.copyBtn.interactable = false;
    }

    public void Onclick_LeaveEndGame()
    {
        this.strategyGameManager.LeaveGame();
    }
    #endregion

    #region private Methods
    private void SetTextResult(int result)
    {
        switch (result)
        {
            case 0:
                this.textResult.text = "Thất bại";
                break;
            case 1:
                this.textResult.text = "Chiến thắng";
                break;
            case 2:
                this.textResult.text = "Hòa";
                break;
        }
    }

    private void SetTopImage(int result)
    {
        switch (result)
        {
            case 1:
                this.topImage.sprite = this.winSprite;
                break;
            default:
                this.topImage.sprite = this.loseDrawSprite;
                break;
        }
    }

    private void OpenActions()
    {
        this.panelActions.SetActive(true);
    }

    public void OpenCloseSettingPanel(bool isOpen)
    {
        this.settingPanel.SetActive(isOpen);
    }

    public void OpenCloseConfirmPanel(bool isOpen)
    {
        this.confirmPanel.SetActive(isOpen);
    }
    #endregion
}