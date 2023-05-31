using System.Collections;
using System.Collections.Generic;
using DesignPatterns.Singleton;
using UnityEngine;
using System;
using DG.Tweening;


public enum CameraRotation { ToZ, ToX, ToMinusZ, ToMinusX, Top }

public class GameLevel : MonoBehaviour
{
    protected Player _player;
    protected CameraController _cameraController;

    [SerializeField] FallZone _fallZone;
    [SerializeField] ExitZone _exitZone;
    [SerializeField] protected Transform _startPlayerPos;
    [SerializeField] protected Transform _startCameraPos;
    [SerializeField] protected Transform _startCameraLookAtPos;
    [SerializeField] int _levelIndex;
    [SerializeField] GameObject _uiPrefab;

    [SerializeField] protected LevelUI _ui;

    [SerializeField] protected CameraRotation _startCameraRotation;
    [SerializeField] protected Transform _respawnPos;
    [SerializeField] protected GameEventsSO _gameEvents;
    [SerializeField] Transform _enterPipe;
    [SerializeField] protected Light _sceneLight;


    public int LevelIndex => _levelIndex;

    [Header("Audio")]
    [SerializeField] AudioSource _pipeSpitSound;
    [SerializeField] AudioSource _pipeSuckSound;

    protected virtual void Awake()
    {
        // _ui = Instantiate(_uiPrefab).GetComponentInChildren<LevelUI>();
        _ui.Init();
        _gameEvents.DialogChannelEventSO.OnTextChannelEvent += ShowDialog;
        _gameEvents.CommandChannelEventSO.OnCommandTriggered += ProcessCommand;
    }

    protected virtual void OnDestroy()
    {
        _gameEvents.DialogChannelEventSO.OnTextChannelEvent -= ShowDialog;
        _gameEvents.CommandChannelEventSO.OnCommandTriggered -= ProcessCommand;
    }

    void Start()
    {
        _fallZone.OnPlayerFall += OnPlayerFall;
        _exitZone.OnPlayerExit += OnPlayerExit;
        // _ui = FindObjectsOfTypeAll<LevelUI>()[0];
        // _ui.gameObject.SetActive(true);

        // TODO remove
        Debug.Log($"Level {_levelIndex} started\nCompletedLevels: {PlayerData.CompletedLevels}");
    }

    public virtual void SetPlayerAndCamera(Player player, CameraController cameraController)
    {
        _player = player;
        _player.SetPlayerInputs(CreateInput());
        _cameraController = cameraController;
        _player.SetCameraController(_cameraController);
        _player.OnRespawnClick += () => StartCoroutine(RestartCoroutine());
        _player.OnEscClick += () => _ui.HideScreen(1, () => Game.Instance.GoToMainMenu());
        _cameraController.SetTargetToFollow(_player.transform);
    }

    protected virtual PlayerInput CreateInput()
    {
        return new PlayerInput();
    }

    public void CleanTestStuff()
    {
        var objs = FindObjectsOfType<Player>();
        foreach (var obj in objs)
        {
            Destroy(obj.gameObject);
        }
        var objs2 = FindObjectsOfType<CameraController>();
        foreach (var obj in objs2)
        {
            Destroy(obj.gameObject);
        }
        // if (_player != null) Destroy(_player.gameObject);
        // if (_cameraController != null) Destroy(_cameraController.gameObject);
    }

    public void StartLevel(bool testStart = false)
    {
        Debug.Log($"StartLevel: {testStart}");
        if (testStart)  // to skip initial camera animation
        {
            Debug.Log($"Test start level {_levelIndex}");
            _player.EnableMovement();
            _player.MakeKinematic();
            _player.CameraController.MoveOnStart(_startCameraRotation, 0);
            _player.CameraController.SetFollowing(true);
            _player.CameraController.SetLocked(false);
            _player.SetPlayerInputs(CreateInput());
            _ui.RevealScreen(0);
            return;
        }
        
        _cameraController.SetTargetToFollow(_player.transform);
        _player.CameraController.SetCameraPos(_startCameraPos.position, _startCameraLookAtPos.position);
        _player.CameraController.SetFollowing(false);
        _player.CameraController.SetLocked(true);
        _ui.HideScreen(0);
        StartCoroutine(ShowStartSequence());
    }

    protected IEnumerator RestartCoroutine()
    {
        Debug.Log($"Restart level {_levelIndex}");
        _player.SetCharacterControlsProcessing(false);
        _ui.HideScreen(1);
        _ui.HideHint();
        _player.SetAnyInputProcessing(false);
        yield return new WaitForSeconds(1f);
        _player.TeleportTo(_respawnPos.position);
        _player.DropKey();
        _player.transform.rotation = Quaternion.identity;
        _player.CameraController.MoveOnStart(_startCameraRotation, 0);
        _player.CameraController.SetFollowing(true);
        _player.CameraController.SetLocked(false);
        yield return new WaitForSeconds(1f);
        _player.SetCharacterControlsProcessing(true);
        _ui.RevealScreen(1, () => _player.SetAnyInputProcessing(true));
    }

    protected virtual IEnumerator ShowStartSequence()
    {
        _player.TeleportTo(_startPlayerPos.position);
        _player.DisableMovement();  // turn off motor to prevent player from falling
        _player.CameraController.EnableThirdCamera(false);
        yield return new WaitForSeconds(1f);
        _ui.RevealScreen(1);
        _player.SetAnyInputProcessing(false);
        yield return new WaitForSeconds(0.65f);
        _pipeSpitSound.Play();
        yield return new WaitForSeconds(0.35f);
        _player.MakeDynamic();
        SpitPlayer();
        _player.transform.rotation = _enterPipe.rotation;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(EnablePlayerOnGrounding());
        yield return new WaitForSeconds(0.5f);
        _player.CameraController.MoveOnStart(_startCameraRotation);
        yield return new WaitForSeconds(2f);
        _player.SetAnyInputProcessing(true);
        _player.MakeKinematic();
        _player.CameraController.SetFollowing(true);
        _player.CameraController.SetLocked(false);
        _player.CameraController.EnableThirdCamera(true);
    }

    IEnumerator EnablePlayerOnGrounding()
    {
        Debug.Log($"EnablePlayerOnGrounding {_player}");
        while (true)
        {
            yield return new WaitForEndOfFrame();
            Debug.Log($"{_player.transform.position}");
            Physics.Raycast(_player.transform.position, Vector3.down, out RaycastHit hit, 0.5f, LayerMask.GetMask("Ground") | LayerMask.GetMask("MovingPlatform"));
            if (hit.collider != null)
            {
                Debug.Log($"EnablePlayerOnGrounding {_player}222");
                _player.EnableMovement();
                break;
            }
        }
    }

    protected void ShowDialog(List<string> texts, bool force, Command startCommand, Command endCommand)
    {
        // if (_ui == null)  // TODO find why crashes
        // {
        //     Debug.LogWarning("The LevelUI object has been destroyed.");
        //     return;
        // }
        ProcessCommand(startCommand);
        // ProcessCommand(startCommand);
        Debug.Log($"{_ui} {_ui.gameObject.activeSelf}");
        _ui.ShowDialog(texts, force, () => ProcessCommand(endCommand));
    }


    void SpitPlayer()
    {
        _player.GetComponent<Rigidbody>().AddForce(_enterPipe.forward * 6, ForceMode.Impulse);
        _player.gameObject.transform.DORotate(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutBack);
    }

    void OnPlayerFall()
    {
        _player.OnFall();
        _ui.ShowHint("Press T to restart");
    }

    void OnPlayerExit()
    {
        _pipeSuckSound.Play();
        // StartCoroutine(Sound());
        _cameraController.MoveToExit(_exitZone.ExitPos, _exitZone.CameraExitPoint, _exitZone.CameraLookAtPoint, 2f);
        // Debug.Log($"Moving player to ");
        _player.OnExit(_exitZone.ExitPos, () => StartCoroutine(FinishLevel()));
    }

    IEnumerator FinishLevel()
    {
        _player.gameObject.SetActive(false);
        Debug.Log($"Completed levels: {PlayerData.CompletedLevels}");
        Debug.Log($"Next level: {PlayerData.NextLevel}");

        yield return new WaitForSeconds(1f);
        int nextLevel = _levelIndex + 1;
        if (nextLevel <= Globals.TOTAL_LEVELS)
        {
            if (PlayerData.CompletedLevels < _levelIndex)
                PlayerData.CompleteLevel(_levelIndex);
            _ui.HideScreen(2, () => Game.Instance.LoadLevel(nextLevel));
        }
        else
        {
            Game.Instance.MutePlaneMusic();
            _ui.HideScreen(1, () => Game.Instance.GoToEnd());
        }
    }

    void ProcessCommand(Command command)
    {
        Debug.Log($"Process command {command.CommandType}");
        if (command.CommandType == CommandOnTrigger.SetMovement)
            _player.SetAnyInputProcessing(command.BoolArg);
        else if (command.CommandType == CommandOnTrigger.SetObjectActive)
            command.Obj.SetActive(true);
        else if (command.CommandType == CommandOnTrigger.SetObjectNotActive)
            command.Obj.SetActive(false);
        else if (command.CommandType == CommandOnTrigger.EnablePlatform)
            command.Obj.GetComponent<MovingPlatform>().SetEnabled(true);
        else if (command.CommandType == CommandOnTrigger.DisablePlatform)
            command.Obj.GetComponent<MovingPlatform>().SetEnabled(false);
        else if (command.CommandType == CommandOnTrigger.ChangeStepAngle)
            _player.ChangeStepAngle(command.Argument);
    }

    public void CompleteLevel()  // cheat
    {
        var player = FindObjectOfType<Player>();
        player.TeleportTo(_exitZone.ExitPos);
    }
}
