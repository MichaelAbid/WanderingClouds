using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public class CreatureSources : MonoBehaviour
    {
        public CloudState currentState = CloudState.BABY;
        public float destructionRange = 5f;
        [ReadOnly] public bool canBePouffed;
        [MinMaxSlider(0f, 5f)] public Vector2 scale = Vector2.one;
        public BoxCollider collider;

        

        private void Awake()
        {
            SwitchState(currentState);
        }

        public void SwitchState(int newState) => SwitchState((CloudState)newState);
        public void SwitchState(CloudState newState)
        {
            currentState = newState;

            canBePouffed = true;
            collider.isTrigger = true;
            transform.localScale = Vector3.one * scale.y;

            switch (newState)
            {
                case CloudState.BABY:
                    canBePouffed = false;
                    transform.localScale = Vector3.one * scale.x;
                    break;
                case CloudState.SOLID:
                    collider.isTrigger = false;
                    break;
                case CloudState.DESTRUCTOR:
                    break;
                default:
                    break;
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown("a"))
            {
                SwitchState(CloudState.SOLID);
            }
            if (Input.GetKeyDown("z"))
            {
                SwitchState(CloudState.BABY);
            }
            if (Input.GetKeyDown("e"))
            {
                SwitchState(CloudState.DESTRUCTOR);
            }
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
    }
}
