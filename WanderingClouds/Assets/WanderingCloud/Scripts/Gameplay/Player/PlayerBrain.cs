﻿
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
        [field: SerializeField, Foldout("References"), Required()] public CinemachineFreeLook VCamBase { get; private set; }
        [field: SerializeField, Foldout("References"), Required()] public CinemachineFreeLook VCamAuto { get; private set; }
        [field: SerializeField, Foldout("References"), Required()] public CinemachineFreeLook VCamAim { get; private set; }
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
            VCamBase.gameObject.SetActive(true);
            VCamAuto.gameObject.SetActive(true);
            VCamAim.gameObject.SetActive(true);

            VCamBase.gameObject.GetComponent<CinemachineInputProvider>().enabled = false;
            VCamAuto.gameObject.GetComponent<CinemachineInputProvider>().enabled = false;
            VCamAim.gameObject.GetComponent<CinemachineInputProvider>().enabled = false;

            VCamBase.gameObject.GetComponent<CinemachineInputProvider>().PlayerIndex = playerIndex;
            VCamAuto.gameObject.GetComponent<CinemachineInputProvider>().PlayerIndex = playerIndex;
            VCamAim.gameObject.GetComponent<CinemachineInputProvider>().PlayerIndex = playerIndex;

            VCamBase.gameObject.GetComponent<CinemachineInputProvider>().enabled = true;
            VCamAuto.gameObject.GetComponent<CinemachineInputProvider>().enabled = true;
            VCamAim.gameObject.GetComponent<CinemachineInputProvider>().enabled = true;


        }
        public override void PlayerDisconnect(int playerIndex)
        {
            VCamBase.gameObject.SetActive(false);
            VCamAuto.gameObject.SetActive(false);
            VCamAim.gameObject.SetActive(false);
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