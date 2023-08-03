using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGround
{
    void Activate(Vector3 pos, WallRefsSO wallRefsSO, Transform parent, float rotation);
    void Deactive();
    bool GetActivateState();
    void SettingSprite(Sprite newSprite);
    Vector3 GetWorldPosition();
}
