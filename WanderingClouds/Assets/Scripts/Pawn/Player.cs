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

    [Foldout("Movement")]
    [ReadOnly]
    public float momentum;
    [Foldout("Movement")]
    [ReadOnly]
    public Vector2 momentumDirection;

    [Foldout("Camera")]
    public Vector3 CamNormalPosition;
    [Foldout("Camera")]
    public Vector3 CamShoulderPosition;
    [Foldout("Camera")]
    public bool isAiming;



    [Foldout("Jump")]
    [CurveRange(EColor.Blue)]
    public AnimationCurve jumpCurve;
    [Foldout("Jump")]
    public bool jumping = false;
    [Foldout("Jump")]
    public float jumpTime = 2;
    [Foldout("Jump")]
    public float jumpHeight = 2;

    protected virtual void Update()
    {
        base.Update();
        DrawDebug();
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
            float previousY = transform.position.y;
            transform.position += (pivotX.transform.forward * pawnCurMovement.y * pawnSpeed * Time.deltaTime) + (pivotX.transform.right * pawnCurMovement.x * pawnSpeed * Time.deltaTime);

            if (!jumping)
            {
                RaycastHit hit;
                Ray ray = new Ray(transform.position, Vector3.down);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.distance <= (visual.GetComponent<CapsuleCollider>().height / 2) + 0.2f)
                    {
                        transform.position = hit.point + (Vector3.up * (visual.GetComponent<CapsuleCollider>().height / 2));
                    }
                }
            }


            visual.transform.rotation = Quaternion.LookRotation(new Vector3(pawnCurMovement.x,0,pawnCurMovement.y),Vector3.up);
            visual.transform.Rotate(pivotX.transform.rotation.eulerAngles);
            if (transform.position.y - previousY < -0.05f)
            {
                momentum += Time.deltaTime * 10; ;
            }
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



    public override void SouthButtonInput()
    {
        Jump();
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
        GetComponent<Rigidbody>().useGravity = false;
        float time = 0;
        float previous = 0;
        while (time <= jumpTime)
        {

            float ratio = time / jumpTime;

            float upDisplacement = jumpCurve.Evaluate(ratio) - previous;

            transform.position += Vector3.up * (upDisplacement * jumpHeight);


            previous = jumpCurve.Evaluate(ratio);
            yield return  new WaitForEndOfFrame();
            time += Time.deltaTime;
            
        }
        GetComponent<Rigidbody>().useGravity = true;
        jumping = false;
    }

    public override void CalcGrounded()
    {
        isGrounded = Physics.Raycast(this.transform.position, Vector3.down, (visual.GetComponent<CapsuleCollider>().height / 2) + 0.1f) ;
    }

    public void DrawDebug()
    {
        Color col = Color.red;
        if (Physics.Raycast(this.transform.position, Vector3.down, (visual.GetComponent<CapsuleCollider>().height / 2) + 0.1f))
        {
            col = Color.green;            
        }
        Debug.DrawRay(transform.position, transform.up * -1 * ((visual.GetComponent<CapsuleCollider>().height / 2)), col, 0);

        Color col2 = Color.red;
        if (Physics.Raycast(this.transform.position + visual.transform.forward*0.1f, Vector3.down, (visual.GetComponent<CapsuleCollider>().height / 2) + 0.2f))
        {
            col2 = Color.green;
        }
        Debug.DrawRay(transform.position + visual.transform.forward * 0.1f, transform.up * -1 * ((visual.GetComponent<CapsuleCollider>().height / 2) + 0.2f), col2, 0);
    }

}
