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

    #region Input
    public override void CameraMovementInput(Vector2 input) { }
    public override void MovementInput(Vector2 input)
    {
        movement = new Vector3(input.x, 0, input.y) * speed;
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
        if (!isLink)
        {
            body.velocity = movement;
            return;
        }

        Vector3 UrleToGiro = transform.position - urle.transform.position;
        bool isOut = UrleToGiro.magnitude > linkDistance;
        if (isOut)
        {
            Vector3 aimPos = urle.transform.position + (UrleToGiro.normalized * linkDistance);
            body.AddForce((aimPos-transform.position) * 1000 * Time.deltaTime, ForceMode.VelocityChange);
        }
        else
        {
            body.AddForce(movement * 10 * Time.deltaTime, ForceMode.VelocityChange);

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
