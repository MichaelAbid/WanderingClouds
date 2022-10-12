using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class Player : Pawn
{
    public GameObject pivotX;
    public GameObject pivotY;
    public GameObject visual;



    // Camera Movement
    private Vector2 camCurMovement;
    public float camSensibility;

    // Pawn Movement
    private Vector2 pawnCurMovement;
    public float pawnSpeed = 2;

    //Prisme
    public bool prismed = false;

    private void Update()
    {
        CameraUpdate();
        MovementUpdate();
    }

    private void CameraUpdate()
    {
        if(camCurMovement != Vector2.zero)
        {
            if (Mathf.Abs(pivotY.transform.eulerAngles.x + camCurMovement.y) <= 45 || Mathf.Abs(pivotY.transform.eulerAngles.x + camCurMovement.y) >= 360-45) { pivotY.transform.Rotate(Vector3.right, camCurMovement.y * camSensibility); }
            pivotX.transform.Rotate(Vector3.up, camCurMovement.x * camSensibility);

        }
    }

    private void MovementUpdate()
    {
        if (pawnCurMovement != Vector2.zero)
        {
            transform.position += (pivotX.transform.forward * pawnCurMovement.y * pawnSpeed) + (pivotX.transform.right * pawnCurMovement.x * pawnSpeed);
            visual.transform.rotation = Quaternion.Euler(0, pivotX.transform.rotation.eulerAngles.y, 0);
        }
    }

    public override void CameraMovementInput(Vector2 input)
    {
        camCurMovement = input;
        if ( Mathf.Abs(camCurMovement.x) <= 0.2f )  camCurMovement.x = 0 ;
        if ( Mathf.Abs(camCurMovement.y) <= 0.2f ) camCurMovement.y = 0 ;
    }

    public override void EstButtonInput()
    {
        throw new System.NotImplementedException();
    }

    public override void MovementInput(Vector2 input)
    {
        pawnCurMovement = input;
        if (Mathf.Abs(pawnCurMovement.x) <= 0.2f) pawnCurMovement.x = 0;
        if (Mathf.Abs(pawnCurMovement.y) <= 0.2f) pawnCurMovement.y = 0;
    }

    public override void NorthButtonInput()
    {
        if (tag == "Giro")
        {
            Prisme();
        }
    }

    private void Prisme()
    {
        prismed = !prismed;
        Color color = visual.GetComponent<MeshRenderer>().material.color;
        if (prismed)
        {
            color.a = 0.3f;
        }
        else
        {
            color.a = 1f;
        }
        visual.GetComponent<MeshRenderer>().material.color = color;
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
