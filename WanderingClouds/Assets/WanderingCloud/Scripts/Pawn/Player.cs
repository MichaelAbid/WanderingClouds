using System;
using System.Collections.Generic;
using System.Collections;
using Cinemachine;
using NaughtyAttributes;
using UnityEngine;

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
        #region References
        public Rigidbody body;
        public CapsuleCollider collider;
        public CinemachineFreeLook cinemachine;
        #endregion
        
        #region Status
        public bool isGrounded;
        public bool isNearEdge;
        #endregion

        public Vector3 inputMovement;


        #region UnityMethods

        private void Awake()
        {
            cinemachine.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
        }
        private void OnDisable()
        {
        }

        private void Update()
        {
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
        

    }
}