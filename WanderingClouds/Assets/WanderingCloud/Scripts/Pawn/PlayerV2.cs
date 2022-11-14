using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;

namespace WanderingCloud.Controller
{
    public class PlayerV2 : Pawn
    {
        [Foldout("Ref")] public Transform chara;
        [Foldout("Ref")] public GameObject pivotX;
        [Foldout("Ref")] public GameObject pivotY;
        [Foldout("Ref")] public Rigidbody body;
        [Foldout("Ref")] public CapsuleCollider capsuleCollider;

        // Camera Movement
        [MinMaxSlider(30, 90)] [Foldout("Camera")]
        public Vector2 fov = new Vector2(45, 60);

        [Foldout("Camera")] protected Vector2 camCurMovement;
        [Foldout("Camera")] public Vector3 zoomOffset;
        [Foldout("Camera")] public AnimationCurve sensibilityEvolve = AnimationCurve.Linear(0, 0, 1, 1);
        [Foldout("Camera")] public float camSensibility = 1f;
        [Foldout("Camera")] private float aimTime;
        [Foldout("Camera")] public bool isAiming;

        // Pawn Movement
        [ReadOnly, SerializeField] [Foldout("Movement")]
        protected Vector3 inputMovement;

        [Foldout("Movement")] public float speed = 2;

        public bool isGrounded;
        public bool isOnEdge;

        [Foldout("Slide")] public float slopeValue = -15f;
        [Foldout("Slide")] public float curAngle;
        [Foldout("Slide")] public Vector3 slopeVector;

        [Foldout("Jump")] public float jumpForce = 5;
        [Foldout("Jump"), Range(0, 1)] public float flotiness;

        protected virtual void Update()
        {
            CalcGrounded();
            if (allowCameraMovement) CameraUpdate();
        }

        protected virtual void FixedUpdate()
        {
            if (allowBodyMovement) MovementUpdate();
        }

        protected virtual void CameraUpdate()
        {
            if (camCurMovement != Vector2.zero)
            {
                pivotX.transform.Rotate(Vector3.up, camCurMovement.x);
                float maxAngle = 45;
                if (Mathf.Abs(pivotY.transform.eulerAngles.x + camCurMovement.y) <= maxAngle ||
                    Mathf.Abs(pivotY.transform.eulerAngles.x + camCurMovement.y) >= 360 - maxAngle)
                {
                    pivotY.transform.Rotate(Vector3.right, camCurMovement.y);
                }
            }
        }

        public void MovementUpdate()
        {
            CalcGrounded();

            if (inputMovement.magnitude < Mathf.Epsilon) return;
            var aimRot = Quaternion.LookRotation(inputMovement, Vector3.up);
            chara.rotation = Quaternion.Slerp(chara.rotation, aimRot, 4 * Time.deltaTime);

            if (isGrounded)
            {
                body.AddForce(slopeVector.normalized * (inputMovement.magnitude * (speed * Time.deltaTime)),
                    ForceMode.VelocityChange);

                if (!isOnEdge)
                {
                    body.velocity = slopeVector.normalized * body.velocity.magnitude;
                }
            }
            else
            {
                body.AddForce(inputMovement * (speed * Time.deltaTime), ForceMode.VelocityChange);
                body.AddForce(Vector3.up * (flotiness * Time.deltaTime), ForceMode.VelocityChange);
            }


            if (curAngle < slopeValue)
            {
                body.AddForce(slopeVector.normalized * Mathf.InverseLerp(-20, -60, curAngle), ForceMode.VelocityChange);
            }

        }

        #region input

        public override void CameraMovementInput(Vector2 input)
        {
            camCurMovement = new Vector2(Mathf.Sign(input.x) * sensibilityEvolve.Evaluate(Mathf.Abs(input.x)),
                Mathf.Sign(input.y) * -sensibilityEvolve.Evaluate(Mathf.Abs(input.y)));
            camCurMovement *= camSensibility;
        }
        public override void MovementInput(Vector2 input) =>
            inputMovement = pivotX.transform.forward * input.y + pivotX.transform.right * input.x;
        public override void RightTriggerInput()
        {
        }
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
            if (!isGrounded) return;

            body.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }

        public void CalcGrounded()
        {
            var height = capsuleCollider.height;
            var feetPos = transform.position + Vector3.down * ((height - 0.05f) / 2);

            RaycastHit underHit;
            Ray underRay = inputMovement.magnitude < Mathf.Epsilon
                ? new Ray(feetPos, Vector3.down)
                : new Ray(feetPos, Vector3.down);
            isGrounded = Physics.Raycast(underRay, out underHit, 0.5f);
            Debug.DrawRay(underRay.origin, underRay.direction * 0.5f, isGrounded ? Color.green : Color.red);

            if (!isGrounded)
            {
                curAngle = float.NaN;
                return;
            }

            float predictDist = capsuleCollider.radius + 0.05f;
            RaycastHit forwardHit;
            Ray forwardRay = new Ray(feetPos + Vector3.up * height + chara.forward * predictDist, Vector3.down);
            isOnEdge = !Physics.Raycast(forwardRay, out forwardHit, height * 2);
            Debug.DrawRay(forwardRay.origin, forwardRay.direction * (capsuleCollider.height * 2),
                isOnEdge ? Color.green : Color.red);

            if (isOnEdge)
            {
                curAngle = float.NaN;
                return;
            }

            //Calcul Slope
            slopeVector = forwardHit.point - underHit.point;
            Debug.DrawLine(forwardHit.point, underHit.point, Color.yellow);

            curAngle = (Mathf.Atan2(slopeVector.y, predictDist) * Mathf.Rad2Deg);

        }
    }
}
