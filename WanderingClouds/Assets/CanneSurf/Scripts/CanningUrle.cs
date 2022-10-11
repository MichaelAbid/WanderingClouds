using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanningUrle : Pawn
{
    public Rigidbody body;
    public float speed = 5f;
    public Vector3 movement;

    #region Input
    public override void CameraMovementInput(Vector2 input) {}
    public override void MovementInput(Vector2 input)
    {
        movement = new Vector3(input.x,  0, input.y) *  speed;
    }

    public override void NorthButtonInput() { }
    public override void EstButtonInput() { }
    public override void SouthButtonInput() { }
    public override void WestButtonInput() { }
    #endregion

    public void Update()
    {
        body.velocity = movement;
    }
}
