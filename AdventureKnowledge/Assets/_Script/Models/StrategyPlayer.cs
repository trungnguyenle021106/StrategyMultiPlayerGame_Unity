using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class StrategyPlayer : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    #region private Serialize Fields
    [SerializeField] private DisplayStrategyPlayer displayStrategyPlayer;
    [SerializeField] private StrategyCharacterAnim strategyCharacterAnim;
    [SerializeField] private DisplaySkill displaySkill;
    [SerializeField] private DisplayUIStrategyPlayer displayUIStrategyPlayer;
    [SerializeField] private GameObject defAction;
    [SerializeField] private GameObject healAction;
    [SerializeField] private GameObject buffAction;
    [SerializeField] private GameObject punishSlash;
    #endregion

    #region private Fields
    private DisplaySound displaySound;
    public int atk { get; private set; } = 100;
    public int def { get; private set; } = 75;
    public int heal { get; private set; } = 50;
    public int buff { get; private set; } = 1;
    private int maxHp = 2000;
    public int curHp { get; private set; } = 2000;
    private int buffFromQA;
    private int fakeValue;
    private bool isSetQABuff;
    public bool hasDef;
    public string action { get; private set; }
    #endregion

    private void Start()
    {
        this.displaySound = GameObject.Find("Main Camera").GetComponent<DisplaySound>();
        this.displayUIStrategyPlayer.SetHealthBar(maxHp);
        this.displayUIStrategyPlayer.SetHealthText($"{curHp}/{maxHp}");
        this.displayUIStrategyPlayer.SettextInfoActionSelected(atk.ToString());
        this.displayUIStrategyPlayer.SetTextInfoAction1(this.buff.ToString());
        this.displaySkill.dmg = atk;
        this.isSetQABuff = false;
        this.hasDef = false;
    }


    #region Pun Callbacks
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        StrategyGameManager strategyGameManager = GameObject.Find("Strategy Game Manager").GetComponent<StrategyGameManager>();
        strategyGameManager.AddStartegyPlayer(this);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("buffQA") && !this.isSetQABuff)
        {
            this.isSetQABuff = true;
            this.buffFromQA = (int)propertiesThatChanged["buffQA"];
            if (this.action == null) { this.SelectAction("atk"); }
            this.SelectAction(this.action);
        }

        if (propertiesThatChanged.ContainsKey("animationIsEnd"))
        {
            this.isSetQABuff = false;
        }
    }
    #endregion

    #region Pun RPC
    [PunRPC]
    private void GetListRandomQA(int[] randomList)
    {
        QAPooler.Instance.InitData(randomList);
    }

    [PunRPC] // Sát thương gây ra không phải do skill hiển thị qua collider
    private void GetAttacked(int dmg)
    {
        this.curHp -= dmg;
        if (this.curHp < 0) this.curHp = 0;
        this.SetHP();
    }

    [PunRPC]
    private void GetAttackedBySystem(int dmg)
    {
        if (PhotonNetwork.IsMasterClient) { this.strategyCharacterAnim.SetSenderAnimationEnd(true); }
        this.punishSlash.SetActive(true);
        this.displaySound.PlaySound("waterSound");
        this.SetAnim("getAtk");
        this.curHp -= dmg;
        if (this.curHp < 0) this.curHp = 0;
        this.SetHP();
    }

    [PunRPC]
    private void ActionSelected(string action)
    {
        this.action = action;

        if (action.Equals("buff")) { this.fakeValue = this.buff + this.buffFromQA; }
        else { this.fakeValue = this.GetAttributeAction(action) * buff * buffFromQA; }

        this.displayUIStrategyPlayer.SettextInfoActionSelected(this.fakeValue.ToString());
        this.displayUIStrategyPlayer.SetimageAcionSelected(action);
    }


    [PunRPC]
    private void TurnOffSkill()
    {
        this.displaySkill.GetSkill().SetActive(false);
    }

    [PunRPC]
    private void TurnOnSkill()
    {
        this.displaySkill.dmg = this.fakeValue;
        this.displaySound.PlaySound(this.displaySkill.GetTypeSound());
        this.buff = 1;

        this.SetBuff(1);
        this.ChangeScaleSkillFollowBuffQA();

        if (displaySkill.GetTypeSkill().Equals("spawn"))
        {
            this.displaySkill.GetSkill().transform.position = this.displayStrategyPlayer.enemyTransform.position + new Vector3(0f, -1.66f, 0);
            this.displaySkill.GetSkill().SetActive(true);
            return;
        }
        Vector2 enemyPos = this.displayStrategyPlayer.enemyTransform.position;

        this.displaySkill.GetSkill().SetActive(true);
        StartCoroutine(this.WaitSkillTypeMove(enemyPos));
        // this.displaySkill.GetSkill().transform.position = Vector2.Lerp(myPos, enemyPos, Time.deltaTime * 20f);
    }

    [PunRPC]
    private void DefIsGetDmg()
    {
        this.hasDef = false;
        this.defAction.GetComponent<ActionAnim>().SetIsDefGetDmg(true);
        this.defAction.SetActive(true);
        this.displayUIStrategyPlayer.SetOffActionUsing2();
        this.def = 75;
    }

    [PunRPC]
    private void DefIsNotGetDmg()
    {
        if (photonView.IsMine)
        {

        }
        this.def = 75;
        this.hasDef = false;
        this.displayUIStrategyPlayer.SetOffActionUsing2();
    }

    [PunRPC]
    private void Attack()
    {
        this.displaySkill.dmg = this.fakeValue;
        this.SetAnim("atk");
    }

    [PunRPC]
    private void Def()
    {
        this.hasDef = true;
        this.def = this.fakeValue;
        this.SetAnim("def");
        this.displaySound.PlaySound("healdefActionSound");

        this.buff = 1;
        this.SetBuff(1);
    }

    [PunRPC]
    private void Heal()
    {
        this.SetAnim("heal");
        this.displaySound.PlaySound("healdefActionSound");

        this.buff = 1;
        this.SetBuff(1);
    }

    [PunRPC]
    private void Buff()
    {
        this.buff = this.fakeValue;
        this.SetAnim("buff");
        this.displaySound.PlaySound("buffSound");
    }
    #endregion

    #region public Methodds
    public string GetPlayerName()
    {
        return this.displayUIStrategyPlayer.GetPlayerName();
    }
    public DisplaySound GetDisplaySound()
    {
        return this.displaySound;
    }

    public void SendRandomListQA(List<int> randomList)
    {
        photonView.RPC("GetListRandomQA", RpcTarget.AllBuffered, randomList.ToArray());
    }

    public void SelectAction(string action)
    {
        photonView.RPC("ActionSelected", RpcTarget.AllBuffered, action);
    }

    public void BuffAfterEndAnim()
    {
        this.SetBuff(this.fakeValue);
    }

    public void DefAfterEndAnim()
    {
        this.displayUIStrategyPlayer.SetTextInfoAction2(this.fakeValue.ToString());
        this.displayUIStrategyPlayer.SetOnactionUsing2();
    }

    public void HealAfterEndAnim()
    {
        int newHp = this.curHp + this.fakeValue;
        if (newHp > 2000) this.curHp = 2000;
        else { this.curHp = newHp; }
        this.SetHP();
    }
    public void ActionWhenTrueAnswer()
    {
        switch (this.action)
        {
            case "atk":
                photonView.RPC("Attack", RpcTarget.AllBuffered);
                break;
            case "def":
                photonView.RPC("Def", RpcTarget.AllBuffered);
                break;
            case "heal":
                photonView.RPC("Heal", RpcTarget.AllBuffered);
                break;
            case "buff":
                photonView.RPC("Buff", RpcTarget.AllBuffered);
                break;
            default:
                break;
        }
    }

    public void ActionWhenFalse()
    {
        int dmg;
        switch (this.action)
        {
            case "atk":
                dmg = 75;
                break;
            default:
                dmg = 50;
                break;
        }
        photonView.RPC("GetAttackedBySystem", RpcTarget.AllBuffered, dmg);
    }
    #endregion

    #region private Methods


    private void SetAnim(string animation)
    {
        switch (animation)
        {
            case "atk":
                this.strategyCharacterAnim.SetAttack();
                break;
            case "getAtk":
                this.strategyCharacterAnim.SetGetAttacked();
                break;
            case "def":
                this.defAction.SetActive(true);
                break;
            case "heal":
                this.healAction.SetActive(true);
                break;
            case "buff":
                this.buffAction.SetActive(true);
                break;
        }
    }

    private int GetAttributeAction(string action)
    {
        switch (action)
        {
            case "atk":
                return this.atk;
            case "def":
                return 75;
            case "heal":
                return this.heal;
            case "buff":
                return this.buff;
            default:
                return 0;
        }
    }

    private void ResetAttribute(string attribute)
    {
        switch (attribute)
        {
            case "atk":
                this.atk = 100;
                break;
            case "def":
                this.def = 75;
                break;
            case "heal":
                this.heal = 50;
                break;
            case "buff":
                this.buff = 1;
                break;
            default:
                break;
        }
    }

    private void SetHP()
    {
        this.displayUIStrategyPlayer.SetHealthText($"{this.curHp}/{this.maxHp}");
        this.displayUIStrategyPlayer.SetHealthBar(this.curHp);
    }

    private void SetBuff(int valueBuff)
    {
        this.displayUIStrategyPlayer.SetTextInfoAction1(valueBuff.ToString());
    }

    private IEnumerator WaitSkillTypeMove(Vector2 enemyPos)
    {
        float duration = 1.0f; // Thời gian di chuyển (1 giây)
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            this.displaySkill.GetSkill().transform.position = Vector2.Lerp(this.displaySkill.GetStartTransform().position, enemyPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Đảm bảo đối tượng đến đúng vị trí đích
        // this.displaySkill.GetSkill().transform.position = enemyPos;
    }

    private void ChangeScaleSkillFollowBuffQA()
    {
        switch (this.buffFromQA)
        {
            case 1:
                this.displaySkill.GetSkill().transform.localScale = new Vector2(1, 1);
                break;
            case 2:
                this.displaySkill.GetSkill().transform.localScale = new Vector2(1.25f, 1.25f);
                break;
            case 3:
                this.displaySkill.GetSkill().transform.localScale = new Vector2(1.5f, 1.5f);
                break;
        }
    }
    #endregion
}