using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Settings SO", menuName = "ScriptableObjects/GameSettingRefsSO")]

public class GameSettingRefsSO : ScriptableObject
{
    public LayerMask walkableLayerMask;
    public LayerMask unwalkableLayerMask;
    public LayerMask newPlaceLayerMask;
    public FloatVariableSO 
        countdownToStartTime, countdownToStartTimeMax, 
        playerProtectedTime, 
        currentHealth, maxHealth,
        currentScore,
        turnPlayLeft, turnPlayMax,
        mapHeight;
    public BoolVariableSO
        isMuteSound,
        isProtectedPlayer;
}
