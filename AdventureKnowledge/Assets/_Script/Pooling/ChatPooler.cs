using System.Collections.Generic;
using UnityEngine;

public class ChatPooler : MonoBehaviour
{

    #region private Serialize Fields
    [SerializeField] private GameObject chat;
    [SerializeField] private GameObject parentObject;
    [SerializeField] private int chatAmount;
    #endregion

    #region private Fields
    private List<GameObject> listChat;
    #endregion

    #region public Fields
    public static ChatPooler Instance { get; private set; }
    #endregion

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
        this.listChat = new List<GameObject>();
        this.InitChat();
    }
    
    #region public Methods
    public GameObject GetPooledChat()
    {
        foreach (GameObject friend in listChat)
        {
            if (!friend.activeInHierarchy)
            {
                return friend;
            }
        }
        GameObject child = Instantiate(chat, chat.transform.position, Quaternion.identity);
        child.transform.SetParent(parentObject.transform, false);
        this.listChat.Add(child);
        return child;
    }

    public void ReturnChatToPooler(GameObject friend)
    {
        friend.SetActive(false);
    }

    public List<GameObject> GetListChat()
    {
        return this.listChat;
    }
    #endregion

    #region private Methods
    private void InitChat()
    {
        for (int i = 0; i < this.chatAmount; i++)
        {
            GameObject child = Instantiate(chat, chat.transform.position, Quaternion.identity);
            child.transform.SetParent(parentObject.transform, false);
            child.SetActive(false);
            this.listChat.Add(child);
        }
    }
    #endregion
}