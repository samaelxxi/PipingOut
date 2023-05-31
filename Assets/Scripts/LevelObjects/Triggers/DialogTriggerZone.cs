using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogTriggerZone : MonoBehaviour
{
    [SerializeField] TextChannelEventSO _dialogChannel;

    [Header("Dialog")]
    [SerializeField] List<string> _texts;
    [SerializeField] bool _forceShow;
    [SerializeField] bool _onlyOnce = true;
    [SerializeField] float _delay = 0;


    [Header("Start Command")]
    [SerializeField] CommandType _startCommandType;
    [SerializeField] int _startIntArg;
    [SerializeField] IntCommand _startIntCommand;
    [SerializeField] bool _startBoolArg;
    [SerializeField] BoolCommand _startBoolCommand;
    [SerializeField] GameObject _startObj;
    [SerializeField] ObjCommand _startObjCommand;

    [Header("End Command")]
    [SerializeField] CommandType _endCommandType;
    [SerializeField] int _endIntArg;
    [SerializeField]IntCommand _endIntCommand;
    [SerializeField]bool _endBoolArg;
    [SerializeField]BoolCommand _endBoolCommand;
    [SerializeField]GameObject _endObj;
    [SerializeField]ObjCommand _endObjCommand;


    bool _isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (_isTriggered && _onlyOnce)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(StartCommandAfterDelay());
            _isTriggered = true;
        }
    }

    IEnumerator StartCommandAfterDelay()
    {
        yield return new WaitForSeconds(_delay);
        _dialogChannel.RaiseTextChannelEvent(_texts, _forceShow, StartCommand(), EndCommand());
    }

    // no time to refactor this
    Command StartCommand()
    {
        return _startCommandType switch
        {
            CommandType.Int => new Command( (CommandOnTrigger)Enum.Parse( typeof( CommandOnTrigger ), _startIntCommand.ToString() ),
                                _startIntArg, _startObj, _startBoolArg ),
            CommandType.Bool => new Command( (CommandOnTrigger)Enum.Parse( typeof( CommandOnTrigger ), _startBoolCommand.ToString() ),
                                _startIntArg, _startObj, _startBoolArg ),
            CommandType.GameObject => new Command( (CommandOnTrigger)Enum.Parse( typeof( CommandOnTrigger ), _startObjCommand.ToString() ),
                                _startIntArg, _startObj, _startBoolArg ),
            _ => new Command( CommandOnTrigger.None, 0, null, false ),
        };
    }

    Command EndCommand()
    {
        return _endCommandType switch
        {
            CommandType.Int => new Command( (CommandOnTrigger)Enum.Parse( typeof( CommandOnTrigger ), _endIntCommand.ToString() ),
                                _endIntArg, _endObj, _endBoolArg ),
            CommandType.Bool => new Command( (CommandOnTrigger)Enum.Parse( typeof( CommandOnTrigger ), _endBoolCommand.ToString() ),
                                _endIntArg, _endObj, _endBoolArg ),
            CommandType.GameObject => new Command( (CommandOnTrigger)Enum.Parse( typeof( CommandOnTrigger ), _endObjCommand.ToString() ),
                                _endIntArg, _endObj, _endBoolArg ),
            _ => new Command( CommandOnTrigger.None, 0, null, false ),
        };
    }
}
