using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
public class DisplayFriendChat : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPlayerName;
    [SerializeField] private Image avatar;
    [SerializeField] private Image status;

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

    public Sprite GetStatus()
    {
        return this.status.sprite;
    }

}