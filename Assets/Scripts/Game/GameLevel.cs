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
    }

    public void StartLevel(bool testStart = false)
    {
        if (testStart)  // to skip initial camera animation and don't move player
        {
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
        _ui.HideScreen(1);
        _player.SetAnyInputProcessing(false);
        yield return new WaitForSeconds(1f);
        _player.TeleportTo(_respawnPos.position);
        _player.DropKey();
        _player.transform.rotation = Quaternion.identity;
        _player.CameraController.MoveOnStart(_startCameraRotation, 0);
        _player.CameraController.SetFollowing(true);
        _player.CameraController.SetLocked(false);
        yield return new WaitForSeconds(1f);
        _ui.HideHint();
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
        while (true)
        {
            yield return new WaitForEndOfFrame();
            Physics.Raycast(_player.transform.position, Vector3.down, out RaycastHit hit, 0.5f, LayerMask.GetMask("Ground") | LayerMask.GetMask("MovingPlatform"));
            if (hit.collider != null)
            {
                _player.EnableMovement();
                break;
            }
        }
    }

    protected void ShowDialog(List<string> texts, bool force, Command startCommand, Command endCommand)
    {
        ProcessCommand(startCommand);
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
        _cameraController.MoveToExit(_exitZone.ExitPos, _exitZone.CameraExitPoint, _exitZone.CameraLookAtPoint, 2f);
        _player.OnExit(_exitZone.ExitPos, () => StartCoroutine(FinishLevel()));
    }

    IEnumerator FinishLevel()
    {
        _player.gameObject.GetComponent<Rigidbody>().isKinematic = true;

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
            Game.Instance.MutePlaneMusic(1);
            _ui.HideScreen(1, () => Game.Instance.GoToEnd());
        }
    }

    void ProcessCommand(Command command)
    {
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
