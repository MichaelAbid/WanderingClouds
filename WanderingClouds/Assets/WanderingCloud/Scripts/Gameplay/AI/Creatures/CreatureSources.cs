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
        BABY,
        SOLID,
        DESTRUCTOR
    }
    public class CreatureSources : MonoBehaviour
    {
        public CloudState currentState;
        [HideInInspector] public bool canBePouffed;
        public float sizeAugment;
        public new BoxCollider collider;
        
        void Start()
        {
            if (currentState == CloudState.BABY)
            {
                canBePouffed = true;
            } else
                canBePouffed = false;

            if(currentState != CloudState.SOLID)
            {
                //collider.isTrigger = true;
            }
        }


        public void TurnSolid()
        {
            canBePouffed = true;
            currentState = CloudState.SOLID;
            collider.isTrigger = false;
            transform.localScale = transform.localScale * sizeAugment;
        }

        public void TurnDestructor()
        {
            canBePouffed = true;
            currentState = CloudState.DESTRUCTOR;
            transform.localScale = transform.localScale * sizeAugment;

        }

        public void TurnBaby()
        {
            currentState = CloudState.BABY;
            canBePouffed = false;
            //collider.isTrigger = true;
            transform.localScale = transform.localScale / sizeAugment;
        }

        private void Update()
        {
            if(Input.GetKeyDown("a"))
            {
                TurnSolid();
            }
            if (Input.GetKeyDown("z"))
            {
                TurnBaby();
            }
            if (Input.GetKeyDown("e"))
            {
                TurnDestructor();
            }
        }
    }
}
