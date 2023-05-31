using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallZone : MonoBehaviour
{
    public event Action OnPlayerFall;

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log($"FallZone.OnTriggerEnter");
        if (other.gameObject.CompareTag("Player"))
        {
            OnPlayerFall?.Invoke();
        }
    }
}
