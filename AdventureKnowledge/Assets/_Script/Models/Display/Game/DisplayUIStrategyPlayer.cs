using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayUIStrategyPlayer : MonoBehaviour, IPunInstantiateMagicCallback
{
    #region private Serialize Fields
    [SerializeField] private GameObject infoactionNeedActive;
    [SerializeField] private TextMeshProUGUI textInfoActionSelected;
    [SerializeField] private TextMeshProUGUI textPlayerName;
    [SerializeField] private Image imageAcionSelected;
    [SerializeField] private Slider sliderHealth;
    [SerializeField] private TextMeshProUGUI textHealth;
    [SerializeField] private TextMeshProUGUI textInfoAction2;
    [SerializeField] private Image imageActionUsing2;
    [SerializeField] private TextMeshProUGUI textInfoAction1;
    [SerializeField] private Image imageActionUsing1;
    [SerializeField] private List<ActionData> actionDatas;
    #endregion private Serialize Fields

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        this.SetPlayerName(info.Sender.NickName);
        this.SetOffActionUsing2();
        this.SettingIfEnemy(info.photonView.AmOwner);
    }

    #region public Methods
    public void SetOffActionUsing2()
    {
        infoactionNeedActive.SetActive(false);
    }

    public void SetOnactionUsing2()
    {
        infoactionNeedActive.SetActive(true);
    }

    public void SetHealthBar(int hp)
    {
        sliderHealth.value = hp;
    }

    public void SetHealthText(string hp)
    {
        textHealth.text = hp;
    }

    private void SetPlayerName(string playerName)
    {
        this.textPlayerName.text = playerName;
    }

    public void SetTextInfoAction2(string textInfoActionSelected)
    {
        this.textInfoAction2.text = textInfoActionSelected;
    }

    public void SettextInfoActionSelected(string textInfoActionSelected)
    {
        this.textInfoActionSelected.text = textInfoActionSelected;
    }

    public void SetimageAcionSelected(string action)
    {
        foreach (ActionData actionData in actionDatas)
        {
            if (actionData.Name == action)
            {
                imageAcionSelected.sprite = actionData.sprite;
                return;
            }
        }
    }


    public void SetimageActionUsing2(string action)
    {
        foreach (ActionData actionData in actionDatas)
        {
            if (actionData.name == action)
            {
                imageActionUsing2.sprite = actionData.sprite;
                return;
            }
        }
    }

    public void SetTextInfoAction1(string text)
    {
        this.textInfoAction1.text = text;
    }

    public string GetPlayerName()
    {
        return this.textPlayerName.text;
    }
    #endregion

    #region private Methods
    private void SettingIfEnemy(bool isMine)
    {
        if (!isMine)
        {
            // infoactionNeedActive.transform.rotation = Quaternion.Euler(0, 180, 0);
            sliderHealth.transform.rotation = Quaternion.Euler(0, 180, 0);
            textInfoActionSelected.transform.rotation = Quaternion.Euler(0, 180, 0);
            textPlayerName.transform.rotation = Quaternion.Euler(0, 180, 0);
            textHealth.transform.rotation = Quaternion.Euler(0, 180, 0);
            // imageActionUsing2.transform.rotation = Quaternion.Euler(0, 180, 0);
            textInfoAction2.transform.rotation = Quaternion.Euler(0, 180, 0);
            imageActionUsing1.transform.rotation = Quaternion.Euler(0, 180, 0);
            textInfoAction1.transform.rotation = Quaternion.Euler(0, 180, 0);
            imageAcionSelected.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
    #endregion
}