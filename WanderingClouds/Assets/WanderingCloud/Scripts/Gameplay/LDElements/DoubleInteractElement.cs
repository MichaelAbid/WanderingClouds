using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using WanderingCloud.Controller;

namespace WanderingCloud
{
    [RequireComponent(typeof(BoxCollider))]
    public class DoubleInteractElement : MonoBehaviour
    {
        [SerializeField, Range(0f,3f)] public float chargeTime = .5f;
        [Space(10)]
        [SerializeField, ReadOnly, ProgressBar("Urle Charge",1f, EColor.Blue)] public float urleCharge = 0f;
        [SerializeField, ReadOnly, ProgressBar("Giro Charge",1f, EColor.Red)] public float giroCharge = 0f;
        [SerializeField, ReadOnly] public bool urleCharging = false;
        [SerializeField, ReadOnly] public bool giroCharging = false;
        [Space(25)]
        [SerializeField] public bool isActivated = false;
        [SerializeField] public UnityEvent onActivation;
        [SerializeField] public UnityEvent onDesactivation;
        [Space(15)]
        public BoxCollider coll;

        [Button()] private void Activate()
        {
            if(!isActivated)onActivation?.Invoke();
            isActivated = true;
        }
        [Button()] private void Desactivate()
        {
            if(isActivated)onDesactivation?.Invoke();
            isActivated = false;
        }
        
        private void Awake()
        {
            coll = GetComponent<BoxCollider>();
            coll.isTrigger = true;
        }
        void FixedUpdate()
        {
            urleCharge += urleCharging? chargeTime * Time.deltaTime : chargeTime * -Time.deltaTime;
            giroCharge += giroCharging? chargeTime * Time.deltaTime : chargeTime * -Time.deltaTime;

            if (urleCharge + giroCharge >= 2)
            {
                if (!isActivated)Activate();
                //else Desactivate();
                giroCharge = urleCharge = 0f;
            }
            
            urleCharge = Mathf.Clamp01(urleCharge);
            giroCharge = Mathf.Clamp01(giroCharge);
        }

        public void Press(Pawn pawn)
        {
            if (pawn.isGyro)
            {
                giroCharging = true;
            }
            else
            {
                urleCharging = true;
            }
        }
        public void Release(Pawn pawn)
        {
            if (pawn.isGyro)
            {
                giroCharging = false;
            }
            else
            {
                urleCharging = false;
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<PlayerBrain>())
            {
                var interact = other.GetComponentInParent<PlayerInteraction>();
                interact.onInteractBegin.AddListener(Press);
                interact.onInteractEnd.AddListener(Release);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<PlayerBrain>())
            {
                var interact = other.GetComponentInParent<PlayerInteraction>();
                interact.onInteractBegin.RemoveListener(Press);
                interact.onInteractEnd.RemoveListener(Release);
                
                Release(other.GetComponentInParent<PlayerBrain>());
            }
        }
        
        private void OnDrawGizmos()
        {

        }
    }
}