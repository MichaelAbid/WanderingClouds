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

        public BoxCollider collider;

        List<PlayerBrain> playerBrains;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            foreach(var playerBrain in playerBrains)
            {
                //playerBrain.Movement.externalForce = transform.forward * PushForce;
            }
        }



        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<PlayerBrain>() != null)
            {
                playerBrains.Add((PlayerBrain)other.GetComponent<PlayerBrain>());
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<PlayerBrain>() != null)
            {
                playerBrains.Remove((PlayerBrain)other.GetComponent<PlayerBrain>());
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

            collider.center = new Vector3(0,0,PushDistance / 2);
            collider.size = new Vector3(PushSize.x, PushSize.y, PushDistance);

        }
    }
}
