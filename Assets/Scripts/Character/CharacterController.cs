using System.Collections;
using UnityEngine;

public class CharacterController : MonoBehaviour, IEnviromentUser, IMoveInputable
{
    public enum Controllers
    {
        GROUND,
        LADDER,
    }
    private CharacterAnimator animator;

    public Collider2D bodyStraitCollider;
    public Collider2D bodyCrouchCollider;
    public Collider2D platformCollider;

    public ColliderTriggerController standUpTrigger = null;
    public ColliderTriggerController groundTrigger = null;

    [SerializeField]
    private CharacterStates state = CharacterStates.STAND_IDLING;
    public IMoveInputable curentMoveController;
    private IMoveInputable[] moveControllers; //Custom Drower
    
    public Transform groundParent = null;

    private void Start()
    {
        animator = new CharacterAnimator(GetComponent<Animator>());
        //curentMoveController = GetComponent<GroundMoveController>();
        //curentMoveController = GetComponent<LadderMovementController>();
        moveControllers = GetComponents<IMoveInputable>(); // //0 -is the CharacterController itself TODO change order, characterController isnt needed here
        changeController(Controllers.GROUND);

        standUpTrigger.onTriggerEnter += onRoofEnter;
        standUpTrigger.onTriggerExit += onRoofExit;

        groundTrigger.onTriggerEnter += onGroundEnter;
        groundTrigger.onTriggerExit += onGroundExit;
    }
    private void Update()
    {
        CharacterStates state = this.state;
        this.state = curentMoveController.getState();
        if (state != this.state)
        {
            animator.update(this.state);
        }
    }

    public void useEnviroment()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(bodyStraitCollider.bounds.center, bodyStraitCollider.bounds.size, 0f, Vector2.zero, 1, GlobalProperties.gp.usableLayerMask);
        if (raycastHit2D.collider != null)
        {
            Debug.Log("COLLIDED");
            IEnviromentUsable usable = (raycastHit2D.collider.gameObject.GetComponent<IEnviromentUsable>());

            if (usable != null)
            {
                Debug.Log("Trying to use");
                useEnviroment(usable);
            }
        }
        Debug.Log("NONE");
    }
    public void useEnviroment(IEnviromentUsable usable)
    {
        usable.beUsed(this);
    }

    public void changeController(Controllers controller)
    {
        //Custom Drower
        Debug.Log("The controller is - " + controller);
        switch (controller)
        {
            case Controllers.GROUND:
                curentMoveController = moveControllers[1];
                break;
            case Controllers.LADDER:
                curentMoveController = moveControllers[2];
                break;
        }
        curentMoveController.onChangeMoveController();
    }

    public void setCrouchCollider(bool flag)
    {
        if (flag)
        {
            bodyStraitCollider.enabled = false;
            bodyCrouchCollider.enabled = true;
        }
        else
        {
            bodyCrouchCollider.enabled = false;
            bodyStraitCollider.enabled = true;
        }
    }
    public void jump()
    {
        curentMoveController.jump();
    }
    public void moveHorizontal(float movementMultiplier)
    {
        curentMoveController.moveHorizontal(movementMultiplier);
    }
    public void moveVertical(float movementMultiplier)
    {
        curentMoveController.moveVertical(movementMultiplier);
    }
    public void doSpecialMoves()
    {
        curentMoveController.doSpecialMoves();
    }
    public void onGroundEnter(Transform groundParent)
    {
        curentMoveController.onGroundEnter(groundParent);
    }
    public void onGroundExit()
    {
        Debug.Log("Ground Exit");
        curentMoveController.onGroundExit();
    }
    public void onRoofEnter(Transform roofParent)
    {
        curentMoveController.onRoofEnter(roofParent);
    }
    public void onRoofExit()
    {
        Debug.Log("Ground Exit");
        curentMoveController.onRoofExit();
    }

    public CharacterStates getState()
    {
        return state;
    }

    public void onChangeMoveController()
    {

    }

    public CharacterController getCharacterController()
    {
        return this;
    }
}
