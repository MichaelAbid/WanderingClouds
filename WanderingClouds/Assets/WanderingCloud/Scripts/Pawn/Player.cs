#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

namespace WanderingCloud.Controller
{
    public enum PlayerMovementState
    {
        Walking = 0,
        Running = 1,
        Sliding = 2,
    }

    public class Player : Pawn
    {
        [SerializeField] private bool debugMode;
        
        #region References
        [SerializeField] private Rigidbody body;
        [SerializeField] private Transform avatar;
        [SerializeField] private CapsuleCollider collider;
        [SerializeField] private CinemachineFreeLook cinemachine;
        #endregion
        
        #region Parameter

        [SerializeField] private float walkStrength = 20f;
        [SerializeField] private float jumpForce = 5f;
        #endregion

        #region Status
        [field :SerializeField, ReadOnly] public bool isGrounded { get; private set; }
        [field :SerializeField, ReadOnly] public bool isNearEdge { get; private set; }
        #endregion
        public Vector3 inputMovement;
        public Vector3 slopeMovement;


        #region UnityMethods
        private void Awake()
        {
            cinemachine.gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            MovementUpdate();
        }

        #endregion

        public override void MovementInput(Vector2 input)
        {
            Vector3 forward = Vector3.ProjectOnPlane(Camera.transform.forward, Vector3.up);
            Vector3 right = Vector3.ProjectOnPlane(Camera.transform.right, Vector3.up);
            inputMovement = input.y * forward + input.x * right;
        }
        public override void SouthButtonInput() => Jump();

        public void MovementUpdate()
        {
            ComputeState();

            if (inputMovement.magnitude < Mathf.Epsilon) return;
            var aimRot = Quaternion.LookRotation(inputMovement, Vector3.up);
            body.transform.rotation = Quaternion.Slerp(body.transform.rotation, aimRot, 4 * Time.deltaTime);

            if (isGrounded)
            {
                body.AddForce(slopeMovement.normalized * (slopeMovement.magnitude * (walkStrength * Time.deltaTime)), ForceMode.VelocityChange);
            }
        }
        public void Jump()
        {
            if (!isGrounded) return;
            body.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
        
        public override void PlayerConnect(int playerIndex)
        {
            cinemachine.gameObject.SetActive(true);
            var inputLink = cinemachine.GetComponent<CinemachineInputProvider>();

            inputLink.PlayerIndex = playerIndex;
        }
        public override void PlayerDisconnect(int playerIndex)
        {
            cinemachine.gameObject.SetActive(false);
        }
        
        
        public void ComputeState()
        {
            slopeMovement = inputMovement;

            var height = collider.height;
            var feetPos = transform.position + Vector3.down * ((height - 0.05f) / 2);

            RaycastHit underHit;
            Ray underRay = new Ray(feetPos, Vector3.down);
            isGrounded = Physics.Raycast(underRay, out underHit, 0.5f);
            Debug.DrawRay(underRay.origin, underRay.direction * 0.5f, isGrounded ? Color.green : Color.red);

            if (!isGrounded)
            {
                //curAngle = float.NaN;
                return;
            }

            float predictDist = collider.radius + 0.05f;
            RaycastHit forwardHit;
            Ray forwardRay = new Ray(feetPos + Vector3.up * height + avatar.forward * predictDist, Vector3.down);
            isNearEdge = !Physics.Raycast(forwardRay, out forwardHit, height * 2);
            Debug.DrawRay(forwardRay.origin, forwardRay.direction * (collider.height * 2), isNearEdge ? Color.green : Color.red);

            if (isNearEdge)
            {
                //curAngle = float.NaN;
                return;
            }

            //Calcul Slope
            slopeMovement = forwardHit.point - underHit.point;
            Debug.DrawLine(forwardHit.point, underHit.point, Color.yellow);

            //curAngle = (Mathf.Atan2(slopeVector.y, predictDist) * Mathf.Rad2Deg);

        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
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