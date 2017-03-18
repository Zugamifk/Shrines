using UnityEngine;
using System.Collections;

public class PlayerController : CharacterController {

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        InputManager.RegisterAxisUpdateCallback(InputKey.MOVE_HORIZONTAL, Move);
        InputManager.RegisterButtonDownCallback(InputKey.BUTTON0, Jump);
    }
}
