using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayDetail : MonoBehaviour
{
    public QAPair qAPair;
    public TMP_InputField Q;
    public TMP_InputField Qinput;
    public TMP_InputField A1;
    public TMP_InputField A2;
    public TMP_InputField A3;
    public TMP_InputField A4;
    public TMP_InputField AInputText;
    public TMP_Dropdown hardPointsDrop;
    public TMP_Dropdown categoryDrop;
    public TMP_Dropdown typeDrop;
    public GameObject UpdateBtn;
    public GameObject AddBtn;
    public TextMeshProUGUI textUpdateBtn;
    public Button BackBtn;
    public GameObject qaMultiple;
    public GameObject qaInputText;


    public void OnclickBack()
    {
        gameObject.SetActive(false);
    }

    public void OnclickAdd()
    {
        GameObject UI = GameObject.Find("UI Manager");
        QAPairFB qAPairFB = new QAPairFB();
        if (typeDrop.options[typeDrop.value].text == "Chọn đáp án") { qAPairFB.Question = Q.text; qAPairFB.Answer = $"{A1.text},{A2.text},{A3.text},{A4.text}"; }
        else { qAPairFB.Question = Qinput.text; qAPairFB.Answer = AInputText.text; }
        qAPairFB.Type = typeDrop.options[typeDrop.value].text;
        qAPairFB.Category = categoryDrop.options[categoryDrop.value].text;
        qAPairFB.HardPoint = hardPointsDrop.value + 1;

        if (qAPairFB.Question == "")
        {
            UI.GetComponent<UIManager>().ShowFailure();
            return;
        }
        if (typeDrop.options[typeDrop.value].text == "Chọn đáp án" && (A1.text == "" || A2.text == "" || A3.text == "" || A4.text == ""))
        {
            UI.GetComponent<UIManager>().ShowFailure();
            return;
        }
        else if (typeDrop.options[typeDrop.value].text != "Chọn đáp án" && AInputText.text == "")
        {
            UI.GetComponent<UIManager>().ShowFailure();
            return;
        }
        FirestoreDatabase.Instance.AddAsync<QAPairFB>(qAPairFB, (qa) =>
        {
            UI.GetComponent<UIManager>().CloseDDetailPanel();
            UI.GetComponent<UIManager>().ShowSuccess();
            Pooling.Instance.Reset();
            Pooling.Instance.InitQA();
        });
    }

    public void UpdateOnclick()
    {
        GameObject UI = GameObject.Find("UI Manager");
        QAPairFB qAPairFB = new QAPairFB();
        if (typeDrop.options[typeDrop.value].text == "Chọn đáp án") { qAPairFB.Question = Q.text; qAPairFB.Answer = $"{A1.text},{A2.text},{A3.text},{A4.text}"; }
        else { qAPairFB.Question = Qinput.text; qAPairFB.Answer = AInputText.text; }
        qAPairFB.Type = typeDrop.options[typeDrop.value].text;
        qAPairFB.Category = categoryDrop.options[categoryDrop.value].text;
        qAPairFB.HardPoint = hardPointsDrop.value + 1;

        if (qAPairFB.Question == "")
        {
            UI.GetComponent<UIManager>().ShowFailure();
            return;
        }
        if (typeDrop.options[typeDrop.value].text == "Chọn đáp án" && (A1.text == "" || A2.text == "" || A3.text == "" || A4.text == ""))
        {
            UI.GetComponent<UIManager>().ShowFailure();
            return;
        }
        else if (typeDrop.options[typeDrop.value].text != "Chọn đáp án" && AInputText.text == "")
        {
            UI.GetComponent<UIManager>().ShowFailure();
            return;
        }

        FirestoreDatabase.Instance.UpdateAsync<QAPairFB>(qAPair.id, qAPairFB, (qaFB) =>
        {
            this.qAPair.Question = qaFB.Question;
            this.qAPair.Answer = qaFB.Answer;
            this.qAPair.Type = qaFB.Type;
            this.qAPair.Category = qaFB.Category;
            this.qAPair.HardPoint = qaFB.HardPoint;
            this.setQAPair(qAPair);

            Pooling.Instance.UpdateGO(qAPair);
            UI.GetComponent<UIManager>().CloseDDetailPanel();
            UI.GetComponent<UIManager>().ShowSuccess();
        });
    }

    public void setQAPair(QAPair qAPair)
    {
        this.qAPair = qAPair;
        this.Q.text = qAPair.Question;
        this.OpenQAPair(qAPair.Type);
        this.setHardPoint(qAPair.HardPoint);
        this.setType(qAPair.Type);
    }

    void OpenQAPair(string type)
    {
        switch (type)
        {
            case "Multiple":
                this.qaMultiple.SetActive(true);
                this.qaMultiple.SetActive(false);
                break;
            default:
                this.qaInputText.SetActive(true);
                this.qaMultiple.SetActive(false);
                break;
        }
    }

    void setHardPoint(int hardPoints)
    {
        switch (hardPoints)
        {
            case 1:
                hardPointsDrop.value = 0;
                break;
            case 2:
                hardPointsDrop.value = 1;
                break;
            case 3:
                hardPointsDrop.value = 2;
                break;
        }
    }

    void setType(string type)
    {
        switch (type)
        {
            case "Multiple":
                typeDrop.value = 0;
                SetAnswerMultiple();
                break;
            default:
                typeDrop.value = 1;
                SetAnswerText();
                break;
        }
    }

    void SetAnswerMultiple()
    {
        string[] answers = qAPair.Answer.Split(',');
        this.A1.text = answers[0];
        this.A2.text = answers[1];
        this.A3.text = answers[2];
        this.A4.text = answers[3];
    }

    void SetAnswerText()
    {
        this.AInputText.text = qAPair.Answer;
    }

    public void OnchangeType()
    {
        if (typeDrop.value == 0)
        {
            this.qaMultiple.SetActive(true);
            this.qaInputText.SetActive(false);
            return;
        }
        this.qaInputText.SetActive(true);
        this.qaMultiple.SetActive(false);
    }
}