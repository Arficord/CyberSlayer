using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private enum Keys
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        ACCEPT,
        CANCEL,
    }
    public enum ControllMasks
    {
        PLAYER
    }
    public CharacterController character;
    public ControllMasks controllMask = ControllMasks.PLAYER;

    private bool isCrouchKeyDown;

    private KeyCode up = KeyCode.UpArrow;
    private KeyCode down = KeyCode.DownArrow;
    private KeyCode left = KeyCode.LeftArrow;
    private KeyCode right = KeyCode.RightArrow;
    private KeyCode accept = KeyCode.Z; //fire
    private KeyCode cancel = KeyCode.X; //escape

    

    private void Start()
    {
        //up = new KeyCode();
    }

    void Update()
    {
        switch(controllMask)
        {
            case ControllMasks.PLAYER:
                controllPlayer();
                break;
        }

    }
    private void controllPlayer()
    {
        isCrouchKeyDown = Input.GetKey(KeyCode.DownArrow)? true : false;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            character.jump();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            character.doSpecialMoves();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            character.useEnviroment();
        }
        float v_movement = Input.GetAxis("Vertical");
        if(v_movement>0)
        {
            v_movement = 1;
        }
        else
        {
            if(v_movement<0)
            {
                v_movement = -1;
            }
        }
        character.moveVertical(v_movement);
        character.moveHorizontal(Input.GetAxis("Horizontal"));
    }
}
