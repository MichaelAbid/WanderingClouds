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
        #region Variables
        public bool isGrounded;
        public CinemachineFreeLook cinemachine;

        #endregion

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