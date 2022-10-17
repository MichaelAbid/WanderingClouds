using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
using NaughtyAttributes;

public class ObjectAngleView : MonoBehaviour
{
    public Vector3 alignView; 

    [Range(0.5f, 1)] public float threshold = 0.9f;

    public Transform[] composite;
    public TransformData[] shufleData;
    public TransformData[] clearData;

    public Pawn[] players;
    public bool done;

    [Button()]
    public void SaveView()
    {
        alignView = SceneView.lastActiveSceneView.camera.transform.forward;
        for (int i = 0; i < composite.Length; i++)
        {
            shufleData[i] = composite[i];
        }
    }
    [Button()]
    public void SaveClear()
    {
        for (int i = 0; i < composite.Length; i++)
        {
            clearData[i] = composite[i];
        }
    }

    [Button()]
    public void ToClear()
    {
        for (int i = 0; i < composite.Length; i++)
        {
            composite[i].transform.SetTransformData(clearData[i]);
        }
    }
    [Button()]
    public void ToView()
    {
        for (int i = 0; i < composite.Length; i++)
        {
            composite[i].transform.SetTransformData(shufleData[i]);
        }
    }

    [Button("Reset")]
    private void Start()
    {
        done = false;
        ToView();
    }

    void Update()
    {
        if (done) return;

        foreach (var player in players)
        {
            Debug.DrawRay(transform.position, -player.Camera.transform.forward, Color.blue, 0.2f);
            Debug.DrawRay(transform.position, (player.transform.position - transform.position).normalized, Color.red, 0.2f);

            if (Mathf.Abs(Vector3.Dot(player.Camera.transform.forward.normalized, alignView.normalized)) > threshold &&
                Mathf.Abs(Vector3.Dot((player.transform.position - transform.position).normalized, alignView.normalized)) > threshold &&
                player.Camera.rect.Contains(player.Camera.rect.size * player.Camera.GetScreenRatioPosOfGameObject(transform.position)))
            {

                done = true;
                for (int i = 0; i < composite.Length; i++)
                {
                    composite[i].transform.DoTransform(clearData[i], 2f);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, alignView, Color.blue, 0.2f);
    }
}