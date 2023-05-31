using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class DoorEyes : MonoBehaviour
{
    [SerializeField] Transform _leftEye;
    [SerializeField] Transform _rightEye;
    [SerializeField] Transform _eyeTarget;


    void Update()
    {
        // if (_eyeTarget == null || !_canUpdate)
        //     return;

        Vector3 leftEyeDirection, rightEyeDirection;
        if (_eyeTarget == null)
        {
            leftEyeDirection = -transform.right;
            rightEyeDirection = -transform.right;
        }
        else
        {
            leftEyeDirection = _eyeTarget.position - _leftEye.position;
            rightEyeDirection = _eyeTarget.position - _rightEye.position;
        }

        Quaternion leftTargetRotation = Quaternion.LookRotation(leftEyeDirection, Vector3.up);
        _leftEye.rotation = Quaternion.RotateTowards(_leftEye.rotation, leftTargetRotation, 100 * Time.deltaTime);

        Quaternion rightTargetRotation = Quaternion.LookRotation(rightEyeDirection, Vector3.up);
        _rightEye.rotation = Quaternion.RotateTowards(_rightEye.rotation, rightTargetRotation, 100 * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _eyeTarget = other.transform;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            _eyeTarget = null;
    }
}
