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

    Command StartCommand()
    {
        return MakeCommand(_startCommandType, _startIntArg, _startIntCommand, _startBoolArg, _startBoolCommand, _startObj, _startObjCommand);
    }

    Command EndCommand()
    {
        return MakeCommand(_endCommandType, _endIntArg, _endIntCommand, _endBoolArg, _endBoolCommand, _endObj, _endObjCommand);
    }

    Command MakeCommand(CommandType type, int intArg, IntCommand intCommand, bool boolArg, BoolCommand boolCommand, GameObject obj, ObjCommand objCommand)
    {
        return type switch
        {
            CommandType.Int => new Command( (CommandOnTrigger)Enum.Parse( typeof( CommandOnTrigger ), intCommand.ToString() ),
                                intArg, obj, boolArg ),
            CommandType.Bool => new Command( (CommandOnTrigger)Enum.Parse( typeof( CommandOnTrigger ), boolCommand.ToString() ),
                                intArg, obj, boolArg ),
            CommandType.GameObject => new Command( (CommandOnTrigger)Enum.Parse( typeof( CommandOnTrigger ), objCommand.ToString() ),
                                intArg, obj, boolArg ),
            _ => new Command( CommandOnTrigger.None, 0, null, false ),
        };
    }
}
