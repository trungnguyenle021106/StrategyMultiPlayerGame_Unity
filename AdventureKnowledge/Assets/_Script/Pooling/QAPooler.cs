using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QAPooler : MonoBehaviourPunCallbacks
{
    #region private Serialize Fields
    [SerializeField] private GameObject QATypeMultiple;
    [SerializeField] private GameObject QATypeText;
    [SerializeField] private GameObject parentObject;
    [SerializeField] private int QAAmount;
    [SerializeField] private int QAAmountInGame;
    #endregion

    #region private Fields
    private List<GameObject> listQATypeMultiple;
    private List<GameObject> listQATypeText;
    List<QAPair> listQA;
    #endregion
    public static QAPooler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        this.listQATypeMultiple = new List<GameObject>();
        this.listQATypeText = new List<GameObject>();
        this.InitQATypeMultiple();
        this.InitQATypeText();
    }


    #region public Methods
    public void ReturnQAToPool(GameObject GOQA)
    {
        GOQA.SetActive(false);
    }

    public int GetQAAmount()
    {
        return this.QAAmount;
    }

    public List<GameObject> GetListQATypeMultiple()
    {
        return this.listQATypeMultiple;
    }

    public List<GameObject> GetListQATypeText()
    {
        return this.listQATypeText;
    }

    public int GetQAAmountInGame()
    {
        return this.QAAmountInGame;
    }

    public void InitData(int[] randomList)
    {
        FirestoreDatabase.Instance.GetListAsync<QAPair>((list) =>
        {
            this.listQA = list;
            int numberQ = 0;

            Debug.Log($"listasync{list.Count}, listrandom{randomList.Length}");
            foreach (int i in randomList)
            {

                Debug.Log("init:" + listQA[i].Type + ", " + listQA[i].HardPoint);
                numberQ++;
                Debug.Log(numberQ);
                if (listQA[i].Type.Equals("Multiple"))
                {
                    this.GetPooledQATypeMultiple().GetComponent<DisplayQAMultiple>().SetQAPair(listQA[i], numberQ);
                    continue;
                }

                if (listQA[i].Type.Equals("Text"))
                {
                    this.GetPooledQATypeText().GetComponent<DisplayQAText>().SetQAPair(listQA[i], numberQ);
                    continue;
                }
            }

            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties["QAAreReady"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        });

    }

    public GameObject GetQAByNumberFromPool(int numberQ)
    {
        foreach (GameObject GOQATypeText in this.listQATypeText)
        {
            if (GOQATypeText.GetComponent<DisplayQAText>().numberQ == numberQ)
            {
                GOQATypeText.SetActive(true);
                return GOQATypeText;
            }
        }

        foreach (GameObject GOQATypeMultiple in this.listQATypeMultiple)
        {
            if (GOQATypeMultiple.GetComponent<DisplayQAMultiple>().numberQ == numberQ)
            {
                GOQATypeMultiple.SetActive(true);
                return GOQATypeMultiple;
            }
        }
        return null;
    }

    public QAPair GetQAByNumber(int numberQ)
    {
        foreach (GameObject GOQATypeText in this.listQATypeText)
        {
            if (GOQATypeText.GetComponent<DisplayQAText>().numberQ == numberQ)
            {
                return GOQATypeText.GetComponent<DisplayQAText>().qaPair;
            }
        }

        foreach (GameObject GOQATypeMultiple in this.listQATypeMultiple)
        {
            if (GOQATypeMultiple.GetComponent<DisplayQAMultiple>().numberQ == numberQ)
            {
                return GOQATypeMultiple.GetComponent<DisplayQAMultiple>().qaPair;
            }
        }
        return null;
    }
    #endregion

    #region private Methods
    private void InitQATypeMultiple()
    {
        for (int i = 0; i < this.QAAmount; i++)
        {
            GameObject child = Instantiate(QATypeMultiple, QATypeMultiple.transform.position, Quaternion.identity);
            child.transform.SetParent(parentObject.transform, false);
            child.SetActive(false);
            this.listQATypeMultiple.Add(child);
        }
    }

    private void InitQATypeText()
    {
        for (int i = 0; i < this.QAAmount; i++)
        {
            GameObject child = Instantiate(QATypeText, QATypeText.transform.position, Quaternion.identity);
            child.transform.SetParent(parentObject.transform, false);
            child.SetActive(false);
            this.listQATypeText.Add(child);
        }
    }

    private GameObject GetPooledQATypeText()
    {
        foreach (GameObject GOQATypeText in this.listQATypeText)
        {
            if (!GOQATypeText.activeInHierarchy && GOQATypeText.GetComponent<DisplayQAText>().qaPair == null)
            {
                return GOQATypeText;
            }
        }
        GameObject child = Instantiate(QATypeText, QATypeText.transform.position, Quaternion.identity);
        child.transform.SetParent(parentObject.transform, false);
        this.listQATypeText.Add(child);
        return child;
    }

    private GameObject GetPooledQATypeMultiple()
    {
        foreach (GameObject GOQATypeMultiple in this.listQATypeMultiple)
        {
            if (!GOQATypeMultiple.activeInHierarchy && GOQATypeMultiple.GetComponent<DisplayQAMultiple>().qaPair == null)
            {
                return GOQATypeMultiple;
            }
        }
        GameObject child = Instantiate(QATypeMultiple, QATypeMultiple.transform.position, Quaternion.identity);
        child.transform.SetParent(parentObject.transform, false);
        this.listQATypeText.Add(child);
        return child;
    }
    #endregion
}
