using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitZone : MonoBehaviour
{
    [SerializeField] Transform _cameraExitPoint;
    [SerializeField] Transform _cameraLookAtPoint;
    [SerializeField] Transform _exitPoint;
    public event Action OnPlayerExit;

    public Vector3 CameraExitPoint => _cameraExitPoint.position;
    public Vector3 CameraLookAtPoint => _cameraLookAtPoint.position;
    public Vector3 ExitPos => _exitPoint.position;

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log($"ExitZone OnTriggerEnter");
        if (other.CompareTag("Player"))
        {
            OnPlayerExit?.Invoke();
        }
    }
}
