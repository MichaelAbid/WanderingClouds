using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WanderingCloud
{
    [RequireComponent(typeof(Rigidbody))]
    public class PushableObject : MonoBehaviour
    {

        public Vector3 externalForce;
        public bool ShouldReturnToOriginalPos;
        public float speed;
        private Vector3 pos;
        // Start is called before the first frame update
        void Start()
        {
            if (ShouldReturnToOriginalPos)
            {
                pos = transform.position;
            }

            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (externalForce != Vector3.zero)
            {
                transform.position += externalForce * Time.deltaTime;
                externalForce = Vector3.zero;
            }
            else
            {
                if (ShouldReturnToOriginalPos)
                {
                    transform.position += (pos - transform.position).normalized * Time.deltaTime * speed;
                }
            }
        }
    }
}
