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
    public CharacterController character;

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
        isCrouchKeyDown = Input.GetKey(KeyCode.DownArrow) ? true : false;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            character.jump();
        }
        character.move(Input.GetAxis("Horizontal"), isCrouchKeyDown);
    }
}
