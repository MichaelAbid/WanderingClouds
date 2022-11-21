using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PointCloud : MonoBehaviour
{

    public float distanceBeforeBreak;
    public float distanceBeforePull;
    public float distanceBeforeNotLinked;
    public Vector3 velocity = Vector3.zero;
    public bool notDriven;


    Vector3 CalculedVelocity = Vector3.zero;
    float speed = 0;

    public float speedWhenBreak;
    public float speedWhenPull;

    private void Update()
    {
        MovementUpdate();
    }

    private void MovementUpdate() 
    {


        List<PointCloud> pointClouds = FindObjectsOfType<PointCloud>().ToList();
        CalculedVelocity = Vector3.zero;
        speed = 0;
        foreach (PointCloud pointCloud in pointClouds)
        {
            if (pointCloud != this)
            {
                float distance = Mathf.Abs(Vector3.Distance(pointCloud.transform.position, transform.position));
                Vector3 vectorToPoint = (pointCloud.transform.position - transform.position).normalized;
                if (distance <= distanceBeforeNotLinked)
                {
                    if (distance >= distanceBeforePull)
                    {
                        if (distance >= distanceBeforeBreak)
                        {
                            // Add a different velocity because this point is too far. 
                            if (CalculedVelocity == Vector3.zero)
                            {
                                CalculedVelocity = vectorToPoint;
                            }
                            else
                            {
                                CalculedVelocity = Vector3.Lerp(vectorToPoint, CalculedVelocity, 0.25f);
                            }
                            speed = Mathf.Lerp(speedWhenBreak, speed, 0.8f);
                        }
                        else
                        {
                            if (CalculedVelocity == Vector3.zero)
                            {
                                CalculedVelocity = vectorToPoint;
                            }
                            else
                            {
                                CalculedVelocity = Vector3.Lerp(vectorToPoint, CalculedVelocity, 0.5f);
                            }
                            speed = Mathf.Lerp(speedWhenPull, speed, 0.8f);
                        }
                    }
                    else
                    {
                        if (CalculedVelocity == Vector3.zero)
                        {
                            CalculedVelocity = -vectorToPoint;
                        }
                        else
                        {
                            CalculedVelocity = Vector3.Lerp(-vectorToPoint, CalculedVelocity, 0.5f);
                        }
                        speed = Mathf.Lerp(speedWhenPull, speed, 0.8f);
                    }
                }
            }

        }

        transform.position+= CalculedVelocity*speed*Time.deltaTime;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        List<PointCloud> pointClouds = FindObjectsOfType<PointCloud>().ToList();


        CalculedVelocity = Vector3.zero;
        speed = 0;
        foreach (PointCloud pointCloud in pointClouds)
        {
            if (pointCloud != this)
            {
                float distance = Mathf.Abs(Vector3.Distance(pointCloud.transform.position, transform.position));
                Vector3 vectorToPoint = (pointCloud.transform.position - transform.position).normalized;
                if (distance <= distanceBeforeNotLinked)
                {
                    Gizmos.color = Color.green;
                    GUI.color = Color.green;
                    if (distance >= distanceBeforePull)
                    {
                        Gizmos.color = Color.yellow;
                        GUI.color = Color.yellow;
                        if (distance >= distanceBeforeBreak)
                        {
                            Gizmos.color = Color.red;
                            GUI.color = Color.red;
                            // Add a different velocity because this point is too far. 
                            if(CalculedVelocity == Vector3.zero)
                            {
                                CalculedVelocity = vectorToPoint ;
                            }
                            else
                            {
                                CalculedVelocity = Vector3.Lerp(vectorToPoint, CalculedVelocity, 0.25f);
                            }
                            speed = Mathf.Lerp(speedWhenBreak,speed,0.8f);
                        }
                        else
                        {
                            if (CalculedVelocity == Vector3.zero)
                            {
                                CalculedVelocity = vectorToPoint;
                            }
                            else
                            {
                                CalculedVelocity = Vector3.Lerp(vectorToPoint , CalculedVelocity,0.5f) ;
                            }
                            speed = Mathf.Lerp(speedWhenPull, speed, 0.8f);
                        }
                    }
                    else
                    {
                        if (CalculedVelocity == Vector3.zero)
                        {
                            CalculedVelocity = -vectorToPoint;
                        }
                        else
                        {
                            CalculedVelocity = Vector3.Lerp(-vectorToPoint, CalculedVelocity, 0.5f);
                        }
                        speed = Mathf.Lerp(speedWhenPull, speed, 0.8f);
                    }

                    Gizmos.DrawLine(pointCloud.transform.position, transform.position);
                    Handles.Label(Vector3.Lerp(pointCloud.transform.position, transform.position, 0.5f), distance.ToString());
                }
            }

        }
        
        GUI.color = Color.cyan;
        Handles.ArrowHandleCap(0, transform.position , Quaternion.LookRotation(CalculedVelocity), speed, EventType.Repaint);
    }
#endif



}
