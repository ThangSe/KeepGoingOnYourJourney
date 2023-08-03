using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    void Act();
    void Active(Vector3 pos, Transform parent, ItemRefsSO itemRefsSO);
    void Deactive();
    bool GetActivateState();
    bool GetActivateStateGlobal();
    void SettingSprite(Sprite sprite);
    Vector3 GetWorldPosition();
}
