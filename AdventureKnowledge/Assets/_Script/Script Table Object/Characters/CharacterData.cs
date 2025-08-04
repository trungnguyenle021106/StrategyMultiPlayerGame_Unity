using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Data", menuName = "CharacterData")]
public class CharacterData : ScriptableObject
{
   public string Name ;
   public Sprite sprite;
   public Animator animator;
}
