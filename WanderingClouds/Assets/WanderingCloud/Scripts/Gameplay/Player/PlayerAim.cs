using System.Collections;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using NaughtyAttributes;
using Cinemachine;
using DG.Tweening;
using WanderingCloud.Gameplay;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WanderingCloud.Controller
{
    public class PlayerAim : MonoBehaviour
    {

        #region Variables
        [Header("Follow Cam")]
        [field: SerializeField, Foldout("Data")] private float waitTimeFollow;
        [field: SerializeField, Foldout("Data"), Range(0f, 1f)] private float yThresholdFollow = 0.75f;
        [field: SerializeField, Foldout("Data")] private float desiredSmoothTime;
        [field: SerializeField, Foldout("Data")] private float followMaxSpeed;
        [field: SerializeField, Foldout("Data"), MinMaxSlider(float.Epsilon, 1)] private Vector2 outOfCenterYThreshold;
        [field: SerializeField, Foldout("Data"), MinMaxSlider(float.Epsilon, 100)] private Vector2 outOfCenterZThreshold;

        [Header("Aim Assist")]
        [field: SerializeField, Foldout("Data")] private float maxTargetDistance;
        [field: SerializeField, Foldout("Data")] private float aimAssistRadius;
        [field: SerializeField, Foldout("Data")] private Color detectionColor;

        [field: SerializeField, Foldout("States")] public bool isAiming;
        [field: SerializeField, Foldout("States")] private float timeSinceInactivity;
        [field: SerializeField, Foldout("States")] private bool isActive;

        [field: SerializeField, Foldout("References")] public PlayerBrain player;
        [field: SerializeField, Foldout("References")] private Transform followTarget;
        [field: SerializeField, Foldout("References")] private Transform anchor;
        [field: SerializeField, Foldout("References")] private Image crosshair;
        [field: SerializeField, Foldout("References")] private GameObject ProjectilePrefab;
        [field: SerializeField, Foldout("References")] private Transform throwSocket;
        [field: SerializeField, Foldout("References")] private PlayerInventory inventory;
        [field: SerializeField, Foldout("References")] private AiGraber grabAI;

        [field: SerializeField, Foldout("Events")] private UnityEvent onAim;

        private float ghostPositionY;
        private Vector3 velocity = Vector3.zero;
        private bool canFollow = true;

        private Transform assistTarget;
        private Vector3 defaultTargetPosition;

        Coroutine blackMagicScript;
        #endregion

        private void Update()
        {
            CheckForActivity();

            if (player.CinemachineBase.m_YAxis.Value > yThresholdFollow) return;

            if (timeSinceInactivity >= waitTimeFollow && player.CinemachineBase.m_BindingMode == CinemachineTransposer.BindingMode.WorldSpace)
            {
                if (blackMagicScript is not null) StopCoroutine(blackMagicScript);
                blackMagicScript = StartCoroutine(SwitchToSimpleFollow());
            }

            //if player is rushing, new target fov

        }
        private void FixedUpdate()
        {
            if (!isAiming) return;
            assistTarget = CheckForAssistTarget();

            if (grabAI.aiGrabed is not null)
            {
                
                return;
            }
            
            if(assistTarget is null)
            {
                RaycastHit hit;

                if (Physics.Raycast(player.Camera.transform.position, player.Camera.transform.forward, out hit, maxTargetDistance))
                    defaultTargetPosition = hit.point;
                else
                    defaultTargetPosition = player.Camera.transform.position + player.Camera.transform.forward * maxTargetDistance;

                crosshair.transform.localPosition = Vector3.zero;
                crosshair.color = Color.white;
            }
            else
            {
                Vector3 targetViewportPos = player.Camera.WorldToViewportPoint(assistTarget.position);

                crosshair.transform.localPosition = new Vector3((targetViewportPos.x - 0.5f) * 960, (targetViewportPos.y - 0.5f) * 1080, 0f);
                crosshair.color = detectionColor;
            }
        }

        /// <summary>
        /// Inspired by https://levelup.gitconnected.com/implement-a-third-person-camera-just-like-mario-odyssey-in-unity-e21744911733
        /// </summary>
        private void LateUpdate()
        {
            Vector3 characterViewPos = player.Camera.WorldToViewportPoint(anchor.position + player.Body.velocity * Time.deltaTime);

            if (isAiming)
            {
                canFollow = true;
            }
            else if (player.Movement.state.isGrounded && player.Movement.moveState != MovementState.Jump)
            {
                canFollow = true;
            }
            else if (characterViewPos.y > outOfCenterYThreshold.y || characterViewPos.y < outOfCenterYThreshold.x || characterViewPos.z > outOfCenterZThreshold.y)
            {
                canFollow = true;
                //Ptit coup de tween
                //DOTween.To(() => ghostPositionY, x => ghostPositionY = x, anchor.position.y, 0.2f);
            }


            if (canFollow)
            {
                ghostPositionY = anchor.position.y;
            }

            var desiredPosition = new Vector3(anchor.position.x, ghostPositionY, anchor.position.z);
            followTarget.position = Vector3.SmoothDamp(followTarget.position, desiredPosition, ref velocity, desiredSmoothTime, followMaxSpeed);
            followTarget.eulerAngles = anchor.eulerAngles;
        }

        private void Initialize()
        {
        }

        public void BeginAim()
        {
            isAiming = true;

            onAim?.Invoke();

            crosshair.enabled = true;

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

            onAim?.Invoke();

            player.CinemachineBase.m_YAxis = player.CinemachineAim.m_YAxis;
            player.CinemachineBase.m_XAxis = player.CinemachineAim.m_XAxis;

            //Aim VCam takes priority
            player.CinemachineAim.Priority = player.CinemachineBase.Priority - 1;

            crosshair.enabled = false;

            isAiming = false;
        }

        public void Throw()
        {
            if (!isAiming) return;
            if (grabAI.aiGrabed is not null) return;
            if (!inventory.RemovePullet()) return;
            var projectile = Instantiate(ProjectilePrefab, throwSocket.transform.position, Quaternion.identity);

            CloudProjectile bullet = projectile.GetComponent<CloudProjectile>();
            if (assistTarget is not null) bullet.Target = assistTarget;
            else bullet.targetPosition = defaultTargetPosition;

            bullet.CanMove = true;
            bullet.type = player.isGyro? CloudType.ENERGIZER : CloudType.SOLIDIFIER;
        }

        private Transform CheckForAssistTarget()
        {
            var p1 = player.Camera.transform.position;
            var p2 = player.Camera.transform.position + player.Camera.transform.forward * maxTargetDistance;

            var hits = Physics.CapsuleCastAll(p1, p2, aimAssistRadius, player.Camera.transform.forward, maxTargetDistance);

            if (hits.Length <= 0) return null;

            //Keep only the targetable objects
            var assistTargets = hits.Where(x => x.collider.GetComponent<Target>()).Select(x => x.collider).ToArray();
            //Order them by the distance from the center of the screen
            assistTargets = assistTargets.OrderBy(x =>  Vector2.Distance(player.Camera.WorldToViewportPoint(x.transform.position), Vector2.one * 0.5f)).ToArray();

            //For each object on the list, check occlusion
            for (int i = 0; i < assistTargets.Length; i++)
            {
                //Debug.DrawLine(player.transform.position, assistTargets[i].transform.position, Color.Lerp(Color.red, Color.green, Vector2.Distance(player.Camera.WorldToViewportPoint(assistTargets[i].transform.position), Vector2.one / 2f) * 5));
            }
            for (int i = 0; i < assistTargets.Length; i++)
            {
                RaycastHit hit;
                if (Physics.Linecast(player.transform.position, assistTargets[i].transform.position, out hit))
                {
                    if (hit.collider == assistTargets[i]) return assistTargets[i].transform;
                    
                }
            }
            return null;
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

        /// <summary>
        /// Called in unityEvent
        /// </summary>
        public void OnLeaveGround()
        {
            canFollow = false;
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

        IEnumerator SwitchToSimpleFollow()
        {
            while (!player.Movement.state.isGrounded)
            {
                yield return new WaitForEndOfFrame();
            }
            Vector3 offset = player.CinemachineBase.State.RawPosition - player.CinemachineBase.Follow.position;
            offset.y = 0; // project onto plane
            float value = Vector3.SignedAngle(Vector3.back, offset, Vector3.up);
            player.CinemachineBase.m_XAxis.Value = value;
            player.CinemachineBase.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
            player.CinemachineBase.InternalUpdateCameraState(Vector3.up, -1);
            player.CinemachineBase.PreviousStateIsValid = false;

            blackMagicScript = null;
        }

        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            using (new Handles.DrawingScope(Color.yellow))
            {
                var p1 = player.Camera.transform.position;
                var p2 = player.Camera.transform.position + player.Camera.transform.forward * maxTargetDistance;
                Extension_Handles.DrawWireCapsule(p1, p2, aimAssistRadius);
            }
        }

#endif
    }
}
