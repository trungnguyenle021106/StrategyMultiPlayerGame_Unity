using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginRegisterManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField textUserName;
     [SerializeField]
    private TMP_InputField textPassword;
    [SerializeField]

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Login()
    {
        // FirestoreDatabase.Login(textUserName.text, textPassword.text, GoToLobby);
    }

    private void GoToLobby()
    {
          SceneManager.LoadScene(0);
    }
}
