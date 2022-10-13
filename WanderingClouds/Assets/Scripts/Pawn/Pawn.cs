using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A Script that can be controlled by a Controller
// Class that derived from this one will be able to be controlled as well
public abstract class Pawn : MonoBehaviour
{

    public bool controlled;
    public Camera Camera;
    public virtual void MovementInput(Vector2 input) { }

    public virtual void CameraMovementInput(Vector2 input) { }

    public virtual void SouthButtonInput() { }

    public virtual void NorthButtonInput() { }

    public virtual void EstButtonInput() { }

    public virtual void WestButtonInput() { }


    public virtual void SouthButtonInputReleased() { }

    public virtual void NorthButtonInputReleased() { }

    public virtual void EstButtonInputReleased() { }

    public virtual void WestButtonInputReleased() { }
}
