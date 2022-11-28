using UnityEngine;
using NaughtyAttributes;
using Cinemachine;
using DG.Tweening;

namespace WanderingCloud.Controller
{
    public class PlayerAim : MonoBehaviour
    {
        [field: SerializeField, Foldout("Data")] private float waitTimeFollow;
        [field: SerializeField, Foldout("Data")] private float desiredSmoothTime;
        [field: SerializeField, Foldout("Data")] private float followMaxSpeed;

        [field: SerializeField, Foldout("States")] private bool isAiming;
        [field: SerializeField, Foldout("States")] private float timeSinceInactivity;
        [field: SerializeField, Foldout("States")] private bool isActive;

        [field: SerializeField, Foldout("References")] public PlayerBrain player;
        [field: SerializeField, Foldout("References")] private Transform followTarget;
        [field: SerializeField, Foldout("References")] private Transform anchor;

        private float ghostPositionY;
        private Vector3 velocity = Vector3.zero;
        private bool canFollow = true;

        private void Update()
        {
            CheckForActivity();

            if (timeSinceInactivity >= waitTimeFollow && player.CinemachineBase.m_BindingMode == CinemachineTransposer.BindingMode.WorldSpace && !isAiming)
            {
                SwitchToSimpleFollow();
            }

        }

        /// <summary>
        /// Inspired by https://levelup.gitconnected.com/implement-a-third-person-camera-just-like-mario-odyssey-in-unity-e21744911733
        /// </summary>
        private void LateUpdate()
        {
            Vector3 characterViewPos = player.Camera.WorldToViewportPoint(anchor.position + player.Body.velocity * Time.deltaTime);

            if (player.Movement.state.isGrounded)
            {
                ghostPositionY = anchor.position.y;
            }
            else if (characterViewPos.y > 0.85f || characterViewPos.y < 0.3f)
            {
                ghostPositionY = anchor.position.y;
                //Ptit coup de tween
                //DOTween.To(() => ghostPositionY, x => ghostPositionY = x, anchor.position.y, 0.2f);
            }

            var desiredPosition = new Vector3(anchor.position.x, ghostPositionY, anchor.position.z);
            followTarget.position = Vector3.SmoothDamp(followTarget.position, desiredPosition, ref velocity, desiredSmoothTime, followMaxSpeed);
            followTarget.eulerAngles = anchor.eulerAngles;
        }

        public void OnLeaveGround()
        {
            ghostPositionY = anchor.position.y;
        }

        private void CheckForActivity()
        {
            if (player.CinemachineBase != null)
            {
                if (player.CinemachineBase.m_XAxis.m_InputAxisValue != 0f && player.CinemachineBase.m_YAxis.m_InputAxisValue != 0f)
                {
                    isActive = true;
                    timeSinceInactivity = 0f;

                    if (player.CinemachineBase.m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
                    {
                        SwitchToWorldBinding();
                    }
                }
                   
                else
                {
                    isActive = false;
                    timeSinceInactivity += Time.deltaTime;
                }     
            }
        }

        public void BeginAim()
        {
            isAiming = true;

            if (player.CinemachineBase.m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
            {
                SwitchToWorldBinding();
            }

            player.CinemachineAim.m_YAxis = player.CinemachineBase.m_YAxis;
            player.CinemachineAim.m_XAxis = player.CinemachineBase.m_XAxis;

            //Aim VCam takes priority
            player.CinemachineAim.Priority = player.CinemachineBase.Priority + 1;
        }

        public void EndAim()
        {
            if (player.CinemachineBase.m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
            {
                timeSinceInactivity = 0f;
                SwitchToWorldBinding();
            }

            player.CinemachineBase.m_YAxis = player.CinemachineAim.m_YAxis;
            player.CinemachineBase.m_XAxis = player.CinemachineAim.m_XAxis;

            //Aim VCam takes priority
            player.CinemachineAim.Priority = player.CinemachineBase.Priority - 1;

            isAiming = false;
        }

        #region Transposer binding switch
        void SwitchToWorldBinding()
        {
            Vector3 offset = player.CinemachineBase.State.RawPosition - player.CinemachineBase.Follow.position;
            offset.y = 0; // project onto plane
            float value = Vector3.SignedAngle(Vector3.back, offset, Vector3.up);
            player.CinemachineBase.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
            player.CinemachineBase.InternalUpdateCameraState(Vector3.up, -1);
            player.CinemachineBase.m_XAxis.Value = value;
            player.CinemachineBase.PreviousStateIsValid = false;
        }

        void SwitchToSimpleFollow()
        {
            player.CinemachineBase.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
            player.CinemachineBase.InternalUpdateCameraState(Vector3.up, -1);
            player.CinemachineBase.m_XAxis.Value = 0;
            player.CinemachineBase.PreviousStateIsValid = false;
        }
        #endregion
    }
}