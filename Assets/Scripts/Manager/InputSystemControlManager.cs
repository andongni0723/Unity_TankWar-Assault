using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystemControlManager : Singleton<InputSystemControlManager>
{
    //[Header("Component")]
    // [Header("Settings")]
    [HideInInspector] public PlayerInputControl inputSystem;
    //[Header("Debug")]

    public override void Awake()
    {
        base.Awake();
        inputSystem = new PlayerInputControl();
        inputSystem.Enable();
        InputSystemInitial();
    }
    
    private void InputSystemInitial()
    {
        // _inputSystem.Player.Move.performed += ctx => Movement(ctx.ReadValue<Vector2>());
    }
}