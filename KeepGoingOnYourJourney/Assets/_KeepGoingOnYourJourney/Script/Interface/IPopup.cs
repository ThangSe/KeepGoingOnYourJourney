using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPopup
{
    void Act();
    void Activate(Vector3 pos, Vector3 dir);
    void Deactivate();
    bool GetActivateState();
    void SettingSprite(Sprite sprite);
}
