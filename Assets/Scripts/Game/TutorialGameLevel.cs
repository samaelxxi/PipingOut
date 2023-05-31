using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Tutorial
{
    public class TutorialGameLevel : GameLevel
    {
        [Header("Tutorial")]
        [SerializeField] GameObject _jumpTrigger;

        TutorialPlayerInput _input;


        [Header("Home Sequence")]
        [SerializeField] GameObject _house;
        [SerializeField] GameObject _fakePlayer;
        [SerializeField] Transform _exitPos;
        [SerializeField] List<AudioSource> _startSequenceSounds;
        [SerializeField] GameObject _startDoor;
        [SerializeField] Transform _homeCameraPos;
        [SerializeField] Transform _homeCameraLookAt;

        protected override void Awake()
        {
            base.Awake();
            var tutorialEvents = (TutorialEventsSO)_gameEvents;
            tutorialEvents.tutorialPhaseChannelSO.OnTutorialPhaseEvent += OnTutorialPhaseRaised;
            Game.Instance.MutePlaneMusic(0);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            var tutorialEvents = (TutorialEventsSO)_gameEvents;
            tutorialEvents.tutorialPhaseChannelSO.OnTutorialPhaseEvent -= OnTutorialPhaseRaised;
        }

        protected override PlayerInput CreateInput()
        {
            _input = new TutorialPlayerInput();
            _input.OnFirstJumpClick += OnFirstJumpClicked;
            _input.OnFirstHorizontalRotationClick += OnFirstHorizontalRotationClicked;
            _input.OnFirstVerticalRotationClick += OnFirstVerticalRotationClicked;
            _input.OnFirstRestartClick += OnFirstRestartClicked;
            return _input;
        }

        protected override IEnumerator ShowStartSequence()
        {
            _player.TeleportTo(_startPlayerPos.position);
            _player.SetIsMuted(true);
            _player.CameraController.SetNearPlane(0.01f);
            _player.CameraController.SetLocked(true);
            _player.CameraController.SetFollowing(false);
            _player.CameraController.SetCameraPos(_homeCameraPos.position, _homeCameraLookAt.position);
            _sceneLight.intensity = 0;
            _player.SetAnyInputProcessing(false);
            yield return new WaitForSeconds(1f);
            _startSequenceSounds[0].Play();
            yield return new WaitForSeconds(4f);

            yield return _ui.ShowDialogCoroutine("Ah, a splendid cup of tea, indeed.");
            _startSequenceSounds[1].Play();
            yield return new WaitForSeconds(5f);
            _ui.RevealScreen(1);
            yield return _ui.ShowDialogCoroutine("A second cup would be most delightful, I must say.");
            yield return _ui.ShowDialogCoroutine("Time to pay a visit to my dear friend for another cup of tea.");
            yield return new WaitForSeconds(1f);
            _fakePlayer.transform.DOLookAt(_exitPos.position, 0.5f).OnComplete(
                delegate
                {
                    _fakePlayer.GetComponent<FakePlayer>().SetMoving(true);
                    _fakePlayer.transform.DOMove(_exitPos.position, 3);
                }
            );
            
            yield return new WaitForSeconds(2f);
            _ui.HideScreen(1);
            yield return _ui.ShowDialogCoroutine("Farewell, my lovely abode. I shall return shortly for our next tea session.");
            yield return new WaitForSeconds(1f);
            _startSequenceSounds[2].Play();
            _sceneLight.intensity = 1;
            yield return new WaitForSeconds(7f);
            _player.CameraController.SetCameraPos(_startCameraPos.position, _startCameraLookAtPos.position);
            _ui.RevealScreen(1);
            yield return _ui.ShowDialogCoroutine("Goodness gracious! This is quite an unexpected turn of events.");
            yield return new WaitForSeconds(2f);
            _startDoor.transform.DOScale(Vector3.zero, 2f).SetEase(Ease.InOutElastic).WaitForCompletion();
            yield return new WaitForSeconds(0.5f);
            _startSequenceSounds[3].Play();
            yield return new WaitForSeconds(1.5f);
            yield return _ui.ShowDialogCoroutine("Oh, dear me! It appears I'm trapped in this peculiar place. How extraordinary!");
            Destroy(_startDoor, 2);
            _player.CameraController.MoveOnStart(_startCameraRotation);
            yield return new WaitForSeconds(2f);
            _player.SetAnyInputProcessing(true);
            _player.MakeKinematic();
            _player.CameraController.SetFollowing(true);
            _player.CameraController.SetLocked(false);
            _ui.ShowHint("Press A and D to move", 5);
            Destroy(_house);
            _player.SetIsMuted(false);
        }

        void OnTutorialPhaseRaised(TutorialPhase phase)
        {
            switch (phase)
            {
                case TutorialPhase.OpenJump:
                    EnableJump();
                    break;
                case TutorialPhase.OpenVerticalRotation:
                    StartCoroutine(EnableVerticalRotation());
                    break;
                case TutorialPhase.OpenHorizontalRotation:
                    StartCoroutine(EnableHorizontalRotation());
                    break;
                case TutorialPhase.Restart:
                    EnableRestart();
                    break;
            }
        }

        void EnableRestart()
        {
            _ui.ShowDialog("Oh, bother! Alas, I cannot rewind time to undo my misstep.", true, delegate
            {
                _jumpTrigger.SetActive(true);
                _input.EnableRestart();
                _ui.ShowHint("Press T to respawn");
            });
        }

        void EnableJump()
        {
            _player.SetAnyInputProcessing(false);
            _ui.ShowDialog("Let us try this again, shall we? This time, I shall bravely leap over the treacherous pit.", true, delegate
            {
                _input.EnableJump();
                _player.SetAnyInputProcessing(true);
                _ui.ShowHint("Press W to jump");
            });
        }

        IEnumerator EnableVerticalRotation()
        {
            yield return _ui.ShowDialogCoroutine("Ah, kind sir, I find myself in a predicament. Might you lend me your assistance?");
            yield return new WaitForSeconds(1.5f);
            _ui.ShowDialog("Oh, well. If words will not do, then perhaps I shall seek a solution in a different realm.", true, delegate
            {
                _ui.ShowHint("Press Spacebar to change dimension");
                _input.EnableVerticalRotation();
            });
        }

        IEnumerator EnableHorizontalRotation()
        {
            yield return _ui.ShowDialogCoroutine("Oh, dear. It seems my acrobatic skills won't suffice in this realm.");
            _ui.ShowDialog("Alas, I must return from whence I came and seek a different path.", true, delegate
            {
                _input.EnableHorizontalRotation();
                _ui.ShowHint("Press Q and E to change dimension");
            });
        }

        void OnFirstHorizontalRotationClicked()
        {
            _ui.HideHint();
        }

        void OnFirstVerticalRotationClicked()
        {
            _ui.ShowHint("Use WASD to move", 5);
        }

        void OnFirstJumpClicked()
        {
            _ui.HideHint();
        }

        void OnFirstRestartClicked()
        {
            _ui.HideHint();
        }
    }
}
