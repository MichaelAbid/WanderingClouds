using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;



namespace WanderingCloud.Gameplay
{
    public enum CLOUD_STATE
    {
        STATE_SOLID,
        STATE_BRUME
    }

    public enum BEHAVIOR_STATE
    {
        STATE_WANDERING,
        STATE_LAUNCHED

    }

    public class StatesClouds : GrabableObject
    {


        public bool grabed;
        public CLOUD_STATE state = CLOUD_STATE.STATE_BRUME;
        public BEHAVIOR_STATE bState = BEHAVIOR_STATE.STATE_WANDERING;


        public float movementSpeed;
        public CloudApparitionZone zoneToMove;
        public Vector3 positionToGo;
        public bool shouldMoveToAZone;
        public bool landing = false;
        public bool departure = false;
        public float searchDistance = 10;
        List<CloudApparitionZone> zones;

        public MeshFilter meshFilter;

        // Start is called before the first frame update
        void Start()
        {
            zones = FindObjectsOfType<CloudApparitionZone>().ToList();
            zoneToMove = GetARandomZoneFromRange(searchDistance);
            positionToGo.x = zoneToMove.transform.position.x + Random.Range(-zoneToMove.zoneSize, zoneToMove.zoneSize);
            positionToGo.y = zoneToMove.transform.position.y + 1;
            positionToGo.z = zoneToMove.transform.position.z + Random.Range(-zoneToMove.zoneSize, zoneToMove.zoneSize);
            shouldMoveToAZone = true;

        }


        public CloudApparitionZone GetARandomZoneFromRange(float range)
        {
            List<CloudApparitionZone> temp = new List<CloudApparitionZone>();
            foreach (CloudApparitionZone item in zones)
            {
                if (item != zoneToMove && (Vector3.Distance(transform.position, item.transform.position) <= range))
                {
                    temp.Add(item);
                }
            }
            if (temp.Count > 0)
            {
                return temp.Random();
            }
            return null;
        }

        // Update is called once per frame
        void Update()
        {
            MovementUpdate();
        }

        private void MovementUpdate()
        {
            switch (state)
            {
                case CLOUD_STATE.STATE_SOLID:
                    break;
                case CLOUD_STATE.STATE_BRUME:
                    switch (bState)
                    {
                        case BEHAVIOR_STATE.STATE_WANDERING:
                            if (shouldMoveToAZone)
                            {
                                if (Vector3.Distance(positionToGo, transform.position) > 0.2f)
                                {
                                    Vector3 direction = (positionToGo - transform.position).normalized;
                                    transform.position += direction * movementSpeed * Time.deltaTime;

                                }
                                else
                                {
                                    if (!departure)
                                    {
                                        if (landing == true)
                                        {
                                            shouldMoveToAZone = false;
                                            meshFilter.mesh = zoneToMove.posedMesh;
                                            StartCoroutine(WaitForDeparture(Random.Range(zoneToMove.minTimeBeforeMoving, zoneToMove.maxTimeBeforeMoving)));
                                        }
                                        else
                                        {
                                            landing = true;
                                            positionToGo.y = positionToGo.y - 1;
                                        }
                                    }


                                }
                            }
                            break;
                        case BEHAVIOR_STATE.STATE_LAUNCHED:
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        IEnumerator WaitForDeparture(float v)
        {
            yield return new WaitForSeconds(v);
            meshFilter.mesh = zoneToMove.cloudMesh;
            zoneToMove = GetARandomZoneFromRange(searchDistance);
            positionToGo.x = transform.position.x;
            positionToGo.y = transform.position.y + 1;
            positionToGo.z = transform.position.z;
            landing = false;
            shouldMoveToAZone = true;
            departure = true;
            while (Vector3.Distance(positionToGo, transform.position) > 0.2f) yield return new WaitForEndOfFrame();
            departure = false;
            positionToGo.x = zoneToMove.transform.position.x + Random.Range(-zoneToMove.zoneSize, zoneToMove.zoneSize);
            positionToGo.y = zoneToMove.transform.position.y + 1;
            positionToGo.z = zoneToMove.transform.position.z + Random.Range(-zoneToMove.zoneSize, zoneToMove.zoneSize);



        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (shouldMoveToAZone && bState == BEHAVIOR_STATE.STATE_WANDERING)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, positionToGo);
            }
        }
#endif



    }

}