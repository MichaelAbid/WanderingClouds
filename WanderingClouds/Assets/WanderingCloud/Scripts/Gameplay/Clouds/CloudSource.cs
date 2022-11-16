using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WanderingCloud.Gameplay
{

    public class CloudSource : MonoBehaviour
    {

        [Foldout("Boulette Spawn")] [OnValueChanged("CheckForMaxValue"), OnValueChanged("RandomizePosition"), OnValueChanged("UpdateScale")][Min(1)] public int numberOfBoulletToCreate;
        [Foldout("Boulette Spawn")] public UnityEngine.Object boulettePrefab;
        [Foldout("Boulette Spawn")] [OnValueChanged("RandomizePosition")] public float spraySize;
        [Foldout("Boulette Spawn")] [SerializeField][Min(0.1f)] private float boulettesize = 0.5f;
        [Foldout("Boulette Spawn")] private List<Vector3> randomPositions = new List<Vector3>();
        [Foldout("Boulette Spawn")] [OnValueChanged("RandomizePosition")][Range(0, 1)][SerializeField] private float randomThreshold = 0.75f;

        [Foldout("Respawn")] [SerializeField] private bool isActive = true;
        [Foldout("Respawn")] [SerializeField] private float timeBeforeRespawn = 60;

        [Foldout("Ref")] public MeshRenderer meshRenderer;
        [Foldout("Ref")] public Collider cCollider;

        [Foldout("Source")][SerializeField] private float ratioForEachBoulette = 0.1f;
        [Foldout("Source")][SerializeField] private float maxScaleAddition = 3;
        [Foldout("Source")][OnValueChanged("CheckForMaxValue")][SerializeField] private int maxBouletteToAdd = 10;
        

        [Foldout("Boulette")] public List<CloudBoulette> boulettes = new List<CloudBoulette>();
        internal CloudExploder cgUrle;
        internal CloudExploder cgGiro;

        private void UpdateScale()
        {
            float i = 1;
            i += (numberOfBoulletToCreate * ratioForEachBoulette);
            if(i > maxScaleAddition)
            {
                i = maxScaleAddition;
            }
            transform.localScale = new Vector3(i, i, i);
        }

        private void CheckForMaxValue()
        {
            if(numberOfBoulletToCreate> maxBouletteToAdd)
            {
                numberOfBoulletToCreate = maxBouletteToAdd;
                UpdateScale();
                RandomizePosition();
            }
        }

        // Create Random Position for Pullet
        [Button("Generate Random Preview Position")]
        public void RandomizePosition()
        {
            randomPositions.Clear();
            float slice = 2 * Mathf.PI/ numberOfBoulletToCreate;
            for (int i = 0; i < numberOfBoulletToCreate; i++)
            {
                float radius = Random.Range(Mathf.Lerp(0, spraySize,randomThreshold), spraySize);
                float angle = slice * i;
                float newX = (radius * Mathf.Cos(angle));
                float newZ = (radius * Mathf.Sin(angle));
                float maxHeightSpawn = Mathf.Sqrt((spraySize * spraySize) - (radius * radius));
                float newY = maxHeightSpawn;
                Vector3 pos = new Vector3(newX, newY, newZ);
                randomPositions.Add(pos);
            }
        }


        // Create Pullet and Desactive 
        [Button]
        public void ExplodeSource()
        {
           
            if (isActive)
            {
                RandomizePosition();
                if (randomPositions.Count > 0)
                {
                    foreach (Vector3 pos in randomPositions)
                    {
                        GameObject obj = (GameObject)Instantiate(boulettePrefab, transform.position + pos, Quaternion.identity);
                        boulettes.Add(obj.GetComponent<CloudBoulette>());
                    }
                }
                StartCoroutine(DesactiveAfterExplode());
            }
            
        }

        IEnumerator DesactiveAfterExplode()
        {
            meshRenderer.enabled = false;
            cCollider.enabled = false;
            isActive = false;
            yield return new WaitForSeconds(timeBeforeRespawn);
            meshRenderer.enabled = true;
            cCollider.enabled = true;
            isActive = true;
        }


        private void OnDrawGizmos()
        {

            if (isActive)
            {
                Gizmos.color = new Color(0, 0, 1, 0.3f);
                Gizmos.DrawSphere(transform.position, spraySize);

                if (randomPositions.Count > 0)
                {
                    Gizmos.color = new Color(0, 0, 1, 0.5f);
                    foreach (Vector3 pos in randomPositions)
                    {
                        Gizmos.DrawSphere(transform.position + pos, 0.5f);

                    }
                }
            }

        }

        internal void UnShowGrabUI(bool isGyro)
        {
            throw new NotImplementedException();
        }

        internal void ShowGrabUI(bool isGyro)
        {
            throw new NotImplementedException();
        }
    }

}