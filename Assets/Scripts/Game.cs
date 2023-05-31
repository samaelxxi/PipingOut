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
            // Debug.Log($"Test load level {_testStart}");
            StartLevel();
            _gameOst.StartLevelMusic();
        }
    }

    public void LoadLevel(int levelIndex)
    {
        // Debug.Log($"Loading level {levelIndex}");
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
        // var testPlayer = FindObjectOfType<Player>();
        // if (testPlayer != null)
        //     Destroy(testPlayer.gameObject);
        if (_level == null)
        {
            // Debug.LogError("No GameLevel found in scene");
            return;
        }
        // Debug.Log($"Init level {_level}");
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
            // Debug.Log($"Test start level {_testStart}");
            player = _testPlayer == null ? Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity) : _testPlayer;
            camera = _testCamera == null ? Instantiate(_cameraPrefab, Vector3.zero, Quaternion.identity) : _testCamera;
        }
        else
        {
            // Debug.Log($"Start level {_level.LevelIndex}");
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
        SetPlaneMusic(PlaneMusic.X, 0, 1);
        SetPlaneMusic(PlaneMusic.Y, 0, 1);
        SetPlaneMusic(PlaneMusic.Z, 0, 1);
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
        SetPlaneMusic(PlaneMusic.X, 0, duration);
        SetPlaneMusic(PlaneMusic.Y, 0, duration);
        SetPlaneMusic(PlaneMusic.Z, 0, duration);
        SetPlaneMusic(PlaneMusic.BG, 0, duration);
    }

    public void MutePlaneMusic()
    {
        SetPlaneMusic(PlaneMusic.X, 0, 1);
        SetPlaneMusic(PlaneMusic.Y, 0, 1);
        SetPlaneMusic(PlaneMusic.Z, 0, 1);
    }
}
