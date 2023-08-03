using System;
using UnityEngine;

public class NewItem : IItem
{
    public static event EventHandler<OnItemEatenEventArgs> OnItemEaten;

    public class OnItemEatenEventArgs : EventArgs
    {
        public int score;
        public Vector3 localPos;
    }
    private LayerMask _playerLayerMask = 512;
    private GameObject _object;
    private int _score;

    public NewItem(GameObject newObject, Vector3 pos, ItemRefsSO itemRefsSO)
    {
        _object = newObject;
        _object.transform.localPosition = pos;
        _score = itemRefsSO.score;
    }

    public void Act()
    {
        if (Physics2D.CircleCast(_object.transform.position, .8f / 2, Vector2.zero, .05f, _playerLayerMask))
        {
            Debug.Log("Here");
            OnItemEaten?.Invoke(this, new OnItemEatenEventArgs
            {
                score = _score,
                localPos = _object.transform.localPosition,
            });
            Deactive();
        }
    }

    public void Active(Vector3 pos, Transform parent, ItemRefsSO itemRefsSO)
    {
        _object.SetActive(true);
        _object.transform.parent = parent;
        _object.transform.localPosition = pos;
        _score = itemRefsSO.score;
    }

    public void Deactive()
    {
        _object.SetActive(false);
    }

    public bool GetActivateState()
    {
        return _object.activeSelf;
    }

    public void SettingSprite(Sprite newSprite)
    {
        _object.GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    public Vector3 GetWorldPosition()
    {
        return _object.transform.position;
    }

    public bool GetActivateStateGlobal()
    {
        return _object.transform.parent.gameObject.activeSelf;
    }
}
