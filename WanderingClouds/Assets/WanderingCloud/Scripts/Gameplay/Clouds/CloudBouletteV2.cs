using UnityEngine;
using NaughtyAttributes;
using WanderingCloud.Controller;
using WanderingCloud.Gameplay;

namespace WanderingCloud
{
    public enum CloudType
    {
        NEUTRAL = 0,
        ENERGIZER = 1,
        SOLIDIFIER = 2,
    }
    public class CloudBouletteV2 : MonoBehaviour
    {
        [SerializeField, Foldout("Data")] private float travelSpeed = 1f;
        [SerializeField, Foldout("Data")] private float collisionThreshold = 0.2f;
        [SerializeField, Foldout("Data")] public CloudType type = CloudType.NEUTRAL;

        public Transform Target
        {
            set 
            { 
                target = value;
                targetPosition = target.position;
            }
        }
         private Transform target;
         public Vector3 targetPosition;
        [field: SerializeField, Foldout("States"), ReadOnly] public bool CanMove;

        private void Update()
        {
            if (!CanMove) return;

            Vector3 thisToTarget = targetPosition - transform.position;

            if (thisToTarget.magnitude <= collisionThreshold) collisionEvent();

            if (float.IsNaN(thisToTarget.x)) return;
            transform.Translate(thisToTarget * (travelSpeed * Time.deltaTime));
        }

        private void collisionEvent()
        {            
            Component component;
            if (target.parent.TryGetComponent(typeof(PlayerInventory), out component))
            {
                var otherInventory = (PlayerInventory)component;
                otherInventory.ReceivedCloud();
            }
            if (target.TryGetComponent(typeof(Source), out component))
            {
                var source = (Source)component;
                source.Feed(type);
            }
            Destroy(gameObject);
        }
    }
}
