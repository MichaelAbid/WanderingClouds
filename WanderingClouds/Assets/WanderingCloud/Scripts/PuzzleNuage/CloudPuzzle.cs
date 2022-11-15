using EzySlice;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEditor;
using UnityEngine;


namespace WanderingCloud.Gameplay
{
    [Serializable]
    public class Vector3Slice
    {
        [Range(-1, 1)] public float x;
        [Range(-1, 1)] public float y = 1;
        [Range(-1, 1)] public float z;

        public Vector3 AsVector3()
        {
            return new Vector3(x, y, z).normalized;
        }
    }

    [Serializable]
    public class SliceOrder
    {

        public Vector3 slicePosition;
        [Header("Should not be zero")]
        public Vector3Slice sliceOrientation;
        public Color sliceColor;

        public bool OrientationIsCorrect(Vector3Slice slice)
        {
            if (slice.AsVector3() == Vector3.zero) return false;
            return true;
        }
    }


    public class CloudPuzzle : MonoBehaviour
    {

        public List<SliceOrder> sliceOrders;
        public bool showOnlyWhenSelected = true;
        public Material material;
        public List<CloudPuzzle> puzzlePart;

        private void OnDrawGizmosSelected()
        {
            if (showOnlyWhenSelected)
            {
                DrawSliceGizmo();
            }
        }

        private void OnDrawGizmos()
        {
            if (!showOnlyWhenSelected)
            {
                DrawSliceGizmo();
            }
        }

        public void DrawSliceGizmo()
        {
            if (sliceOrders == null || sliceOrders.Count == 0) return;
            Gizmos.color = Color.white;
            int number = 0;
            foreach (SliceOrder sliceOrder in sliceOrders)
            {
                number++;
                Vector3 vec = sliceOrder.sliceOrientation.AsVector3();
                Quaternion slicePlaneUpVector = Quaternion.LookRotation(vec);
                Quaternion slicePlaneRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.Cross(vec, Vector3.up), vec));


                // Draw Slice Plane
                Gizmos.color = sliceOrder.sliceColor;
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position + sliceOrder.slicePosition, slicePlaneRotation, Vector3.one);
                Gizmos.matrix = rotationMatrix;
                float hypo = Mathf.Sqrt((transform.lossyScale.x * transform.lossyScale.x) + (transform.lossyScale.z * transform.lossyScale.z));
                Gizmos.DrawCube(Vector3.zero, new Vector3(hypo, 0.01f, hypo));

                // Draw Arrow that represent the slice Vector



                Handles.color = sliceOrder.sliceColor;
                Handles.ArrowHandleCap(0, transform.position + sliceOrder.slicePosition, slicePlaneUpVector, 1, EventType.Repaint);



                // Draw Position Point
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(Vector3.zero, 0.1f);
                GUI.color = Color.black;
                Handles.Label(transform.position + sliceOrder.slicePosition, number.ToString());



            }
            Gizmos.color = Color.white;
        }

        [Button]
        public void SliceItem()
        {

            GameObject objectToCut = gameObject;
            foreach (SliceOrder sliceOrder in sliceOrders)
            {

                SlicedHull sliced = Slice(objectToCut, transform.position + sliceOrders[0].slicePosition, sliceOrder.sliceOrientation.AsVector3());
                if (sliced != null)
                {
                    GameObject halfup = sliced.CreateUpperHull();
                    halfup.GetComponent<MeshRenderer>().material = material;
                    halfup.name = name + " - up";
                    halfup.transform.position = objectToCut.transform.position;
                    halfup.transform.rotation = objectToCut.transform.rotation;
                    halfup.transform.parent = transform;
                    halfup.transform.localScale = Vector3.one;
                    CloudPuzzle upPuzzle = halfup.AddComponent<CloudPuzzle>();
                    upPuzzle.material = material;
                    puzzlePart.Add(upPuzzle);
                    GameObject halfdown = sliced.CreateLowerHull();
                    halfdown.GetComponent<MeshRenderer>().material = material;
                    halfdown.name = name + " - down";
                    halfdown.transform.position = objectToCut.transform.position;
                    halfdown.transform.rotation = objectToCut.transform.rotation;
                    halfdown.transform.parent = transform;
                    halfdown.transform.localScale = Vector3.one;
                    CloudPuzzle downPuzzle = halfdown.AddComponent<CloudPuzzle>();
                    downPuzzle.material = material;
                    puzzlePart.Add(downPuzzle);
                    objectToCut = halfup;
                }
                else
                {
                    Debug.Log("Can't slice " + objectToCut.name);
                }
            }

            gameObject.GetComponent<MeshRenderer>().enabled = false;

        }


        [Button]
        public void ResetSlice()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            if (puzzlePart == null || puzzlePart.Count == 0) return;
            foreach (CloudPuzzle item in puzzlePart)
            {
                if (item != null)
                {
                    item.ResetSlice();
                    DestroyImmediate(item.gameObject);
                }

            }

            puzzlePart.Clear();

        }

        public SlicedHull Slice(GameObject go, Vector3 planeWorldPosition, Vector3 planeWorldDirection)
        {
            return go.Slice(planeWorldPosition, planeWorldDirection, go.GetComponent<MeshRenderer>().sharedMaterial);
        }

    }
}
