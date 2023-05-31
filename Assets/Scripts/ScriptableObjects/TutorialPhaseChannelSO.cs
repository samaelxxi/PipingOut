using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TutorialPhase
{
    OpenJump,
    OpenVerticalRotation,
    OpenHorizontalRotation,
    Restart
}


[CreateAssetMenu(fileName = "TutorialPhaseChannelSO", menuName = "ScriptableObjects/TutorialPhaseChannelSO")]
public class TutorialPhaseChannelSO : ScriptableObject
{
    public event Action<TutorialPhase> OnTutorialPhaseEvent;

    public void RaiseTutorialEvent(TutorialPhase text)
    {
        OnTutorialPhaseEvent?.Invoke(text);
    }
}
