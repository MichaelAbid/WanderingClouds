using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using NaughtyAttributes;
public class Player : Pawn
{
    [Foldout("Ref")]
    public GameObject pivotX;
    [Foldout("Ref")]
    public GameObject pivotY;
    [Foldout("Ref")]
    public GameObject visual;
    [Foldout("Ref")]
    public GameObject pivotArm;


    // Camera Movement
    [Foldout("Camera")]
    protected Vector2 camCurMovement;
    [Foldout("Camera")]
    public float camSensibility;

    // Pawn Movement
    [Foldout("Movement")]
    protected Vector2 pawnCurMovement;
    [Foldout("Movement")]
    public float pawnSpeed = 2;


    [Foldout("Jump")]
    [CurveRange(EColor.Blue)]
    public AnimationCurve jumpCurve;
    [Foldout("Jump")]
    public bool jumping = false;

    protected virtual void Update()
    {
        base.Update();
        if (allowCameraMovement) CameraUpdate();
        if (allowMovement) MovementUpdate();
    }

    protected virtual void CameraUpdate()
    {
        if (camCurMovement != Vector2.zero)
        {
            if (Mathf.Abs(pivotY.transform.eulerAngles.x + camCurMovement.y) <= 45 || Mathf.Abs(pivotY.transform.eulerAngles.x + camCurMovement.y) >= 360 - 45) { pivotY.transform.Rotate(Vector3.right, camCurMovement.y * camSensibility); }
            pivotX.transform.Rotate(Vector3.down, camCurMovement.x * camSensibility);

        }
    }

    protected virtual void MovementUpdate()
    {
        if (pawnCurMovement != Vector2.zero)
        {
            transform.position += (pivotX.transform.forward * pawnCurMovement.y * pawnSpeed * Time.deltaTime) + (pivotX.transform.right * pawnCurMovement.x * pawnSpeed * Time.deltaTime);
            visual.transform.rotation = Quaternion.LookRotation(new Vector3(pawnCurMovement.x,0,pawnCurMovement.y),Vector3.up);
        }
    }

    public override void CameraMovementInput(Vector2 input)
    {
        camCurMovement = input;
        if (Mathf.Abs(camCurMovement.x) <= 0.2f) camCurMovement.x = 0;
        if (Mathf.Abs(camCurMovement.y) <= 0.2f) camCurMovement.y = 0;
    }



    public override void MovementInput(Vector2 input)
    {
        pawnCurMovement = input;
        if (Mathf.Abs(pawnCurMovement.x) <= 0.2f) pawnCurMovement.x = 0;
        if (Mathf.Abs(pawnCurMovement.y) <= 0.2f) pawnCurMovement.y = 0;
    }


    public void Jump()
    {
        if (isGrounded && !jumping)
        {
            StartCoroutine(JumpCoroutine());
        }
    }


    IEnumerator JumpCoroutine()
    {
        jumping = true;

        float time = 0;



        yield return null;
        jumping = false;
    }
    
}
