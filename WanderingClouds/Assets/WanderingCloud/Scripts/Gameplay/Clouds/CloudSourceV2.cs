using System;
using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using WanderingCloud.Controller;

namespace WanderingCloud
{
    [RequireComponent(typeof(Collider))]
    public class CloudSourceV2 : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponentInParent<PlayerMovement>()) return;
            if (!other.GetComponentInParent<PlayerInventory>()) return;

            if (other.GetComponentInParent<PlayerMovement>().moveState is MovementState.Rush or MovementState.Dash)
            {
                other.GetComponentInParent<PlayerInventory>().CloudContact();
            }
        }
    }
}
