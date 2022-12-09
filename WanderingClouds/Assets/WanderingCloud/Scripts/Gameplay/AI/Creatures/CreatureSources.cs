using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
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
        public bool isAgent;
        public NavMeshAgent agent;
        public ShaderLink shaderLink;
        public Transform self;
        public GameObject destructorArea;

        public CloudState currentState = CloudState.BABY;
        public float destructionRange = 5f;
        [ReadOnly] public bool canBePouffed;
        [MinMaxSlider(0f, 4f)] public Vector2 scaleX = Vector2.one;
        [MinMaxSlider(0f, 4f)] public Vector2 scaleY = Vector2.one;
        [MinMaxSlider(0f, 4f)] public Vector2 scaleZ  = Vector2.one;
        [MinMaxSlider(1f, 15f)] public Vector2 speed = Vector2.one;
        public Collider collider;

        [SerializeField, ReadOnly] private float debugCurrentSpeed;
        [SerializeField, ReadOnly] private float debugSizeSpeed;

        public UnityEvent onBaby;
        public UnityEvent onSolidy;
        public UnityEvent onDestructor;

        private void Start()
        {
            SwitchState(currentState);
        }
        void Update()
        {
            if (currentState is CloudState.DESTRUCTOR)
            {
                Collider[] hitCollider = Physics.OverlapSphere(self.position, destructionRange);
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
            //if (canBePouffed) return false;

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
            if (destructorArea is not null)
            {
                destructorArea.SetActive(false);
            }

            var currentSpeed = speed.x;
            Vector3 currentSize = new Vector3( scaleX.y, scaleY.y, scaleZ.y);

            switch (newState)
            {
                case CloudState.BABY:
                    canBePouffed = false;
                    currentSpeed = speed.y;
                    currentSize = new Vector3(scaleX.x, scaleY.x, scaleZ.x); ;
                    onBaby?.Invoke();
                    break;
                case CloudState.SOLID:
                    collider.isTrigger = false;
                    onSolidy?.Invoke();
                    break;
                case CloudState.DESTRUCTOR:
                    if (destructorArea is not null)
                        destructorArea.SetActive(true);
                    onDestructor?.Invoke();
                    break;
            }

            debugCurrentSpeed = currentSpeed;

            if (isAgent)
            {
                agent.speed = currentSpeed;
                agent.acceleration = (currentSpeed * currentSpeed) / 2;

                agent.radius = currentSize.x;
                agent.height = currentSize.x + 0.5f;
            }
            
            self.localScale = currentSize;

        }

    }
}
