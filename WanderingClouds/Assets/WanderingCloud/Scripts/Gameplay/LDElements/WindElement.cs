#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using WanderingCloud.Controller;

namespace WanderingCloud
{
    [RequireComponent(typeof(BoxCollider))]
    public class WindElement : MonoBehaviour
    {
        public bool isNegative = false;
        public WindElement negateZone;

        [SerializeField] public float windForce = 5f;
        [SerializeField] public Vector2 windSize = Vector2.one * 5f;
        [SerializeField] public float windDistance = 15f;
        public BoxCollider coll;
        public Transform visual;

        public List<PlayerBrain> playerBrains = new List<PlayerBrain>();

        private void Awake()
        {
            coll = GetComponent<BoxCollider>();
            coll.isTrigger = true;
        }
        void FixedUpdate()
        {

            foreach (var playerBrain in playerBrains)
            {
                if (!isNegative)
                {
                    if (Vector3.Distance(coll.ClosestPoint(playerBrain.transform.position),playerBrain.transform.position ) <= 0 )
                    {
                        playerBrain.Movement.externalForce += transform.forward * windForce;
                    }
                    continue;
                }

                if (Vector3.Distance(negateZone.coll.ClosestPoint(playerBrain.transform.position),playerBrain.transform.position ) <= 0 )
                {
                    if (Vector3.Distance(coll.ClosestPoint(playerBrain.transform.position),playerBrain.transform.position ) <= 0 )
                    {
                        playerBrain.Movement.externalForce -= transform.forward * negateZone.windForce;
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<PlayerBrain>() != null)
            {
                //playerBrains.Add(other.GetComponentInParent<PlayerBrain>());
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<PlayerBrain>() != null)
            {
                //playerBrains.Remove(other.GetComponentInParent<PlayerBrain>());
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 windVector = new Vector3(windSize.x, windSize.y, windDistance);
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position + (transform.forward * windDistance / 2),Quaternion.LookRotation(transform.forward, transform.up), Vector3.one);

            using (new Handles.DrawingScope(Color.yellow, rotationMatrix))
            {
                Handles.DrawWireCube(Vector3.zero, windVector);
            }
            using (new Handles.DrawingScope(Color.yellow))
            {
                Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(transform.forward), windForce, EventType.Repaint);
            }
            
            coll.center = new Vector3(0, 0, (windDistance / transform.localScale.z) / 2);
            coll.size = new Vector3(windVector.x / transform.localScale.x, windVector.y / transform.localScale.y, windVector.z / transform.localScale.z);

            visual.position = transform.position + (transform.forward * windDistance / 2);
            visual.localScale = windVector;
            
        }
#endif
    }
}