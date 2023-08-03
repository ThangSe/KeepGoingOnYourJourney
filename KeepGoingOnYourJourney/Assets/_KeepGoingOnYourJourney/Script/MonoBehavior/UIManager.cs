using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _fullHeartImgRef, _emptyHeartImgRef, _scorePlusImgRef, _loseHeartImgRef;
    [SerializeField] private Image[] _hearts;
    [SerializeField] private Text _scoreText, _gameEndText, _gameEndScoreText;
    [SerializeField] private Transform _textSpawnPos;
    [SerializeField] private GameObject _backgroundPanel, _homePage, _tutorialPage, _basePage, _gamePage, _gameEndPage;

    [SerializeField] private GameObject _popupGO;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioGamePlay, _audioPoint, _audioLoseHeart, _audioGameOver;

    [SerializeField] private FloatReference currentHealth, currentScore;


    private void Start()
    {
        _scoreText.text = currentScore.Value.ToString();
        MainManager.Instance.OnStateChanged += MainManager_OnStateChanged;
        MainManager.Instance.OnHeartChanged += Instance_OnHeartChanged;
        MainManager.Instance.OnSpawnPopup += MainManager_OnSpawnPopup;
        MainManager.Instance.OnIncreaseScore += MainManager_OnIncreaseScore;
        for(int i = 0; i < _hearts.Length; i++)
        {
            _hearts[i].sprite = _fullHeartImgRef.sprite;
        }
    }

    private void MainManager_OnIncreaseScore(object sender, System.EventArgs e)
    {
        _audioPoint.Play();
        _scoreText.text = currentScore.Value.ToString();
    }

    private void SetTimerText(float timer)
    {

    }

    private void Instance_OnHeartChanged(object sender, System.EventArgs e)
    {
        _audioLoseHeart.Play(); 
        SetUIHeart((int)currentHealth.Value);
    }
    private void SetUIHeart(int currentHeart)
    {
        if(currentHeart >= 0)
        {
            _hearts[currentHeart].sprite = _emptyHeartImgRef.sprite;
        }
    }

    private void MainManager_OnSpawnPopup(object sender, MainManager.OnSpawnPopupEventArgs e)
    {
        CreatePopup(e.newPopups, _popupGO, e.type, e.poupPos);
    }


    private void MainManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (MainManager.Instance.IsHomeState())
        {
            _homePage.SetActive(true);
            _gameEndPage.SetActive(false);
            _audioGameOver.volume = 0f;
        }
        if (MainManager.Instance.IsWaitingToStartState())
        {
            _audioGameOver.volume = 0f;
            _tutorialPage.SetActive(true);
            _homePage.SetActive(false);
            _gameEndPage.SetActive(false);
            for (int i = 0; i < _hearts.Length; i++)
            {
                _hearts[i].sprite = _fullHeartImgRef.sprite;
            }
            _scoreText.text = currentScore.Value.ToString();
        }
        if (MainManager.Instance.IsCountdownToStartState())
        {
            _audioGamePlay.time = 0f;
            if(!_audioGamePlay.isPlaying) _audioGamePlay.Play();
            _audioGamePlay.volume = 1f;
            _backgroundPanel.SetActive(false);
            _tutorialPage.SetActive(false);
            _gamePage.SetActive(true);
        }
        if (MainManager.Instance.IsGamePlayingState())
        {

        }
        if (MainManager.Instance.IsGameOverState())
        {
            _audioGamePlay.volume = 0f;
            _audioGameOver.time = 0f;
            if(!_audioGameOver.isPlaying) _audioGameOver.Play();
            _audioGameOver.volume = 1f;
            _gamePage.SetActive(false);
            _backgroundPanel.SetActive(true);
            _gameEndPage.SetActive(true);
            _gameEndScoreText.text = currentScore.Value.ToString();
        }
    }

    private void CreatePopup(List<IPopup> newPopup, GameObject popupGO, int type, Vector3 pos)
    {
        AddPopupToList(pos, newPopup, popupGO, type);
    }

    private void AddPopupToList(Vector3 pos, List<IPopup> newPopup, GameObject popupGO, int type) {
        bool isAdded = false;
        for(int i = 0; i < newPopup.Count; i++)
        {
            if (!newPopup[i].GetActivateState())
            {
                newPopup[i].Activate(pos, Vector3.zero);
                if (type == 0) newPopup[i].SettingSprite(_scorePlusImgRef.sprite);
                if (type == 1) newPopup[i].SettingSprite(_loseHeartImgRef.sprite);
                isAdded = true;
                break;
            }
        }
        if(!isAdded) {
            newPopup.Add(Factory.CreatePopup(GameObject.Instantiate(popupGO, _basePage.transform), pos));
            if (type == 0) newPopup[newPopup.Count - 1].SettingSprite(_scorePlusImgRef.sprite);
            if (type == 1) newPopup[newPopup.Count - 1].SettingSprite(_loseHeartImgRef.sprite);
        }
    }
}
