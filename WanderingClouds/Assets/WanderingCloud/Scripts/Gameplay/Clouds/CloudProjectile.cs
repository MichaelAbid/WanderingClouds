using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using UnityEngine;
using NaughtyAttributes;

namespace WanderingCloud
{
    public enum CloudType
    {
        NEUTRAL = 0,
        ENERGIZER = 1,
        SOLIDIFIER = 2,
    }
    public class CloudProjectile : MonoBehaviour
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
            print("collided");
            Destroy(gameObject);
        }
    }
}
