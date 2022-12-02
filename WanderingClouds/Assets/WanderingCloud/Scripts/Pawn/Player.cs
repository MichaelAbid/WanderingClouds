#if UNITY_EDITOR
using UnityEditor;
#endif
using WanderingCloud.Gameplay;
using System.Linq;
using System.Collections;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;
using WanderingCloud.Gameplay.AI;

namespace WanderingCloud.Controller
{
    public class Player : Pawn
    {
        [SerializeField] private bool debugMode;

        #region References
        [field: SerializeField] public Rigidbody Body { get; private set; }
        [field: SerializeField] public Transform Avatar { get; private set; }
        [field: SerializeField] public CapsuleCollider Collider { get; private set; }
        [field: SerializeField] public CinemachineFreeLook Cinemachine { get; private set; }

        [field: SerializeField] public CloudGrabber cloudGrabber { get; private set; }
        [field: SerializeField] public CloudExploder cloudExploder { get; private set; }

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

        #region Aiming
        [field: SerializeField, Foldout("Aim"), ReadOnly] public bool isAiming { get; private set; }
        [SerializeField, Foldout("Aim")] public Source currentTarget = null;
        #endregion
        
        #region GrabObject
        [SerializeField] private Transform grabSocket;
        [SerializeField] private AI_Base grabObject;
        #endregion

        #region UnityMethods
        private void Awake()
        {
            moveState = MovementState.Idle;
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
            inputMovement = input.y * forward + input.x * right;
        }
        public override void SouthButtonInput() => Jump();

        public override void WestButtonInput()
        {
            isRunPressed = true;
            sprintTime = sprintTimeDuration;
            switch (moveState)
            {
                case MovementState.Walk:
                    moveState = MovementState.Rush;
                    break;
                default:
                    break;
            }
        }
        public override void WestButtonInputReleased()
        {
            isRunPressed = false;
        }

        public override void LeftTriggerInput()
        {
            Aim();
        }


        public override void LeftTriggerInputReleased()
        {
            UnAim();
        }


        public void Aim()
        {
            isAiming = true;
            var nearObject = Physics.OverlapSphere(Avatar.position, 10f);
            if (nearObject.Length == 0) return;
            var nearTarget = nearObject.
                Where(x => x.GetComponent<Source>()||x.GetComponent<BumperSource>() || x.GetComponent<CloudSource>()).
                Select(x => x.GetComponent<Source>()).ToArray();
            if (nearTarget.Length == 0) return;
            var target = nearTarget.OrderBy(x => Vector3.Distance(x.transform.position, Avatar.position)).First();
            Cinemachine.LookAt = target.transform;
            currentTarget = target;
        }

        public void UnAim()
        {
            isAiming = false;
            Cinemachine.LookAt = Avatar;
        }

        public void CamUpdate()
        {

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
                moveState = isRunPressed? MovementState.Rush : MovementState.Walk;
            }

            
            if (isGrounded)
            {
                switch (moveState)
                {
                    case MovementState.Walk:
                        Body.AddForce(slopeMovement.normalized * (slopeMovement.magnitude * (walkSpeed * Time.deltaTime)),
                            ForceMode.VelocityChange);
                        break;
                    case MovementState.Rush:
                        Body.AddForce(slopeMovement.normalized * (slopeMovement.magnitude * (runSpeed * Time.deltaTime)),
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
        public void Jump(float height)
        {
            //if (!isGrounded || jump is not null) return;
            if (jump != null) { 
                StopCoroutine(jump);
            }
            jump = StartCoroutine(Jumping(height));
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
            Body.AddForce(Vector3.up * Mathf.Sqrt(-2.0f * Physics2D.gravity.y * (height)),
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
            if (isAiming)
            {
                Launch();
            }
            else
            {
                bool grabIAsucess = GrabIA();
                if (!grabIAsucess)
                {
                    bool grabPulletsucess = Grab();
                    if (!grabPulletsucess)
                    {
                        bool explodesucess = ExplodeSource();
                    }
                }
            }
        }

        public void Launch()
        {
            if (currentTarget != null)
            {
                //cloudGrabber.LaunchPullet();
            }
        }

        public bool Grab()
        {
            return cloudGrabber.GrabNearestPullet();
        }

        public bool GrabIA()
        {
            var nearObject = Physics.OverlapSphere(Avatar.position, 1f);
            if (nearObject.Length == 0) return false;
            var nearTarget = nearObject.
                Where(x => x.GetComponent<AI_Base>() && x.GetComponent<AI_Base>().isGrabbable).
                Select(x => x.GetComponent<AI_Base>()).ToArray();
            if (nearTarget.Length == 0) return false;
            var target = nearTarget.OrderBy(x => Vector3.Distance(x.transform.position, Avatar.position)).First();
            grabObject = target;
            target.isAiActive = false;
            target.agent.enabled = false;
            return true;
        }

        public bool ExplodeSource()
        {
            return cloudExploder.ExplodeNearest();
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
                
                Handles.DrawWireDisc(Avatar.position, Avatar.up, 10f);
            }
        }
#endif
    }
}