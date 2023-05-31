using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "CommandChannelEventSO", menuName = "ScriptableObjects/CommandChannelEventSO")]
public class CommandChannelEventSO : ScriptableObject
{
    public event Action<Command> OnCommandTriggered;

    public void RaiseTextChannelEvent(Command command)
    {
        OnCommandTriggered?.Invoke(command);
    }
}
