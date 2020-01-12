using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveInputable
{
    void jump();
    void moveHorizontal(float movementMultiplier);
    void moveVertical(float movementMultiplier);
    void slide();
}
