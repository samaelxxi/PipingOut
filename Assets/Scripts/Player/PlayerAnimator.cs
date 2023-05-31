using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] List<AudioSource> _steps;

    bool _isMoving = false;
    bool _isInAir = false;


    Animator _animator;
    readonly int _movingHash = Animator.StringToHash("IsMoving");
    readonly int _inAirHash = Animator.StringToHash("IsInAir");
    readonly int _restingHash = Animator.StringToHash("IsResting");
    readonly int _jumpHash = Animator.StringToHash("IsJumped");

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetMoving(bool isMoving)
    {
        if (_isMoving == isMoving)
            return;
        _animator.SetBool(_movingHash, isMoving);
        _isMoving = isMoving;
    }

    public void SetInAir(bool isInAir)
    {
        if (_isInAir == isInAir)
            return;
        _animator.SetBool(_inAirHash, isInAir);
        _isInAir = isInAir;
    }

    public void PlayJump()
    {
        _animator.SetTrigger(_jumpHash);
    }

    public void PlayStepSound()
    {
        int index = Random.Range(0, _steps.Count);
        _steps[index].pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        _steps[index].Play();
    }

    public void EnableResting()
    {
        _animator.SetBool(_restingHash, true);
    }
}
