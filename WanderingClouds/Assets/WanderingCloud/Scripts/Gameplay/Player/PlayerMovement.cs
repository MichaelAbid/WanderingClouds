using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using WanderingCloud.Gameplay;
using UnityEngine.Animations.Rigging;

namespace WanderingCloud.Controller
{
    public enum MovementState
    {
        Idle = 0,
        Walk = 1,
        Rush = 2,
        Dash = 3,
        Jump = 4,
        Fall = 5,
    }

    public class PlayerMovement : MonoBehaviour
    {
        [HideInInspector] private PlayerBrain player;
        [HideInInspector] private Transform defaultParent;
        public PlayerState state = new PlayerState();
        public Vector3 externalForce;

        [field: SerializeField, ReadOnly] public MovementState moveState { get; private set; }

        #region Run Parameter
        [Foldout("Run"), SerializeField] private float airSpeed = 10f;
        [Foldout("Run"), SerializeField] private float walkSpeed = 20f;
        [Foldout("Run"), SerializeField] private float runSpeed = 40f;
        [Foldout("Run"), SerializeField] private float maxSlopeAngle = 45f;
        [Foldout("Run"), SerializeField] private float snapGroundDist = 0.15f;
        #endregion

        #region Jump Parameter
        [Foldout("Jump"), SerializeField, Range(0f, 5f)] private float jumpHeight = 5f;
        [Foldout("Jump"), SerializeField, Range(0f, 4f)] private float fallFactor = 1.666f;
        [Foldout("Jump"), SerializeField] private float fallSpeedMax = 10f;
        [Foldout("Jump")] private Coroutine jump = null;
        [Foldout("Jump")] public UnityEvent onJump;
        public bool isJumping => jump is not null;
        #endregion

        #region Dash Parameter
        [Foldout("Dash"), SerializeField, Range(0f, 5f)] private float dashDistance = 5f;
        [Foldout("Dash"), SerializeField, Range(0f, 1f)] private float dashDuration = 0.5f;
        [Foldout("Dash"), SerializeField, Range(0f, 1f)] private float dashCD = 0.3f;
        [Foldout("Dash"), SerializeField] private bool canDash = true;
        [Foldout("Dash")] public UnityEvent onDash;
        [Foldout("Dash")] private Coroutine dash = null;
        public bool isDashing => moveState == MovementState.Dash;
        #endregion

        #region ??? Parameter
        [Foldout("Debug"), SerializeField, ReadOnly()] private Vector3 movementXZ;
        [Foldout("Debug"), SerializeField, ReadOnly()] private Vector3 movementSurface;
        [Foldout("Debug"), SerializeField, ReadOnly()] private float movementStrenght = 0f;
        #endregion

        [Foldout("Fall")] public UnityEvent onFall;

        [SerializeField] private RigBuilder lookAtRig;


        #region UnityMethods
        private void Awake()
        {
            player = GetComponent<PlayerBrain>();
            state.onLanding.AddListener(() => canDash = true);
            state.onLanding.AddListener(() => { if (moveState is MovementState.Fall or MovementState.Jump) moveState = MovementState.Idle; });

            state.onLanding.AddListener(() =>
            {
                var feetPos = player.Avatar.position + Vector3.down * ((player.Collider.height - 0.05f) / 2);
                RaycastHit underHit;
                Ray underRay = new Ray(feetPos, Vector3.down);
                Physics.Raycast(underRay, out underHit, state.groundCheckDistance);

                Component comp;
                underHit.collider.TryGetComponent(typeof(CreatureSources), out comp);
                if (comp is not null);
                {
                    var creature = (CreatureSources)comp;
                    if(creature != null && creature.currentState is CloudState.SOLID)
                        transform.SetParent(comp.transform);
                }                
            });
            state.onQuitGround.AddListener(() =>
            {
                transform.SetParent(null);
                //transform.parent.DetachChildren();                
            });
        }

        private void Update()
        {
            //To Cam XZ
            Vector3 forward = Vector3.ProjectOnPlane(player.Camera.transform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(player.Camera.transform.right, Vector3.up).normalized;
            movementXZ = player.moveInput.y * forward + player.moveInput.x * right;
            movementStrenght = movementXZ.magnitude;

            movementSurface = movementXZ;
            if (state.slopeAngle > maxSlopeAngle || float.IsNaN(state.slopeAngle))
            {
            }
            else
            {
                //movementSurface = Vector3.ProjectOnPlane(movementXZ, state.slopeNormal).normalized;
            }

            SnapToGround();
        }

        private void FixedUpdate()
        {
            state.RefreshState();
            Run();
            if (!state.isGrounded) Falling();
        }
        #endregion

        private void Run()
        {
            if (isDashing) return;
            //if (player.moveInput.magnitude < Mathf.Epsilon) return;

            //Turn where you run
            AvatarOrientation();

            if (!state.isGrounded)
            {
                var temp = movementXZ * (movementStrenght * airSpeed);
                player.Body.velocity = new Vector3(temp.x, player.Body.velocity.y, temp.z);

                player.Body.AddForce(movementXZ.normalized * (movementXZ.magnitude * (airSpeed * Time.deltaTime)), ForceMode.VelocityChange);
                return;
            }

            Debug.DrawRay(player.Avatar.position, movementSurface.normalized, Color.green, Time.deltaTime, true);
            Debug.DrawRay(player.Avatar.position + Vector3.up * 0.1f, movementSurface.normalized * movementStrenght, Color.red, Time.deltaTime, true);
            var speed = movementSurface * (movementStrenght * runSpeed);
            switch (moveState)
            {
                case MovementState.Idle:
                    if (movementStrenght > float.Epsilon) moveState = MovementState.Walk;
                    if (lookAtRig.enabled is false && player.Inventory.pelletStock <= 0) lookAtRig.enabled = true;
                    break;
                case MovementState.Walk:
                    if (movementStrenght < float.Epsilon) moveState = MovementState.Idle;
                    if (lookAtRig.enabled is false && player.Inventory.pelletStock <= 0) lookAtRig.enabled = true;
                    speed = movementSurface * (movementStrenght * walkSpeed);
                    break;
                case MovementState.Rush:
                    if (Vector3.Dot(player.Body.velocity.normalized, movementSurface.normalized) < 0.5f || movementStrenght < 0.5f || player.Aim.isAiming)
                    {
                        moveState = MovementState.Walk;
                    }
                    speed = movementSurface * (movementStrenght * runSpeed);
                    if (lookAtRig.enabled is true) lookAtRig.enabled = false;
                    break;
                default:
                    break;
            }
            player.Body.velocity = new Vector3(speed.x, player.Body.velocity.y, speed.z);
            player.Body.velocity += externalForce;
            externalForce = Vector3.zero;
        }
        private void AvatarOrientation()
        {
            if (movementStrenght > float.Epsilon && !player.Aim.isAiming)
            {
                var aimRot = Quaternion.LookRotation(movementXZ, Vector3.up);
                player.Avatar.transform.rotation = Quaternion.Slerp(player.Avatar.transform.rotation, aimRot, 5 * Time.deltaTime);
            }
            else if (player.Aim.isAiming)

            {

                var aimRot = Quaternion.LookRotation(new Vector3(player.Camera.transform.forward.x, player.transform.rotation.y, player.Camera.transform.forward.z), Vector3.up);

                player.Avatar.transform.rotation = Quaternion.Slerp(player.Avatar.transform.rotation, aimRot, 5 * Time.deltaTime);

            }
        }

        public void Jump()
        {
            if (!state.isGrounded || isJumping) return;
            onJump?.Invoke();
            jump = StartCoroutine(Jumping(jumpHeight));

        }
        public void ForcedJump(float height)
        {
            if(jump != null)StopCoroutine(jump);
            jump = StartCoroutine(Jumping(height));
            onJump?.Invoke();
        }

        /// <summary>
        /// Inspired by this
        /// https://answers.unity.com/questions/854006/jumping-a-specific-height-using-velocity-gravity.html
        /// but issue with the drag
        /// https://answers.unity.com/questions/49001/how-is-drag-applied-to-force.html
        /// Nique le drag, il sera à 0
        /// </summary>
        private IEnumerator Jumping(float height)
        {
            var previousState = moveState;
            moveState = MovementState.Jump;
            if (lookAtRig.enabled is false && player.Inventory.pelletStock <= 0) lookAtRig.enabled = true;

            //player.Body.AddForce(Vector3.up * Mathf.Sqrt(-2.0f * Physics2D.gravity.y * (jumpHeight)), ForceMode.VelocityChange);
            player.Body.velocity = Vector3.Scale(new Vector3(1, 0, 1), player.Body.velocity)
                                   + state.slopeNormal * Mathf.Sqrt(-2.0f * Physics2D.gravity.y * height);

            yield return new WaitForFixedUpdate();
            while (player.Body.velocity.y > 0)
            {
                //Counter *drag by /dragFactor
                float dragFactor = (1.0f - (100 * float.Epsilon)) - (player.Body.drag * Time.fixedDeltaTime);
                player.Body.velocity *= 1f / dragFactor;

                yield return new WaitForFixedUpdate();
            }
            jump = null;
            moveState = previousState;
        }

        private void Falling()
        {
            if (!state.isGrounded && !isJumping && !isDashing)
            {
                //Apply Gravity Scale
                float fallValue = fallFactor - 1.0f;
                player.Body.AddForce(Physics.gravity * fallValue, ForceMode.Acceleration);

                if (moveState is not MovementState.Fall)
                {
                    onFall?.Invoke();

                    moveState = MovementState.Fall;
                    if (lookAtRig.enabled is false && player.Inventory.pelletStock <= 0) lookAtRig.enabled = true;
                }
            }

            if (player.Body.velocity.y < -Mathf.Abs(fallSpeedMax))
            {
                player.Body.velocity = new Vector3(player.Body.velocity.x, -fallSpeedMax, player.Body.velocity.z);
            }
        }

        private void SnapToGround()
        {
            if (state.isNearEdge || isJumping) return;

            var feetPos = player.Avatar.position - player.Avatar.up * ((player.Collider.height - player.Collider.radius) / 2);
            RaycastHit hit;
            Ray groundRay = new Ray(feetPos, -player.Avatar.up);
            if (Physics.Raycast(groundRay, out hit, player.Collider.radius + snapGroundDist))
            {
                if (hit.distance > player.Collider.radius)
                    transform.position += (hit.distance - player.Collider.radius) * -player.Avatar.up;
            }
            Debug.DrawRay(groundRay.origin, groundRay.direction * hit.distance, Color.green);
            Debug.DrawRay(groundRay.origin + groundRay.direction * hit.distance, groundRay.direction * (player.Collider.radius + snapGroundDist - hit.distance), Color.red);

        }
        public void Dash()
        {
            //Can dash Mid Air
            if (dash is not null || !canDash) return;
            if (isJumping)
            {
                StopCoroutine(jump);
                jump = null;
            }
            dash = StartCoroutine(Dashing());
            onDash?.Invoke();
        }
        private IEnumerator Dashing()
        {
            canDash = false;
            player.Aim.EndAim();
            var vel = player.Body.velocity;
            var previousState = moveState;
            moveState = MovementState.Dash;
            var dashDirection = player.Avatar.forward;
            var aimRot = Quaternion.LookRotation(dashDirection, Vector3.up);
            if (lookAtRig.enabled is true) lookAtRig.enabled = false;

            if (player.moveInput.magnitude > float.Epsilon)
            {
                dashDirection = movementSurface.normalized;
                aimRot = Quaternion.LookRotation(movementXZ, Vector3.up);
            }

            //DOTween.To(() => player.Avatar.transform.rotation, x => player.Avatar.transform.rotation = x, aimRot, 0.2f);
            player.Avatar.transform.DORotateQuaternion(aimRot, .3f);

            Vector3 startPos = player.Avatar.position;
            float dashSpeed = (dashDirection.magnitude * dashDistance) / dashDuration;
            float time = dashDuration;
            Debug.DrawRay(startPos, dashDirection * dashDistance, Color.blue, dashDuration);

            while (time > 0)
            {
                player.Body.velocity = Vector3.ProjectOnPlane(dashDirection, state.slopeNormal).normalized * dashSpeed;

                yield return new WaitForEndOfFrame();
                time -= Time.deltaTime;
            }
            player.Body.velocity = vel;
            if (state.isGrounded && !player.Aim.isAiming)
            {
                //moveState = Vector3.Dot(dashDirection, movementXZ.normalized) > 0 ? MovementState.Rush : previousState;
                moveState = MovementState.Rush;
                yield return new WaitForSecondsRealtime(dashCD);
                canDash = true;
            }
            else
            {
                moveState = MovementState.Idle;
                while (!state.isGrounded)
                {
                    yield return new WaitForEndOfFrame();
                }
                canDash = true;
            }
            dash = null;
        }
    }

    [System.Serializable]
    public class PlayerState
    {
        [SerializeField] private PlayerBrain player;

        [field: SerializeField, ReadOnly]
        public bool isGrounded
        {
            get => grounded;
            private set
            {
                if (grounded != value)
                {
                    if (value)
                    {
                        onLanding?.Invoke();
                    }
                    else
                    {
                        onQuitGround?.Invoke();
                    }
                }
                grounded = value;
            }
        }

        [SerializeField, ReadOnly] private bool grounded;
        [field: SerializeField, ReadOnly] public bool isNearEdge { get; private set; }
        [field: SerializeField, ReadOnly] public float slopeAngle { get; private set; }
        [field: SerializeField, ReadOnly] public Vector3 slopeVector { get; private set; }
        [field: SerializeField, ReadOnly] public Vector3 slopeNormal { get; private set; }
        [SerializeField] public float groundCheckDistance = .5f;
        public UnityEvent onLanding;
        public UnityEvent onQuitGround;

        public void RefreshState()
        {
            Transform avatar = player.Avatar;
            var height = player.Collider.height;
            var feetPos = avatar.position + Vector3.down * ((height - 0.05f) / 2);

            slopeAngle = float.NaN;
            slopeVector = avatar.forward;
            slopeNormal = Vector3.up;

            RaycastHit underHit;
            Ray underRay = new Ray(feetPos, Vector3.down);
            isGrounded = Physics.Raycast(underRay, out underHit, groundCheckDistance);
            //Debug.DrawRay(underRay.origin, underRay.direction * underHit.distance, Color.green);
            //Debug.DrawRay(underRay.origin + underRay.direction * underHit.distance, underRay.direction *(groundCheckDistance - underHit.distance),Color.red);

            if (!isGrounded) return;
            //Calcul Slope
            float predictDist = player.Collider.radius;
            slopeNormal = underHit.normal;
            slopeVector = Vector3.ProjectOnPlane(avatar.forward * predictDist, slopeNormal);
            slopeAngle = Mathf.Atan2(slopeVector.y, predictDist) * Mathf.Rad2Deg;
            slopeVector = slopeVector.normalized;
            Debug.DrawLine(feetPos, feetPos + slopeVector, Color.yellow);

            //Edge verif
            Ray forwardRay = new Ray(feetPos + Vector3.up + avatar.forward * predictDist, Vector3.down);
            isNearEdge = !Physics.Raycast(forwardRay, height + 1);
            Debug.DrawRay(forwardRay.origin, forwardRay.direction * height, isNearEdge ? Color.green : Color.red);
        }
    }
}