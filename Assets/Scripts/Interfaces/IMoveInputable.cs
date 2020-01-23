using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public interface IMoveInputable
{
    void jump();
    void moveHorizontal(float movementMultiplier);
    void moveVertical(float movementMultiplier);
    void doSpecialMoves();

    void onGroundEnter(Transform groundParent);
    void onGroundExit();
    void onRoofEnter(Transform roofParent);
    void onRoofExit();

    void onChangeMoveController();
    CharacterStates getState();
}
