using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using WanderingCloud.Gameplay;

namespace WanderingCloud
{
    public class Fogger : Source
    {
        public NavMeshAgent agent;
        [SerializeField][OnValueChanged("UpdatePulletNumber")] public float  currentPullet = 0;
        [SerializeField] public float maxPullet = 5;
        [SerializeField] private AnimationCurve speedForPulletNumber;
        [SerializeField] private AnimationCurve sizeForPulletNumber;
        [SerializeField] private Transform fogCircle;
        [SerializeField] private AnimationCurve FogCircleSizeForPulletNumber;
        public float currentSpeed
        {
            get
            {
                return speedForPulletNumber.Evaluate(currentPullet / maxPullet);
            }
        }
        public float currentSize
        {
            get
            {
                return sizeForPulletNumber.Evaluate(currentPullet / maxPullet);
            }
        }
        public float currentFogCircleSize
        {
            get
            {
                return  FogCircleSizeForPulletNumber.Evaluate(currentPullet / maxPullet)/ currentSize;
            }
        }
        public override bool Feed(CloudType cType)
        {
            if (currentPullet < maxPullet)
            {
                currentPullet++;
                return true;
            }
            else
            {
                return false;
            }
        }
        [Button]
        // Start is called before the first frame update
        void Start()
        {
            UpdatePulletNumber();
        }

        // Update is called once per frame
        public void UpdatePulletNumber()
        {
            agent.speed = currentSpeed;
            agent.acceleration = (currentSpeed * currentSpeed) / 2;
            transform.localScale = new Vector3(currentSize, currentSize, currentSize);
            agent.radius = currentSize;
            agent.height = currentSize + 0.5f;
            fogCircle.localScale = new Vector3(currentFogCircleSize, currentFogCircleSize, currentFogCircleSize);
        }
    }
}
