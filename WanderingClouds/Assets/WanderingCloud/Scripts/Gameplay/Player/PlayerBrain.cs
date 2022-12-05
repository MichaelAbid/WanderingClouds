
﻿using Cinemachine;
using NaughtyAttributes;
using UnityEngine;
using WanderingCloud.Gameplay;

namespace WanderingCloud.Controller
{
    [RequireComponent(typeof(PlayerMovement))/*, RequireComponent(typeof(PlayerAim))*/]
    public class PlayerBrain : Pawn
    {
        #region References
        [field: SerializeField, Foldout("References"), Required()] public Rigidbody Body { get; private set; }
        [field: SerializeField, Foldout("References"), Required()] public Transform Avatar { get; private set; }
        [field: SerializeField, Foldout("References"), Required()] public CapsuleCollider Collider { get; private set; }
        [field: SerializeField, Foldout("References"), Required()] public CinemachineFreeLook CinemachineBase { get; private set; }
        [field: SerializeField, Foldout("References"), Required()] public CinemachineFreeLook CinemachineAim { get; private set; }
        [field: SerializeField, Foldout("References"), Required()] public CinemachineInputProvider provider { get; private set; }
        [field: SerializeField, Foldout("References"), Required()] public AiGraber aiGrabber { get; private set; }
        [field: SerializeField, Foldout("References"), Required()] public CloudGrabber cloudGrabber { get; private set; }
        [field: SerializeField, Foldout("References"), Required()] public CloudExploder cloudExploder { get; private set; }
        #endregion

        [field: SerializeField, Foldout("Components"), Required()] public PlayerMovement Movement { get; private set; }
        [field: SerializeField, Foldout("Components"), Required()] public PlayerAim Aim { get; private set; }

        [field: SerializeField, Foldout("Components"), ReadOnly()] public Vector3 moveInput { get; private set; }
        [field: SerializeField, Foldout("Components"), ReadOnly()] public Vector3 throwInput{ get; private set; }

        public override void PlayerConnect(int playerIndex)
        {
            CinemachineBase.gameObject.SetActive(true);
            CinemachineAim.gameObject.SetActive(true);
            provider.enabled = true;
            
            provider.gameObject.SetActive(false);
            provider.gameObject.SetActive(true);
            provider.PlayerIndex = playerIndex;
        }
        public override void PlayerDisconnect(int playerIndex)
        {
            CinemachineBase.gameObject.SetActive(false);
            CinemachineAim.gameObject.SetActive(false);
            provider.enabled = false;
        }

        public override void MovementInput(Vector2 input) => moveInput = input;

        public override void SouthButtonInput() => Movement.Jump();
        public override void WestButtonInput() => Movement.Dash();
        public override void LeftTriggerInput() => Aim.BeginAim();
        public override void LeftTriggerInputReleased() => Aim.EndAim();
        public override void RightTriggerInput() => Aim.Throw();

        public override void EstButtonInput()
        {
            if (aiGrabber.aiGrabed != null)
            {
                aiGrabber.UnGrab();
            }
            else
            {
                if (!aiGrabber.Grab())
                {
                    if (!cloudGrabber.GrabNearestPullet())
                    {
                        if (!cloudExploder.ExplodeNearest())
                        {

                        }
                    }
                }
            }
        }

    }
}