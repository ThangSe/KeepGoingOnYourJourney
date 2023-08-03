using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance { get; private set; }

    public event EventHandler<OnMapChangedEventArgs> OnMapChanged;

    public class OnMapChangedEventArgs : EventArgs
    {
        public Vector3 newOrigin;
        public MapRefsSO mapRefsSO;
    }
    public event EventHandler OnStateChanged;
    public event EventHandler OnIncreaseScore;
    public event EventHandler OnHeartChanged;
    public event EventHandler<OnSpawnPopupEventArgs> OnSpawnPopup;
    public class OnSpawnPopupEventArgs : EventArgs
    {
        public List<IPopup> newPopups;
        public int type;
        public Vector3 poupPos;
    }

    public bool enableEditMap;

    [SerializeField] private MyGrid _myGrid;
    [SerializeField] private MapRefsSO[] _mapRefsSO;
    [SerializeField] private MapRefsSO _currentMap;
    [SerializeField] private GameSettingRefsSO _gameSettingRefsSO;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _cameraFollow;
    [SerializeField] private Camera _mainCamera;

    [SerializeField] private Transform _world;
    [SerializeField] private GameObject _wallGO, _groundGO, _obstacleGO, _mapGO, _itemGO;

    [Header("Button")]
    [SerializeField] private Button _tutorialButton, _playButton, _replayButton;
    private Node _currentPosNode;
    private Node _nextMoveNode;
    private Node _checkPointNode;

    private Vector3 _mouseWorldPosition;
    private Vector3 _moveDir;
    private Vector3 _velocityCam;

    private List<IWall> _wallsList;
    private List<IObstacle> _obstaclesList;
    private List<IMap> _mapList;
    private List<IItem> _itemList;
    private List<IGround> _groundList;
    private List<IPopup> _popupList;

    private GameObject _currentMapGO;
    private int index;
    [SerializeField] private UIManager _uiManager;
    private enum State
    {
        Home,
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,

    }

    private State _state;
    private Vector3 _lastMoveDir = Vector3.zero;
    private float _threshold = 0.05f;
    private float _thresholdSquared;
    private int _randomMap;
    private bool _isFirstTime = true;
    [SerializeField] private SpriteRenderer _playerSprite;

    [SerializeField] private Image[] _groundImages;
    [SerializeField] private Image[] _itemImages;
    [SerializeField] private Image[] _wallImages;
    [SerializeField] private Image[] _obstacleImages;
    [SerializeField] private Image[] _playerImages;  

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        } else
        {
            Instance = this;
        }
        _wallsList = new List<IWall>();
        _obstaclesList = new List<IObstacle>();
        _mapList = new List<IMap>();
        _itemList = new List<IItem>();
        _groundList = new List<IGround>();
        _popupList = new List<IPopup>();
        _gameSettingRefsSO.turnPlayLeft.value = _gameSettingRefsSO.turnPlayMax.value;
        _playerSprite.sprite = _playerImages[0].sprite;
        _state = State.WaitingToStart;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
        _currentMap = _mapRefsSO[0];
    }

    private void Start()
    {
        _thresholdSquared = _threshold * _threshold;
        AddMap(Vector3.zero, _mapRefsSO[0]);
        AddMap(new Vector3(0, 50), _mapRefsSO[1]);
        SetUpDefault();
        _isFirstTime = false;
        _myGrid.UpdateGrid();
        NewObstacle.OnHitPlayer += NewObstacle_OnHitPlayer;
        NewItem.OnItemEaten += NewItem_OnItemEaten;
        _tutorialButton.onClick.AddListener(() =>
        {
            if(_gameSettingRefsSO.turnPlayLeft.value > 0)
            {
                _state = State.WaitingToStart;
                OnStateChanged?.Invoke(this, EventArgs.Empty);
            }
        });
        _replayButton.onClick.AddListener(() =>
        {
            SetUpDefault();
            if(_gameSettingRefsSO.turnPlayLeft.value > 0)
            {
                _state = State.WaitingToStart;
                OnStateChanged?.Invoke(this, EventArgs.Empty);
            } else
            {
                _state = State.Home;
                OnStateChanged?.Invoke(this, EventArgs.Empty);
            }
        });
        _playButton.onClick.AddListener(() =>
        {
            _state = State.CountdownToStart;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        });
    }

    private void NewItem_OnItemEaten(object sender, NewItem.OnItemEatenEventArgs e)
    {
        for (int i = 0; i < _mapList.Count; i++)
        {
            if ((_currentMapGO.transform.position - _mapList[i].GetMapPosition()).sqrMagnitude < _thresholdSquared) {
                _mapList[i].RemoveItemFromMap(e.localPos);
                break;
            }
        }
        IncScore(e.score);
        OnIncreaseScore?.Invoke(this, EventArgs.Empty);
        OnSpawnPopup?.Invoke(this, new OnSpawnPopupEventArgs
        {
            newPopups = _popupList,
            type = 0,
            poupPos = PlayerPosToScreenPos()
        });
    }

    private void NewObstacle_OnHitPlayer(object sender, EventArgs e)
    {
        if (!_gameSettingRefsSO.isProtectedPlayer.value)
        {
            _gameSettingRefsSO.currentHealth.value--;
            OnHeartChanged?.Invoke(this, EventArgs.Empty);
            OnSpawnPopup?.Invoke(this, new OnSpawnPopupEventArgs
            {
                newPopups = _popupList,
                type = 1,
                poupPos = PlayerPosToScreenPos()
            });
            StartCoroutine(ProtectTimer(_gameSettingRefsSO.playerProtectedTime.value));
        }
    }

    IEnumerator ProtectTimer(float protectTimer)
    {
        _gameSettingRefsSO.isProtectedPlayer.value = true;
        _playerSprite.color = Color.yellow;
        yield return new WaitForSeconds(protectTimer);
        _gameSettingRefsSO.isProtectedPlayer.value = false;
        _playerSprite.color = Color.white;

    }

    private void MuteSound()
    {
        if (!_gameSettingRefsSO.isMuteSound.value)
        {
            AudioListener.volume = 0f;
            _gameSettingRefsSO.isMuteSound.value = !_gameSettingRefsSO.isMuteSound.value;
        }
        if (_gameSettingRefsSO.isMuteSound.value)
        {
            AudioListener.volume = 1f;
            _gameSettingRefsSO.isMuteSound.value = !_gameSettingRefsSO.isMuteSound.value;
        }
    }
    private Vector3 PlayerPosToScreenPos()
    {
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(_player.transform.position);
        return screenPos;
    }

    private void SetUpDefault()
    {
        _gameSettingRefsSO.countdownToStartTime.value = _gameSettingRefsSO.countdownToStartTimeMax.value;
        _currentMapGO = _mapList[0].GetMapGO();
        OnMapChanged?.Invoke(this, new OnMapChangedEventArgs
        {
            mapRefsSO = _mapRefsSO[0],
            newOrigin = _currentMapGO.transform.position,
        });
        _mainCamera.transform.position = new Vector3(0, 0, -10);
        _player.transform.position = new Vector3(0, -21f, 0);  
        _currentPosNode = _myGrid.NodeFromWorldPoint(_player.position);
        _checkPointNode = _currentPosNode;
        _gameSettingRefsSO.currentHealth.value = _gameSettingRefsSO.maxHealth.value;
        _gameSettingRefsSO.isProtectedPlayer.value = false;
        _gameSettingRefsSO.isMuteSound.value = false;
        _gameSettingRefsSO.currentScore.value = 0;

        if(!_isFirstTime)
        {
            StartCoroutine(DelayReset());
        }  
    }
    IEnumerator DelayReset()
    {
        for (int i = 0; i < _popupList.Count; i++)
        {
            if (_popupList[i].GetActivateState()) _popupList[i].Deactivate();
        }
        for (int i = 0; i < _mapList.Count; i++)
        {
            _mapList[i].ResetItemInMap();
            if (_mapList[i].GetActivateState()) _mapList[i].Deactivate();
        }
        yield return null;  
        for (int i = 0; i < 2; i++)
        {
            _mapList[i].Activate();
            LoadStuffToMap(_mapList[i]);
        }
    }
    private void LocateNewPlace()
    {
        if (Mathf.Abs(_currentMapGO.transform.position.y - _player.transform.position.y) > _gameSettingRefsSO.mapHeight.value)
        {
            if (_currentMapGO.transform.position.y < _player.transform.position.y)
            {
                _currentMap = _mapList[index + 1].GetMapRefsSO();
                _currentMapGO = _mapList[index + 1].GetMapGO();
                index++;
            }
            else if (_currentMapGO.transform.position.y > _player.transform.position.y)
            {
                _currentMap = _mapList[index - 1].GetMapRefsSO();
                _currentMapGO = _mapList[index - 1].GetMapGO();
                index--;

            }
            if(_currentMapGO.GetInstanceID() != _mapList[0].GetMapGO().GetInstanceID())
            {
                MapChanged();
                
            }
            OnMapChanged?.Invoke(this, new OnMapChangedEventArgs
            {
                mapRefsSO = _currentMap,
                newOrigin = _currentMapGO.transform.position,
            });
            _currentPosNode = _myGrid.NodeFromWorldPoint(_player.position);
        }
    }
    private void Update()
    {
        switch (_state)
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                _gameSettingRefsSO.countdownToStartTime.value -= Time.deltaTime;
                if(_gameSettingRefsSO.countdownToStartTime.value < 0f)
                {
                    _state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty); 
                }
                break;
            case State.GamePlaying:
                MovementController();
                ChangePlayerSprite();
                LocateNewPlace();
                UpdateAction();
                if(_gameSettingRefsSO.currentHealth.value <= 0)
                {
                    _state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
    }

    private void MovementController()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            _mouseWorldPosition = GetMouseWorldPosition();
            _moveDir = (_mouseWorldPosition - _player.position).normalized;
            if (Mathf.Abs(_moveDir.x) > Mathf.Abs(_moveDir.y))
            {
                if (_moveDir.x < 0)
                {
                    _nextMoveNode = _currentPosNode.neighbours[1];
                    _lastMoveDir = Vector3.left;
                }
                if (_moveDir.x > 0)
                {
                    _nextMoveNode = _currentPosNode.neighbours[6];
                    _lastMoveDir = Vector3.right;
                }
            }
            if (Mathf.Abs(_moveDir.x) < Mathf.Abs(_moveDir.y))
            {
                if (_moveDir.y < 0)
                {
                    _nextMoveNode = _currentPosNode.neighbours[3];
                    _lastMoveDir = Vector3.down;
                }
                if (_moveDir.y > 0)
                {
                    _nextMoveNode = _currentPosNode.neighbours[4];
                    _lastMoveDir = Vector3.up;
                }
            }
            Movement();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _nextMoveNode = _currentPosNode.neighbours[4];
            _lastMoveDir = Vector3.up;
            Movement();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _nextMoveNode = _currentPosNode.neighbours[3];
            _lastMoveDir = Vector3.down;
            Movement();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _nextMoveNode = _currentPosNode.neighbours[1];
            _lastMoveDir = Vector3.left;
            Movement();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _nextMoveNode = _currentPosNode.neighbours[6];
            _lastMoveDir = Vector3.right;
            Movement();
        }

        _velocityCam = (_cameraFollow.position - _mainCamera.transform.position) * 3;
        _mainCamera.transform.position = Vector3.SmoothDamp(_mainCamera.transform.position, _cameraFollow.position, ref _velocityCam, 1f, Time.deltaTime);
    }

    private IEnumerator CameraShake()
    {
        Vector3 originalPos = _mainCamera.transform.position;
        float duration = .15f;
        float elapsed = 0f;
        float shakeAmount = .1f;
        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(originalPos.x - shakeAmount, originalPos.x + shakeAmount);
            float y = UnityEngine.Random.Range(originalPos.y - shakeAmount, originalPos.y + shakeAmount);
            _mainCamera.transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }

    private void Movement()
    {
        if (_nextMoveNode.Iswalkable() && !_currentPosNode.IsDead())
        {
            _player.position = Vector3.MoveTowards(_player.position, _nextMoveNode.GetWorldPos(), 2f);
            _currentPosNode = _nextMoveNode;
            if (!_nextMoveNode.IsDead()) _checkPointNode = _nextMoveNode;
            if(_nextMoveNode.IsDead()) StartCoroutine(DelayCheckPoint(1f));
        }
        if (!_nextMoveNode.Iswalkable())
        {
            StartCoroutine(CameraShake());
        }
    }

    private int _runBackSprite = 1;
    private int _runFontSpite = 3;
    private int _runSideSprite = 5;
    private float _delayChangeMax = .1f;
    private float _delayChange;
    private void ChangePlayerSprite()
    {
        if (_lastMoveDir.x != 0)
        {
            if (_lastMoveDir.x > 0)
            {
                _player.transform.localScale = new Vector3(1, 1, 1);
            } else
            {
                _player.transform.localScale = new Vector3(-1, 1, 1);
            }
            _delayChange += Time.deltaTime;
            if (_delayChange > _delayChangeMax)
            {
                _delayChange -= _delayChangeMax;
                _playerSprite.sprite = _playerImages[_runSideSprite].sprite;
                _runSideSprite++;
            }
            if (_runSideSprite > 8) _runSideSprite = 5;
        }
        if (_lastMoveDir.y != 0)
        {
            if (_lastMoveDir.y > 0)
            {
                _delayChange += Time.deltaTime;
                if (_delayChange > _delayChangeMax * 2)
                {
                    _delayChange -= _delayChangeMax * 2;
                    _playerSprite.sprite = _playerImages[_runBackSprite].sprite;
                    _runBackSprite++;
                }
                if (_runBackSprite > 2) _runBackSprite = 1;
            }
            if (_lastMoveDir.y < 0)
            {
                _delayChange += Time.deltaTime;
                if (_delayChange > _delayChangeMax * 2)
                {
                    _delayChange -= _delayChangeMax * 2;
                    _playerSprite.sprite = _playerImages[_runFontSpite].sprite;
                    _runFontSpite++;
                }
                if (_runFontSpite > 4) _runFontSpite = 3;
            }
        }
    }

    IEnumerator DelayCheckPoint(float delayTimer)
    {
        if(!_gameSettingRefsSO.isProtectedPlayer.value)
        {
            _gameSettingRefsSO.currentHealth.value--;
            OnHeartChanged?.Invoke(this, EventArgs.Empty);
            OnSpawnPopup?.Invoke(this, new OnSpawnPopupEventArgs
            {
                newPopups = _popupList,
                type = 1,
                poupPos = PlayerPosToScreenPos()
            });
        }
        Color newColor = Color.white;
        newColor.a = 0;
        yield return new WaitForSeconds(delayTimer / 3);
        _playerSprite.color = newColor;
        yield return new WaitForSeconds(delayTimer);
        _player.position = _checkPointNode.GetWorldPos();
        _playerSprite.color = Color.white;
        _currentPosNode = _checkPointNode;
        StartCoroutine(ProtectTimer(_gameSettingRefsSO.playerProtectedTime.value));
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f;
        return worldPosition;
    }

    private void IncScore(int scoreAmount)
    {
        _gameSettingRefsSO.currentScore.value += scoreAmount;
    }

    public MapRefsSO CurrentMap(GameObject currentMap)
    {
        for(int i = 0; i < _mapList.Count;i++)
        {
            if(currentMap.transform.position.y == _mapList[i].GetMapPosition().y)
            {
                return _mapList[i].GetMapRefsSO();
            }
        }
        return _currentMap;
    }

    public bool IsHomeState()
    {
        return _state == State.Home;
    }

    public bool IsWaitingToStartState()
    {
        return _state == State.WaitingToStart;
    }

    public bool IsCountdownToStartState()
    {
        return _state == State.CountdownToStart;
    }

    public bool IsGamePlayingState()
    {
        return _state == State.GamePlaying;
    }

    public bool IsGameOverState()
    {
        return _state == State.GameOver;
    }

    private void UpdateAction()
    {
        for(int i = 0; i < _obstaclesList.Count;i++)
        {
            if (_obstaclesList[i].GetActivateState() == true && Mathf.Abs(_player.position.y - _obstaclesList[i].GetWorldPosition().y) < _gameSettingRefsSO.mapHeight.value) 
            {
                _obstaclesList[i].Act();
            }
        }
        for(int i = 0; i < _itemList.Count;i++) {
            if (_itemList[i].GetActivateState() == true && Vector3.Distance(_player.position, _itemList[i].GetWorldPosition()) < _gameSettingRefsSO.mapHeight.value / 10)
            {
                _itemList[i].Act();
            }
        }
        for(int i = 0; i < _popupList.Count;i++)
        {
            if (_popupList[i].GetActivateState() == true)
            {
                _popupList[i].Act();
            }
        }
    }

    private void AddItem(Vector3 pos, ItemRefsSO itemRefsSO, int itemVisual, Transform parent)
    {
        bool isAdded = false;
        for(int i = 0; i < _itemList.Count; i++)
        {
            if (_itemList[i].GetActivateStateGlobal() == false)
            {
                _itemList[i].Active(pos, parent, itemRefsSO);
                _itemList[i].SettingSprite(_itemImages[itemVisual].sprite);
                isAdded = true;
                break;
            }
        }
        if(!isAdded)
        {
            _itemList.Add(Factory.CreateItem(GameObject.Instantiate(_itemGO, parent), pos, itemRefsSO));
            _itemList[_itemList.Count - 1].SettingSprite(_itemImages[itemVisual].sprite);
        }
    }

    private float GroundRotationAngle(int groundPos, int groundVisual)
    {
        float angle = 0f;
        if(groundVisual == (int)MapRefsSO.GroundInfo.GroundVisual.OneBorder)
        {
            if (groundPos == (int)MapRefsSO.GroundInfo.GroundPos.Top) angle = 0f;
            if (groundPos == (int)MapRefsSO.GroundInfo.GroundPos.Left) angle = 90f;
            if (groundPos == (int)MapRefsSO.GroundInfo.GroundPos.Right) angle = -90f;
            if (groundPos == (int)MapRefsSO.GroundInfo.GroundPos.Bottom) angle = 180f;
        }
        if(groundVisual == (int)MapRefsSO.GroundInfo.GroundVisual.TwoAdjacentBorder)
        {
            if (groundPos == (int)MapRefsSO.GroundInfo.GroundPos.TopLeft) angle = 0f;
            if (groundPos == (int)MapRefsSO.GroundInfo.GroundPos.TopRight) angle = -90f;
            if (groundPos == (int)MapRefsSO.GroundInfo.GroundPos.BottomLeft) angle = 90f;
            if (groundPos == (int)MapRefsSO.GroundInfo.GroundPos.BottomRight) angle = 180f;
        }
        if(groundVisual == (int)MapRefsSO.GroundInfo.GroundVisual.TwoParallelBorder)
        {
            if (groundPos == (int)MapRefsSO.GroundInfo.GroundPos.Top) angle = 0f;
            if (groundPos == (int)MapRefsSO.GroundInfo.GroundPos.Left) angle = 90f;
        }
        if(groundVisual == (int)MapRefsSO.GroundInfo.GroundVisual.ThereBorder)
        {
            if (groundPos == (int)MapRefsSO.GroundInfo.GroundPos.Top) angle = 90f;
            if (groundPos == (int)MapRefsSO.GroundInfo.GroundPos.Left) angle = 180f;
            if (groundPos == (int)MapRefsSO.GroundInfo.GroundPos.Right) angle = 0f;
            if (groundPos == (int)MapRefsSO.GroundInfo.GroundPos.Bottom) angle = -90f;
        }

        return angle;
    }

    private void AddGround(Vector3 pos, WallRefsSO wallRefsSO, int groundPos, int groundVisual, Transform parent)
    {
        bool isAdded = false;
        for(int i = 0; i < _groundList.Count; i++)
        {
            if (!_groundList[i].GetActivateState())
            {
                _groundList[i].Activate(pos, wallRefsSO, parent, GroundRotationAngle(groundPos, groundVisual));
                _groundList[i].SettingSprite(_groundImages[groundVisual].sprite);
                isAdded = true;
                break;
            }
        }
        if(!isAdded)
        {
            _groundList.Add(Factory.CreateGround(GameObject.Instantiate(_groundGO, parent), pos, wallRefsSO, GroundRotationAngle(groundPos, groundVisual)));
            _groundList[_groundList.Count - 1].SettingSprite(_groundImages[groundVisual].sprite);
        }
    }
    private void AddWall(Vector3 pos, WallRefsSO wallRefsSO, int wallVisual, Transform parent)
    {
        bool isAdded = false;
        for(int i = 0; i < _wallsList.Count; i++)
        {
            if (!_wallsList[i].GetActivateState())
            {
                _wallsList[i].Activate(pos, wallRefsSO, parent);
                _wallsList[i].SettingSprite(_wallImages[wallVisual].sprite);
                isAdded = true;
                break;
            }
        }
        if(!isAdded)
        {
            _wallsList.Add(Factory.CreateWall(GameObject.Instantiate(_wallGO, parent), pos, wallRefsSO));
            _wallsList[_wallsList.Count - 1].SettingSprite(_wallImages[wallVisual].sprite);
        }
    }

    private void AddObstacle(Vector3 pos, WallRefsSO wallRefsSO, Transform parent, int obstacleVisual, int obstacleType, Vector3 moveDir, float moveDst)
    {
        bool isAdded = false;
        for (int i = 0; i < _obstaclesList.Count; i++)
        {
            if (!_obstaclesList[i].GetActivateState())
            {
                _obstaclesList[i].Activate(pos, wallRefsSO, parent, obstacleType, moveDir, moveDst);
                _obstaclesList[i].SettingSprite(_obstacleImages[obstacleVisual].sprite);
                isAdded = true;
                break;
            }
        }
        if (!isAdded)
        {
            _obstaclesList.Add(Factory.CreateObstacle(GameObject.Instantiate(_obstacleGO, parent), pos, wallRefsSO, obstacleType, moveDir, moveDst));
            _obstaclesList[_obstaclesList.Count - 1].SettingSprite(_obstacleImages[obstacleVisual].sprite);
        }
    }

    private void AddMap(Vector3 pos, MapRefsSO mapRefsSO)
    {
        GameObject newMap = Instantiate(_mapGO, _world);
        _mapList.Add(Factory.CreateMap(newMap, pos, mapRefsSO));
        foreach (var wall in mapRefsSO.wallInfo)
        {
            AddWall(new Vector3(wall.posX, wall.posY), wall.wallRefsSO, (int)wall.wallVisual, newMap.transform);
        }
        foreach (var obstacle in mapRefsSO.obstacleInfo)
        {
            AddObstacle(new Vector3(obstacle.posX, obstacle.posY), obstacle.wallRefsSO, newMap.transform, (int)obstacle.obstacleVisual, (int)obstacle.obstacleType, obstacle.moveDir, obstacle.moveDistance.Value);
        }
        foreach(var ground in mapRefsSO.groundInfo)
        {
            AddGround(new Vector3(ground.posX, ground.posY), ground.wallRefsSO, (int)ground.groundPos, (int)ground.groundVisual, newMap.transform);
        }
        foreach(var item in mapRefsSO.itemInfo)
        {
            AddItem(new Vector3(item.posX, item.posY), item.itemRefsSO, (int)item.itemVisual, newMap.transform);
        }
    }

    private void MapChanged()
    {
        StartCoroutine(DelayMapChanged());   
    }

    IEnumerator DelayMapChanged()
    {
        for (int i = 0; i < _mapList.Count;i++)
        {
            if (Vector3.Distance(_mapList[i].GetMapPosition(), _currentMapGO.transform.position) > _gameSettingRefsSO.mapHeight.value * 2)
            {
                _mapList[i].Deactivate();
            }
        }
        yield return null;
        for (int i = 0; i < 2; i++)
        {
            Transform child = _currentMapGO.transform.GetChild(i);
            bool detectPlace = false;
            foreach(var map in  _mapList)
            {
                if(map.GetMapPosition() == child.position && map.GetActivateState())
                {
                    detectPlace = true;
                    break;
                }
            }
            bool hasMap = false;
            if(!detectPlace)
            {
                for (int j = 0; j < _mapList.Count; j++)
                {
                    if (_mapList[j].GetMapPosition() == child.position)
                    {
                        _mapList[j].Activate();
                        LoadStuffToMap(_mapList[j]);
                        hasMap = true;
                        break;
                    }
                }
                if(!hasMap)
                {
                    AddMap(child.position, _mapRefsSO[UnityEngine.Random.Range(1, _mapRefsSO.Length)]);
                }
            }
        }
    }

    private void LoadStuffToMap(IMap map)
    {
        foreach (var wall in map.GetMapRefsSO().wallInfo)
        {
            bool hasWall = false;
            foreach (var existedWall in _wallsList)
            {
                if (existedWall.GetWorldPosition().x == wall.posX + map.GetMapPosition().x && existedWall.GetWorldPosition().y == wall.posY + map.GetMapPosition().y && existedWall.GetActivateState())
                {
                    hasWall = true;
                    break;
                }
            }
            if (!hasWall)
            {
                AddWall(new Vector3(wall.posX, wall.posY), wall.wallRefsSO, (int)wall.wallVisual, map.GetMapGO().transform);
            }
        }

        foreach (var obstacle in map.GetMapRefsSO().obstacleInfo)
        {
            bool hasObstacle = false;
            foreach (var existedObstacle in _obstaclesList)
            {
                if (existedObstacle.GetWorldPosition().x == obstacle.posX + map.GetMapPosition().x && existedObstacle.GetWorldPosition().y == obstacle.posY + map.GetMapPosition().y && existedObstacle.GetActivateState())
                {
                    hasObstacle = true;
                    break;
                }
            }
            if (!hasObstacle)
            {
                AddObstacle(new Vector3(obstacle.posX, obstacle.posY), obstacle.wallRefsSO, map.GetMapGO().transform, (int)obstacle.obstacleVisual, (int)obstacle.obstacleType, obstacle.moveDir, obstacle.moveDistance.Value);
            }
        }

        foreach (var ground in map.GetMapRefsSO().groundInfo)
        {
            bool hasGround = false;
            foreach (var existedGround in _groundList)
            {
                if (existedGround.GetWorldPosition().x == ground.posX + map.GetMapPosition().x && existedGround.GetWorldPosition().y == ground.posY + map.GetMapPosition().y && existedGround.GetActivateState())
                {
                    hasGround = true;
                    break;
                }
            }
            if (!hasGround)
            {
                AddGround(new Vector3(ground.posX, ground.posY), ground.wallRefsSO, (int)ground.groundPos, (int)ground.groundVisual, map.GetMapGO().transform);
            }
        }

        foreach (var item in map.GetItemExistedInfo())
        {
            bool hasItem = false;
            foreach (var existedItem in _itemList)
            {
                if (existedItem.GetWorldPosition().x == item.posX + map.GetMapPosition().x && existedItem.GetWorldPosition().y == item.posY + map.GetMapPosition().y && existedItem.GetActivateState())
                {
                    hasItem = true;
                    break;
                }
            }
            if (!hasItem)
            {
                AddItem(new Vector3(item.posX, item.posY), item.itemRefsSO, (int)item.itemVisual, map.GetMapGO().transform);
            }
        }
    }
}
