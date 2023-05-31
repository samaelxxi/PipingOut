using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] List<AudioSource> _steps;

    bool _isMoving = false;
    bool _isInAir = false;


    Animator _animator;
    int _movingHash = Animator.StringToHash("IsMoving");
    int _inAirHash = Animator.StringToHash("IsInAir");

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
        _animator.SetTrigger("IsJumped");
    }

    public void PlayStepSound()
    {
        int index = Random.Range(0, _steps.Count);
        _steps[index].pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        _steps[index].Play();
    }

    public void EnableResting()
    {
        _animator.SetBool("IsResting", true);
    }
}
