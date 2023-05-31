using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraMover : MonoBehaviour
{
    [SerializeField] Transform _pos;
    [SerializeField] Transform _lookAt;
    [SerializeField] bool _place;

    void OnValidate()
    {
        if (_place)
        {
            _place = false;
            transform.position = _pos.position;
            transform.LookAt(_lookAt);
        }
    }
}
