using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Tutorial
{
    [CreateAssetMenu(fileName = "TutorialEventsSO", menuName = "ScriptableObjects/TutorialEventsSO")]
    public class TutorialEventsSO : GameEventsSO
    {
        public TutorialPhaseChannelSO tutorialPhaseChannelSO;
    }
}
