using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    public class TutorialTriggerZone : MonoBehaviour
    {
        [SerializeField] TutorialPhaseChannelSO _tutorialEventsSO;
        [SerializeField] TutorialPhase _tutorialPhase;
        [SerializeField] bool _onlyOnce = true;

        bool _isTriggered = false;

        void OnTriggerEnter(Collider other)
        {
            if (_isTriggered && _onlyOnce)
                return;
            if (other.gameObject.CompareTag("Player"))
            {
                _tutorialEventsSO.RaiseTutorialEvent(_tutorialPhase);
                _isTriggered = true;
            }
        }
    }
}
