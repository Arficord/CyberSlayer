using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMoveController : MonoBehaviour, IMoveInputable
{
    private CharacterController character;
    private Rigidbody2D rb;

    public float jumpForce = 10;
    public float movementSpeed = 5;
    [Range(0, 1)] public float crouchSpeedMultiplier = 0.6f;
    public int maxAirJumps = 1;
    private int airJumpsLeft;

    private Coroutine slidingCoroutine;
    private void Awake()
    {
        character = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody2D>();
    }
    public void onChangeMoveController()
    {
        rb.gravityScale = 2;
    }

    private bool _crouching = false;
    private bool facingRight = true;
    private bool sliding = false;
    [SerializeField]
    private bool grounded = true;
    private bool canStay = true;
    private int _roofsAbove = 0;
    private bool Crouching
    {
        get
        {
            return _crouching;
        }
        set
        {
            if (grounded && !canStay || sliding)
            {
                _crouching = true;
            }
            else
            {
                _crouching = value;
            }
            character.setCrouchCollider(Crouching);
        }
    }

    private int RoofsAbove
    {
        get
        {
            return _roofsAbove;
        }
        set
        {
            _roofsAbove = value;
            canStay = (_roofsAbove == 0) ? true : false;
        }
    }

    #region IMoveInputable
    public void jump()
    {
        if (!grounded)
        {
            if (airJumpsLeft > 0)
                airJumpsLeft--;
            else
                return;
        }
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    public void moveHorizontal(float movementMultiplier)
    {
        if (sliding)
            return;

        if (Crouching)
            movementMultiplier *= crouchSpeedMultiplier;

        if (movementMultiplier > 0)
            turnRight();
        else if (movementMultiplier < 0)
            turnLeft();

        float real_speed = movementSpeed * movementMultiplier;
        rb.velocity = new Vector2(real_speed, rb.velocity.y);
    }
    public void moveVertical(float movementMultiplier)
    {
        if(movementMultiplier<0)
        {
            Crouching = true;
            dropThroughPlatform();
        }
        else
        {
            Crouching = false;
            stopDroppingThroughPlatform();

        }
    }
    public void doSpecialMoves()
    {
        slideStart();
    }

    public void onGroundEnter(Transform groundParent)
    {
        grounded = true;
        airJumpsLeft = maxAirJumps;
        character.groundParent = groundParent;
    }
    public void onGroundExit()
    {
        grounded = false;
        character.groundParent = null;
        slideStop();
    }
    public void onRoofEnter(Transform roofParent)
    {
        RoofsAbove++;
    }
    public void onRoofExit()
    {
        RoofsAbove--;
    }
    
    public CharacterStates getState()
    {
        if (grounded == false)
        {
            return rb.velocity.y > 0 ? CharacterStates.JUMPING : CharacterStates.FALLING;
        }

        if (sliding)
        {
            return CharacterStates.SLIDING;
        }

        if (Crouching)
        {
            return isMoving() ? CharacterStates.CROUCHING : CharacterStates.CROUCH_IDLING;

        }

        return isMoving() ? CharacterStates.RUN : CharacterStates.STAND_IDLING;
    }
    #endregion

    private bool isMoving()
    {
        float range = 1f;
        return rb.velocity.x > range || rb.velocity.x < -range;
    }

    public void turnLeft()
    {
        if (facingRight)
            turnAround();
    }
    public void turnRight()
    {
        if (!facingRight)
            turnAround();
    }
    public void turnAround()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
    public void slideStart()
    {
        if (!grounded || getState() != CharacterStates.RUN)
        {
            return;
        }
        sliding = true;
        character.setCrouchCollider(true);
        int facingMultiplier = facingRight ? 1 : -1;

        StartCoroutine(slideForce());
    }
    public void slideStop()
    {
        sliding = false;
        character.setCrouchCollider(true);
        if (slidingCoroutine != null)
        {
            StopCoroutine(slidingCoroutine);
        }
    }
    private IEnumerator slideForce()
    {
        float speedForce = 1.5f;
        float iterations = 50;
        for (int i = 0; i < iterations; i++)
        {
            float movement = movementSpeed * (1 + speedForce * (iterations - i) / 100f);
            if (!facingRight)
                movement *= -1;
            rb.velocity = new Vector2(movement, rb.velocity.y);
            yield return new WaitForFixedUpdate();
        }
        slideStop();
    }
    public void dropThroughPlatform()
    {
        character.platformCollider.enabled = false;
    }
    public void stopDroppingThroughPlatform()
    {
        character.platformCollider.enabled = true;
    }

}
/*
 * Привет, хочу отключать не нужные контроллеры передвижения, по этому есть варианты и вопросы к ним. Для начала каждый контролер это класс который наследует MonoBehaviour и мой интерфейс IMoveInputable. В CharacterController есть масив всех IMoveInputable и переменная текушего контроллера. Так как это сделать:
1. Каждый IMoveInputable имеет доступ к своему скрипту и может его выключить
*/