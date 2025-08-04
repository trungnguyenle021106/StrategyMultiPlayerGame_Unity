using Photon.Pun;
using UnityEngine;

public class ActionAnim : MonoBehaviourPunCallbacks
{
    #region private Serialize Fields
    [SerializeField] private Animator animator;
    [SerializeField] private StrategyPlayer strategyPlayer;
    #endregion

    #region private Fields
    private AnimatorStateInfo animatorStateInfo;
    private bool isActiveAction;
    private bool isDefGetDmg;
    #endregion

    private void Start()
    {
        this.isActiveAction = false;
    }

    private void Update()
    {
        this.animatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
        if (this.animatorStateInfo.IsName("Idle") && this.animatorStateInfo.normalizedTime >= 1.0f
          && !this.isActiveAction && !isDefGetDmg)
        {
            this.isActiveAction = true;
            this.DoAction(this.strategyPlayer.action);
            if (PhotonNetwork.IsMasterClient) this.SetAnimationIsEnd();
        }

        if (this.animatorStateInfo.IsName("Idle") && this.animatorStateInfo.normalizedTime >= 1.0f
          && !this.isActiveAction && isDefGetDmg)
        {
            this.isActiveAction = true;
            this.SetIsDefGetDmg(false);
            if (PhotonNetwork.IsMasterClient) this.SetAnimationIsEnd();
        }
    }

    #region Pun Callbacks
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("currentQuestion"))
        {
            this.isActiveAction = false;
            this.gameObject.SetActive(false);
        }
    }
    #endregion

    #region public Methods
    public void SetIsDefGetDmg(bool isDefGetDmg)
    {
        this.isDefGetDmg = isDefGetDmg;
    }
    #endregion

    #region private Methods
    private void DoAction(string action)
    {
        switch (action)
        {
            case "def":
                this.strategyPlayer.DefAfterEndAnim();
                break;
            case "heal":
                this.strategyPlayer.HealAfterEndAnim();
                break;
            case "buff":
                this.strategyPlayer.BuffAfterEndAnim();
                break;
        }
    }

    private void SetAnimationIsEnd()
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["animationIsEnd"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }
    #endregion
}