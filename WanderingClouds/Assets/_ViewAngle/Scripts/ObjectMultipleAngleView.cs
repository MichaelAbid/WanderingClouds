using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ObjectMultipleAngleView : MonoBehaviour
{
    public Pawn playersLeft;
    public Pawn playersRight;
    [Space(5)]
    public Transform[] leftLink;
    public Transform[] rightLink;
    [Space(10)]
    [Range(0.0f, 0.1f)] public float threshold = 0.05f;

    public bool isAlign;
    public float distance;
    void Update()
    {
        Vector2[] leftScreenPos = new Vector2[leftLink.Length];
        for (int i = 0; i < leftLink.Length; i++)
        {
            leftScreenPos[i] = playersLeft.Camera.GetScreenRatioPosOfGameObject(leftLink[i].position) * playersLeft.Camera.rect.size;
        }
        Vector2[] rightScreenPos = new Vector2[rightLink.Length];
        for (int i = 0; i < rightLink.Length; i++)
        {
            rightScreenPos[i] = playersRight.Camera.GetScreenRatioPosOfGameObject(rightLink[i].position) * playersRight.Camera.rect.size;
        }
        
        distance = 0f;
        for (int i = 0; i < leftScreenPos.Length; i++)
        {
            var x = Vector2.Distance(leftScreenPos[i], rightScreenPos[i]);
            distance += x;

            Debug.DrawLine(Vector3.up * 5 + (Vector3)leftScreenPos[i],Vector3.up * 5 + (Vector3)rightScreenPos[i], x > threshold? Color.red : Color.green);
        }

        isAlign =distance < threshold;
    }
    
}
