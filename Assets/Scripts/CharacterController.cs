using System.Collections;
using UnityEngine;

public class CharacterController : MonoBehaviour, IEnviromentUser
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


    private int baseLayer;

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
        baseLayer = gameObject.layer;
    }

    private void Update()
    {
        grounded = isGrounded();

        if ( grounded )
        {
            airJumpsLeft = maxAirJumps;
        }
        else
        {
            slideStop();
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

        if(!isCrouch && grounded && !canStandUp())
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
        setCrouchCollider(false);
        int facingMultiplier = facingRight ? 1 : -1;

        StartCoroutine(slideForce());
    }
    public void slideStop()
    {
        sliding = false;
        setCrouchCollider(true);
        if (slidingCoroutine!=null)
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
    public void dropThroughPlatform()
    {
        gameObject.layer = GlobalProperties.gp.fallingLayer ;
    }
    public void stopDroppingThroughPlatform()
    {
        gameObject.layer = baseLayer;
    }
    public void useEnviroment()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(circleCollider.bounds.center, circleCollider.bounds.size, 0f, Vector2.zero, 1, GlobalProperties.gp.usableLayerMask);
        if(raycastHit2D.collider != null)
        {
            IEnviromentUsable usable = (raycastHit2D.collider.gameObject.GetComponent<IEnviromentUsable>());
            
            if(usable!=null)
            {
                useEnviroment(usable);
            }
        }
    }
    public void useEnviroment(IEnviromentUsable usable)
    {
        usable.beUsed(this);
    }
    private bool isGrounded()
    {
        if(rb.velocity.y>0)
        {
            return false;//TODO 
        }
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(circleCollider.bounds.center, circleCollider.bounds.size, 0f, Vector2.down,0.1f, GlobalProperties.gp.groundLayerMask);
        return raycastHit2D.collider != null;
    }
    private bool canStandUp()
    {
        Vector3 center = new Vector3(circleCollider.bounds.center.x, circleCollider.bounds.center.y+1, circleCollider.bounds.center.z);
        return !Physics2D.BoxCast(center, circleCollider.bounds.size, 0f, Vector2.up, 1, GlobalProperties.gp.ceilingLayerMask);
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
            return;
        }

        if (sliding)
        {
            state = CharacterStates.SLIDING;
            return;
        }

        if (crouching)
        {
            state = isMoving() ? CharacterStates.CROUCHING : CharacterStates.CROUCH_IDLING;
            return;
        }

        state = isMoving() ? CharacterStates.RUN : CharacterStates.STAND_IDLING;
    }

    private void setCrouchCollider(bool flag)
    {
        crouchCollider.enabled = flag;
    }
}
