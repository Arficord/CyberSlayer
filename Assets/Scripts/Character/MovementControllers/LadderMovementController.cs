using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderMovementController : MonoBehaviour, IMoveInputable
{
    private CharacterController character;
    private Rigidbody2D rb;

    public float movementSpeed_Up = 5;
    public float movementSpeed_Down = 5;
    private void Awake()
    {
        character = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody2D>();
    }
    public void onChangeMoveController()
    {
        rb.gravityScale = 0;
        rb.velocity = Vector3.zero;
    }
    public void jump()
    {
        character.changeController(CharacterController.Controllers.GROUND);
        character.jump();
    }
    public void moveHorizontal(float movementMultiplier)
    {
        //empty
    }
    public void doSpecialMoves()
    {
        //empty
    }

    public CharacterStates getState()
    {
        //empty
        return CharacterStates.CROUCH_IDLING;
    }



    public void moveVertical(float movementMultiplier)
    {
        float movementSpeed;
        if(movementMultiplier>0)
        {
            movementSpeed = movementSpeed_Up;
        }
        else
        {
            movementSpeed = movementSpeed_Down;
        }
        float real_speed = movementSpeed * movementMultiplier;
        rb.velocity = new Vector2(rb.velocity.x, real_speed);
    }

    public void onGroundEnter(Transform groundParent)
    {
        //empty
    }

    public void onGroundExit()
    {
        //empty
    }

    public void onRoofEnter(Transform roofParent)
    {
        //empty
    }

    public void onRoofExit()
    {
        //empty
    }
}
