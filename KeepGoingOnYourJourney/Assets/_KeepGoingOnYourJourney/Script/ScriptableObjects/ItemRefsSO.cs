using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Settings SO", menuName = "ScriptableObjects/ItemRefsSO")]
public class ItemRefsSO : ScriptableObject
{
    public enum ItemType
    {
        None,
        Score
    }
    public ItemType itemType;
    public int score;
}
