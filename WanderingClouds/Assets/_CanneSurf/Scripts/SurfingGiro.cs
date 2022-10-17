using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class SurfingGiro : Pawn
{
    public Rigidbody body;
    public float speed = 5f;
    public Vector3 movement;
    [Space(10)]
    public CanningUrle urle;
    public float linkDistance;
    public bool isLink;
    public bool isOut;

    #region Input
    public override void CameraMovementInput(Vector2 input) { }
    public override void MovementInput(Vector2 input)
    {
        movement = new Vector3(input.x, 0, input.y);
    }

    public override void NorthButtonInput() { }
    public override void EstButtonInput() { }
    public override void SouthButtonInput() 
    {
        isLink = !isLink;
        linkDistance = Vector3.Distance(transform.position, urle.transform.position)+ 1;
    }
    public override void WestButtonInput() { }
    #endregion

    public void FixedUpdate()
    {
        Vector3 UrleToGiro = transform.position - urle.transform.position;
        if (isLink)
        {
            isOut = UrleToGiro.magnitude > linkDistance;
            if (isOut)
            {
                body.position = urle.transform.position + (transform.position - urle.transform.position).normalized * linkDistance;
                body.AddForce(movement * speed * Time.deltaTime, ForceMode.VelocityChange);
                body.velocity = body.velocity.magnitude > speed ? body.velocity.normalized * speed : body.velocity;
            }
            else
            {
                body.AddForce(movement * speed * Time.deltaTime, ForceMode.VelocityChange);
                body.velocity = body.velocity.magnitude > speed ? body.velocity.normalized * speed : body.velocity;
            }
        }
        else
        {
            body.AddForce(movement * speed * Time.deltaTime, ForceMode.VelocityChange);
            body.velocity = body.velocity.magnitude > speed ? body.velocity.normalized * speed : body.velocity;
        }
    }

    private void OnDrawGizmos()
    {
        if (!isLink) return;
        using (new Handles.DrawingScope(Color.red))
        {
            Handles.DrawWireDisc(urle.transform.position, Vector3.up, linkDistance);
        }
    }
}
