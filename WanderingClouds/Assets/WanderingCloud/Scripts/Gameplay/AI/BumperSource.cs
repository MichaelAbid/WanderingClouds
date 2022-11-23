using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WanderingCloud.Gameplay
{
    public class BumperSource : Source
    {

        [SerializeField] public float currentPullet { get; private set; } = 0;
        [SerializeField] public float maxPullet { get; private set; } = 5;
        [SerializeField] public float numberPulletStage1 { get; private set; } = 2;
        [SerializeField] public float numberPulletStage2 { get; private set; } = 5;
        public override bool Feed(CloudType cType)
        {
            if(currentPullet < maxPullet)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }


    }
}
