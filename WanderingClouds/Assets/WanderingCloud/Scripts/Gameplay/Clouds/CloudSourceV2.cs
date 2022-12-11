using UnityEngine;
using UnityEngine.Events;
using WanderingCloud.Controller;
using WanderingCloud.Gameplay;

namespace WanderingCloud
{
    [RequireComponent(typeof(Collider))]
    public class CloudSourceV2 : ShaderLink
    {
        public bool isGD2 = false;
        public UnityEvent onPushed;
        public UnityEvent onRefill;

        private bool pushed = false;
        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            pushed = false;
        }
        public void Refill()
        {
            UpdateProperty("_EntryPos", Vector3.up *10);
            UpdateProperty("_ExitPos", Vector3.up * 10);
            onRefill?.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponentInParent<PlayerMovement>()) return;
            if (!other.GetComponentInParent<PlayerInventory>()) return;

            if (other.GetComponentInParent<PlayerMovement>().moveState is MovementState.Rush or MovementState.Dash)
            {
                if (other.GetComponentInParent<PlayerInventory>().pelletStock > 0) return;
                if (isGD2)
                {
                    if (GetComponent<CreatureSources>().currentState is CloudState.BABY) return;
                }

                other.GetComponentInParent<PlayerInventory>().CloudContact();
                pushed = true;
                onPushed?.Invoke();
                var pos = other.transform.position - transform.position;
                UpdateProperty("_EntryPos", pos);
                UpdateProperty("_ExitPos", pos);
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.collider.GetComponentInParent<PlayerMovement>()) return;
            if (!collision.collider.GetComponentInParent<PlayerInventory>()) return;

            if (collision.collider.GetComponentInParent<PlayerMovement>().moveState is MovementState.Rush or MovementState.Dash)
            {
                GetComponent<Collider>().isTrigger = true;
                collision.collider.GetComponentInParent<PlayerInventory>().CloudContact();
                pushed = true;
                onPushed?.Invoke();
                var pos = collision.transform.position - transform.position;
                UpdateProperty("_EntryPos", pos);
                UpdateProperty("_ExitPos", pos);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.GetComponentInParent<PlayerMovement>()) return;
            if (!other.GetComponentInParent<PlayerInventory>()) return;

            if (pushed)
            {
                UpdateProperty("_ExitPos", other.transform.position - transform.position);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.GetComponentInParent<PlayerMovement>()) return;
            if (!other.GetComponentInParent<PlayerInventory>()) return;

            if (pushed)
            {
                pushed = false;
                UpdateProperty("_ExitPos", other.transform.position - transform.position);
            }
        }
    }
}
