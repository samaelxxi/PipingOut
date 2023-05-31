using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TextChannelEventSO", menuName = "ScriptableObjects/TextChannelEventSO")]
public class TextChannelEventSO : ScriptableObject
{
    public event Action<List<string>, bool, Command, Command> OnTextChannelEvent;

    public void RaiseTextChannelEvent(List<string> text, bool force, Command command, Command endCommand)  // smells bad
    {
        OnTextChannelEvent?.Invoke(text, force, command, endCommand);
    }
}
