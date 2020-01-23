using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder: MonoBehaviour, IEnviromentUsable
{
    public void beUsed(IEnviromentUser user)
    {
        Debug.Log("CLIMBING");
        user.getCharacterController().changeController(CharacterController.Controllers.LADDER);
    }

}
