using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class DisplayFriendInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPlayerName;
    [SerializeField] private Image avatar;
    [SerializeField] private Image status;
    [SerializeField] private Button buttonChat;
    [SerializeField] private Button buttonAcceptInvited;
    [SerializeField] private GameObject inviteGO;
    [SerializeField] private TextMeshProUGUI textTime;
    [SerializeField] private GameObject warningImage;
    public IUserProfile userProfile { get; private set; }
    private void Awake()
    {

    }

    public void SetUserProfile(IUserProfile userProfile)
    {
        this.userProfile = userProfile;
        this.SetFriendName(this.userProfile.userName);
        this.SetAvatarFriend(this.userProfile.image);
    }

    private void SetFriendName(string friendName)
    {
        this.textPlayerName.text = friendName;
    }

    private void SetAvatarFriend(Texture2D avatarTexture)
    {
        this.avatar.sprite = Sprite.Create(avatarTexture, new Rect(0, 0, avatarTexture.width, avatarTexture.height), new Vector2(0.5f, 0.5f));
    }

    public void SetStatus(Sprite status)
    {
        this.status.sprite = status;
    }

    public void SetWarningImg(bool isShow)
    {
        this.warningImage.SetActive(isShow);
    }

    public Sprite GetStatus()
    {
        return this.status.sprite;
    }

    public Button GetChatButton()
    {
        return this.buttonChat;
    }

    public Button GetAcceptInvitedButton()
    {
        return this.buttonAcceptInvited;
    }



    public void GetInvite(string myName)
    {
        StartCoroutine(CountDownShowInvite(myName));
    }

    private IEnumerator CountDownShowInvite(string myName)
    {
        this.inviteGO.SetActive(true);
        int time = 10;
        while (true)
        {
            this.textTime.text = time.ToString();
            yield return new WaitForSeconds(1);
            time -= 1;
            if (time == 0) { break; }
        }
        FirebaseRealtimeDB.Instance.DenyInvite(this.userProfile.userName, myName, () =>
        {
            this.inviteGO.SetActive(false);
        });
    }
}