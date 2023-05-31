using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns.Singleton;
using DG.Tweening;
using KinematicCharacterController;
using UnityEngine;


[SelectionBase]
public class Player: MonoBehaviour
{
    [SerializeField] Transform _keyPlace;
    [SerializeField] PlayerAnimator _animator;


    CameraController _cameraController;

    HatCharacterController _hatController;
    KinematicCharacterMotor _motor;
    Rigidbody _rigidbody;


    bool IsTopView => _cameraController.IsTopView;
    int CameraRotation => _cameraController.Rotation;

    bool _shouldProcessInput = true;

    PlayerInput _playerInput;
    PlayerCharacterInputs _characterInputs = new();
    

    float _timeInAir = 0;


    public CameraController CameraController => _cameraController;
    public event Action OnRespawnClick;
    public event Action OnEscClick;


    [SerializeField] AudioSource _jumpSound;
    [SerializeField] AudioSource _landSound;

    Key _key;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _motor = GetComponent<KinematicCharacterMotor>();
        _hatController = GetComponent<HatCharacterController>();
        _hatController.OnJumpPerformed += OnJumpPerformed;
    }

    public void SetIsMuted(bool isMuted)
    {
        _jumpSound.mute = isMuted;
        _landSound.mute = isMuted;
    }

    public void SetPlayerInputs(PlayerInput playerInput)
    {
        _playerInput = playerInput;
    }

    public void TeleportTo(Vector3 pos)
    {
        _motor.SetPosition(pos);
    }

    public void SetCameraController(CameraController cameraController)
    {
        _cameraController = cameraController;
        _cameraController.OnCameraRotated += delegate { SetCharacterControlsProcessing(true); };
    }

    public void DisableMovement()
    {
        _motor.enabled = false;
        _hatController.enabled = false;
    }

    public void OnJumpPerformed()
    {
        _animator.PlayJump();
        _jumpSound.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        _jumpSound.Play();
    }

    public void SetAnyInputProcessing(bool shouldProcessInput)
    {
        _shouldProcessInput = shouldProcessInput;
        if (!_shouldProcessInput)
            _hatController.SetInputs(_playerInput.GetStopMoveAxis());
    }

    public void SetCharacterControlsProcessing(bool isCanControl)
    {
        _playerInput.SetCanControl(isCanControl);
    }

    public void EnableMovement()
    {
        _motor.enabled = true;
        _hatController.enabled = true;
        _motor.SetPosition(transform.position);
    }

    public void MakeDynamic()
    {
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
    }

    public void MakeKinematic()
    {
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
    }

    public void SetMoving(bool isMoving)
    {
        _animator.SetMoving(isMoving);
    }

    void Update()
    {
        ProcessInput();

        if (Mathf.Abs(_characterInputs.MoveAxisForward) > 0.1f || 
            Mathf.Abs(_characterInputs.MoveAxisRight) > 0.1f)
            _animator.SetMoving(true);
        else
            _animator.SetMoving(false);

        bool isGrounded = _hatController.Motor.GroundingStatus.FoundAnyGround;
        if (isGrounded)
        {
            if (_timeInAir > 0.1f)
            {
                _landSound.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                _landSound.Play();
            }
            _timeInAir = 0;
        }
        else
            _timeInAir += Time.deltaTime;


        if (!isGrounded && Vector3.Dot(_hatController.Velocity, Vector3.down) > 0)
        {   // try to predict if we will be grounded soon to play landing animation in advance
            Physics.Raycast(transform.position, _hatController.Velocity.normalized, out var hit, 10, LayerMask.GetMask("Ground"));
            bool groundingSoon = false;
            if (hit.collider != null)
            {
                var dist = Vector3.Distance(transform.position, hit.point);
                var t = dist / _hatController.Velocity.magnitude;
                groundingSoon = t < 0.2f;   // very approximate
            }
            _animator.SetInAir(!groundingSoon);
        }
        else
            _animator.SetInAir(!isGrounded);
    }

    void ProcessInput()
    {
        if (!_shouldProcessInput)
            return;

        if (_playerInput.GetRestartCommand())
            OnRespawnClick?.Invoke();

        if (_playerInput.GetEscCommand())
            OnEscClick?.Invoke();

        if (_cameraController.TryRotateCamera(_playerInput.GetCameraRotationCommand()))
            SetCharacterControlsProcessing(false);
        _characterInputs = _playerInput.GetMoveAxis(IsTopView, CameraRotation);
        _hatController.SetInputs(_characterInputs);
    }

    public void OnFall()
    {
        SetCharacterControlsProcessing(false);
        MakeDynamic();
        _cameraController.OnFall();
    }

    public void OnExit(Vector3 exitPos, Action onExitComplete)
    {
        const float MOVE_TO_EXIT_TIME = 1.5f;

        DisableMovement();
        SetAnyInputProcessing(false);
        _rigidbody.useGravity = false;
        transform.DOMove(exitPos, MOVE_TO_EXIT_TIME).SetEase(Ease.InOutSine).OnComplete(
            () => transform.DOMove(transform.position + Vector3.up * 4f, 2f).SetEase(Ease.InQuint).OnComplete(
                () => onExitComplete()));
    }

    public void PickUpKey(Key key)
    {
        _key = key;
        _key.transform.SetParent(_keyPlace);
        _key.transform.DOScale(_key.transform.localScale * 0.25f, 0.5f);
        _key.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
        _key.transform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
    }

    public bool HasAnyKey()
    {
        return _key != null;
    }

    public bool HasKey(int keyValue)
    {
        return _key != null && _key.KeyValue == keyValue;
    }

    public Key TakeKeyAway()
    {
        var key = _key;
        _key = null;
        return key;
    }

    public void DropKey()
    {
        if (_key == null)
            return;
        _key.DropKey();
        _key = null;
    }

    public void ChangeStepAngle(float stepAngle)
    {
        _hatController.Motor.MaxStableSlopeAngle = stepAngle;
    }
}
