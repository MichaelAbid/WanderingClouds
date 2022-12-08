using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using WanderingCloud.Controller;

namespace WanderingCloud.Gameplay
{
    public enum CloudState
    {
        BABY = 0,
        SOLID = 1,
        DESTRUCTOR = 2,
    }
    public class CreatureSources : Source
    {
        public NavMeshAgent agent;
        public CloudState currentState = CloudState.BABY;
        public float destructionRange = 5f;
        [ReadOnly] public bool canBePouffed;
        [MinMaxSlider(1f, 4f)] public Vector2 scale = Vector2.one;
        [MinMaxSlider(1f, 5f)] public Vector2 speed = Vector2.one;
        public BoxCollider collider;

        [SerializeField, ReadOnly] private float debugCurrentSpeed;
        [SerializeField, ReadOnly] private float debugSizeSpeed;

        private void Awake()
        {
            SwitchState(currentState);
        }
        void Update()
        {
            if(Input.GetKeyDown("a"))SwitchState(CloudState.SOLID);
            if (Input.GetKeyDown("z"))SwitchState(CloudState.BABY);
            if (Input.GetKeyDown("e"))SwitchState(CloudState.DESTRUCTOR);

            if (currentState == CloudState.DESTRUCTOR)
            {
                Collider[] hitCollider = Physics.OverlapSphere(transform.position, destructionRange);
                foreach (Collider collider in hitCollider)
                {
                    if (collider.GetComponent<DestructibleBlocks>())
                    {
                        Destroy(collider.gameObject);
                    }
                }
            }
        }
        
        public override bool Feed(CloudType cType)
        {
            if (canBePouffed) return false;
            
            switch (cType)
            {
                case CloudType.ENERGIZER:
                    SwitchState(CloudState.DESTRUCTOR);
                    break;                
                case CloudType.SOLIDIFIER:
                    SwitchState(CloudState.SOLID);
                    break; 
            }
            return true;
        }
        
        public void SwitchState(int newState) => SwitchState((CloudState)newState);
        public void SwitchState(CloudState newState)
        {
            currentState = newState;

            canBePouffed = true;
            collider.isTrigger = true;
            var currentSpeed = speed.x;
            float currentSize = scale.y;
            
            switch (newState)
            {
                case CloudState.BABY:
                    canBePouffed = false;
                    currentSpeed = speed.y;
                    currentSize = scale.x;
                    break;
                case CloudState.SOLID:
                    collider.isTrigger = false;
                    break;
            }
            
            agent.speed = currentSpeed;
            agent.acceleration = (currentSpeed *  currentSpeed)/2;
            
            agent.radius = currentSize;
            agent.height = currentSize + 0.5f;
            transform.localScale = Vector3.one * currentSize;
            
            debugCurrentSpeed = currentSpeed;
            debugSizeSpeed = currentSize;
        }
        
    }
}
