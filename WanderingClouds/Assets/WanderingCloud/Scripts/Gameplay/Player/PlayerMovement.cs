﻿using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using NaughtyAttributes;

namespace WanderingCloud.Controller
{
    public enum MovementState
    {
        Idle = 0,
        Walk = 1,
        Rush = 2,
        Dash = 3,
        Jump = 4,
    }

    public class PlayerMovement : MonoBehaviour
    {
        [HideInInspector] private PlayerBrain player;
        public PlayerState state = new PlayerState();

        [field: SerializeField, ReadOnly] public MovementState moveState { get; private set; }
        
        #region Run Parameter
        [Foldout("Run"),SerializeField] private float airSpeed = 10f;
        [Foldout("Run"),SerializeField] private float walkSpeed = 20f;
        [Foldout("Run"),SerializeField] private float runSpeed = 40f;
        #endregion

        #region Jump Parameter
        [Foldout("Jump"), SerializeField, Range(0f, 5f)]  private float jumpHeight = 5f;
        [Foldout("Jump"), SerializeField, Range(0f, 4f)] private float fallFactor = 1.666f;
        [Foldout("Jump"), SerializeField] private float fallSpeedMax = 10f;
        [Foldout("Jump")] private Coroutine jump = null;
        [Foldout("Jump")] public UnityEvent onJump;
        public bool isJumping => jump is not null;
        #endregion
        
        #region Dash Parameter
        [Foldout("Dash"), SerializeField, Range(0f, 5f)]  private float dashDistance = 5f;
        [Foldout("Dash"), SerializeField, Range(0f, 1f)] private float dashDuration = 0.5f;
        [Foldout("Dash")] private Coroutine dash = null;
        [Foldout("Dash")] public UnityEvent onDash;
        public bool isDashing => dash is not null;
        #endregion
        

        #region UnityMethods
        private void Awake()
        {
            player = GetComponent<PlayerBrain>();
        }

        private void Update()
        {
            state.RefreshState();
            SnapToGround();
        }
        private void FixedUpdate()
        {
            Run();
            if (!state.isGrounded) Falling();
        }
        #endregion

        private void Run()
        {
            if(isDashing)return;
            //if (player.moveInput.magnitude < Mathf.Epsilon) return;

            //To Cam XZ
            Vector3 forward = Vector3.ProjectOnPlane(player.Camera.transform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(player.Camera.transform.right, Vector3.up).normalized;
            var movement = player.moveInput.y * forward + player.moveInput.x * right;
            var movementStrenght = movement.magnitude;
            var movementDirection = Vector3.ProjectOnPlane(movement, state.slopeNormal).normalized;
            
            //Turn where you run
            if (movementStrenght > float.Epsilon)
            {
                //if not aiming
                var aimRot = Quaternion.LookRotation(movementDirection, state.slopeNormal);
                player.Body.transform.rotation = Quaternion.Slerp(player.Body.transform.rotation, aimRot, 5 * Time.deltaTime);
            }

            if (!state.isGrounded)
            {
                var temp = movementDirection * (movementStrenght * airSpeed);
                player.Body.velocity = new Vector3(temp.x, player.Body.velocity.y, temp.z) ;

                player.Body.AddForce(movement.normalized * (movement.magnitude * (airSpeed * Time.deltaTime)), ForceMode.VelocityChange);
                return;
            }
            
            Debug.DrawRay(player.Avatar.position,movementDirection.normalized, Color.green, Time.deltaTime, true);
            Debug.DrawRay(player.Avatar.position + Vector3.up * 0.1f,movementDirection.normalized * movementStrenght, Color.red, Time.deltaTime, true);
            var speed = movementDirection * (movementStrenght * runSpeed);
            switch (moveState)
            {
                case MovementState.Idle:
                    speed = movementDirection * (movementStrenght * walkSpeed);
                    break;                
                case MovementState.Walk:
                    speed = movementDirection * (movementStrenght * walkSpeed);
                    break;
                case MovementState.Rush:
                    speed = movementDirection * (movementStrenght * runSpeed);
                    break;
                default:
                    break;
            }
            player.Body.velocity = new Vector3(speed.x,  player.Body.velocity.y, speed.z);
        }

        public void Jump()
        {
            if (!state.isGrounded || isJumping) return;
            jump = StartCoroutine(Jumping(jumpHeight));
            onJump?.Invoke();
        }
        public void ForcedJump(float height)
        {
            StopCoroutine(jump);
            jump = StartCoroutine(Jumping(jumpHeight));
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

            //player.Body.AddForce(Vector3.up * Mathf.Sqrt(-2.0f * Physics2D.gravity.y * (jumpHeight)), ForceMode.VelocityChange);
            player.Body.velocity = Vector3.Scale(new Vector3(1,0,1), player.Body.velocity)
                                   + state.slopeNormal * Mathf.Sqrt(-2.0f * Physics2D.gravity.y * jumpHeight);

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
            if(!state.isGrounded && !isJumping )
            {
                //Apply Gravity Scale
                float fallValue = fallFactor - 1.0f;
                player.Body.AddForce(Physics.gravity * fallValue, ForceMode.Acceleration);
            }
            
            if (player.Body.velocity.y < -Mathf.Abs(fallSpeedMax))
            {
                player.Body.velocity =new Vector3(player.Body.velocity.x,  fallSpeedMax, player.Body.velocity.z);
            }
        }

        private void SnapToGround()
        {
            if (state.isNearEdge || isJumping) return;
            
            var feetPos = player.Avatar.position - player.Avatar.up * ((player.Collider.height - player.Collider.radius)/ 2);
            RaycastHit hit;
            Ray groundRay = new Ray(feetPos, -player.Avatar.up);
            if (Physics.Raycast(groundRay, out hit, player.Collider.radius + 0.25f))
            {
                if(hit.distance > player.Collider.radius)
                transform.position += (hit.distance-player.Collider.radius) * -player.Avatar.up;
            }
            Debug.DrawRay(groundRay.origin,groundRay.direction.normalized * .25f,Color.cyan);
        }
        public void Dash()
        {
            //Can dash Mid Air
            if (isDashing) return;
            dash = StartCoroutine(Dashing());
            onDash?.Invoke();        
        }
        private IEnumerator Dashing()
        {
            var vel = player.Body.velocity;
            var previousState = moveState;
            moveState = MovementState.Dash;
            var movementDirection = player.Avatar.forward;
            if (player.moveInput.magnitude > float.Epsilon)
            {
                Vector3 forward = Vector3.ProjectOnPlane(player.Camera.transform.forward, Vector3.up).normalized;
                Vector3 right = Vector3.ProjectOnPlane(player.Camera.transform.right, Vector3.up).normalized;
                var movement = player.moveInput.y * forward + player.moveInput.x * right;
                movementDirection = Vector3.ProjectOnPlane(movement, state.slopeNormal).normalized;
            }
            
            Vector3 startPos = player.Avatar.position;
            float dashSpeed = (movementDirection.magnitude * dashDistance)/ dashDuration;
            float time = dashDuration;
            Debug.DrawRay(startPos,movementDirection * dashDistance,Color.blue, dashDuration );
            
            while (time > 0)
            {
                player.Body.velocity = Vector3.ProjectOnPlane(movementDirection, state.slopeNormal).normalized * dashSpeed;
                
                yield return new WaitForEndOfFrame();
                time -= Time.deltaTime;
            }
            player.Body.velocity = vel;
            moveState = /*dot prod?MovementState.Rush:*/ previousState;
            dash = null;
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
            isNearEdge = !Physics.Raycast(forwardRay, height + 1);
            Debug.DrawRay(forwardRay.origin, forwardRay.direction * height, isNearEdge ? Color.green : Color.red);
        }
    }
}