using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewMap : IMap
{
    private GameObject _object;
    private MapRefsSO _mapRefsSO;
    private Vector3 _position;
    private List<MapRefsSO.ItemInfo> _itemExistedInfos;

    public NewMap(GameObject newObject, Vector3 pos, MapRefsSO mapRefsSO)
    {
        _itemExistedInfos = new List<MapRefsSO.ItemInfo>();
        _object = newObject;
        _object.transform.position = pos;
        _object.SetActive(true);
        _mapRefsSO = mapRefsSO;
        _position = pos;
        ResetItemInMap();
    }

    public void Activate(Vector3 pos, MapRefsSO mapRefsSO)
    {
        _object.SetActive(true);
        _object.transform.position = pos;
        _position = pos;
        _mapRefsSO = mapRefsSO;
    }

    public void RemoveItemFromMap(Vector3 itemPosition)
    {
        for(int i = 0; i < _itemExistedInfos.Count; i++)
        {
            if (_itemExistedInfos[i].posX == itemPosition.x && _itemExistedInfos[i].posY == itemPosition.y)
            {
                _itemExistedInfos.RemoveAt(i);
                break;
            }
        }
    }

    public void Activate()
    {
        _object.SetActive(true);
    }

    public void Deactivate()
    {
        _object.SetActive(false);
    }

    public bool GetActivateState()
    {
        return _object.activeSelf;
    }

    public Vector3 GetMapPosition()
    {
        return _position;
    }

    public MapRefsSO GetMapRefsSO()
    {
        return _mapRefsSO;
    }

    public GameObject GetMapGO()
    {
        return _object;
    }

    public List<MapRefsSO.ItemInfo> GetItemExistedInfo()
    {
        return _itemExistedInfos;
    }

    public void ResetItemInMap()
    {
        _itemExistedInfos.Clear();
        _itemExistedInfos.AddRange(_mapRefsSO.itemInfo);
    }
}
