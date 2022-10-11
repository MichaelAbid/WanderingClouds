using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Pawn
{
    public override void CameraMovementInput(Vector2 input)
    {
        throw new System.NotImplementedException();
    }

    public override void EstButtonInput()
    {
        throw new System.NotImplementedException();
    }

    public override void MovementInput(Vector2 input)
    {
        Debug.Log(input);
    }

    public override void NorthButtonInput()
    {
        throw new System.NotImplementedException();
    }

    public override void SouthButtonInput()
    {
        throw new System.NotImplementedException();
    }

    public override void WestButtonInput()
    {
        throw new System.NotImplementedException();
    }
}
