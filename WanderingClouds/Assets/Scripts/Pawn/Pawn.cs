using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A Script that can be controlled by a Controller
// Class that derived from this one will be able to be controlled as well
public abstract class Pawn : MonoBehaviour
{

    [Foldout("Controller")]
    public bool controlled;
    [Foldout("Controller")]
    public Camera Camera;
    [Foldout("Controller")]
    public bool atStartControlledPawn = false;
    [Foldout("Movement")]
    public bool allowMovement = true;
    [Foldout("Movement")]
    public bool allowCameraMovement = true;


    [Foldout("Grounded")]
    public bool isGrounded;
    [Foldout("Slope")]
    public bool onSlope;
    protected void Update()
    {
        CalcGrounded();
        
    }

    public virtual void MovementInput(Vector2 input) { }

    public virtual void CameraMovementInput(Vector2 input) { }

    public virtual void SouthButtonInput() { }

    public virtual void NorthButtonInput() { }

    public virtual void EstButtonInput() { }

    public virtual void WestButtonInput() { }

    public virtual void RightTriggerInput() { }
    public virtual void LeftTriggerInput() { }

    public virtual void RightBumperInput() { }
    public virtual void LeftBumperInput() { }

    public virtual void SouthButtonInputReleased() { }

    public virtual void NorthButtonInputReleased() { }

    public virtual void EstButtonInputReleased() { }

    public virtual void WestButtonInputReleased() { }

    public virtual void RightTriggerInputReleased() { }
    public virtual void LeftTriggerInputReleased() { }

    public virtual void RightBumperInputReleased() { }
    public virtual void LeftBumperInputReleased() { }


    public virtual void CalcGrounded()
    {
        
    }
}


