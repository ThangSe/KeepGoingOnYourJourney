using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWall : IWall
{
    private GameObject _object;
    private Vector3 _wallSize;

    private enum SpecialWall
    {
        Normal = 0,
        Fake = 1,
        Finish = 2,
        Environment = 3,
        Obstacle = 4,
    }
    private SpecialWall _specialWall;

    public NewWall (GameObject newObject, Vector3 pos, WallRefsSO wallRefsSO)
    {
        _object = newObject;
        _object.transform.localPosition = pos;
        SetWallSize(wallRefsSO);
        _object.GetComponent<SpriteRenderer>().sortingOrder = 3;
    }

    public void Activate(Vector3 pos, WallRefsSO wallRefsSO, Transform parent)
    {
        _object.SetActive(true);
        _object.transform.parent = parent;
        _object.transform.localPosition = pos;
        SetWallSize(wallRefsSO);
    }

    public void Deactive()
    {
        _object.SetActive(false);
    }

    public bool GetActivateState()
    {
        return _object.transform.parent.gameObject.activeSelf;
    }

    public void SettingSprite(Sprite newSprite)
    {
        _object.GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    private void SetWallSize(WallRefsSO wallRefsSO)
    {
        if (wallRefsSO.type == WallRefsSO.WallType.Horizontal)
        {
            _wallSize = new Vector3(wallRefsSO.defaultSize * wallRefsSO.multipler, wallRefsSO.defaultSize, 1);
            _object.transform.localScale = _wallSize;
        }
        if (wallRefsSO.type == WallRefsSO.WallType.Vertical)
        {
            _wallSize = new Vector3(wallRefsSO.defaultSize, wallRefsSO.defaultSize * wallRefsSO.multipler, 1);
            _object.transform.localScale = _wallSize;
        }
        if (wallRefsSO.type == WallRefsSO.WallType.Square)
        {
            _wallSize = new Vector3(wallRefsSO.defaultSize * wallRefsSO.multipler, wallRefsSO.defaultSize * wallRefsSO.multipler, 1);
            _object.transform.localScale = _wallSize;
        }
    }

    public Vector3 GetWorldPosition()
    {
        return _object.transform.position;
    }
}
