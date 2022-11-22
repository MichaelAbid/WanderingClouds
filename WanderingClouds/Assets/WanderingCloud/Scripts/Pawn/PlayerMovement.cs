using System;
using NaughtyAttributes;
using UnityEngine;
using System.Collections;

namespace WanderingCloud.Controller
{
    public enum MovementState
    {
        Idle = 0,
        Walk = 1,
        Rush = 2,
        Dash = 3,
    }

    public class PlayerMovement : MonoBehaviour
    {
        [HideInInspector] private PlayerBrain player;

        #region Run Parameter
        [Header("Run Parameter")] 
        [SerializeField] private float airSpeed = 10f;
        [SerializeField] private float walkSpeed = 20f;
        [SerializeField] private float runSpeed = 40f;
        [SerializeField] private float dashSpeed = 60f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField, ReadOnly] private bool isDashing = false;
        [field: SerializeField, ReadOnly] public MovementState moveState { get; private set; }
        #endregion

        #region Jump Parameter
        [Foldout("Jump"), SerializeField, Range(0f, 5f)]  private float jumpHeight = 5f;
        [Foldout("Jump"), SerializeField, Range(0f, 4f)] private float fallFactor = 1.666f;
        [Foldout("Jump"), SerializeField] private float fallSpeedMax = 10f;
        [Foldout("Jump")] private Coroutine jump = null;
        #endregion

        public PlayerState state = new PlayerState();

        #region UnityMethods
        private void Awake()
        {
            player = GetComponent<PlayerBrain>();
        }

        private void Update()
        {
            state.RefreshState();
        }
        private void FixedUpdate()
        {
            Run();
            if (!state.isGrounded) Falling();
        }
        #endregion

        private void Run()
        {
            //if (player.moveInput.magnitude < Mathf.Epsilon) return;

            //To Cam XZ
            Vector3 forward = Vector3.ProjectOnPlane(player.Camera.transform.forward, Vector3.up);
            Vector3 right = Vector3.ProjectOnPlane(player.Camera.transform.right, Vector3.up);
            var movement = player.moveInput.y * forward + player.moveInput.x * right;
            var movementStrenght = movement.magnitude;
            var movementDirection = Vector3.ProjectOnPlane(movement, state.slopeNormal).normalized;
            //Turn where you run
            if (movementStrenght > float.Epsilon)
            {
                var aimRot = Quaternion.LookRotation(movementDirection, state.slopeNormal);
                player.Body.transform.rotation = Quaternion.Slerp(player.Body.transform.rotation, aimRot, 5 * Time.deltaTime);
            }

            if (!state.isGrounded)
            {
                player.Body.AddForce(movement.normalized * (movement.magnitude * (airSpeed * Time.deltaTime)), ForceMode.VelocityChange);
                return;
            }

            switch (moveState)
            {
                case MovementState.Walk:
                    player.Body.AddForce(movementDirection * (movementStrenght * (walkSpeed * Time.deltaTime)),
                        ForceMode.VelocityChange);
                    break;
                case MovementState.Rush:
                    player.Body.AddForce(movementDirection * (movementStrenght * (runSpeed * Time.deltaTime)),
                        ForceMode.VelocityChange);
                    break;
                default:
                    break;
            }
        }

        public void Jump()
        {
            if (!state.isGrounded || jump is not null) return;
            jump = StartCoroutine(Jumping(jumpHeight));
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
            player.Body.velocity = Vector3.Scale(new Vector3(1,0,1), player.Body.velocity) + state.slopeNormal * Mathf.Sqrt(-2.0f * Physics2D.gravity.y * jumpHeight);
            
            //player.Body.AddForce(Vector3.up * Mathf.Sqrt(-2.0f * Physics2D.gravity.y * (jumpHeight)), ForceMode.VelocityChange);

            yield return new WaitForFixedUpdate();
            while (player.Body.velocity.y > 0)
            {
                //Counter *drag by /dragFactor
                float dragFactor = (1.0f - (100 * float.Epsilon)) - (player.Body.drag * Time.fixedDeltaTime);
                player.Body.velocity *= 1f / dragFactor;

                yield return new WaitForFixedUpdate();
            }

            while (!state.isGrounded && player.Body.velocity.y < 0)
            {
                //Apply Gravity Scale
                float fallValue = fallFactor - 1.0f;
                player.Body.AddForce(Physics.gravity * fallValue, ForceMode.Acceleration);

                yield return new WaitForFixedUpdate();
            }

            jump = null;
        }
        
        private void Falling()
        {
            if (player.Body.velocity.y < -Mathf.Abs(fallSpeedMax))
            {
                player.Body.velocity =new Vector3(player.Body.velocity.x,  fallSpeedMax, player.Body.velocity.z);
            }
        }


        public void Dash()
        {
            //Can dash Mid Air
            Debug.Log("dash");
        }
    }

    [System.Serializable]
    public class PlayerState
    {
        [SerializeField] private PlayerBrain player;

        [field: SerializeField, ReadOnly] public bool isGrounded { get; private set; }
        [field: SerializeField, ReadOnly] public bool isNearEdge { get; private set; }
        [field: SerializeField, ReadOnly] public float slopeAngle { get; private set; }
        [field: SerializeField, ReadOnly] public Vector3 slopeVector { get; private set; }
        [field: SerializeField, ReadOnly] public Vector3 slopeNormal { get; private set; }
        
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
            isGrounded = Physics.Raycast(underRay, out underHit, 0.5f);
            Debug.DrawRay(underRay.origin, underRay.direction * 0.5f, isGrounded ? Color.green : Color.red);

            if (!isGrounded) return;
            //Calcul Slope
            float predictDist = player.Collider.radius;
            slopeNormal = underHit.normal;
            slopeVector = Vector3.ProjectOnPlane(avatar.forward * predictDist,slopeNormal);
            slopeAngle = Mathf.Atan2(slopeVector.y, predictDist) * Mathf.Rad2Deg;
            slopeVector = slopeVector.normalized;
            Debug.DrawLine(feetPos, feetPos+slopeVector, Color.yellow);
            
            //Edge verif
            Ray forwardRay = new Ray(feetPos + Vector3.up + avatar.forward * predictDist, Vector3.down);
            isNearEdge = !Physics.Raycast(forwardRay, height);
            Debug.DrawRay(forwardRay.origin, forwardRay.direction * height, isNearEdge ? Color.green : Color.red);
        }
    }
}