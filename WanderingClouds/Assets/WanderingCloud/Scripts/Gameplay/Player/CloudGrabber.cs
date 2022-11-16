using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WanderingCloud.Controller;

namespace WanderingCloud.Gameplay
{
    public class CloudGrabber : MonoBehaviour
    {


        public List<CloudBoulette> BouletteList = new List<CloudBoulette>();

        public Player playerComponent;

        public int nbOfPullet = 0;

        private void Start()
        {
            
        }

        private void Update()
        {
            BouletteUiManager();
        }


        public bool GrabNearestPullet()
        {
            
            if (BouletteList.Count > 0)
            {
                CloudBoulette nearest = BouletteList[0];
                for (int i = 1; i < BouletteList.Count; i++)
                {
                    if (Vector3.Distance(BouletteList[i].transform.position, transform.position) < Vector3.Distance(nearest.transform.position, transform.position))
                    {
                        nearest = BouletteList[i];
                    }
                }
                nbOfPullet++;
                if (nearest.cgUrle != null)
                {
                    nearest.cgUrle.BouletteList.Remove(nearest);
                }
                if (nearest.cgGiro != null)
                {
                    nearest.cgGiro.BouletteList.Remove(nearest);
                }
                Destroy(nearest.gameObject);
                return true;
            }
            return false;
        }


        private void BouletteUiManager()
        {
            if (BouletteList.Count > 0)
            {
                CloudBoulette nearest = BouletteList[0];
                for (int i = 1; i < BouletteList.Count; i++)
                {
                    BouletteList[i].UnShowGrabUI(playerComponent.isGyro);
                    if (Vector3.Distance(BouletteList[i].transform.position, transform.position) < Vector3.Distance(nearest.transform.position, transform.position))
                    {
                        nearest = BouletteList[i];
                    }
                }
                nearest.ShowGrabUI(playerComponent.isGyro);
            }
        }

    }
}
