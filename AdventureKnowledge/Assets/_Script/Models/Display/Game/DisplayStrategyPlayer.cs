using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DisplayStrategyPlayer : MonoBehaviour, IPunInstantiateMagicCallback
{
    #region private Serialize Fields
    [SerializeField] private GameObject parent;
    #endregion

    #region public Fields
    public Transform myTransform { get; private set; }
    public Transform enemyTransform { get; private set; }
    #endregion
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region public Methods
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        this.SetPositionPlayer(info.photonView.AmOwner);
    }
    #endregion

    #region private Methods
    private void SetPositionPlayer(bool isMine)
    {
        IStrategyGameUI strategyUI = GameObject.Find("Strategy Game UI").GetComponent<StrategyGameUI>();
        this.myTransform = strategyUI.GetMyPostion(isMine);
        this.enemyTransform = strategyUI.GetMyPostion(!isMine);

        parent.transform.position = myTransform.position;
        gameObject.transform.rotation = myTransform.rotation;
    }
    #endregion
}
