using System.Collections;
using UnityEngine;

public class CharacterController : MonoBehaviour, IEnviromentUser
{

    private Rigidbody2D rb;
    private CharacterAnimator animator;

    public float jumpForce = 10;
    public float movementSpeed = 5;
    public int maxAirJumps = 1;
    private int airJumpsLeft;
    [Range(0, 1)] public float crouchSpeedMultiplier = 0.6f;
    public Collider2D bodyStraitCollider;
    public Collider2D bodyCrouchCollider;
    public Collider2D platformCollider;


    private int baseLayer;

    private bool crouching = false;
    private bool facingRight = true;
    private bool sliding = false;
    private bool _grounded = true;
    public bool Grounded
    {
        get
        {
            return _grounded;
        }
        set
        {
            _grounded = value;
            if(_grounded)
            {
                airJumpsLeft = maxAirJumps;
            }
            else
            {
                slideStop();
            }
        }
    }
    private bool canStay = true;
    private int _roofsAbove = 0;
    public int RoofsAbove 
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
    public void incrementRoof()
    {
        RoofsAbove++;
    }
    public void decrimentRoof()
    {
        RoofsAbove--;
    }
    [SerializeField]
    private CharacterStates state = CharacterStates.STAND_IDLING;
    private Coroutine slidingCoroutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = new CharacterAnimator(GetComponent<Animator>());
        airJumpsLeft = maxAirJumps;
        baseLayer = gameObject.layer;
    }

    private void Update()
    {
        CharacterStates state = this.state;
        changeState();
        if (state != this.state)
        {
            animator.update(this.state);
        }
    }
    public void jump()
    {
        if(!Grounded)
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

        if(!isCrouch && Grounded && !canStay)
            isCrouch = true;

        crouching = isCrouch;
        setCrouchCollider(crouching);

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
        setCrouchCollider(true);
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
        platformCollider.enabled = false ;
    }
    public void stopDroppingThroughPlatform()
    {
        platformCollider.enabled = true;
    }
    public void useEnviroment()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(bodyStraitCollider.bounds.center, bodyStraitCollider.bounds.size, 0f, Vector2.zero, 1, GlobalProperties.gp.usableLayerMask);
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


    private bool isMoving()
    {
        float range = 1f;
        return rb.velocity.x > range || rb.velocity.x < -range;
    }
    private void changeState()
    {
        if (Grounded == false)
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
        if(flag)
        {
            bodyStraitCollider.enabled = false;
            bodyCrouchCollider.enabled = true ;
        }
        else
        {
            bodyCrouchCollider.enabled = false;
            bodyStraitCollider.enabled = true;
        }
    }
    void OnDrawGizmosSelected()
    {
        Vector2 origin = new Vector2(bodyStraitCollider.bounds.center.x, bodyStraitCollider.bounds.min.y);
        Vector2 size = new Vector2(bodyStraitCollider.bounds.size.x, 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(origin, size);
    }


    /*
     * Un used just for lookup
     * 
     *  private bool canStandUp()
        {
            Vector3 center = new Vector3(bodyCollider.bounds.center.x, bodyCollider.bounds.center.y+1, bodyCollider.bounds.center.z);
            return !Physics2D.BoxCast(center, bodyCollider.bounds.size, 0f, Vector2.up, 1, GlobalProperties.gp.ceilingLayerMask);
        }
        private bool isGrounded()
        {
            Vector2 origin = new Vector2 (bodyCollider.bounds.center.x, bodyCollider.bounds.min.y);
            Vector2 size = new Vector2(bodyCollider.bounds.size.x, 0.1f);
            size = bodyCollider.bounds.size;
            //Debug.Log(origin);
            RaycastHit2D raycastHit2D = Physics2D.BoxCast(origin, bodyCollider.bounds.size, 0f, Vector2.down,0, GlobalProperties.gp.groundLayerMask);
            return raycastHit2D.collider != null;
        }
    */
}
