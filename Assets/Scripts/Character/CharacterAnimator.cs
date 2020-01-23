using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator
{
    private const string ANIM_TRIGGER_NAME_IDLE = "Idle";
    private const string ANIM_TRIGGER_NAME_CROUCH_IDLE = "CrouchIdle";
    private const string ANIM_TRIGGER_NAME_RUN = "Run";
    private const string ANIM_TRIGGER_NAME_CROUCH = "Crouch";
    private const string ANIM_TRIGGER_NAME_JUMP = "Jump";
    private const string ANIM_TRIGGER_NAME_FALLING = "Falling";
    private const string ANIM_TRIGGER_NAME_SLIDE = "Slide";
    Animator animator;

    public CharacterAnimator(Animator animator)
    {
        this.animator = animator;
    }

    public void update(CharacterStates state)
    {
        //resetAnimationTriggers();
        //Debug.Log(state.ToString());
        switch (state)
        {
            case CharacterStates.STAND_IDLING:
                animator.SetTrigger(ANIM_TRIGGER_NAME_IDLE);
                break;
            case CharacterStates.CROUCH_IDLING:
                animator.SetTrigger(ANIM_TRIGGER_NAME_CROUCH_IDLE);
                break;
            case CharacterStates.RUN:
                animator.SetTrigger(ANIM_TRIGGER_NAME_RUN);
                break;
            case CharacterStates.CROUCHING:
                animator.SetTrigger(ANIM_TRIGGER_NAME_CROUCH);
                break;
            case CharacterStates.JUMPING:
                animator.SetTrigger(ANIM_TRIGGER_NAME_JUMP);
                break;
            case CharacterStates.FALLING:
                animator.SetTrigger(ANIM_TRIGGER_NAME_FALLING);
                break;
            case CharacterStates.SLIDING:
                animator.SetTrigger(ANIM_TRIGGER_NAME_SLIDE);
                break;
            default:
                throw new ArgumentOutOfRangeException( $"{nameof(state)} {state}" );
        }
    }
}
