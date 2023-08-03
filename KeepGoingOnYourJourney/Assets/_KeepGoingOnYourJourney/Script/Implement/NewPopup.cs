using UnityEngine;
using UnityEngine.UI;

public class NewPopup : IPopup
{
    private GameObject _object;
    private float _speed = 100f;
    private float _disappearTimer;
    private float _disappeartTimerMax = 1f;
    //private float _disappearSpeed = 1f;

    public NewPopup(GameObject newObject, Vector3 pos)
    {
        _object = newObject;
        _object.transform.position = pos;
        _disappearTimer = _disappeartTimerMax;
        _speed = Random.Range(60f, 100f);
    }
    public void Act()
    {
        _disappearTimer -= Time.deltaTime;
        if(_disappearTimer < 0)
        {
            Deactivate();
        }
        else
        {
            _object.transform.position += Vector3.up * _speed * Time.deltaTime;
        }
    }

    public void Activate(Vector3 pos, Vector3 dir)
    {
        _disappearTimer = _disappeartTimerMax;
        _object.transform.position = pos;
        _object.SetActive(true);
        _speed = Random.Range(60f, 100f);
    }

    public void Deactivate()
    {
        _object?.SetActive(false);
    }

    public bool GetActivateState()
    {
        return _object.activeSelf;
    }

    public void SettingSprite(Sprite newSprite)
    {
        _object.GetComponent<Image>().sprite = newSprite;
    }
}
