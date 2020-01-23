using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnviromentUser
{
    void useEnviroment(IEnviromentUsable usable);

    CharacterController getCharacterController();
}
