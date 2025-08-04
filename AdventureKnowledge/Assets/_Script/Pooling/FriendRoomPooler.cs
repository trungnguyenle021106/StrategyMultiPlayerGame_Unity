using System.Collections.Generic;
using UnityEngine;

public class FriendRoomPooler : MonoBehaviour
{
    #region private Serialize Fields
    [SerializeField] private GameObject friendRoom;
    [SerializeField] private GameObject parentObject;
    [SerializeField] private int friendAmount;
    #endregion

    #region private Fields
    private List<GameObject> listFriendRoom;
    #endregion

    #region public Fields
    public static FriendRoomPooler Instance { get; private set; }
    #endregion
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            this.listFriendRoom = new List<GameObject>();
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
    public GameObject GetPooledFriendRoom()
    {
        foreach (GameObject friend in listFriendRoom)
        {
            if (!friend.activeInHierarchy)
            {
                return friend;
            }
        }
        GameObject child = Instantiate(friendRoom, friendRoom.transform.position, Quaternion.identity);
        child.transform.SetParent(parentObject.transform, false);
        this.listFriendRoom.Add(child);
        return child;
    }

    public void ReturnFriendRoomToPooler(GameObject friend)
    {
        friend.SetActive(false);
    }

    public List<GameObject> GetListFriendRoom()
    {
        return this.listFriendRoom;
    }
    #endregion

    #region private Methods
    private void InitFriend()
    {
        for (int i = 0; i < this.friendAmount; i++)
        {
            GameObject child = Instantiate(friendRoom, friendRoom.transform.position, Quaternion.identity);
            child.transform.SetParent(parentObject.transform, false);
            child.SetActive(false);
            this.listFriendRoom.Add(child);
        }
    }
    #endregion
}