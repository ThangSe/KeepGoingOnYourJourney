using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGround : IGround
{
    private GameObject _object;
    private Vector3 _groundSize;

    public NewGround(GameObject newObject, Vector3 pos, WallRefsSO wallRefsSO, float rotation)
    {
        _object = newObject;
        _object.transform.localPosition = pos;
        _groundSize = new Vector3(wallRefsSO.defaultSize * wallRefsSO.multipler, wallRefsSO.defaultSize * wallRefsSO.multipler, 1);
        _object.layer = 7;
        _object.transform.rotation = Quaternion.Euler(0, 0, rotation);
        _object.transform.localScale = _groundSize;
    }
    public void Activate(Vector3 pos, WallRefsSO wallRefsSO, Transform parent, float rotation)
    {
        _object.SetActive(true);
        _object.layer = 7;
        _object.transform.parent = parent;
        _object.transform.localPosition = pos;
        _object.transform.rotation = Quaternion.Euler(0, 0, rotation);
        _groundSize = new Vector3(wallRefsSO.defaultSize * wallRefsSO.multipler, wallRefsSO.defaultSize * wallRefsSO.multipler, 1);
        _object.transform.localScale = _groundSize;
    }

    public void Deactive()
    {
        _object.SetActive(false);
    }

    public bool GetActivateState()
    {
        return _object.transform.parent.gameObject.activeSelf;
    }

    public Vector3 GetWorldPosition()
    {
        return _object.transform.position;
    }

    public void SettingSprite(Sprite newSprite)
    {
        _object.GetComponent<SpriteRenderer>().sprite = newSprite;
    }

}
