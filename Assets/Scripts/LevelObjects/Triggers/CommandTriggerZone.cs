using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CommandTriggerZone : MonoBehaviour
{
    [SerializeField] CommandChannelEventSO _commandChannel;
    [SerializeField] bool _onlyOnce = true;

    [SerializeField] CommandOnTrigger _command;
    [SerializeField] int _intArg;
    [SerializeField] bool _boolArg;
    [SerializeField] GameObject _obj;


    bool _isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (_isTriggered && _onlyOnce)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            var command = new Command(_command, _intArg, gameObject, _boolArg);
            _commandChannel.RaiseTextChannelEvent(command);
            _isTriggered = true;
        }
    }
}
