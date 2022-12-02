using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace WanderingCloud
{
    public class Projectile : MonoBehaviour
    {
        [field: SerializeField, Foldout("Data")] private float travelSpeed = 1f;
        [field: SerializeField, Foldout("Data")] private float collisionThreshold = 0.2f;

         public Vector3 targetPosition;
        [field: SerializeField, Foldout("States"), ReadOnly] public bool CanMove;

        private void Update()
        {
            if (!CanMove) return;

            Vector3 thisToTarget = (targetPosition - transform.position).normalized;

            if (thisToTarget.magnitude <= collisionThreshold) collisionEvent();

            if (float.IsNaN(thisToTarget.x)) return;
            transform.Translate((thisToTarget) * travelSpeed * Time.deltaTime);
        }

        private void collisionEvent()
        {
            print("collided");
            Destroy(gameObject);
        }
    }
}
