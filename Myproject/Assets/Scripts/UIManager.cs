using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject panel;
    public GameObject successPanel;
    public GameObject failurePanel;
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;
    public TextMeshProUGUI text4;
    public TextMeshProUGUI text5;
    public TextMeshProUGUI text6;
    public TextMeshProUGUI allsuvat;
    public TextMeshProUGUI alltoanhoc;
    public GameObject all;
    public void showALL()
    {
        this.all.SetActive(true);
    }

    public void offAll()
    {
        this.all.SetActive(false);
    }
    private void Start()
    {
        FirestoreDatabase.Instance.GetQAPairsAsync((listqa) =>
               {
                   int amounttext1 = 0;
                   int amounttext2 = 0;
                   int amounttext3 = 0;
                   int amounttext4 = 0;
                   int amounttext5 = 0;
                   int amounttext6 = 0;
                   int amountsuvat = 0;
                   int amounttoan = 0;
                   foreach (QAPair QA in listqa)
                   {
                       if (QA.Category.Equals("Toán học"))
                       {
                           if (QA.HardPoint == 1) amounttext4++;
                           if (QA.HardPoint == 2) amounttext5++;
                           if (QA.HardPoint == 3) amounttext6++;
                           amounttoan++;
                       }
                       else
                       {
                           if (QA.HardPoint == 1) amounttext1++;
                           if (QA.HardPoint == 2) amounttext2++;
                           if (QA.HardPoint == 3) amounttext3++;

                           amountsuvat++;
                       }
                   }

                   text1.text = amounttext1.ToString();
                   text2.text = amounttext2.ToString();
                   text3.text = amounttext3.ToString();
                   text4.text = amounttext4.ToString();
                   text5.text = amounttext5.ToString();
                   text6.text = amounttext6.ToString();
                   allsuvat.text = "Tổng : " + amountsuvat.ToString();
                   alltoanhoc.text = "Tổng : " + amounttoan.ToString();
               });
    }

    public void OnClickOpenDetailPanel(QAPair qAPair)
    {
        this.OpenCloseDetailPanel(true);
        panel.GetComponent<DisplayDetail>().setQAPair(qAPair);
        panel.GetComponent<DisplayDetail>().AddBtn.SetActive(false);
        panel.GetComponent<DisplayDetail>().UpdateBtn.SetActive(true);
    }

    public void OnclickOpenCreatePanel()
    {
        this.OpenCloseDetailPanel(true);
        panel.GetComponent<DisplayDetail>().UpdateBtn.SetActive(false);
        panel.GetComponent<DisplayDetail>().AddBtn.SetActive(true);
    }

    public void CloseDDetailPanel()
    {
        this.OpenCloseDetailPanel(false);
    }

    private void OpenCloseDetailPanel(bool isOpen)
    {
        this.panel.SetActive(isOpen);
    }

    public void Onclick_CloseSuccess()
    {
        OpenCloseSuccessPanel(false);
    }

    public void Onclick_CloseFailure()
    {
        this.OpenCloseFailurePanel(false);
    }

    public void ShowSuccess()
    {
        OpenCloseSuccessPanel(true);
    }

    public void ShowFailure()
    {
        this.OpenCloseFailurePanel(true);
    }

    private void OpenCloseSuccessPanel(bool isOpen)
    {
        this.successPanel.SetActive(isOpen);
    }

    private void OpenCloseFailurePanel(bool isOpen)
    {
        this.failurePanel.SetActive(isOpen);
    }


}