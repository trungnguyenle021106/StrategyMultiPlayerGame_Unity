using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayQAText : MonoBehaviour
{
    #region private Serialize Fields
    [SerializeField] private TextMeshProUGUI textQuestion;
    [SerializeField] private TextMeshProUGUI textNumberQuestion;
    [SerializeField] private Button btnAnswer;
    [SerializeField] private TMP_InputField inputAnswer;
    [SerializeField] private GameObject hardPoint1;
    [SerializeField] private GameObject hardPoint2;
    [SerializeField] private GameObject hardPoint3;
    #endregion

    #region private Fields
    public QAPair qaPair { get; private set; }
    private string answer;
    public int numberQ { get; private set; }
    #endregion

    private void Start()
    {

    }

    #region public Methods
    public void ShowTrueAnswer()
    {
        this.btnAnswer.interactable = false;
        this.inputAnswer.text = answer;
        this.inputAnswer.image.color = Color.green;
    }

    public void SetQAPair(QAPair qaPair, int numberQuestion)
    {
        this.qaPair = qaPair;
        this.numberQ = numberQuestion;
        this.textNumberQuestion.text = "Câu hỏi số " + numberQuestion.ToString();
        this.answer = this.qaPair.Answer.Split(",")[0];

        this.SetTextQuestion();
        this.SetHardPoint(this.qaPair.HardPoint);
        this.SetOnclick();
    }
    #endregion

    #region  private Methods
    private void SetTextQuestion()
    {
        string text = this.qaPair.Question;
        this.textQuestion.text = text;
    }

    private void SetOnclick()
    {
        StrategyGameUI strategyGameUI = GameObject.Find("Strategy Game UI").GetComponent<StrategyGameUI>();
        this.btnAnswer.onClick.AddListener(() =>
        {
            if (this.inputAnswer.text.ToLower().Equals(answer.ToLower())) { strategyGameUI.Onclick_Answer(true); return; }
            strategyGameUI.Onclick_Answer(false); return;
        });
    }

    private void SetHardPoint(int hardPoint)
    {
        if (hardPoint > 0)
        {
            this.hardPoint1.SetActive(true);
        }
        if (hardPoint > 1)
        {
            this.hardPoint2.SetActive(true);
        }
        if (hardPoint > 2)
        {
            this.hardPoint3.SetActive(true);
        }
    }
    #endregion
}