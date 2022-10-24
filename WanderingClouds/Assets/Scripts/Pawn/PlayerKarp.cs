using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;

public class PlayerKarp : Pawn
{
    [Foldout("Ref")] public Transform chara;
    [Foldout("Ref")] public GameObject pivotX;
    [Foldout("Ref")] public GameObject pivotY;
    [Foldout("Ref")] public Rigidbody body;
    [Foldout("Ref")] public CapsuleCollider capsuleCollider;

    // Camera Movement
    [MinMaxSlider(30, 90)]
    [Foldout("Camera")] public Vector2 fov = new Vector2(45, 60);
    [Foldout("Camera")] protected Vector2 camCurMovement;
    [Foldout("Camera")] public Vector3 zoomOffset;
    [Foldout("Camera")] public float camSensibility = 1f;
    [Foldout("Camera")] private float aimTime;
    [Foldout("Camera")] public bool isAiming;

    // Pawn Movement
    [ReadOnly, SerializeField] 
    [Foldout("Movement")] protected Vector3 inputMovement;
    [Foldout("Movement")] public float speed = 2;

    [Foldout("Slide")] public float slopeValue = 10f;
    [Foldout("Slide")] public float curAngle;

    [Foldout("Jump")] public float jumpForce = 5;
    [Foldout("Jump"), Range(0,1)] public float flotiness;

    protected virtual void Update()
    {
        base.Update();
        if (allowCameraMovement) CameraUpdate();
    }
    protected virtual void FixedUpdate()
    {
        if (allowMovement) MovementUpdate();
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
    public void MovementUpdate()
    {
        
        body.AddForce(inputMovement * ( speed * Time.deltaTime), ForceMode.VelocityChange);

        if (body.velocity.y < 0)
        {
            CalcGrounded();
            CalcSlopAngle();

            body.AddForce(Vector3.up * (flotiness * Time.deltaTime), ForceMode.Force);
                
        }
    }
    
    #region input
    public override void CameraMovementInput(Vector2 input) => camCurMovement = new Vector2(input.x, -input.y);
    public override void MovementInput(Vector2 input) => inputMovement = pivotX.transform.forward * input.y + pivotX.transform.right * input.x;

    public override void RightTriggerInput(){}
    public override void SouthButtonInput() => Jump();
    public override void LeftTriggerInput()
    {
        isAiming = true;
        Camera.transform.DOLocalMove(zoomOffset, 0.2f);
        Camera.DOFieldOfView(fov.y, 0.2f);
    }
    public override void LeftTriggerInputReleased()
    {
        isAiming = false;
        Camera.transform.DOLocalMove(Vector3.zero, 0.2f);
        Camera.DOFieldOfView(fov.x, 0.2f);
    }

    #endregion

    public void Jump()
    {
        if (isGrounded) body.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }
    
    public override void CalcGrounded()
    {
        isGrounded = Physics.Raycast(this.transform.position, Vector3.down, (capsuleCollider.height / 2) + 0.1f);
    }
    public void CalcSlopAngle()
    {
        RaycastHit forwardPos;
        Physics.Raycast(new Ray(transform.position + Vector3.down * capsuleCollider.height/2+ (Vector3)inputMovement.normalized * 0.25f, Vector3.down), out forwardPos);
        
        curAngle = Mathf.Atan2(forwardPos.distance - capsuleCollider.height/2,  0.25f) * Mathf.Rad2Deg;
    }
}
