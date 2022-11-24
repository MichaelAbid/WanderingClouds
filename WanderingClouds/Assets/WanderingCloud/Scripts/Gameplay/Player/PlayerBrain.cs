﻿using Cinemachine;
using NaughtyAttributes;
using UnityEngine;

namespace WanderingCloud.Controller
{
    [RequireComponent(typeof(PlayerMovement)), RequireComponent(typeof(PlayerAim))]
    public class PlayerBrain : Pawn
    {
        #region References
        [field: SerializeField, Foldout("References"), Required()] public Rigidbody Body { get; private set; }
        [field: SerializeField, Foldout("References"), Required()] public Transform Avatar { get; private set; }
        [field: SerializeField, Foldout("References"), Required()] public CapsuleCollider Collider { get; private set; }
        [field: SerializeField, Foldout("References"), Required()] public CinemachineFreeLook Cinemachine { get; private set; }
        #endregion

        [field: SerializeField, Foldout("Components"), Required()] public PlayerMovement Movement { get; private set; }
        [field: SerializeField, Foldout("Components"), Required()] public PlayerAim Aim { get; private set; }
        
        [field: SerializeField, Foldout("Components"), ReadOnly()] public Vector3 moveInput { get; private set; }

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

        public override void MovementInput(Vector2 input) => moveInput = input;

        public override void SouthButtonInput() => Movement.Jump();
        public override void WestButtonInput() => Movement.Dash();
    }
}