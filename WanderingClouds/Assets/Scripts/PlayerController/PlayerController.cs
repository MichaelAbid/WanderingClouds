using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private int ControllerNumber;
    public Pawn pawn;

    private void Start()
    {
        ControllerNumber = FindObjectsOfType<PlayerController>().Length;
        ControllerJoin();
    }

    public void ControllerJoin()
    {
        Debug.Log($"Controller Joined on number {ControllerNumber}");
        
    }

    public void LeftJoyStick(InputAction.CallbackContext callback)
    {
        if (pawn != null)
        {
            pawn.MovementInput(callback.ReadValue<Vector2>());
        }
    }

    public void RightJoyStick(InputAction.CallbackContext callback)
    {
        if (pawn != null)
        {
            pawn.CameraMovementInput(callback.ReadValue<Vector2>());
        }
    }


    public void ButtonEst(InputAction.CallbackContext callback)
    {
        if (pawn != null && callback.performed)
        {
            pawn.EstButtonInput();
        }
    }

    public void ButtonWest(InputAction.CallbackContext callback)
    {
        if (pawn != null && callback.performed)
        {
            pawn.WestButtonInput();
        }
    }

    public void ButtonSouth(InputAction.CallbackContext callback)
    {
        if (pawn != null && callback.performed)
        {
            pawn.SouthButtonInput();
        }
    }

    public void ButtonNorth(InputAction.CallbackContext callback)
    {
        if (pawn != null && callback.performed)
        {
            pawn.NorthButtonInput();
        }
    }


}
