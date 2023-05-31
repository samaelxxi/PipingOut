using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class CameraController : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;
    [SerializeField] Camera _playerCamera;
    [SerializeField] Camera _thirdCamera;
    [SerializeField] Transform _target;
    [SerializeField] float _playerDist = 0;

    bool _isMoving = false;
    bool _isTopView = true;
    public bool IsTopView => _isTopView;
    int _rotation = 0;
    public int Rotation => _rotation;

    bool _isFollowing = false;
    bool _isLocked = false;
    [SerializeField] float _rotationTime = 0.5f;
    Quaternion _rotateTo;

    public event System.Action OnCameraRotated;
    public bool IsCanMove => !_isMoving && !_isLocked;

    Tweener _cameraTween;

    void Start()
    {
        _playerCamera.enabled = false;
        UseMainCameraOnly();  // to make player invisible inside pipes
    }

    void Update()
    {
        _thirdCamera.nearClipPlane = _mainCamera.nearClipPlane;
        if (!_isFollowing)
            return;

        SmoothFollow();
        if (!_isTopView && !_isMoving)
        {
            _mainCamera.nearClipPlane = CalcNearPlane();
        }
    }

    public void SetNearPlane(float nearPlane)
    {
        _mainCamera.nearClipPlane = nearPlane;
        _thirdCamera.nearClipPlane = nearPlane;
    }

    public void EnableThirdCamera(bool enable)
    {
        _thirdCamera.enabled = enable;
    }

    public void SetTargetToFollow(Transform target)
    {
        _target = target;
    }

    void UsePlayerCamera()
    {
        _playerCamera.enabled = true;
        _mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
    }

    void UseMainCameraOnly()
    {
        _playerCamera.enabled = false;
        _mainCamera.cullingMask |= 1 << LayerMask.NameToLayer("Player");
    }

    public void SetFollowing(bool isFollowing)
    {
        _isFollowing = isFollowing;
    }

    public void SetLocked(bool isLocked)
    {
        _isLocked = isLocked;
    }

    public bool TryRotateCamera(CameraRotationCommand command)
    {
        if (!IsCanMove || 
                command == CameraRotationCommand.None ||
                (IsTopView && (command == CameraRotationCommand.Left || 
                               command == CameraRotationCommand.Right)))
            return false;

        switch (command)
        {
            case CameraRotationCommand.Left:
                RotateHorizontal(true);
                break;
            case CameraRotationCommand.Right:
                RotateHorizontal(false);
                break;
            case CameraRotationCommand.Top:
                RotateVertical();
                break;
        }
        return true;
    }

    void RotateVertical()
    {
        _rotateTo = transform.rotation * Quaternion.Euler((_isTopView ? -90 : 90), 0, 0);
        _isMoving = true;
        DOTween.Sequence().AppendInterval(_rotationTime / 2).OnComplete(delegate 
        {
            if (!_isTopView)            // All this thing is needed to hide entities which are closer 
                // UseMainCameraOnly();   // to camera than player during sideview while not cut player
            // else
                UsePlayerCamera();
        }).Play();

        _cameraTween?.Kill();
        _cameraTween = transform.DORotateQuaternion(_rotateTo, _rotationTime).SetEase(Ease.InOutCubic).OnComplete(RotationEnd);

        _isTopView = !_isTopView;
        SetPlaneMusic();
        if (_isTopView)
            SetTopViewPlane(_rotationTime);
        else
            StartCoroutine(CameraNearClipMove());
    }

    void SetPlaneMusic()
    {
        var game = Game.Instance;
        if (_isTopView)
        {
            game.SetPlaneMusic(PlaneMusic.X);
            game.SetPlaneMusic(PlaneMusic.Y, 0);
            game.SetPlaneMusic(PlaneMusic.Z);
        }
        else
        {
            game.SetPlaneMusic(PlaneMusic.Y);
            if (_rotation % 2 == 0)
            {
                game.SetPlaneMusic(PlaneMusic.X);
                game.SetPlaneMusic(PlaneMusic.Z, 0);
            }
            else
            {
                game.SetPlaneMusic(PlaneMusic.X, 0);
                game.SetPlaneMusic(PlaneMusic.Z);
            }
        }
    }

    void RotateHorizontal(bool toLeft)
    {
        _rotateTo = transform.rotation * Quaternion.Euler(0, 90 * (toLeft ? 1 : -1), 0);
        _rotation = (_rotation + (toLeft ? 1 : -1) + 4) % 4;
        _cameraTween?.Kill();
        _cameraTween = transform.DORotateQuaternion(_rotateTo, _rotationTime).SetEase(Ease.InOutCubic).OnComplete(RotationEnd);
        StartCoroutine(CameraNearClipMove());
        _isMoving = true;
        SetPlaneMusic();
    }

    public void OnFall()
    {
        SetLocked(true);
        SetFollowing(false);
        var finalPos = _target.position;
        finalPos.y += 3 + CalcNearPlane();  // TODO play with near clip plane so that lower pipes dont dissapear
        transform.DOMove(finalPos, 1.5f).SetEase(Ease.OutCubic);
        _cameraTween?.Kill();
        _cameraTween = transform.DORotate(new Vector3(90, 0, 0), 1.5f).SetEase(Ease.OutCubic);
    }

    public void OnExit(Vector3 exitPos, Vector3 lootAtPos, float duration)
    {
        SetLocked(true);
        SetFollowing(false);

        transform.DOMove(exitPos, duration).SetEase(Ease.OutCubic).OnComplete(RotationEnd);
        var ofset = lootAtPos - exitPos;
        transform.DOLookAt(transform.position + ofset, duration).SetEase(Ease.OutCubic).OnComplete(UseMainCameraOnly);
        _mainCamera.DONearClipPlane(1, duration).SetEase(Ease.OutCubic);
    }

        public void MoveToExit(Vector3 playerPos, Vector3 exitPos, Vector3 lootAtPos, float duration)
        {
            SetLocked(true);
            SetFollowing(false);

            var exitDir = (exitPos - transform.position).normalized;
            var exitDist = Vector3.Distance(exitPos, transform.position);
            var midPoint = transform.position + exitDir * exitDist / 2;
            var midToPlayerDir = (midPoint - playerPos).normalized;
            var midToPlayerDist = Vector3.Distance(midPoint, playerPos);
            var orbitPoint = midPoint + midToPlayerDir * midToPlayerDist/3;

            // Move the camera along the circular arc
            var path = new Vector3[] { transform.position, orbitPoint, exitPos };
            transform.DOPath(path, duration, PathType.CatmullRom, PathMode.Full3D, 10, Color.red).SetEase(Ease.InOutCubic).OnComplete(RotationEnd);
            transform.DODynamicLookAt(lootAtPos, duration).SetEase(Ease.InOutCubic).OnComplete(UseMainCameraOnly);
            _mainCamera.DONearClipPlane(1, duration).SetEase(Ease.OutCubic);
        }

    void StartMoveToTopView(float duration = 2)
    {
        _isTopView = true;
        _isMoving = true;
        var topPos = _target.position + Vector3.up * _playerDist;
        _rotation = 0;

        transform.DOMove(topPos, duration).SetEase(Ease.OutCubic).OnComplete(RotationEnd);
        _cameraTween?.Kill();
        _cameraTween = transform.DORotate(new Vector3(90, 0, 0), duration).SetEase(Ease.OutCubic);
        SetTopViewPlane();
    }

    public void SetCameraPos(Vector3 pos, Vector3 lookAt)
    {
        transform.position = pos;
        transform.LookAt(lookAt);
    }

    void MoveToSideView(int newRotation, float duration = 2)
    {
        _isTopView = false;
        _isMoving = true;
        _rotation = newRotation;

        Quaternion rotation = Quaternion.Euler(0, 90 * newRotation, 0);
        Vector3 direction = rotation * Vector3.forward;
        Vector3 sidePos = _target.position - direction * _playerDist;
        // TODO adjust near clip movement
        transform.DOMove(sidePos, duration).SetEase(Ease.InOutCubic).OnComplete(RotationEnd);
        _cameraTween?.Kill();
        _cameraTween = transform.DORotate(new Vector3(0, 90 * newRotation, 0), duration).SetEase(Ease.InOutCubic).OnComplete(()=>CameraNearClipMove());
        UsePlayerCamera();
    }

    public void MoveOnStart(CameraRotation rotation, float duration = 2)
    {
        Debug.Log($"MoveOnStart {rotation}");
        if (rotation == CameraRotation.Top)
            StartMoveToTopView(duration);
        else
            MoveToSideView((int)rotation, duration);
        SetPlaneMusic();
    }

    void SmoothFollow()
    {
        Vector3 targetPos =  _target.position - transform.forward * _playerDist;
        Vector3 smoothFollow = Vector3.Lerp(transform.position, targetPos, 0.1f);
        transform.position = smoothFollow;
    }

    float CalcNearPlane()
    {
        float res = 0;
        if ((_rotation == 0 && _target.position.z >= 0) || (_rotation == 2 && _target.position.z < 0))
            res = _playerDist - Mathf.Abs(_target.position.z % 1);
        else if ((_rotation == 2 && _target.position.z >= 0) || (_rotation == 0 && _target.position.z < 0))
            res = _playerDist - (1 - Mathf.Abs(_target.position.z % 1));
        else if ((_rotation == 1 && _target.position.x >= 0) || (_rotation == 3 && _target.position.x < 0))
            res = _playerDist -  Mathf.Abs(_target.position.x % 1);
        else if ((_rotation == 3 && _target.position.x >= 0) || (_rotation == 1 && _target.position.x < 0))
            res = _playerDist - (1 - Mathf.Abs(_target.position.x % 1));
        return res;
    }

    IEnumerator CameraNearClipMove()
    {
        float t = 0;
        float nearPlane = _mainCamera.nearClipPlane;
        while (t < _rotationTime)
        {
            t += Time.deltaTime;
            var percentage = DOVirtual.EasedValue(0, 1, t / _rotationTime, Ease.InCubic);
            _mainCamera.nearClipPlane = Mathf.Lerp(nearPlane, CalcNearPlane(), percentage);
            yield return null;
        }
        _mainCamera.nearClipPlane = CalcNearPlane();
    }

    void SetTopViewPlane(float duration = 1)
    {
        _mainCamera.DONearClipPlane(4f, duration).SetEase(Ease.InOutCubic);
    }

    void RotationEnd()
    {
        _isMoving = false;
        OnCameraRotated?.Invoke();
    }
}
