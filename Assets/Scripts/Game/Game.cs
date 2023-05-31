using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.Singleton;
using UnityEngine.SceneManagement;

public class Game : Singleton<Game>
{
    [SerializeField] GameLevel _level;
    [SerializeField] Player _playerPrefab;
    [SerializeField] CameraController _cameraPrefab;


    [SerializeField] TextChannelEventSO _dialogEvents;


    // test fields
    [SerializeField] bool _testLoad;
    [SerializeField] bool _testStart;
    [SerializeField] Player _testPlayer;
    [SerializeField] CameraController _testCamera;
    [SerializeField] GameOST _gameOst;


    void Start()
    {
        if (_testLoad)  // teset game inside level scene so can start
        {
            StartLevel();
            _gameOst.StartLevelMusic();
        }
    }

    public void LoadLevel(int levelIndex)
    {
        Debug.Log($"Loading level {levelIndex}");
        SceneManager.sceneLoaded += OnLevelLoaded;
        SceneManager.LoadScene($"Level{levelIndex}", LoadSceneMode.Single);
    }

    public void RestartLevel()
    {
        LoadLevel(_level.LevelIndex);
    }

    void InitLevel()
    {
        _level = FindObjectOfType<GameLevel>();
        if (_level == null)
        {
            Debug.LogError("No GameLevel found in scene");
            return;
        }
        Debug.Log($"Init level {_level}");
        StartLevel();
    }

    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        InitLevel();
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

    public void StartLevel()
    {
        Player player;
        CameraController camera;
        if (_testLoad)
        {
            player = _testPlayer == null ? Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity) : _testPlayer;
            camera = _testCamera == null ? Instantiate(_cameraPrefab, Vector3.zero, Quaternion.identity) : _testCamera;
        }
        else
        {
            _level.CleanTestStuff();
            player = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);
            camera = Instantiate(_cameraPrefab, Vector3.zero, Quaternion.identity);
        }
        _level.SetPlayerAndCamera(player, camera);
        _level.StartLevel(_testStart);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        MutePlaneMusic(1);
    }

    public void GoToEnd()
    {
        SceneManager.LoadScene("EndScene");
    }

    public void SetPlaneMusic(PlaneMusic type, float loudness = -1, float duration = -1)
    {
        _gameOst.SetPlaneMusic(type, loudness, duration);
    }

    public void MuteAllMusic(float duration)
    {
        MutePlaneMusic(duration);
        SetPlaneMusic(PlaneMusic.BG, 0, duration);
    }

    public void MutePlaneMusic(float duration)
    {
        _gameOst.StopPlanesMusic(duration);
    }
}
