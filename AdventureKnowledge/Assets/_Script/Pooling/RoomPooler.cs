using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomPooler : MonoBehaviour
{
    #region private Serialize Fields
    [SerializeField] private GameObject room;
    [SerializeField] private GameObject parentObject;
    [SerializeField] private int roomAmount;
    #endregion

    #region private Fields
    private List<GameObject> listRoom;
    #endregion

    #region public Fields
    public static RoomPooler Instance { get; private set; }
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
        this.listRoom = new List<GameObject>();
        this.InitRoom();
    }
    #region public Methods
    public GameObject GetPooledRoom()
    {
        foreach (GameObject room in listRoom)
        {
            if (!room.activeInHierarchy)
            {
                return room;
            }
        }
        GameObject child = Instantiate(room, room.transform.position, Quaternion.identity);
        child.transform.SetParent(parentObject.transform, false);
        this.listRoom.Add(child);
        return child;
    }

    public void ReturnRoomToPooler(GameObject room)
    {
        room.SetActive(false);
    }

    public List<GameObject> GetListRoom()
    {
        return this.listRoom;
    }
    #endregion

    #region private Methods
    private void InitRoom()
    {
        for (int i = 0; i < this.roomAmount; i++)
        {
            GameObject child = Instantiate(room, room.transform.position, Quaternion.identity);
            child.transform.SetParent(parentObject.transform, false);
            child.SetActive(false);
            this.listRoom.Add(child);
        }
    }
    #endregion
}
