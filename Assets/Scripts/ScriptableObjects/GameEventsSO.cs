using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameEvents", menuName = "ScriptableObjects/GameEvents")]
public class GameEventsSO : ScriptableObject
{
    public TextChannelEventSO DialogChannelEventSO;
    public CommandChannelEventSO CommandChannelEventSO;
}
