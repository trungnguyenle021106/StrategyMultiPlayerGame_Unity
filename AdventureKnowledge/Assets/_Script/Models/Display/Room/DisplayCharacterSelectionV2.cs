using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCharacterSelectionV2 : MonoBehaviour
{
    #region private Serialize Fields        
    [SerializeField] private CharacterData characterData;
    #endregion

    public void SetImage(CharacterData characterData)
    {
        this.characterData = characterData;
    }
}
