using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ViewAnglePlayer : Pawn
{
    public Rigidbody body;
    public Transform pivotX;
    public Transform pivotY;

    [Header("Move")]
    public Vector3 movement;
    public float speed = 20f;

    [Header("Cam")]
    public Vector2 camCurMovement;
    public float camSensibility = 10f;

    #region Input
    public override void CameraMovementInput(Vector2 input)
    {
        camCurMovement = input * new Vector2(1,-1);
        if (Mathf.Abs(camCurMovement.x) <= 0.2f) camCurMovement.x = 0;
        if (Mathf.Abs(camCurMovement.y) <= 0.2f) camCurMovement.y = 0;
    }
    public override void MovementInput(Vector2 input)
    {
        movement = Vector3.ProjectOnPlane(Camera.transform.forward * input.y + Camera.transform.right * input.x, Vector3.up);
        if (Mathf.Abs(movement.x) <= 0.2f) movement.x = 0;
        if (Mathf.Abs(movement.z) <= 0.2f) movement.z = 0;

    }

    public override void NorthButtonInput()
    {
    }
    public override void EstButtonInput()
    {
    }
    public override void SouthButtonInput()
    {
        body.AddForce(Vector3.up * speed, ForceMode.VelocityChange);
    }
    public override void WestButtonInput()
    {
    }
    #endregion

    public void Update()
    {
        CameraUpdate();
    }

    public void FixedUpdate()
    {
        body.AddForce(movement * (5 * speed * Time.deltaTime), ForceMode.VelocityChange);
        body.velocity = body.velocity.magnitude > speed ? body.velocity.normalized * speed : body.velocity;
    }

    private void CameraUpdate()
    {
        if (camCurMovement != Vector2.zero)
        {
            pivotX.transform.Rotate(Vector3.up, camCurMovement.x * camSensibility * Time.deltaTime);

            var tempRot = pivotY.transform.rotation;
            pivotY.transform.Rotate(Vector3.right, camCurMovement.y * camSensibility * Time.deltaTime);
            if (Mathf.Abs(pivotY.transform.eulerAngles.x + camCurMovement.y) > 45 && Mathf.Abs(pivotY.transform.eulerAngles.x + camCurMovement.y) < 360- 45)
            pivotY.transform.rotation = tempRot;
        }
    }
}
