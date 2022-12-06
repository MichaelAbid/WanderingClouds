using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace WanderingCloud.Gameplay
{
    public class BumperSource : Source
    {
        public NavMeshAgent agent;
        [SerializeField] public float currentPullet = 0;
        [SerializeField] public float maxPullet = 5;
        [SerializeField] private AnimationCurve speedForPulletNumber;
        [SerializeField] private AnimationCurve sizeForPulletNumber;
        public float currentSpeed {
            get { 
                return speedForPulletNumber.Evaluate(currentPullet/maxPullet);
            } 
        }
        public float currentSize
        {
            get
            {
                return sizeForPulletNumber.Evaluate(currentPullet / maxPullet);
            }
        }

        [SerializeField]
        [ReadOnly]
        private float debugCurrentSpeed;
        [SerializeField]
        [ReadOnly]
        private float debugSizeSpeed;

        public override bool Feed(CloudType cType)
        {
            if(currentPullet < maxPullet)
            {
                currentPullet++;
                return true;
            }
            else
            {
                return false;
            }
        }
        
        void Update()
        {
            agent.speed = currentSpeed;
            agent.acceleration = (currentSpeed * currentSpeed)/2;
            transform.localScale = new Vector3(currentSize, currentSize, currentSize);
            agent.radius = currentSize;
            agent.height = currentSize + 0.5f;
            debugCurrentSpeed = currentSpeed;
            debugSizeSpeed = currentSize;
        }


    }
}
