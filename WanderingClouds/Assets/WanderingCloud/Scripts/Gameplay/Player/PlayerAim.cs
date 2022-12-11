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
        [field: SerializeField, Foldout("Data"), Range(0f, 1f)] private float transitionSensitivityScale = 0.35f;
        [field: SerializeField, Foldout("Data"), Range(0f, 1f)] private float transitionDuration = 0.25f;

        [Header("Aim Assist")]
        [field: SerializeField, Foldout("Data")] private float maxTargetDistance;
        [field: SerializeField, Foldout("Data")] private float aimAssistRadius;
        [field: SerializeField, Foldout("Data")] private Color detectionColor;

        [field: SerializeField, Foldout("States"), ReadOnly] private CinemachineFreeLook activeVCam;
        [field: SerializeField, Foldout("States"), ReadOnly] public bool isAiming;
        [field: SerializeField, Foldout("States"), ReadOnly] private bool isPlayerActive;
        [field: SerializeField, Foldout("States"), ReadOnly] private float timeSinceInactivity;

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

        [SerializeField, ReadOnly] private Transform assistTarget;
        private Vector3 defaultTargetPosition;

        private Coroutine transition = null;
        #endregion

        private void Awake()
        { 
            Initialize();
        }

        private void Update()
        {
            CheckForActivity();

            //If camera is bird's eye, prevent changes
            if (player.VCamBase.m_YAxis.Value > yThresholdFollow) return;

            if (timeSinceInactivity >= waitTimeFollow && activeVCam == player.VCamBase)
            {
                SetVCam(player.VCamAuto);
            }
        }
        private void FixedUpdate()
        { 
            if (grabAI.aiGrabed is not null)
            {
                if (isAiming)
                {
                    Debug.Log(grabAI.aiGrabed.transform.rotation);
                    
                    grabAI.aiGrabed.visual.transform.rotation = Quaternion.LookRotation(player.Camera.transform.up*-1, player.Camera.transform.forward);
                    grabAI.aiGrabed.socket.transform.rotation = Quaternion.LookRotation(player.Camera.transform.forward, player.Camera.transform.up);

                    return;
                }

                grabAI.aiGrabed.visual.transform.rotation = Quaternion.LookRotation(player.Avatar.forward, player.Avatar.up);
                grabAI.aiGrabed.socket.transform.rotation = Quaternion.LookRotation(player.Avatar.up, player.Avatar.forward);

            }


            if (!isAiming) return;
            assistTarget = CheckForAssistTarget();

           


            if (assistTarget is null)
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
            SetVCam(player.VCamBase);
        }

        public void BeginAim()
        {
            isAiming = true;

            SetVCam(player.VCamAim);

            onAim?.Invoke();

            crosshair.enabled = true;
        }

        public void EndAim()
        {
            if (!isAiming) return;

            onAim?.Invoke();

            timeSinceInactivity = 0f;
            SetVCam(player.VCamBase);

            crosshair.enabled = false;

            isAiming = false;
        }

        public void Throw()
        {
            if (assistTarget is null) return;
            Component component;
            assistTarget.parent.TryGetComponent(typeof(PlayerInventory), out component);
            if (component is not null)
            {
                var otherInventory = (PlayerInventory)component;
                if (otherInventory.haveCloud) return;
            }
            assistTarget.TryGetComponent(typeof(Source), out component);
            if (component is not null)
            { 
                var source = (Source)component;
                if (source.isFeed) return;
            }
            if (!isAiming) return;
            if (grabAI.aiGrabed is not null) return;
            if (!inventory.RemovePullet()) return;

            var projectile = Instantiate(ProjectilePrefab, throwSocket.transform.position, Quaternion.identity);

            CloudBouletteV2 bullet = projectile.GetComponent<CloudBouletteV2>();
            bullet.Target = assistTarget;
            bullet.CanMove = true;
            bullet.type = player.isGyro? CloudType.SOLIDIFIER : CloudType.ENERGIZER;
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
            if (activeVCam.m_XAxis.m_InputAxisValue != 0f && activeVCam.m_YAxis.m_InputAxisValue != 0f)
            {
                isPlayerActive = true;
                timeSinceInactivity = 0f;

                if (activeVCam == player.VCamAuto)
                {
                    SetVCam(player.VCamBase);
                }
            }
            else
            {
                isPlayerActive = false;
                timeSinceInactivity += Time.deltaTime;
            }
        }

        /// <summary>
        /// Called by unityEvent
        /// </summary>
        public void OnLeaveGround()
        {
            canFollow = false;
        }

        public void SetVCam(CinemachineFreeLook desiredCam)
        {
            if (activeVCam is not null)
            {
                activeVCam.Priority = 9;

                if (activeVCam == player.VCamAuto && desiredCam == player.VCamBase)
                {
                    transition = StartCoroutine(EaseVCamTransition(desiredCam, transitionDuration));
                }
            }

            player.Movement.lookAtRig.enabled = (desiredCam == player.VCamBase && player.Inventory.pelletStock <= 0);

            desiredCam.Priority = 10;
            activeVCam = desiredCam;
        }

        IEnumerator EaseVCamTransition(CinemachineFreeLook toVCam, float transitionDuration)
        {
            var originalMaxSpeed = toVCam.m_YAxis.m_MaxSpeed;
            toVCam.m_YAxis.m_MaxSpeed *= transitionSensitivityScale;

            yield return new WaitForSeconds(transitionDuration);

            toVCam.m_YAxis.m_MaxSpeed = originalMaxSpeed;
            transition = null;
        }

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
