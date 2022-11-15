#if UNITY_EDITOR
using UnityEditor;
#endif
using WanderingCloud.Gameplay;
using System;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

namespace WanderingCloud.Controller
{
    public enum PlayerMovementState
    {
        Walking = 0,
        Run = 1,
        Sliding = 2,
    }

    public class Player : Pawn
    {
        [SerializeField] private bool debugMode;
        
        #region References
        [SerializeField] private Rigidbody body;
        [SerializeField] private CapsuleCollider collider;
        [SerializeField] private CinemachineFreeLook cinemachine;
        #endregion
        
        #region Parameter

        [SerializeField] private float speed = 5f;
        #endregion

        #region Status
        [field :SerializeField, ReadOnly] public bool isGrounded { get; private set; }
        [field :SerializeField, ReadOnly] public bool isNearEdge { get; private set; }
        #endregion
        public Vector3 inputMovement;

        #region GrabObject
        [SerializeField] private Transform grabSocket;
        [SerializeField] private GrabableObject grabObject;
        #endregion

        #region UnityMethods
        private void Awake()
        {
            cinemachine.gameObject.SetActive(false);
        }
        

        private void Update()
        {
            ComputeState();
            
            if (body.velocity.magnitude > speed)
            {
                body.velocity = body.velocity.normalized * speed;
            }
        }

        private void FixedUpdate()
        {
            body.AddForce(inputMovement.normalized * speed, ForceMode.VelocityChange);

            if (body.velocity.magnitude > speed)
            {
                body.velocity = body.velocity.normalized * speed;
            }
        }

        #endregion

        public override void MovementInput(Vector2 input)
        {
            Vector3 forward = Vector3.ProjectOnPlane(Camera.transform.forward, Vector3.up);
            Vector3 right = Vector3.ProjectOnPlane(Camera.transform.right, Vector3.up);
            inputMovement = input.y * forward + input.x * right;
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
            Ray forwardRay = new Ray(feetPos + Vector3.up * height + body.velocity.normalized * predictDist, Vector3.down);
            isNearEdge = !Physics.Raycast(forwardRay, out forwardHit, height * 2);
            Debug.DrawRay(forwardRay.origin, forwardRay.direction * (collider.height * 2), isNearEdge ? Color.green : Color.red);

            if (isNearEdge)
            {
                //curAngle = float.NaN;
                return;
            }

            //Calcul Slope
            Vector3 slopeVector = forwardHit.point - underHit.point;
            Debug.DrawLine(forwardHit.point, underHit.point, Color.yellow);

            //curAngle = (Mathf.Atan2(slopeVector.y, predictDist) * Mathf.Rad2Deg);

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