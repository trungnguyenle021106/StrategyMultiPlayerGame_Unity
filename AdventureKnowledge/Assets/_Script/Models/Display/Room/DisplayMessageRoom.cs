using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMessageRoom : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textNamePlayer;
    [SerializeField] private TextMeshProUGUI textContent;
    [SerializeField] private Image imgBackGround;

    public string GetNamePlayer()
    {
        return textNamePlayer.text;
    }

    public void SetTextNamePlayer(string namePlayer)
    {
        this.textNamePlayer.text = namePlayer;
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