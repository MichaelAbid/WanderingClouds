using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private int ControllerNumber;
    public Pawn cPawn;
    private int pawnIndex = 0;
    public List<Pawn> listOfAllPawn;
    private void Start()
    {
        ControllerNumber = FindObjectsOfType<PlayerController>().Length;
        ControllerJoin();
        listOfAllPawn = FindObjectsOfType<Pawn>().ToList();
        pawnIndex = -1;
        ChangePawn();
        
    }

    public void ChangePawn()
    {
        

        pawnIndex = pawnIndex+1<listOfAllPawn.Count ? pawnIndex+1 : 0;
        int i = 0;
        foreach (Pawn pawn in listOfAllPawn)
        {
            if (!pawn.controlled && i>=pawnIndex)
            {
                if (cPawn != null)
                {
                    cPawn.controlled = false;
                }
                pawn.controlled = true;
                this.cPawn = pawn;
                GetComponent<PlayerInput>().camera = cPawn.Camera;
                break;
            }
            i++;
        }
        
    }

    public void ControllerJoin()
    {
        Debug.Log($"Controller Joined on number {ControllerNumber}");
        
    }

    public void LeftJoyStick(InputAction.CallbackContext callback)
    {
        if (cPawn != null)
        {
            cPawn.MovementInput(callback.ReadValue<Vector2>());
        }
    }

    public void RightJoyStick(InputAction.CallbackContext callback)
    {
        if (cPawn != null)
        {
            cPawn.CameraMovementInput(callback.ReadValue<Vector2>());
        }
    }


    public void ButtonEst(InputAction.CallbackContext callback)
    {
        if (cPawn != null && callback.performed)
        {
            cPawn.EstButtonInput();
        }
    }

    public void ButtonWest(InputAction.CallbackContext callback)
    {
        if (cPawn != null && callback.performed)
        {
            cPawn.WestButtonInput();
        }
    }

    public void ButtonSouth(InputAction.CallbackContext callback)
    {
        if (cPawn != null && callback.performed)
        {
            cPawn.SouthButtonInput();
        }
    }

    public void ButtonNorth(InputAction.CallbackContext callback)
    {
        if (cPawn != null && callback.performed)
        {
            cPawn.NorthButtonInput();
        }
    }

    public void ButtonSelect(InputAction.CallbackContext callback)
    {
        if (callback.performed)
        {
            ChangePawn();
        }
    }

}
