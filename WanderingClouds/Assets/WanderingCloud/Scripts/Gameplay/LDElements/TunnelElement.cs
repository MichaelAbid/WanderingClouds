using UnityEngine;
using WanderingCloud.Controller;

namespace WanderingCloud
{
    [RequireComponent(typeof(BoxCollider))]
    public class TunnelElement : MonoBehaviour
    {
        [SerializeField] private float repulseForce;
        [SerializeField] private bool isForGiro;
        private BoxCollider coll;

        private void Awake()
        {
            coll = this.GetComponent<BoxCollider>();
            coll.isTrigger = true;
        }

        public void Repulse(PlayerBrain player)
        {
            if (isForGiro != player.isGyro)
            {
                player.Movement.externalForce += Vector3.ProjectOnPlane(player.transform.position - transform.position,Vector3.up).normalized * repulseForce;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponentInParent<PlayerBrain>())
            {
                Repulse(other.GetComponentInParent<PlayerBrain>());
            }
        }
		
    }
}