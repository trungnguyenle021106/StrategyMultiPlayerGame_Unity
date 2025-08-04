using Photon.Pun;
using UnityEngine;

public class DisplaySkill : MonoBehaviourPunCallbacks
{
    #region private Serialize Fields
    [SerializeField] private GameObject skill;
    [SerializeField] private string typeSkill;
    [SerializeField] private string typeSound;
    [SerializeField] private Animator skillAnim;
    [SerializeField] private Transform startTransform;
    [SerializeField] private Sprite idleSprite;
    #endregion

    #region private Fields
    #endregion

    #region public Fields
    public int dmg { get; set; }
    #endregion

    private void Start()
    {

    }

    private void Update()
    {
        if (skill.activeInHierarchy)
        {
            AnimatorStateInfo animatorStateInfo = this.skillAnim.GetCurrentAnimatorStateInfo(0);
            if (animatorStateInfo.IsName("End") && animatorStateInfo.normalizedTime >= 1.0f)
            {
                if (idleSprite != null) this.skill.GetComponent<SpriteRenderer>().sprite = idleSprite;
                this.skill.SetActive(false);
                this.skill.transform.localScale = Vector2.one;
                this.skill.transform.position = startTransform.position;
            }
        }
    }

    #region public Methods
    public Transform GetStartTransform()
    {
        return this.startTransform;
    }
    public string GetTypeSkill()
    {
        return this.typeSkill;
    }
    public GameObject GetSkill()
    {
        return this.skill;
    }
    public string GetTypeSound()
    {
        return this.typeSound;
    }
    public Animator GetSkillAnim()
    {
        return this.skillAnim;
    }
    #endregion

    #region private Methods
    #endregion

    #region Pun RPC

    #endregion
}