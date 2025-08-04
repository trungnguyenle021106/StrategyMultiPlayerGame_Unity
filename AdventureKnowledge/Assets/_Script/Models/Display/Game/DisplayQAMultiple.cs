using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayQAMultiple : MonoBehaviour
{
    #region private Serialize Fields
    [SerializeField] private TextMeshProUGUI textQuestion;
    [SerializeField] private TextMeshProUGUI textNumberQuestion;
    [SerializeField] private Button btnAnswer1;
    [SerializeField] private Button btnAnswer2;
    [SerializeField] private Button btnAnswer3;
    [SerializeField] private Button btnAnswer4;

    [SerializeField] private GameObject hardPoint1;
    [SerializeField] private GameObject hardPoint2;
    [SerializeField] private GameObject hardPoint3;
    #endregion

    #region private Fields
    private int posTrueAnswer;
    public QAPair qaPair { get; private set; }
    private string[] answers;
    public int numberQ { get; private set; }
    #endregion

    private void Start()
    {

    }

    #region public Methods
    public void ShowTrueAnswer()
    {
        this.btnAnswer1.interactable = false;
        this.btnAnswer2.interactable = false;
        this.btnAnswer3.interactable = false;
        this.btnAnswer4.interactable = false;

        switch (this.posTrueAnswer)
        {
            case 1:
                this.btnAnswer1.image.color = Color.green;
                break;
            case 2:
                this.btnAnswer2.image.color = Color.green;
                break;
            case 3:
                this.btnAnswer3.image.color = Color.green;
                break;
            case 4:
                this.btnAnswer4.image.color = Color.green;
                break;
        }
    }
    public void SetQAPair(QAPair qaPair, int numberQuestion)
    {
        this.qaPair = qaPair;
        this.numberQ = numberQuestion;
        this.textNumberQuestion.text = "Câu hỏi số " + numberQuestion.ToString();
        this.answers = this.qaPair.Answer.Split(",");

        this.SetHardPoint(this.qaPair.HardPoint);
        this.SetTextQuestion();
        this.RandomAnswer();
    }
    #endregion

    #region  private Methods
    private void SetTextQuestion()
    {
        string text = this.qaPair.Question;
        this.textQuestion.text = text;
    }

    private void SetOnclick(bool isTrueAnswer, StrategyGameUI strategyGameUI)
    {
        strategyGameUI.Onclick_Answer(isTrueAnswer);
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

    private void RandomAnswer()
    {
        StrategyGameUI strategyGameUI = GameObject.Find("Strategy Game UI").GetComponent<StrategyGameUI>();

        List<int> list = new List<int>() { 0, 1, 2, 3 }; // Danh sách chỉ số của các câu trả lời
        List<int> randomList = new List<int>();
        System.Random random = new System.Random();
        for (int i = 0; i < 4; i++)
        {
            int index = list[random.Next(list.Count)]; // Chọn ngẫu nhiên chỉ số câu trả lời từ danh sách
            list.Remove(index); // Xóa chỉ số đã chọn để tránh trùng lặp

            bool isTrueAnswer = (index == 0);
            // Gán sự kiện và văn bản cho các nút
            switch (i)
            {
                case 0:
                    this.btnAnswer1.onClick.AddListener(() => this.SetOnclick(isTrueAnswer, strategyGameUI));
                    this.btnAnswer1.GetComponentInChildren<TextMeshProUGUI>().text = answers[index].ToString();
                    if (isTrueAnswer) this.posTrueAnswer = 1;
                    break;
                case 1:
                    this.btnAnswer2.onClick.AddListener(() => this.SetOnclick(isTrueAnswer, strategyGameUI));
                    this.btnAnswer2.GetComponentInChildren<TextMeshProUGUI>().text = answers[index].ToString();
                    if (isTrueAnswer) this.posTrueAnswer = 2;
                    break;
                case 2:
                    this.btnAnswer3.onClick.AddListener(() => this.SetOnclick(isTrueAnswer, strategyGameUI));
                    this.btnAnswer3.GetComponentInChildren<TextMeshProUGUI>().text = answers[index].ToString();
                    if (isTrueAnswer) this.posTrueAnswer = 3;
                    break;
                case 3:
                    this.btnAnswer4.onClick.AddListener(() => this.SetOnclick(isTrueAnswer, strategyGameUI));
                    this.btnAnswer4.GetComponentInChildren<TextMeshProUGUI>().text = answers[index].ToString();
                    if (isTrueAnswer) this.posTrueAnswer = 4;
                    break;
            }
        }
    }
    #endregion
}