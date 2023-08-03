using System;
using UnityEngine;

public class NewObstacle : IObstacle
{
    public static event EventHandler OnHitPlayer;
    private GameObject _object;
    private Vector3 _wallSize;
    private float _timerAction;
    private float _timerActionMax;
    private Vector3 _starterPos;
    private Vector3 _moveDir;
    private LayerMask _playerLayerMask = 512;
    private float _moveDistance;
    private float _moveSpeed;
    private bool _changeDirection;
    private float _cooldownChangeDirection = .3f;
    private enum ObstacleType
    {
        Static = 0,
        Dynamic = 1,
    }
    private ObstacleType _obstacleType;
    public NewObstacle(GameObject newObject, Vector3 pos, WallRefsSO wallRefsSO, int obstacleType, Vector3 moveDir, float moveDst)
    {
        _object = newObject;
        _object.transform.localPosition = pos;
        _starterPos = _object.transform.position;
        SetWallSize(wallRefsSO);
        _moveDir = moveDir;   
        if ((int)ObstacleType.Dynamic == obstacleType)
        {
            _obstacleType = ObstacleType.Dynamic;
            _moveDistance = moveDst;
            _moveSpeed = UnityEngine.Random.Range(5f, 10f);
            _timerActionMax = UnityEngine.Random.Range(1f, 3f);
            _timerAction = _timerActionMax;
            _object.GetComponent<SpriteRenderer>().color = Color.white;
            _object.GetComponent<SpriteRenderer>().sortingOrder = 6;
        }
        if ((int)ObstacleType.Static == obstacleType)
        {
            _obstacleType = ObstacleType.Static;
            _timerActionMax = UnityEngine.Random.Range(1.5f, 3f);
            _timerAction = _timerActionMax;
            _moveDistance = 0f;
            _moveSpeed = 0f;
            _object.GetComponent<SpriteRenderer>().color = Color.black;
            _object.GetComponent<SpriteRenderer>().sortingOrder = 4;
        }
    }

    public void Act()
    {
        switch(_obstacleType)
        {
            case ObstacleType.Static:
                _timerAction -= Time.deltaTime;
                if(_timerAction < .5f)
                {
                    if(Physics2D.CircleCast(_object.transform.position, .8f / 2, Vector2.zero, .05f, _playerLayerMask))
                    {
                        OnHitPlayer?.Invoke(this, EventArgs.Empty);
                    }
                    if(_object.GetComponent<SpriteRenderer>().color != Color.white) _object.GetComponent<SpriteRenderer>().color = Color.white;
                    if (_timerAction < 0f)
                    {
                        _timerAction += _timerActionMax;
                        _object.GetComponent<SpriteRenderer>().color = Color.black;
                    }
                }
                break;
            case ObstacleType.Dynamic:
                _timerAction -= Time.deltaTime;
                if (_timerAction < 0f)
                {
                    _moveSpeed = UnityEngine.Random.Range(5f, 10f);
                    _timerAction += _timerActionMax;
                }
                if(Vector3.Distance(_starterPos, _object.transform.position) > _moveDistance && !_changeDirection)
                {
                    _changeDirection = true;
                    _moveDir = Vector3.zero - _moveDir;
                }
                if(_changeDirection)
                {
                    _cooldownChangeDirection -= Time.deltaTime;
                    if(_cooldownChangeDirection < 0f)
                    {
                        _cooldownChangeDirection = .3f;
                        _changeDirection = false;
                    }
                }
                _object.transform.position += _moveDir * _moveSpeed * Time.deltaTime;
                if (Physics2D.CircleCast(_object.transform.position, .8f / 2, Vector2.zero, .05f, _playerLayerMask))
                {
                    OnHitPlayer?.Invoke(this, EventArgs.Empty);
                }
                break;
        }
    }

    public void Activate(Vector3 pos, WallRefsSO wallRefsSO, Transform parent)
    {
        throw new System.NotImplementedException();  
    }

    public void Activate(Vector3 pos, WallRefsSO wallRefsSO, Transform parent, int obstacleType, Vector3 moveDir, float moveDst)
    {
        _object.SetActive(true);
        _object.transform.parent = parent;
        _object.transform.localPosition = pos;
        _starterPos = _object.transform.position;
        SetWallSize(wallRefsSO);
        _moveDir = moveDir;
        if ((int)ObstacleType.Dynamic == obstacleType)
        {
            _obstacleType = ObstacleType.Dynamic;
            _timerActionMax = UnityEngine.Random.Range(1f, 3f);
            _timerAction = _timerActionMax;
            _moveDistance = moveDst;
            _moveSpeed = UnityEngine.Random.Range(5f, 10f);
            _object.GetComponent<SpriteRenderer>().color = Color.white;
            _object.GetComponent<SpriteRenderer>().sortingOrder = 6;
        }
        if ((int)ObstacleType.Static == obstacleType)
        {
            _obstacleType = ObstacleType.Static;
            _timerActionMax = UnityEngine.Random.Range(1.5f, 3f);
            _timerAction = _timerActionMax;
            _moveDistance = moveDst;
            _moveSpeed = 0f;
            _object.GetComponent<SpriteRenderer>().color = Color.black;
            _object.GetComponent<SpriteRenderer>().sortingOrder = 4;
        }
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
        return _starterPos;
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
}
