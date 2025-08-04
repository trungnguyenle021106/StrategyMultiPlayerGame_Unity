using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GooglePlayGames.BasicApi;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StrategyGameManager : MonoBehaviourPunCallbacks
{
    #region private Serialize Field

    [SerializeField] private StrategyGameUI strategyGameUI;

    #endregion

    # region private Field
    private StrategyPlayer myStrategyPlayer;
    private GameObject CurrentQA;
    private bool isFirstQuestion;
    private Coroutine questionCoroutine;
    private bool myAnswerIs;
    private List<StrategyPlayer> listStrategyPlayer;
    private bool gameIsEnd;
    private InfoAccount infoAccount;
    private int stateGame;
    #endregion
    #region public Fields

    #endregion
    private void Awake()
    {
        listStrategyPlayer = new List<StrategyPlayer>();
    }

    void Start()
    {
        this.StartSetting();
        this.UnListenEventFBRealtime();
        this.gameIsEnd = false;
        this.infoAccount = GameObject.Find("InfoAccount").GetComponent<InfoAccount>();
        if (PhotonNetwork.IsMasterClient)
        {
            this.SetPlayerAnswer(0);
            this.CreateRandomListQA();
        }
    }

    void Update()
    {

    }

    #region Pun Callbacks
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        // if (!gameIsEnd)
        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("endGame"))
        {
            // this.gameIsEnd = true;
            this.SetGameIsEnd();
            StopAllCoroutines();
            string otherPlayerNameText = "";
            foreach (StrategyPlayer strategyPlayer in this.listStrategyPlayer)
            {
                if (!strategyPlayer.photonView.IsMine)
                {
                    otherPlayerNameText = strategyPlayer.GetPlayerName();
                }
            }
            StartCoroutine(this.StartCountDownnTimeEndRoom());
            this.strategyGameUI.OpenCloseSettingPanel(false);
            this.strategyGameUI.OpenCloseConfirmPanel(false);
            this.strategyGameUI.OpenResultPanel(otherPlayerNameText, 1, this.GetCoinReward(1), PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Matching"), this.GetPointRankReward(1));
        }
    }

    public override void OnLeftRoom()
    {
        FirebaseRealtimeDB.Instance.AmIJoinRoom(this.infoAccount.account.Name, false);
        this.LoadLobby();
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("QAAreReady") && PhotonNetwork.IsMasterClient)
        {
            List<Photon.Realtime.Player> playerList = PhotonNetwork.CurrentRoom.Players.Values.ToList<Photon.Realtime.Player>();
            foreach (Photon.Realtime.Player player in playerList)
            {
                if (player.CustomProperties["QAAreReady"] == null)
                {
                    return;
                }
            }
            this.SetTimeActionEnd(8);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("endGame"))
        {
            this.CheckResultGame();
            // StartCoroutine(this.StartCountDownnTimeEndRoom());
        }
        // if (this.gameIsEnd) return;
        // Bắt đầu hiển thị bảng hành động ở mọi client 
        if (propertiesThatChanged.ContainsKey("timeActionEnd")) { Debug.Log("timeActionEnd"); this.StartCountDownAction(); }
        // Bắt đầu thiết lập câu hỏi
        if (propertiesThatChanged.ContainsKey("currentQuestion") && PhotonNetwork.IsMasterClient)
        {
            int hardPoint = QAPooler.Instance.GetQAByNumber((int)propertiesThatChanged["currentQuestion"]).HardPoint;
            this.SetTimeQuestionEnd(GetTimeForHardPoint(hardPoint));
        }
        // Bắt đầu hiển thị QA
        if (propertiesThatChanged.ContainsKey("timeQuestionEnd")) { this.StartCountDownQuestion(); Debug.Log("timeQuestionEnd"); }
        // Bắt đầu thiết lập QA kết thúc do người chơi trả lời hoặc hết thời gian
        if (propertiesThatChanged.ContainsKey("QAIsOver") && (bool)propertiesThatChanged["QAIsOver"])
        {
            StopAllCoroutines();
            StopCoroutine(this.questionCoroutine);
            if (PhotonNetwork.IsMasterClient) { this.SetTimeShowAnswerEnd(); Debug.Log("QAIsOver"); }
            ;
        }
        // Bắt đầu hiển thị câu trả lời đúng
        if (propertiesThatChanged.ContainsKey("timeShowAnswerEnd")) { Debug.Log("timeShowAnswerEnd"); this.StartCountDownShowAnswer(); ; }
        // Kết thúc animation
        if (propertiesThatChanged.ContainsKey("animationIsEnd"))
        {
            if ((int)PhotonNetwork.CurrentRoom.CustomProperties["currentQuestion"] == QAPooler.Instance.GetQAAmountInGame())
            {
                this.CheckGame();
                // this.CheckResultGame();
                return;
            }
            else if (this.myStrategyPlayer.curHp <= 0)
            {
                this.CheckGame();
                // this.CheckResultGame();
                return;
            }
            if (PhotonNetwork.IsMasterClient) { Debug.Log("animationIsEnd"); this.SetTimeActionEnd(8); this.SetQAIsOver(false); this.SetPlayerAnswer(0); }
        }
    }
    #endregion
    private void SetGameIsEnd()
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["endGame"] = PhotonNetwork.ServerTimestamp + 15000;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    #region public METHODS
    public void UpdateInfoAccount(int coins, int rankPoint)
    {
        AccountFB newAccountFB = new AccountFB();
        newAccountFB.Name = this.infoAccount.account.Name;
        if (this.infoAccount.account.Energy != 0) { newAccountFB.Energy = this.infoAccount.account.Energy - 1; }
        // newAccountFB.Energy = this.infoAccount.account.Energy - 1;
        newAccountFB.LastLoginTime = this.infoAccount.account.LastLoginTime;
        newAccountFB.ADViewTime = this.infoAccount.account.ADViewTime;
        newAccountFB.Characters = this.infoAccount.account.Characters;
        newAccountFB.Coins = this.infoAccount.account.Coins + coins;
        newAccountFB.RankPoint = this.infoAccount.account.RankPoint + rankPoint;

        if (newAccountFB.RankPoint < 0)
        {
            newAccountFB.RankPoint = 0;
        }

        FirestoreDatabase.Instance.SetNameCollection("Account");
        FirestoreDatabase.Instance.UpdateAsync<AccountFB>(this.infoAccount.account.id, newAccountFB, (accountFB) =>
        {
            this.infoAccount.account.Energy = accountFB.Energy;
            this.infoAccount.account.Coins = accountFB.Coins;
            this.infoAccount.account.RankPoint = accountFB.RankPoint;
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Matching"))
            {
                this.UpdateRank(accountFB.RankPoint, () =>
                {
                    Debug.Log("Da up date rank");
                });
            }
        });
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LeaveGame()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("endGame")) { PhotonNetwork.LeaveRoom(); return; }
        AccountFB newAccountFB = new AccountFB();
        newAccountFB.Name = this.infoAccount.account.Name;
        if (this.infoAccount.account.Energy != 0) { newAccountFB.Energy = this.infoAccount.account.Energy - 1; }
        newAccountFB.LastLoginTime = this.infoAccount.account.LastLoginTime;
        newAccountFB.ADViewTime = this.infoAccount.account.ADViewTime;
        newAccountFB.Characters = this.infoAccount.account.Characters;
        newAccountFB.Coins = this.infoAccount.account.Coins;
        newAccountFB.RankPoint = this.infoAccount.account.RankPoint + this.GetPointRankReward(-1);

        if (newAccountFB.RankPoint < 0)
        {
            newAccountFB.RankPoint = 0;
        }

        FirestoreDatabase.Instance.SetNameCollection("Account");
        FirestoreDatabase.Instance.UpdateAsync<AccountFB>(this.infoAccount.account.id, newAccountFB, (accountFB) =>
        {
            this.infoAccount.account.Energy = accountFB.Energy;
            this.infoAccount.account.Coins = accountFB.Coins;
            this.infoAccount.account.RankPoint = accountFB.RankPoint;
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Matching"))
            {
                this.UpdateRank(accountFB.RankPoint, () =>
                {
                    PhotonNetwork.LeaveRoom();
                });
            }
            else
            {
                PhotonNetwork.LeaveRoom();
            }
        });
    }

    public void AddStartegyPlayer(StrategyPlayer strategyPlayer)
    {
        this.listStrategyPlayer.Add(strategyPlayer);
    }

    public void SelectAnswer(bool isTrueAnswer)
    {
        if ((PhotonNetwork.CurrentRoom.CustomProperties["QAIsOver"] == null
        || !(bool)PhotonNetwork.CurrentRoom.CustomProperties["QAIsOver"])
        && PhotonNetwork.ServerTimestamp <= (int)PhotonNetwork.CurrentRoom.CustomProperties["timeQuestionEnd"])
        {
            this.SetPlayerAnswer(this.myStrategyPlayer.photonView.ViewID);
            this.SetQAIsOver(true);
            this.myAnswerIs = isTrueAnswer;
        }
    }

    public void SelectAction(string action)
    {
        this.myStrategyPlayer.SelectAction(action);
    }

    #endregion

    #region private Methods
    private void UpdateRank(int rankPoint, Action callback)
    {
        GooglePlayGames.PlayGamesPlatform.Instance.ReportScore(rankPoint, "CgkI65ufsf8CEAIQAQ", (bool success) =>
              {
                  Debug.Log("Đã cập nhật điểm số");
                  callback();
                  // Handle success or failure
              });
    }

    private void SetPlayerAnswer(int viewID)
    {
        Debug.Log("SetPlayerAnswer," + viewID);
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["playerAnswer"] = viewID;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    private void SetTimeActionEnd(int timeEnd)
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["timeActionEnd"] = PhotonNetwork.ServerTimestamp + timeEnd * 1000 + 2000;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    private void SetBuffQA(int hardPoint)
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["buffQA"] = hardPoint;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    private void SetCurrentQuestion(int numberQ)
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["currentQuestion"] = numberQ;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    private void SetTimeQuestionEnd(int timeEnd)
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["timeQuestionEnd"] = PhotonNetwork.ServerTimestamp + timeEnd * 1000 + 1000;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    private void SetQAIsOver(bool isOver)
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["QAIsOver"] = isOver;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    private void SetTimeShowAnswerEnd()
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["timeShowAnswerEnd"] = PhotonNetwork.ServerTimestamp + 1500;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    private int GetTimeForHardPoint(int hardPoint)
    {
        switch (hardPoint)
        {
            case 1:
                return 10;
            case 2:
                return 13;
            case 3:
                return 15;
            default:
                return 0;
        }
    }


    private void BeginGame()
    {
        this.StartCountDownAction();
        this.strategyGameUI.ShowAction();
    }


    private void CreateRandomListQA()
    {
        FirestoreDatabase.Instance.GetListCountAsync<QAPair>((count) =>
        {
            List<int> listRandom = this.RandomQAPair(count);
            this.myStrategyPlayer.SendRandomListQA(listRandom);
        });
    }

    private List<int> RandomQAPair(int QACount)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < QACount; i++)
        {
            list.Add(i);
        }

        List<int> randomList = new List<int>();
        System.Random random = new System.Random();

        for (int i = 0; i < QAPooler.Instance.GetQAAmountInGame(); i++)
        {
            if (list.Count == 0) { break; }
            int index = random.Next(list.Count);
            randomList.Add(list[index]);
            list.RemoveAt(index);
        }
        return randomList;
    }

    private GameObject CreateStrategyPlayer()
    {
        return PhotonNetwork.Instantiate((string)PhotonNetwork.LocalPlayer.CustomProperties["characterName"], new Vector3(0, 0, 0), Quaternion.identity);
    }

    private void StartCountDownAction()
    {
        int timeEnd = (int)PhotonNetwork.CurrentRoom.CustomProperties["timeActionEnd"];
        int time = (timeEnd - PhotonNetwork.ServerTimestamp) / 1000;
        if (time <= 0) return;

        // Các thiết lập cho QA kế tiếp
        int numberQ;
        if (isFirstQuestion) { numberQ = 1; this.isFirstQuestion = false; }
        else { numberQ = (int)PhotonNetwork.CurrentRoom.CustomProperties["currentQuestion"] + 1; }


        QAPair qaPair = QAPooler.Instance.GetQAByNumber(numberQ);
        Debug.Log(qaPair.Question + "," + qaPair.HardPoint);
        this.SetBuffQA(qaPair.HardPoint);
        this.strategyGameUI.SetInfoForNextQuestion(qaPair.HardPoint, qaPair.Category);
        this.strategyGameUI.SetValueIfAnswerTrue(this.myStrategyPlayer.atk, this.myStrategyPlayer.def
        , this.myStrategyPlayer.buff, this.myStrategyPlayer.heal, qaPair.HardPoint);

        this.strategyGameUI.ShowAction();
        StartCoroutine(TimeCounting(timeEnd, () =>
        {
            if (PhotonNetwork.IsMasterClient) this.SetCurrentQuestion(numberQ);
        }));
    }



    private void StartCountDownQuestion()
    {
        int timeEnd = (int)PhotonNetwork.CurrentRoom.CustomProperties["timeQuestionEnd"];
        Debug.Log($"timeEnd: {timeEnd}");

        int time = (timeEnd - PhotonNetwork.ServerTimestamp) / 1000;
        Debug.Log($"PhotonNetwork.ServerTimestamp: {PhotonNetwork.ServerTimestamp}");
        Debug.Log($"time: {time}");

        if (time <= 0)
        {
            Debug.Log("Time is less than or equal to 0. Exiting method.");
            return;
        }


        this.strategyGameUI.ShowQuestion();
        this.CurrentQA =
        QAPooler.Instance
        .GetQAByNumberFromPool((int)PhotonNetwork.CurrentRoom.CustomProperties["currentQuestion"]);

        this.questionCoroutine = StartCoroutine(TimeCounting(timeEnd, () =>
          {
              if (PhotonNetwork.IsMasterClient) this.SetQAIsOver(true);
          }));
    }

    private void StartCountDownShowAnswer()
    {
        // this.strategyGameUI.SetOffQA();

        int timeEnd = (int)PhotonNetwork.CurrentRoom.CustomProperties["timeShowAnswerEnd"];
        int time = (timeEnd - PhotonNetwork.ServerTimestamp) / 1000;
        if (time <= 0) return;

        if (this.CurrentQA.GetComponent<DisplayQAMultiple>() != null)
        { this.CurrentQA.GetComponent<DisplayQAMultiple>().ShowTrueAnswer(); }
        else { this.CurrentQA.GetComponent<DisplayQAText>().ShowTrueAnswer(); }

        StartCoroutine(TimeCounting(timeEnd, () =>
              {
                  this.strategyGameUI.SetOffQA();
                  QAPooler.Instance.ReturnQAToPool(this.CurrentQA);
                  this.strategyGameUI.ClosePanelQuestionAnswer();
                  if (PhotonNetwork.IsMasterClient && (int)PhotonNetwork.CurrentRoom.CustomProperties["playerAnswer"] == 0)
                  {
                      foreach (StrategyPlayer strategyPlayer in this.listStrategyPlayer)
                      {
                          strategyPlayer.photonView.RPC("GetAttackedBySystem", RpcTarget.AllBuffered, 25);
                      }
                      return;
                  }
                  if ((int)PhotonNetwork.CurrentRoom.CustomProperties["playerAnswer"] == this.myStrategyPlayer.photonView.ViewID)
                  {
                      if (this.myAnswerIs == true)
                      { this.myStrategyPlayer.ActionWhenTrueAnswer(); return; }
                      if (this.myAnswerIs == false)
                      { this.myStrategyPlayer.ActionWhenFalse(); return; }
                  }

              }));
    }

    private IEnumerator TimeCounting(int timeEnd, Action callback)
    {
        while (PhotonNetwork.ServerTimestamp <= timeEnd)
        {
            int timeLeft = (timeEnd - PhotonNetwork.ServerTimestamp) / 1000;
            this.strategyGameUI.SetTextTimeCounting(Mathf.RoundToInt(timeLeft));
            yield return null;
        }
        callback();
    }


    private void CheckGame()
    {
        int enemyHP = 0;
        foreach (StrategyPlayer strategyPlayer in this.listStrategyPlayer)
        {
            if (!strategyPlayer.photonView.IsMine)
            {
                enemyHP = strategyPlayer.curHp;
            }
        }
        if (enemyHP > this.myStrategyPlayer.curHp)
        {
            this.SetGameIsEnd();
            return;
        }
        if (enemyHP < this.myStrategyPlayer.curHp)
        {
            this.SetGameIsEnd();
            return;
        }
        if (enemyHP == this.myStrategyPlayer.curHp)
        {
            this.SetGameIsEnd();
            return;
        }
    }

    private void CheckResultGame()
    {
        // this.gameIsEnd = true;
        string otherPlayerNameText = "";
        int enemyHP = 0;
        foreach (StrategyPlayer strategyPlayer in this.listStrategyPlayer)
        {
            if (!strategyPlayer.photonView.IsMine)
            {
                enemyHP = strategyPlayer.curHp;
                otherPlayerNameText = strategyPlayer.GetPlayerName();
            }
        }
        if (enemyHP > this.myStrategyPlayer.curHp)
        {
            this.stateGame = 0;
            this.strategyGameUI.OpenResultPanel(otherPlayerNameText, 0, this.GetCoinReward(0),
             PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Matching"), this.GetPointRankReward(this.stateGame));
            StartCoroutine(this.StartCountDownnTimeEndRoom());
            // this.SetGameIsEnd();
            return;
        }
        if (enemyHP < this.myStrategyPlayer.curHp)
        {
            this.stateGame = 1;
            this.strategyGameUI.OpenResultPanel(otherPlayerNameText, 1, this.GetCoinReward(1)
            , PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Matching"), this.GetPointRankReward(this.stateGame));
            StartCoroutine(this.StartCountDownnTimeEndRoom());
            // this.SetGameIsEnd();
            return;
        }
        if (enemyHP == this.myStrategyPlayer.curHp)
        {
            this.stateGame = 2;
            this.strategyGameUI.OpenResultPanel(otherPlayerNameText, 2, this.GetCoinReward(2),
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Matching"), this.GetPointRankReward(this.stateGame));
            StartCoroutine(this.StartCountDownnTimeEndRoom());
            // this.SetGameIsEnd();
            return;
        }
    }

    private int GetPointRankReward(int result)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Matching"))
        {
            switch (result)
            {
                case -1:
                    return -3;
                case 0:
                    return 0;
                case 1:
                    return 5;
                case 2:
                    return 3;
            }
        }
        return 0;
    }

    private int GetCoinReward(int result)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Matching"))
        {
            switch (result)
            {
                case 0:
                    return 3;
                case 1:
                    return 15;
                case 2:
                    return 5;
            }
        }
        switch (result)
        {
            case 0:
                return 1;
            case 1:
                return 5;
            case 2:
                return 3;
        }

        return 0;
    }

    private IEnumerator StartCountDownnTimeEndRoom()
    {
        int i = 15;
        while (i >= 1)
        {
            this.strategyGameUI.SetTextOut(i);
            yield return new WaitForSeconds(1);
            i--;
        }

        this.LeaveGame();
    }

    private void LoadLobby()
    {
        SceneManager.LoadScene("MyLobbyScene");
    }


    private void StartSetting()
    {
        FirestoreDatabase.Instance.SetNameCollection("QAPair");
        this.myStrategyPlayer = this.CreateStrategyPlayer().GetComponent<StrategyPlayer>();
        this.isFirstQuestion = true;
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
