#if UNITY_EDITOR
using UnityEditor;
#endif
using WanderingCloud.Gameplay;
using System;
using System.Collections;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

namespace WanderingCloud.Controller
{
    public enum MovementState
    {
        Walking = 0,
        Running = 1,
        Sprinting = 2,
        Sliding = 3,
    }

    public class Player : Pawn
    {
        [SerializeField] private bool debugMode;

        #region References

        [field: SerializeField] public Rigidbody Body { get; private set; }
        [field: SerializeField] public Transform Avatar { get; private set; }
        [field: SerializeField] public CapsuleCollider Collider { get; private set; }
        [field: SerializeField] public CinemachineFreeLook Cinemachine { get; private set; }

        #endregion

        #region Run Parameter
        [Foldout("Run"), SerializeField] private float airSpeed = 10f;
        [Foldout("Run"), SerializeField] private float walkSpeed = 20f;
        [Foldout("Run"), SerializeField] private float runSpeed = 40f;
        [Foldout("Run"), SerializeField] private float sprintSpeed = 60f;
        [field: SerializeField, Foldout("Run")] public MovementState moveState { get; private set; }
        [Foldout("Run"), SerializeField] private float sprintTimeDuration = 0.2f;
        [Foldout("Run"), SerializeField, ReadOnly] private float sprintTime = 0f;
        [Foldout("Run"), SerializeField, ReadOnly] private bool isRunPressed = false;
        #endregion

        #region Jump Parameter
        [Foldout("Jump"), SerializeField, Range(0f, 5f)] private float jumpHeight = 5f;
        [Foldout("Jump"), SerializeField, Range(0f, 4f)] private float fallFactor = 1.666f;
        [Foldout("Jump"), SerializeField] private float fallSpeedMax = 10f;
        [Foldout("Jump")] private Coroutine jump = null;
        #endregion

        #region Status
        [field: SerializeField, Foldout("Info"), ReadOnly] public bool isGrounded { get; private set; }
        [field: SerializeField, Foldout("Info"), ReadOnly] public bool isNearEdge { get; private set; }
        [field: SerializeField, Foldout("Info"), ReadOnly] public float slopeAngle { get; private set; }
        [field: SerializeField, Foldout("Info"), ReadOnly] public  Vector3 inputMovement{ get; private set; }
        [field: SerializeField, Foldout("Info"), ReadOnly] public  Vector3 slopeMovement{ get; private set; }
        #endregion

        #region GrabObject
        [SerializeField] private Transform grabSocket;
        [SerializeField] private GrabableObject grabObject;
        #endregion

        #region UnityMethods
        private void Awake()
        {
            moveState = MovementState.Walking;
            Cinemachine.gameObject.SetActive(false);
        }

        private void Update()
        {
            MovementUpdate();
            //Clamp FallSpeed
        }

        #endregion

        public override void PlayerConnect(int playerIndex)
        {
            Cinemachine.gameObject.SetActive(true);
            var inputLink = Cinemachine.GetComponent<CinemachineInputProvider>();

            inputLink.PlayerIndex = playerIndex;
        }
        public override void PlayerDisconnect(int playerIndex)
        {
            Cinemachine.gameObject.SetActive(false);
        }

        public override void MovementInput(Vector2 input)
        {
            Vector3 forward = Vector3.ProjectOnPlane(Camera.transform.forward, Vector3.up);
            Vector3 right = Vector3.ProjectOnPlane(Camera.transform.right, Vector3.up);
            Debug.DrawLine(Camera.transform.position, Camera.transform.position + forward * 5, Color.magenta);
            Debug.DrawLine(Camera.transform.position, Camera.transform.position + right * 5, Color.cyan);
            inputMovement = input.y * forward + input.x * right;
            Debug.DrawLine(Camera.transform.position, Camera.transform.position + inputMovement * 5, Color.green);

        }
        public override void SouthButtonInput() => Jump();

        public override void WestButtonInput()
        {
            isRunPressed = true;
            sprintTime = sprintTimeDuration;
            switch (moveState)
            {
                case MovementState.Walking:
                    moveState = MovementState.Running;
                    break;
                case MovementState.Running or MovementState.Sprinting:
                    moveState = MovementState.Sprinting;
                    break;
                default:
                    break;
            }
        }
        public override void WestButtonInputReleased()
        {
            isRunPressed = false;
        }

        public void MovementUpdate()
        {
            ComputeState();

            if (inputMovement.magnitude < Mathf.Epsilon) return;
            var aimRot = Quaternion.LookRotation(inputMovement, Vector3.up);
            Body.transform.rotation = Quaternion.Slerp(Body.transform.rotation, aimRot, 7 * Time.deltaTime);

            if (sprintTime > 0)
            {
                sprintTime -= Time.deltaTime;
            }
            else
            {
                moveState = isRunPressed? MovementState.Running : MovementState.Walking;
            }

            
            if (isGrounded)
            {
                switch (moveState)
                {
                    case MovementState.Walking:
                        Body.AddForce(slopeMovement.normalized * (slopeMovement.magnitude * (walkSpeed * Time.deltaTime)),
                            ForceMode.VelocityChange);
                        break;
                    case MovementState.Running:
                        Body.AddForce(slopeMovement.normalized * (slopeMovement.magnitude * (runSpeed * Time.deltaTime)),
                            ForceMode.VelocityChange);
                        break;
                    case MovementState.Sprinting:
                        Body.AddForce(slopeMovement.normalized * (slopeMovement.magnitude * (sprintSpeed * Time.deltaTime)),
                            ForceMode.VelocityChange);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Body.AddForce(slopeMovement.normalized * (slopeMovement.magnitude * (airSpeed * Time.deltaTime)),
                    ForceMode.VelocityChange);
            }
        }

        public void Jump()
        {
            if (!isGrounded || jump is not null) return;
            jump = StartCoroutine(Jumping(jumpHeight));
        }

        /// <summary>
        /// Inspired by this
        /// https://answers.unity.com/questions/854006/jumping-a-specific-height-using-velocity-gravity.html
        /// but issue with the drag
        /// https://answers.unity.com/questions/49001/how-is-drag-applied-to-force.html
        /// Nique le drag, il sera à 0
        /// </summary>
        public IEnumerator Jumping(float height)
        {
            //body.velocity += Vector3.up  * Mathf.Sqrt(-2.0f * Physics2D.gravity.y * jumpHeight);
            Body.AddForce(Vector3.up * Mathf.Sqrt(-2.0f * Physics2D.gravity.y * (jumpHeight)),
                ForceMode.VelocityChange);

            yield return new WaitForFixedUpdate();
            while (Body.velocity.y > 0)
            {
                //Counter *drag by /dragFactor
                float dragFactor = (1.0f - (100 * float.Epsilon)) - (Body.drag * Time.fixedDeltaTime);
                Body.velocity *= 1f / dragFactor;

                yield return new WaitForFixedUpdate();
            }

            while (!isGrounded && Body.velocity.y < 0)
            {
                //Apply Gravity Scale
                float fallValue = fallFactor - 1.0f;
                Body.AddForce(Physics.gravity * fallValue, ForceMode.Acceleration);


                yield return new WaitForFixedUpdate();
            }

            jump = null;
        }


        public void ComputeState()
        {
            slopeMovement = inputMovement;

            var height = Collider.height;
            var feetPos = transform.position + Vector3.down * ((height - 0.05f) / 2);

            RaycastHit underHit;
            Ray underRay = new Ray(feetPos, Vector3.down);
            isGrounded = Physics.Raycast(underRay, out underHit, 0.5f);
            if (debugMode)
                Debug.DrawRay(underRay.origin, underRay.direction * 0.5f, isGrounded ? Color.green : Color.red);

            if (!isGrounded)
            {
                //curAngle = float.NaN;
                return;
            }

            float predictDist = Collider.radius + 0.05f;
            RaycastHit forwardHit;
            Ray forwardRay = new Ray(feetPos + Vector3.up * height + Avatar.forward * predictDist, Vector3.down);
            isNearEdge = !Physics.Raycast(forwardRay, out forwardHit, height * 2);
            if (debugMode)
                Debug.DrawRay(forwardRay.origin, forwardRay.direction * (Collider.height * 2),
                    isNearEdge ? Color.green : Color.red);

            if (isNearEdge)
            {
                //curAngle = float.NaN;
                return;
            }

            //Calcul Slope
            slopeMovement = forwardHit.point - underHit.point;
            Debug.DrawLine(forwardHit.point, underHit.point, Color.yellow);

            slopeAngle = (Mathf.Atan2(slopeMovement.y, predictDist) * Mathf.Rad2Deg);
        }

        


        public override void EstButtonInput()
        {
            Grab();
        }

        public void Grab()
        {
            if (grabObject != null) UnGrab();
        }

        public void UnGrab()
        { 

        }



#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!debugMode) return;
            using (new Handles.DrawingScope())
            {
                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

                Handles.color = Color.yellow;
                Handles.DrawLine(transform.position, transform.position + inputMovement);
            }
        }
#endif
    }
}