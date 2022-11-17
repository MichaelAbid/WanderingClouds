using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
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


        private CloudSource csTarget;
        private bool shouldMove = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            UIRotation();
        }


        void UIRotation()
        {
            if(giro != null)
            {
                
                Vector3 xyDirection = Vector3.Scale(new Vector3(1,0,1) , ( transform.position- giro.transform.position ).normalized);
                GiroImageRef.transform.rotation = Quaternion.LookRotation(xyDirection, Vector3.up);
            }
            if (urle != null)
            {

                Vector3 xyDirection = Vector3.Scale(new Vector3(1, 0, 1), (urle.transform.position - transform.position).normalized);
                UrleImageRef.transform.rotation = Quaternion.LookRotation(xyDirection, Vector3.up);
            }
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
            else
            {
                CloudSource cs = other.GetComponentInParent<CloudSource>();
                if(cs != null)
                {
                    if (cs.Feed(cType))
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }


        public void SetDestination(CloudSource target, bool shouldMoveImmediate = true)
        {
            csTarget = target;
            shouldMove = shouldMoveImmediate;
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
            if (isGiro)
            {
                GiroImageRef.enabled = true;
            }
            else 
            {
                UrleImageRef.enabled = true;
            }
        }

        public void UnShowGrabUI(bool isGiro)
        {
            if (isGiro)
            {
                GiroImageRef.enabled = false;
            }
            else
            {
                UrleImageRef.enabled = false;
            }
        }
    }

}