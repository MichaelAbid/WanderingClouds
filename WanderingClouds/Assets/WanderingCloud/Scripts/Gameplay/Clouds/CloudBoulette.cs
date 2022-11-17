using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using WanderingCloud.Controller;

namespace WanderingCloud.Gameplay
{

    public enum CloudType
    {
        SOFT,
        THUNDER
    }

    public class CloudBoulette : MonoBehaviour
    {

        [Foldout("Ref")][SerializeField] private RawImage GiroImageRef;
        [Foldout("Ref")][SerializeField] private RawImage UrleImageRef;

        [Foldout("Type")][SerializeField] public CloudType cType;

        private Player giro, urle;
        public CloudGrabber cgGiro, cgUrle;


        [Foldout("Movement")][SerializeField] private CloudSource csTarget;
        [Foldout("Movement")][SerializeField] private bool shouldMove = false;
        [Foldout("Movement")][SerializeField] private float movementSpeed;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            UIRotation();
            if (shouldMove)
            {
                MovementUpdate();
            }
        }

        private void MovementUpdate()
        {
            if(csTarget != null)
            {
                Vector3 direction = csTarget.transform.position - transform.position;
                transform.position += direction.normalized * movementSpeed * Time.deltaTime;
                if (Vector3.Distance(csTarget.transform.position, transform.position) <= 2)
                {
                    shouldMove = false;
                    if (csTarget.Feed(cType))
                    {
                        Destroy(gameObject);
                    }
                }
            }

        }

        void UIRotation()
        {
            if (giro != null)
            {
                Vector3 xyDirection = Vector3.Scale(new Vector3(1, 0, 1), (transform.position - giro.Camera.transform.position).normalized);
                GiroImageRef.transform.rotation = Quaternion.LookRotation(xyDirection, Vector3.up);
            }
            if (urle != null)
            {
                Vector3 xyDirection = Vector3.Scale(new Vector3(1, 0, 1), (transform.position - urle.Camera.transform.position).normalized);
                UrleImageRef.transform.rotation = Quaternion.LookRotation(xyDirection, Vector3.up);
            }
        }

        public void SetDestination(CloudSource target, bool shouldMoveImmediate = true)
        {
            csTarget = target;
            shouldMove = shouldMoveImmediate;
        }


        private void OnTriggerEnter(Collider other)
        {
            CloudGrabber cg = other.GetComponentInParent<CloudGrabber>();
            if (cg != null)
            {
                cg.BouletteList.Add(this);
                if (cg.playerComponent.isGyro)
                {
                    giro = cg.playerComponent;
                    cgGiro = cg;
                }
                else
                {
                    urle = cg.playerComponent;
                    cgUrle = cg;
                }
            }
        }


        private void OnTriggerExit(Collider other)
        {
            CloudGrabber cg = other.GetComponentInParent<CloudGrabber>();
            if (cg != null)
            {
                cg.BouletteList.Remove(this);
                
            }
        }

        public void ShowGrabUI(bool isGiro)
        {
            if (!shouldMove)
            {
                if (isGiro)
                {
                    if (GiroImageRef != null)GiroImageRef.enabled = true;
                }
                else
                {
                    if (UrleImageRef != null)UrleImageRef.enabled = true;
                }
            }
        }

        public void UnShowGrabUI(bool isGiro)
        {
            if (isGiro)
            {
                if (GiroImageRef != null) GiroImageRef.enabled = false;
            }
            else
            {
                if (UrleImageRef != null) UrleImageRef.enabled = false;
            }
        }
    }

}