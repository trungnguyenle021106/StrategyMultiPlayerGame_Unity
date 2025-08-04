using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCharacterUnlock : MonoBehaviour
{
    [SerializeField] private Button buyBtn;
    [SerializeField] private GameObject textNotiGO;
    [SerializeField] private TextMeshProUGUI textNoti;
    [SerializeField] private GameObject valueCharacter;
    [SerializeField] private TextMeshProUGUI coinsText;
    private void SetOnOffBuyBtn(bool canBuy)
    {
        buyBtn.enabled = canBuy;
    }

    private void SetTextNoti(string textValue)
    {
        this.textNoti.text = textValue;
    }

    private void SetOnOffValueCharacter(bool isOn)
    {
        this.valueCharacter.SetActive(isOn);
    }

    private void SetOnOffTextNoti(bool isOn)
    {
        this.textNotiGO.SetActive(isOn);
    }

    public int GetCoins()
    {
        Debug.Log($"coinsText.text: {this.coinsText.text}");

        int coins;
        if (int.TryParse(this.coinsText.text, out coins)) { return coins; }
        else { Debug.LogError("Unable to parse coinsText to an integer."); return 0; }
    }


    public Button GetBuyButton()
    {
        return this.buyBtn;
    }

    //case 1 đủ tiền, case 2 không đủ tiền, case 3 đã mua
    public void SetStatusUnlockCharacter(int myCase)
    {
        switch (myCase)
        {
            case 1:
                this.SetOnOffBuyBtn(true);
                this.SetOnOffTextNoti(false);
                this.SetOnOffValueCharacter(true);
                break;
            case 2:
                this.SetOnOffBuyBtn(false);
                this.SetTextNoti("Chưa đủ vàng");
                this.SetOnOffTextNoti(true);
                this.SetOnOffValueCharacter(false);
                break;
            case 3:
                this.SetOnOffBuyBtn(false);
                this.SetTextNoti("Đã sở hữu");
                this.SetOnOffTextNoti(true);
                this.SetOnOffValueCharacter(false);
                break;
        }
    }
}