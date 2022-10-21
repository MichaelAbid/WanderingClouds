using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : Pawn
{
    [Foldout("Ref")] public GameObject visual;
    [Foldout("Ref")] public MeshRenderer visualMeshRenderer;
    // Camera Movement
    [Foldout("Camera")] public GameObject pivotX;
    [Foldout("Camera")] public GameObject pivotY;
    [Foldout("Camera"), MinMaxSlider(30,90)] public Vector2 fov = new  Vector2(45, 60);
    [Foldout("Camera")] public Vector3 zoomOffset;
    [Foldout("Camera")] public float camSensibility;
    [Foldout("Camera")] public bool isAiming;
    private float aimTime;
    protected Vector2 camCurMovement;
    
    // Pawn Movement
    [ReadOnly, SerializeField] 
    [Foldout("Movement")] protected Vector2 pawnCurMovement;
    [Foldout("Movement")] public float pawnSpeed = 2;
    [Foldout("Movement"), Range(0,0.1f)] public float slopeMagicValue = 0.05f;
    [Foldout("Movement")] public float speedForColorChange;
    [Foldout("Movement")] public Color colorNormal;
    [Foldout("Movement")] public Color colorMaxSpeed;


    [Foldout("Momentum"), ReadOnly] public float momentum;
    [Foldout("Momentum"), ReadOnly] public Vector2 momentumDirection;
    [Foldout("Momentum")] public float momentumMax;
    [Foldout("Momentum")] public float momentumGainPerSeconds;
    [Foldout("Momentum")] public float momentumConsumptionPerSeconds;
    [Foldout("Momentum")] public float momentumConsumptionPerSecondsInAir;
    [Foldout("Momentum")] public float momentumConsumptionPerSecondsWhenBreak;
    [Foldout("Momentum")] public float speedAtMaxMomentum;
    [Foldout("Momentum")] public float gravityWhenMomentumInAir;

    

    [CurveRange(EColor.Blue)] 
    [Foldout("Jump")]public AnimationCurve jumpCurve;
    [Foldout("Jump")] public bool jumping = false;
    [Foldout("Jump")] public float jumpTime = 2;
    [Foldout("Jump")] public float jumpHeight = 2;
    
    private CapsuleCollider CapsuleCollider 
    {
        get
        {
            if(capsuleCollider is null) capsuleCollider = visual.GetComponent<CapsuleCollider>();
            return capsuleCollider;
        }
    }
    private CapsuleCollider capsuleCollider;
    private Rigidbody Body 
    {
        get
        {
            if(body is null) body = GetComponent<Rigidbody>();
            return body;
        }
    }
    private Rigidbody body;

    protected virtual void Update()
    {
        base.Update();
        DrawDebug();
        if (allowCameraMovement) CameraUpdate();
        if (allowMovement) MovementUpdate();

        if (!isGrounded)
        {
            if (momentum > 0)
            {
                Body.drag = gravityWhenMomentumInAir;
            }
            else
            {
                Body.drag = 0;
            }
        }
        else
        {
            Body.drag = 0;
        }
        

    }
    protected virtual void CameraUpdate()
    {
        if (camCurMovement != Vector2.zero)
        {
            pivotX.transform.Rotate(Vector3.up, camCurMovement.x * camSensibility);
            float maxAngle = 45;
            if (Mathf.Abs(pivotY.transform.eulerAngles.x + camCurMovement.y) <= maxAngle ||
                Mathf.Abs(pivotY.transform.eulerAngles.x + camCurMovement.y) >= 360 - maxAngle)
            {
                pivotY.transform.Rotate(Vector3.right, camCurMovement.y * camSensibility);
            }
        }
    }
    protected virtual void MovementUpdate()
    {
        if (pawnCurMovement != Vector2.zero)
        {
            float previousY = transform.position.y;

            float speed = pawnSpeed;
            Vector2 move;
            // Using Momentum
            if (momentum > 0)
            {
                Debug.Log(Vector3.Dot(momentumDirection, pawnCurMovement));
                if (Vector3.Dot(momentumDirection, pawnCurMovement) < 0.5f)
                {
                    momentum -= (momentumConsumptionPerSecondsWhenBreak) * Time.deltaTime;
                    Debug.Log("Breaking");

                }

                speed += Mathf.Clamp((momentum * (speedAtMaxMomentum - pawnSpeed)) / momentumMax, pawnSpeed,
                    speedAtMaxMomentum);

                momentum -= (isGrounded? momentumConsumptionPerSeconds: momentumConsumptionPerSecondsInAir) * Time.deltaTime;
                momentum = Mathf.Clamp(momentum, 0, momentumMax);
                move = momentumDirection * (speed * Time.deltaTime);



            }
            else
            {
                momentumDirection = Vector3.zero;
                move = pawnCurMovement * (speed * Time.deltaTime);
            }

            var movement = (pivotX.transform.forward * move.y + pivotX.transform.right * move.x);
            Ray moveRay = new Ray(transform.position, movement);
            Ray moveRayL = new Ray(transform.position - Vector3.Cross(Vector3.up,movement ).normalized * CapsuleCollider.radius, movement);
            Ray moveRayR = new Ray(transform.position + Vector3.Cross(Vector3.up,movement ).normalized * CapsuleCollider.radius, movement);
            if (   Physics.Raycast(moveRay,CapsuleCollider.radius + 0.2f) 
                || Physics.Raycast(moveRayL,CapsuleCollider.radius + 0.2f) 
                || Physics.Raycast(moveRayR,CapsuleCollider.radius + 0.2f))
            {
                //HitTheWall
                momentum = 0;
            }
            else
            {
                transform.position += movement;
            }
            
            // Stick Player to Slope 
            if (!jumping)
            {
                RaycastHit hit;
                Ray ray = new Ray(transform.position, Vector3.down);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.distance <= (CapsuleCollider.height / 2) + ((0.2f / 10) * speed))
                    {
                        transform.position =
                            hit.point + (Vector3.up * (CapsuleCollider.height / 2));
                        
                    }
                    else
                    {
                        
                    }
                }
            }
            
            // Gain Momentum
            if (transform.position.y - previousY < -slopeMagicValue && momentum < momentumMax)
            {
                momentum += Time.deltaTime * momentumGainPerSeconds;
                momentum = Mathf.Clamp(momentum, 0, momentumMax);
                if (momentumDirection == Vector2.zero)
                {
                    momentumDirection = pawnCurMovement;
                }
                else
                {
                    
                    momentumDirection = Vector2.Lerp(momentumDirection, pawnCurMovement, 0.5f * Time.deltaTime);

                }
            }

            if (momentum == 0)
            {
                // Rotation Visual
                visual.transform.rotation = Quaternion.LookRotation(new Vector3(pawnCurMovement.x, 0, pawnCurMovement.y), Vector3.up);
                visual.transform.Rotate(pivotX.transform.rotation.eulerAngles);
            }

            MaterialUpdate(speed);
        }
        else
        {
            float speed = pawnSpeed;
            float previousY = transform.position.y;
            Vector3 directionToGo = visual.transform.forward;
            if (momentum > 0)
            {
                speed += Mathf.Clamp((momentum * (speedAtMaxMomentum - pawnSpeed)) / momentumMax, pawnSpeed, speedAtMaxMomentum);
                transform.position += (visual.transform.forward * (speed * Time.deltaTime));
            }

            if (!jumping)
            {
                RaycastHit hit;
                Ray ray = new Ray(transform.position, Vector3.down);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.distance <= (CapsuleCollider.height / 2) + ((0.2f / 10) * speed))
                        transform.position = hit.point + (Vector3.up * (CapsuleCollider.height / 2));
                    Body.drag = 0;
                }
                else
                {
                    if (momentum > 0)
                    {
                        Body.drag = gravityWhenMomentumInAir;
                    }
                }
            }

            if (!(transform.position.y - previousY < -slopeMagicValue))
            {
                momentum -= (isGrounded ? momentumConsumptionPerSeconds : momentumConsumptionPerSecondsInAir) * Time.deltaTime;
                momentum = Mathf.Clamp(momentum, 0, momentumMax);
            }
            MaterialUpdate(speed);
        }

    }

    #region input
    public override void CameraMovementInput(Vector2 input)=>camCurMovement = new Vector2(input.x,-input.y);
    public override void MovementInput(Vector2 input)=>pawnCurMovement = input;
    public override void RightTriggerInput() { }
    public override void LeftTriggerInput()
    {
        isAiming = true;
        Camera.transform.DOLocalMove(zoomOffset,0.2f);
        Camera.DOFieldOfView( fov.y,0.2f);

    }
    public override void LeftTriggerInputReleased()     
    {
        isAiming = false;
        Camera.transform.DOLocalMove(Vector3.zero,0.2f);
        Camera.DOFieldOfView( fov.x,0.2f);
    }
    public override void SouthButtonInput() => Jump();
    #endregion

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
        Body.useGravity = false;
        float time = 0, previous = 0;
        while (time <= jumpTime)
        {
            float ratio = time / jumpTime;
            float upDisplacement = jumpCurve.Evaluate(ratio) - previous; 
            transform.position += Vector3.up * (upDisplacement * jumpHeight);
            previous = jumpCurve.Evaluate(ratio);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
        Body.useGravity = true;
        jumping = false;
    }

    public override void CalcGrounded()
    {
        isGrounded = Physics.Raycast(this.transform.position, Vector3.down, (CapsuleCollider.height / 2) + 0.1f);
    }

    public void MaterialUpdate(float speed)
    {
        if (speed >= speedForColorChange)
        {
            visualMeshRenderer.material.color = colorMaxSpeed;
        }
        else
        {
            visualMeshRenderer.GetComponent<MeshRenderer>().material.color = colorNormal;
        }
    }



    public void DrawDebug()
    {
        Color col = Color.red;
        if (Physics.Raycast(this.transform.position, Vector3.down, (CapsuleCollider.height / 2) + 0.1f))
        {
            col = Color.green;
        }
        Debug.DrawRay(transform.position, transform.up * -1 * ((CapsuleCollider.height / 2)), col);
        
        col = Color.red;
        if (Physics.Raycast(this.transform.position + visual.transform.forward * 0.1f, Vector3.down, (CapsuleCollider.height / 2) + 0.2f))
        {
            col = Color.green;
        }
        Debug.DrawRay(transform.position + visual.transform.forward * 0.1f, transform.up * (-1 * ((CapsuleCollider.height / 2) + 0.2f)), col);


        Debug.DrawRay(transform.position , new Vector3(momentumDirection.x,0, momentumDirection.y) * 1 , Color.yellow);

    }
}