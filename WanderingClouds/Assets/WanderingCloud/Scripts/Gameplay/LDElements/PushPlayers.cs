#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using WanderingCloud.Controller;
using UnityEngine;
using System.Collections;
using System;

namespace WanderingCloud
{
    public class PushPlayers : MonoBehaviour
    {

        [SerializeField] public float PushForce = 5f;
        [SerializeField] public float PushDistance = 15f;
        [SerializeField] public float secondsBeforeNextPush = 2;
        
        [SerializeField] public Vector2 PushSize = Vector2.one * 5f;
        public float parentRatio = 1;
        public BoxCollider ccollider;
        public bool shouldPush = true;

        public List<PlayerBrain> playerBrains = new List<PlayerBrain>();
        public List<PushableObject> pushableObjects = new List<PushableObject>();

        List<Tuple<PlayerBrain, bool>> canPushs = new List<Tuple<PlayerBrain, bool>>();
        public bool isIA;

        void FixedUpdate()
        {
            if (shouldPush)
            {
                foreach (var playerBrain in playerBrains)
                {
                    PushPlayers pp = null;
                    if (playerBrain.aiGrabber.aiGrabed != null)
                    {
                        pp = playerBrain.aiGrabber.aiGrabed.GetComponentInChildren<PushPlayers>();
                    }
                    bool canPush = true;
                    Tuple<PlayerBrain, bool> push = null;
                    foreach (var item in canPushs)
                    {
                        if (item.Item1 == playerBrain)
                        {
                            canPush = item.Item2;
                            push = item;
                        }
                    }

                    if (playerBrain != null && (pp == null || pp != this) && canPush)
                    {
                        /*Ray ray = new Ray(transform.position, (playerBrain.transform.position - transform.position));
                        if(!Physics.Raycast(ray,Vector3.Distance(transform.position, playerBrain.transform.position)-0.5f))*/

                        playerBrain.Movement.externalForce += transform.forward * PushForce;
                        if (push == null)
                        {
                            canPushs.Add(new Tuple<PlayerBrain, bool>(playerBrain, false));
                        }
                        StartCoroutine(timerPush(playerBrain, secondsBeforeNextPush));
                    }
                }
                foreach (var pushableObject in pushableObjects)
                {
                    if (pushableObject != null)
                    {
                        /*Ray ray = new Ray(transform.position, (pushableObject.transform.position - transform.position));
                        if (!Physics.Raycast(ray, Vector3.Distance(transform.position, pushableObject.transform.position) - 0.5f))*/
                        pushableObject.externalForce += transform.forward * PushForce;
                    }
                }
            }
        }

        IEnumerator timerPush(PlayerBrain brain,float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Tuple<PlayerBrain, bool> ll = null;
            foreach (var item in canPushs)
            {
                if (item.Item1 == brain)
                {
                    ll = item;
                }
            }
            if (ll != null)
            {
                canPushs.Remove(ll);
            }
            canPushs.Add(new Tuple<PlayerBrain, bool>(brain, true));
                

        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponentInParent<PlayerBrain>() != null)
            {
                playerBrains.Add(other.GetComponentInParent<PlayerBrain>());
            }
            if (other.GetComponent<PushableObject>() != null)
            {
                if(!pushableObjects.Contains(other.GetComponent<PushableObject>()))
                pushableObjects.Add(other.GetComponent<PushableObject>());
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<PlayerBrain>() != null)
            {
                playerBrains.Remove(other.GetComponentInParent<PlayerBrain>());
            }
            if(other.GetComponent<PushableObject>() != null )
            {
                pushableObjects.Remove(other.GetComponent<PushableObject>());
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (parentRatio > 0)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position + (transform.forward * PushDistance / 2), Quaternion.LookRotation(transform.forward, transform.up), Vector3.one);
                Gizmos.matrix = rotationMatrix;
                Gizmos.color = new Color(1, 1, 0, 0.5f);
                Gizmos.DrawCube(Vector3.zero, new Vector3(PushSize.x, PushSize.y, PushDistance));
                Handles.color = new Color(1, 1, 0);
                Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(transform.forward), PushForce, EventType.Repaint);

                ccollider.center = new Vector3(0, 0, (PushDistance / transform.localScale.z / parentRatio) / 2);
                ccollider.size = new Vector3(PushSize.x / transform.localScale.x / parentRatio, PushSize.y / transform.localScale.y / parentRatio, PushDistance / transform.localScale.z / parentRatio);
            }
        }
#endif
    }
}
