using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WanderingCloud.Controller;

namespace WanderingCloud.Gameplay
{
    public class CloudGrabber : MonoBehaviour
    {


        [SerializeField] public List<CloudBoulette> BouletteList = new List<CloudBoulette>();

        [SerializeField] public Player playerComponent;

        [SerializeField] public int nbOfPullet { get; private set; } = 0;

        [SerializeField] private Object pulletPrefab;
        [SerializeField] private Transform launchSocket;


        [SerializeField] private float cooldown;
        private float cooldownTimer = 0;

        private void Start()
        {
            cooldownTimer = cooldown;
        }

        private void Update()
        {
            BouletteUiManager();
            TimerHandler();
        }

        public void TimerHandler()
        {
            if (cooldownTimer < cooldown)
            {
                cooldownTimer += Time.deltaTime;
            }
        }

        public bool GrabNearestPullet()
        {
            
            if (BouletteList.Count > 0)
            {
                CloudBoulette nearest = BouletteList[0];
                for (int i = 1; i < BouletteList.Count; i++)
                {
                    if (BouletteList[i] != null)
                    {
                        if (nearest != null)
                        {
                            if (Vector3.Distance(BouletteList[i].transform.position, transform.position) < Vector3.Distance(nearest.transform.position, transform.position))
                            {
                                nearest = BouletteList[i];
                            }
                        }
                        else
                        {
                            nearest = BouletteList[i];
                        }
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
                    if (BouletteList[i] != null)
                    {
                        if (nearest != null)
                        {
                            BouletteList[i].UnShowGrabUI(playerComponent.isGyro);
                            if (Vector3.Distance(BouletteList[i].transform.position, transform.position) < Vector3.Distance(nearest.transform.position, transform.position))
                            {
                                nearest = BouletteList[i];
                            }
                        }
                        else
                        {
                            nearest = BouletteList[i];
                        }
                    }
                }
                nearest.ShowGrabUI(playerComponent.isGyro);
            }
        }



        public void LaunchPullet()
        {
            if (cooldownTimer >= cooldown)
            {
                if (nbOfPullet > 0)
                {
                    CloudBoulette cb = ((GameObject)Instantiate(pulletPrefab, launchSocket.position, Quaternion.identity)).GetComponent<CloudBoulette>();
                    cb.SetDestination(playerComponent.currentTarget);
                    nbOfPullet--;
                    cooldownTimer = 0;
                }
            }
        }

    }
}
