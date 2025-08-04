using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoAccount : MonoBehaviour
{
    public Account account;
    public Sprite userAvatarSprite;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
