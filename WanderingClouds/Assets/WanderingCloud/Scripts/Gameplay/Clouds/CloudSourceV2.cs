using System;
using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using WanderingCloud.Controller;

namespace WanderingCloud
{
    [RequireComponent(typeof(Collider))]
    public class CloudSourceV2 : ShaderLink
    {
        private bool pushed = false; 
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
                pushed = true;
                var pos = other.transform.position - transform.position;
                UpdateProperty(render, ("_EntryPos",pos));
                UpdateProperty(render, ("_ExitPos",pos));
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (!other.GetComponentInParent<PlayerMovement>()) return;
            if (!other.GetComponentInParent<PlayerInventory>()) return;

            if (pushed)
            {
                pushed = false;
                UpdateProperty(render, ("_ExitPos",other.transform.position - transform.position));
            }         
        }
    }
}
