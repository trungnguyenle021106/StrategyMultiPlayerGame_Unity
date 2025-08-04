using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayQAPair : MonoBehaviour
{
    public QAPair qAPair;
    public TextMeshProUGUI Q;
    public TextMeshProUGUI A;
    public TextMeshProUGUI hardPoints;
    public TextMeshProUGUI category;
    public TextMeshProUGUI type;
    public Button updateBtn;
    public Button deleteBtn;

    public void setQAPair(QAPair qAPair)
    {
        GameObject UI = GameObject.Find("UI Manager");
        this.qAPair = qAPair;
        this.setInfo(qAPair);
        this.updateBtn.onClick.AddListener(() =>
        {
            UI.GetComponent<UIManager>().OnClickOpenDetailPanel(qAPair);
        });
    }


    void setInfo(QAPair qAPair)
    {
        this.Q.text = qAPair.Question;
        this.A.text = qAPair.Answer;
        this.hardPoints.text = this.setHardPoint(qAPair.HardPoint);
        this.category.text = qAPair.Category;
        this.type.text = this.setType(qAPair.Type);
    }


    string setHardPoint(int hardPoints)
    {
        return hardPoints.ToString() + " sao";
    }

    string setType(string type)
    {
        switch (type)
        {
            case "Multiple":
                return "Chọn đáp án";
            default:
                return "Nhập đáp án";
        }
    }
}