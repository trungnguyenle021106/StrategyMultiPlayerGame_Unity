using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooling : MonoBehaviour
{
    List<GameObject> list;
    public GameObject qa;
    public GameObject parent;
    public static Pooling Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        list = new List<GameObject>();
        for (int i = 0; i < 50; i++)
        {
            GameObject newQA = Instantiate(qa, new Vector3(0, 0, 0), Quaternion.identity);
            newQA.transform.SetParent(parent.transform, false);
            newQA.SetActive(false);
            list.Add(newQA);
        }
        this.InitQA();
    }

    public GameObject GetQA()
    {
        foreach (GameObject QA in list)
        {
            if (!QA.activeInHierarchy)
            {
                return QA;
            }
        }

        GameObject newQA = Instantiate(qa, new Vector3(0, 0, 0), Quaternion.identity);
        list.Add(newQA);
        return newQA;
    }

    public void RemoveQA(GameObject qaRemove)
    {
        qaRemove.SetActive(false);
    }


    public void Reset()
    {
        foreach (GameObject qa in list)
        {
            qa.SetActive(false);
        }
    }

    public void InitQA()
    {
        FirestoreDatabase.Instance.GetQAPairsAsync((listqa) =>
        {
            foreach (QAPair QA in listqa)
            {
                GameObject newQA = this.GetQA();
                newQA.SetActive(true);
                newQA.GetComponent<DisplayQAPair>().setQAPair(QA);
            }
        });
    }

    public void UpdateGO(QAPair qAPair)
    {
        foreach (GameObject qa in list)
        {
            if (qa.GetComponent<DisplayQAPair>().qAPair.id == qAPair.id)
            {
                qa.GetComponent<DisplayQAPair>().setQAPair(qAPair);
                return;
            }
        }
    }
}
