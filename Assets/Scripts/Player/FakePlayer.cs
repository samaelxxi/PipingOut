using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePlayer : MonoBehaviour
{
    [SerializeField] PlayerAnimator _animator;

    public void SetMoving(bool moving)
    {
        _animator.SetMoving(moving);
    }

    public void EnableResting()
    {
        _animator.EnableResting();
    }
}
