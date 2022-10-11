using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A Script that can be controlled by a Controller
// Class that derived from this one will be able to be controlled as well
public abstract class Pawn : MonoBehaviour
{
    
    public abstract void MovementInput(Vector2 input);

    public abstract void CameraMovementInput(Vector2 input);

    public abstract void SouthButtonInput();

    public abstract void NorthButtonInput();

    public abstract void EstButtonInput();

    public abstract void WestButtonInput();



}
