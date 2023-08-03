using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMap
{
    void Activate();
    void Activate(Vector3 pos, MapRefsSO mapRefsSO);
    void Deactivate();
    bool GetActivateState();
    Vector3 GetMapPosition();
    MapRefsSO GetMapRefsSO();
    GameObject GetMapGO();
    void ResetItemInMap();
    void RemoveItemFromMap(Vector3 itemPosition);
    List<MapRefsSO.ItemInfo> GetItemExistedInfo();
}
