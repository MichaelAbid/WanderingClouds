using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WanderingCloud.Controller;

namespace WanderingCloud
{
    public class PushPlayers : MonoBehaviour
    {

        [SerializeField] public float PushForce;
        [SerializeField] public float PushDistance;
        [SerializeField] public Vector2 PushSize;
        [SerializeField] public bool isWaterPush;

        public BoxCollider ccollider;

        public List<PlayerBrain> playerBrains = new List<PlayerBrain>();
        public List<PushableObject> pushableObjects = new List<PushableObject>();
        // Start is called before the first frame update
        void Start()
        {

            

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            foreach(var playerBrain in playerBrains)
            {
                Ray ray = new Ray(transform.position, (playerBrain.transform.position - transform.position));
                if(!Physics.Raycast(ray,Vector3.Distance(transform.position, playerBrain.transform.position)-0.5f))
                playerBrain.Movement.externalForce += transform.forward * PushForce;
            }
            foreach (var pushableObject in pushableObjects)
            {
                if (pushableObject != null) { 
                Ray ray = new Ray(transform.position, (pushableObject.transform.position - transform.position));
                if (!Physics.Raycast(ray, Vector3.Distance(transform.position, pushableObject.transform.position) - 0.5f))
                if (ccollider.bounds.Contains(pushableObject.transform.position)) pushableObject.externalForce += transform.forward * PushForce;
                }
            }
        }



        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<PlayerBrain>() != null)
            {
                playerBrains.Add(other.GetComponent<PlayerBrain>());
            }
         
            if (other.GetComponent<PushableObject>() != null)
            {
                
                pushableObjects.Add(other.GetComponent<PushableObject>());
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<PlayerBrain>() != null)
            {
                playerBrains.Remove(other.GetComponent<PlayerBrain>());
            }
            if(other.GetComponent<PushableObject>() != null )
            {
                pushableObjects.Remove(other.GetComponent<PushableObject>());
            }
        }



        private void OnDrawGizmos()
        {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position + (transform.forward* PushDistance/2), Quaternion.LookRotation(transform.forward,transform.up), Vector3.one);
            Gizmos.matrix = rotationMatrix;
            Gizmos.color = new Color(1, 1, 0,0.5f);
            Gizmos.DrawCube(Vector3.zero, new Vector3(PushSize.x, PushSize.y, PushDistance));
            Handles.color = new Color(1, 1, 0);
            Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(transform.forward),PushForce,EventType.Repaint);

            ccollider.center = new Vector3(0,0,(PushDistance / transform.localScale.z) / 2);
            ccollider.size = new Vector3(PushSize.x/transform.localScale.x, PushSize.y / transform.localScale.y, PushDistance / transform.localScale.z);

        }
    }
}
