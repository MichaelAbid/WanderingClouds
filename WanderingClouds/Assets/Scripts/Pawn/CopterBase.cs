using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class CopterBase : Pawn
{
    public GameObject pivotX;
    public GameObject pivotY;
    public GameObject visual;



    // Camera Movement
    protected Vector2 camCurMovement;
    public float camSensibility;

    // Pawn Movement
    protected Vector2 pawnCurMovement;
    public float pawnSpeed = 2;


    protected virtual void Update()
    {
        base.Update();
        if (allowCameraMovement)CameraUpdate();
        if (allowMovement) MovementUpdate();
    }

    protected virtual void CameraUpdate()
    {
        if(camCurMovement != Vector2.zero)
        {
            if (Mathf.Abs(pivotY.transform.eulerAngles.x + camCurMovement.y) <= 45 || Mathf.Abs(pivotY.transform.eulerAngles.x + camCurMovement.y) >= 360-45) { pivotY.transform.Rotate(Vector3.right, camCurMovement.y * camSensibility); }
            pivotX.transform.Rotate(Vector3.up, camCurMovement.x * camSensibility);

        }
    }

    protected virtual void MovementUpdate()
    {
        if (pawnCurMovement != Vector2.zero)
        {
            transform.position += (pivotX.transform.forward * pawnCurMovement.y * pawnSpeed * Time.deltaTime) + (pivotX.transform.right * pawnCurMovement.x * pawnSpeed * Time.deltaTime);
            visual.transform.rotation = Quaternion.Euler(0, pivotX.transform.rotation.eulerAngles.y, 0);
        }
    }

    public override void CameraMovementInput(Vector2 input)
    {
        camCurMovement = input;
        if ( Mathf.Abs(camCurMovement.x) <= 0.2f )  camCurMovement.x = 0 ;
        if ( Mathf.Abs(camCurMovement.y) <= 0.2f ) camCurMovement.y = 0 ;
    }



    public override void MovementInput(Vector2 input)
    {
        pawnCurMovement = input;
        if (Mathf.Abs(pawnCurMovement.x) <= 0.2f) pawnCurMovement.x = 0;
        if (Mathf.Abs(pawnCurMovement.y) <= 0.2f) pawnCurMovement.y = 0;
    }






}
