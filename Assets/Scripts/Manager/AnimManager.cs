using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimManager : MonoBehaviour
{
    public enum AnimationStates { Idle, Run, Win };

    private Animator _animator;

    //Animator Variables
    private int isIdle;
    private int isRun;
    private int isWin;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        //Animator parameter reference
        isRun = Animator.StringToHash("run");
        isWin = Animator.StringToHash("win");
        isIdle = Animator.StringToHash("idle");
    }


    public void SetAnimationState(AnimManager.AnimationStates state)
    {
        switch (state)
        {
            case AnimManager.AnimationStates.Idle:
                _animator.SetBool(isRun, false);
                _animator.SetBool(isIdle, true);
                _animator.SetBool(isWin, false);
                break;

            case AnimManager.AnimationStates.Run:
                _animator.SetBool(isRun, true);
                _animator.SetBool(isIdle, false);
                break;

            case AnimManager.AnimationStates.Win:
                _animator.SetBool(isWin, true);
                _animator.SetBool(isRun, false);
                _animator.SetBool(isIdle, false);
                break;

            default:
                break;
        }
    }
}
