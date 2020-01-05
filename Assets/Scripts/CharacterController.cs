using System.Collections;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    private Rigidbody2D rb;
    private CharacterAnimator animator;
    private CircleCollider2D circleCollider;

    public float jumpForce = 10;
    public float movementSpeed = 5;
    public int maxAirJumps = 1;
    private int airJumpsLeft;
    [Range( 0, 1 )]public float crouchSpeedMultiplier = 0.6f;
    public Collider2D crouchCollider;
    [SerializeField]public LayerMask platformLayerMask;

    private bool crouching = false;
    private bool facingRight = true;
    private bool sliding = false;
    private bool grounded = true;
    [SerializeField]
    private CharacterStates state = CharacterStates.STAND_IDLING;
    private Coroutine slidingCoroutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = new CharacterAnimator(GetComponent<Animator>());
        circleCollider = GetComponent<CircleCollider2D>();
        airJumpsLeft = maxAirJumps;
    }

    private void Update()
    {
        grounded = isGrounded();

        if ( grounded )
        {
            airJumpsLeft = maxAirJumps;
        }

        CharacterStates state = this.state;
        changeState();
        if (state != this.state)
        {
            animator.update(this.state);
        }
    }
    public void jump()
    {
        if( !isGrounded() )
        {
            if( airJumpsLeft > 0 )
                airJumpsLeft --;
            else
                return;
        }
        rb.velocity = new Vector2( rb.velocity.x, 0 );
        rb.AddForce( Vector2.up * jumpForce, ForceMode2D.Impulse );
    }
    public void move(float movementMultiplier, bool isCrouch)
    {
        if (sliding)
            return;

        if(!isCrouch && !canStandUp())
            isCrouch = true;

        crouching = isCrouch;
        setCrouchCollider(!crouching);

        if(crouching)
            movementMultiplier *= crouchSpeedMultiplier;

        if ( movementMultiplier > 0 )
            turnRight();
        else if ( movementMultiplier < 0 )
            turnLeft();

        float real_speed = movementSpeed * movementMultiplier;

        rb.velocity = new Vector2(real_speed, rb.velocity.y);
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
        sliding = true;
        int facingMultiplier = facingRight ? 1 : -1;

        StartCoroutine(slideForce());
    }
    public void slideStop()
    {
        sliding = false;
        if(slidingCoroutine!=null)
        {
            StopCoroutine(slidingCoroutine);
        }
    }
    private IEnumerator slideForce()
    {
        float speedForce = 1.5f;
        float iterations = 50;
        for(int i=0;i<iterations;i++)
        {
            float movement = movementSpeed * (1 + speedForce*(iterations-i)/100f);
            if (!facingRight)
                movement *= -1;
            rb.velocity = new Vector2(movement, rb.velocity.y);
            yield return new WaitForFixedUpdate();
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(circleCollider.bounds.center, circleCollider.bounds.size, 0f, Vector2.down,0.1f, platformLayerMask);
        return raycastHit2D.collider != null;
    }
    private bool canStandUp()
    {
        Vector3 center = new Vector3(circleCollider.bounds.center.x, circleCollider.bounds.center.y+1, circleCollider.bounds.center.z);
        return !Physics2D.BoxCast(center, circleCollider.bounds.size, 0f, Vector2.up, 1, platformLayerMask);
    }
    private bool isMoving()
    {
        float range = 1f;
        return rb.velocity.x > range || rb.velocity.x < -range;
    }
    private void changeState()
    {
        if (grounded == false)
        {
            state = rb.velocity.y > 0 ? CharacterStates.JUMPING : CharacterStates.FALLING;

            Debug.Log($"{state}");
            return;
        }

        if (sliding)
        {
            state = CharacterStates.SLIDING;
            Debug.Log($"{state}");
            return;
        }

        if (crouching)
        {
            state = isMoving() ? CharacterStates.CROUCHING : CharacterStates.CROUCH_IDLING;
            Debug.Log($"{state}");
            return;
        }

        state = isMoving() ? CharacterStates.RUN : CharacterStates.STAND_IDLING;
        Debug.Log($"{state}");
    }

    private void setCrouchCollider(bool flag)
    {
        crouchCollider.enabled = flag;
    }
}
