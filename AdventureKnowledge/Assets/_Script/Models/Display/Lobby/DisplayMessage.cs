using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeOrFriend;
    [SerializeField] private TextMeshProUGUI textContent;
    [SerializeField] private Image imgBackGround;

    public void SetTextMeOrFriend(bool isMine)
    {
        if (isMine) { this.textMeOrFriend.text = "Tôi:"; }
        else { this.textMeOrFriend.text = "Bạn:"; }
    }

    public void SetTextContent(string text)
    {
        this.textContent.text = text;
    }

    public void SetColorBackGround(bool isMine)
    {
        if (isMine) { this.imgBackGround.color = new Color32(224, 183, 96, 255); }
        else { this.imgBackGround.color = new Color32(120, 120, 120, 255); }
    }
}