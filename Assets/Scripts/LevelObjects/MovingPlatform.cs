using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KinematicCharacterController;

public class MovingPlatform : MonoBehaviour, IMoverController
{
    [SerializeField] float _speed = 1f;

    [SerializeField] List<Transform> _points = new();
    [SerializeField] Transform _platform;
    [SerializeField] Ease _ease = Ease.Linear;
    [SerializeField] PhysicsMover _mover;
    [SerializeField] bool _shouldLoop = true;
    // [SerializeField] Rigidbody _rigidbody;


    int _startPoint = 0;
    int _destinationPoint = 1;
    bool _forward = true;
    float _movingTime = 0;
    float _totalTime = 0;
    bool _isReady = false;

    [SerializeField] bool _isStopped = false;

    // Start is called before the first frame update
    private Transform _transform;


    void Awake()
    {
        if (!enabled)
            _mover.enabled = false;
    }

    private void Start()
    {
        _transform = _platform;

        if (_points.Count < 2)
            Debug.LogError("Not enough points for moving platform");
        else if (_points.TrueForAll(x => x == null))
            Debug.LogError("Some points are null");
        else
            _isReady = true;

        _mover.MoverController = this;
        _movingTime = 0;
        SetNewDestination(0, 1);
    }

    public void SetEnabled(bool newEnabled)
    {
        _isStopped = !newEnabled;
    }

    // This is called every FixedUpdate by our PhysicsMover in order to tell it what pose it should go to
    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        if (!_isReady || _isStopped)
        {
            goalPosition = _platform.position;
            goalRotation = _platform.rotation;
            return;
        }

        _movingTime += deltaTime;

        // Remember pose before animation
        Vector3 _positionBeforeAnim = _transform.position;
        Quaternion _rotationBeforeAnim = _transform.rotation;

        // Set our platform's goal pose to the animation's
        float easeVal = GetCurrentTargetPos();
        goalPosition = _points[_startPoint].position;
        var dir = _points[_destinationPoint].position - _points[_startPoint].position;
        // Debug.Log($"start: {goalPosition}, dir: {dir}, goal: {_points[_destinationPoint].position},  res: {dir * easeVal} {easeVal}");
        goalPosition += dir * easeVal;
        goalRotation = _transform.rotation;

        // Reset the actual transform pose to where it was before evaluating. 
        // This is so that the real movement can be handled by the physics mover; not the animation
        _transform.position = _positionBeforeAnim;
        _transform.rotation = _rotationBeforeAnim;

    }

    float GetCurrentTargetPos()
    {
        if (Vector3.Distance(_platform.position, _points[_destinationPoint].position) < 0.001f)
        {
            // Debug.Log($"{_startPoint} {_destinationPoint} pos {_platform.position} {_points[_destinationPoint].position} dist {Vector3.Distance(_platform.position, _points[_destinationPoint].position)}");

            int nextPoint;
            if (_shouldLoop)
                nextPoint = (_destinationPoint + 1) % _points.Count;
            else
            {
                nextPoint = _destinationPoint + (_forward ? 1 : -1);
                if (nextPoint >= _points.Count || nextPoint < 0)
                {
                    _forward = !_forward;
                    nextPoint = _destinationPoint + (_forward ? 1 : -1);
                }
            }
            SetNewDestination(_destinationPoint, nextPoint);
        }
        // Debug.Log($"Ease of {_movingTime / _totalTime}");
        var ease = DOVirtual.EasedValue(0, 1, _movingTime / _totalTime, _ease);
        return ease;
    }

    void SetNewDestination(int start, int destination)
    {

        _startPoint = start;
        _destinationPoint = destination;
        // Debug.Log($"not start is {_points[_startPoint].position} and dest is {_points[_destinationPoint].position}");
        float totalDist = Vector3.Distance(_platform.position, _points[_destinationPoint].position);
        _movingTime = 0;
        _totalTime = totalDist / _speed;
    }

    // void MoveToNextPoint()
    // {
    //     int nextPoint = _startPoint + (_forward ? 1 : -1);
    //     if (nextPoint >= _points.Count || nextPoint < 0)
    //     {
    //         _forward = !_forward;
    //         nextPoint = _startPoint + (_forward ? 1 : -1);
    //     }
    //     float dist = Vector3.Distance(_platform.position, _points[nextPoint].position);
    //     var tween = _platform.transform.DOMove(_points[nextPoint].position, dist / _speed).SetEase(_ease);
    //     tween.OnComplete(() => { _startPoint = nextPoint; MoveToNextPoint(); });
    // }

    void OnDrawGizmosSelected()
    {
        if (_points.Count < 2 || !_points.TrueForAll(p => p != null))
            return;

        for (int i = 0; i < _points.Count - 1; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_points[i].position, _points[i + 1].position);
        }
    }
}
