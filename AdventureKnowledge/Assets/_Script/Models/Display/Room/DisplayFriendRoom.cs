using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayFriendRoom : MonoBehaviour
{
    [SerializeField] private Image avatar;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image status;
    [SerializeField] private Button inviteBtn;
    [SerializeField] private Button chatBtn;
    [SerializeField] private GameObject imgWarning;
    [SerializeField] private TextMeshProUGUI timeText;
    public Button GetInviteBtn()
    {
        return this.inviteBtn;
    }

    public Sprite GetStatus()
    {
        return this.status.sprite;
    }

    public Button GetChatBtn()
    {
        return this.chatBtn;
    }

    public string GetName()
    {
        return this.nameText.text;
    }

    public void SetName(string name)
    {
        this.nameText.text = name;
    }

    public void SetAvatar(Texture2D texture2D)
    {
        this.avatar.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
    }

    public void SetStatus(Sprite sprite)
    {
        this.status.sprite = sprite;
    }
    

    
    public void SetInviteBtnFollowStatus(bool isOnline)
    {
        this.inviteBtn.interactable = isOnline;
        StopAllCoroutines();
        this.timeText.text = "";
    }

    public void SetOnOffWarn(bool isOn)
    {
        this.imgWarning.SetActive(isOn);
    }

    public void SetTime()
    {
        StartCoroutine(this.waitToInviteAgain());
    }

    private IEnumerator waitToInviteAgain()
    {
        int time = 11;
        this.inviteBtn.interactable = false;
        while (true)
        {
            this.timeText.text = time.ToString();
            yield return new WaitForSeconds(1);
            time -= 1;
            if (time == 0) { break; }
        }
        this.inviteBtn.interactable = true;
        this.timeText.text = "";
    }
}