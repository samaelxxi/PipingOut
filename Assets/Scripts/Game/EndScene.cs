using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndScene : MonoBehaviour
{
    [SerializeField] LevelUI _ui;

    [SerializeField] FakePlayer _pupoOnPipes;

    [SerializeField] FakePlayer _pupoInLab;

    [SerializeField] AudioSource _doorSound;
    [SerializeField] AudioSource _spitSound;
    [SerializeField] AudioSource _endEffect1;

    [SerializeField] AudioSource _endEffect2;

    [SerializeField] Transform _movePoint;
    [SerializeField] Transform _door;

    [SerializeField] Transform _camera;
    [SerializeField] Transform _camera1Pos;
    [SerializeField] Transform _camera2Pos;
    [SerializeField] Transform _camera1Look;
    [SerializeField] Transform _camera2Look;


    void Awake()
    {
        _ui.Init();
    }

    void Start()
    {
        StartCoroutine(StartEndSequence());
    }

    IEnumerator StartEndSequence()
    {
        yield return new WaitForSeconds(1f);
        _ui.RevealScreen(1);
        _endEffect1.Play();
        yield return _ui.ShowDialogCoroutine("Oh, here we are again, in another enigmatic realm of pipes.");
        MoveCamera();
        yield return new WaitForSeconds(2f);
        yield return _ui.ShowDialogCoroutine("How peculiar! I don't recall encountering such a door in this world before.");
        _pupoOnPipes.transform.DOMove(_movePoint.position, 3f);
        _pupoOnPipes.SetMoving(true);
        yield return new WaitForSeconds(2.5f);
        _pupoOnPipes.SetMoving(false);
        yield return _ui.ShowDialogCoroutine("Well, there's nowhere else to go, it seems. Perhaps this door holds the answers I seek.");
        _ui.HideScreen(1);
        yield return new WaitForSeconds(1f);
        _doorSound.Play();
        yield return new WaitForSeconds(1f);
        _camera.position = _camera2Pos.position;
        _camera.LookAt(_camera2Look);
        _ui.RevealScreen(1);
        yield return new WaitForSeconds(1f);
        _endEffect2.Play();
        yield return _ui.ShowDialogCoroutine("How can it be?");
        yield return _ui.ShowDialogCoroutine("Am I forever trapped within this bewildering maze, doomed to wander its corridors without escape?");

        _door.transform.DOScale(Vector3.zero, 2f).SetEase(Ease.InOutElastic).WaitForCompletion();
        yield return _ui.ShowDialogCoroutine("Oh, dear heavens! The door... it has disappeared once more.");
        yield return _ui.ShowDialogCoroutine("This labyrinth, it holds an unsettling aura, a foreboding presence that haunts my every step.");

        yield return _ui.ShowDialogCoroutine("I... I can no longer bear this weight upon my soul.");
        yield return _ui.ShowDialogCoroutine("The labyrinth's grip tightens around me, and I am overcome with a sense of helplessness.");
        yield return _ui.ShowDialogCoroutine("I shall lay myself down, surrendering to the labyrinth's embrace.");
        yield return _ui.ShowDialogCoroutine("In this stillness, may I find solace and strength to face the trials that lie ahead.");
        _pupoInLab.EnableResting();
        yield return new WaitForSeconds(1f);
        _ui.HideScreen(2);
        yield return new WaitForSeconds(3f);
        Game.Instance.GoToMainMenu();
    }


    void MoveCamera()
    {    // done in last 2 hours so it's just copypaste from cameracontroller
        var playerPos = _pupoOnPipes.gameObject.transform.position;
            // Calculate the direction from the player to the exit
        var exitDir = (_camera1Pos.position - transform.position).normalized;
        var exitDist = Vector3.Distance(_camera1Pos.position, transform.position);
        var midPoint = transform.position + exitDir * exitDist / 2;
        var midToPlayerDir = (midPoint - playerPos).normalized;
        var midToPlayerDist = Vector3.Distance(midPoint, playerPos);
        var orbitPoint = midPoint + midToPlayerDir * midToPlayerDist/3;

        // Move the camera along the circular arc
        var path = new Vector3[] { transform.position, orbitPoint, _camera1Pos.position };
        // EnableThirdCamera(false);
        _camera.DOPath(path, 2, PathType.CatmullRom, PathMode.Full3D, 10, Color.red).SetEase(Ease.InOutCubic);
        // StartCoroutine(DoLookAtDuring(15, lootAtPos));
        // Rotate the camera to look at the look-at position
        _camera.DODynamicLookAt(_camera1Look.position, 2).SetEase(Ease.InOutCubic);
    }
}
