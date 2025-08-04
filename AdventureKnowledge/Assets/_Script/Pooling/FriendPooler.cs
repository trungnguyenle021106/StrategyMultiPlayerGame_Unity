using System.Collections.Generic;
using UnityEngine;

public class FriendPooler : MonoBehaviour
{
    #region private Serialize Fields
    [SerializeField] private GameObject friend;
    [SerializeField] private GameObject parentObject;
    [SerializeField] private int friendAmount;
    #endregion

    #region private Fields
    private List<GameObject> listFriend;
    #endregion

    #region public Fields
    public static FriendPooler Instance { get; private set; }
    #endregion
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            this.listFriend = new List<GameObject>();
            this.InitFriend();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

    }

    #region public Methods
    public GameObject GetPooledFriend()
    {
        foreach (GameObject friend in listFriend)
        {
            if (!friend.activeInHierarchy)
            {
                return friend;
            }
        }
        GameObject child = Instantiate(friend, friend.transform.position, Quaternion.identity);
        child.transform.SetParent(parentObject.transform, false);
        this.listFriend.Add(child);
        return child;
    }

    public void ReturnFriendToPooler(GameObject friend)
    {
        friend.SetActive(false);
    }

    public List<GameObject> GetListFriend()
    {
        return this.listFriend;
    }
    #endregion

    #region private Methods
    private void InitFriend()
    {
        for (int i = 0; i < this.friendAmount; i++)
        {
            GameObject child = Instantiate(friend, friend.transform.position, Quaternion.identity);
            child.transform.SetParent(parentObject.transform, false);
            child.SetActive(false);
            this.listFriend.Add(child);
        }
    }
    #endregion
}