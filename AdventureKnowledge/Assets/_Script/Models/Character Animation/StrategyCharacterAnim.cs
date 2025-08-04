using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class StrategyCharacterAnim : MonoBehaviourPunCallbacks
{
    #region private Serialize Fields
    [SerializeField] private Animator animator;
    [SerializeField] private StrategyPlayer strategyPlayer;
    #endregion

    #region private Fields
    private AnimatorStateInfo animatorStateInfo;

    private bool isAttacked;
    private bool isGetAttacked;
    private bool isCollisioned;
    private bool isSenderAnimationEnd;
    #endregion

    private void Start()
    {
        this.isAttacked = false;
        this.isGetAttacked = false;
        this.isCollisioned = false;
        this.isSenderAnimationEnd = false;
    }

    private void Update()
    {
        this.animatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);

        if (this.animatorStateInfo.IsName("Atk") && this.animatorStateInfo.normalizedTime >= 1.0f
             && !this.isAttacked
        )
        {
            this.isAttacked = true;
            photonView.RPC("TurnOnSkill", RpcTarget.OthersBuffered);
        }
        if (this.animatorStateInfo.IsName("Hurt") && this.animatorStateInfo.normalizedTime >= 1.0f
        && PhotonNetwork.IsMasterClient && !this.isGetAttacked)
        {
            this.isGetAttacked = true;
            if (this.isSenderAnimationEnd) this.SetAnimationIsEnd();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!this.isCollisioned)
        {
            this.isCollisioned = true;
            int dmg = other.gameObject.GetComponent<DisplaySkill>().dmg;
            int def = 0;

            if (!other.gameObject.GetComponent<DisplaySkill>().GetTypeSkill().Equals("spawn"))
            {
                other.gameObject.GetComponent<DisplaySkill>().GetSkillAnim().SetTrigger("End");
            }

            if (this.strategyPlayer.hasDef) { def = this.strategyPlayer.def; }
            int realDmg = dmg - def;

            if (realDmg > 0)
            {
                if (PhotonNetwork.IsMasterClient) photonView.RPC("GetAttacked", RpcTarget.AllBuffered, realDmg);
                this.SetGetAttacked();
                if (PhotonNetwork.IsMasterClient) { this.SetSenderAnimationEnd(true); }
                if (this.strategyPlayer.hasDef && PhotonNetwork.IsMasterClient) { photonView.RPC("DefIsNotGetDmg", RpcTarget.AllBuffered); }
                return;
            }
            else { if (PhotonNetwork.IsMasterClient) photonView.RPC("DefIsGetDmg", RpcTarget.AllBuffered); return; }
        }
    }

    #region Pun Callbacks
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("currentQuestion"))
        {
            this.isAttacked = false;
            this.isGetAttacked = false;
            this.isCollisioned = false;
            this.isSenderAnimationEnd = false;
        }
    }
    #endregion

    #region public Methods
    public void SetAttack()
    {
        this.animator.SetTrigger("atk");
    }

    public void SetSenderAnimationEnd(bool isSenderAnimationEnd)
    {
        this.isSenderAnimationEnd = isSenderAnimationEnd;
    }

    public void SetGetAttacked()
    {
        this.animator.SetTrigger("hurt");
    }


    #endregion

    #region private Methods


    private void SetAnimationIsEnd()
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["animationIsEnd"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }


    #endregion
}