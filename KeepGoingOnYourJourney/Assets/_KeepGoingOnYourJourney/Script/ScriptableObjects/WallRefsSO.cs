using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wall Settings SO", menuName = "ScriptableObjects/WallRefsSO")]
public class WallRefsSO : ScriptableObject
{
    public float defaultSize = .78f;
    public enum WallType
    {
        Vertical,
        Horizontal,
        Square,
    }
    public WallType type;
    [Range(1, 60)]
    public int multipler;
}
