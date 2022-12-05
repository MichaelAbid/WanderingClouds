using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WanderingCloud.Gameplay
{
    [Serializable]
    public class Inventory :MonoBehaviour
    {
        
        [field: SerializeField] public int nbPullet { get; private set; }
        [field: SerializeField]public int maxPullet { get; private set; }

        public bool AddPullet()
        {
            if (nbPullet < maxPullet)
            {
                nbPullet++;
                return true;
            }
            return false;
        }
        public bool RemovePullet()
        {
            if (nbPullet > 0)
            {
                nbPullet--;
                return true;
            }
            return false;
        }

    }
}
