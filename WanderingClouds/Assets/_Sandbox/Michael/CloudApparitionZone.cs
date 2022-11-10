using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CloudApparitionZone : MonoBehaviour
{

    public Mesh cloudMesh;
    public Mesh posedMesh;

    public float zoneSize;
    public float minTimeBeforeMoving;
    public float maxTimeBeforeMoving;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.CircleHandleCap(0, transform.position, Quaternion.LookRotation(Vector3.up), zoneSize, EventType.Repaint);
    }
}
