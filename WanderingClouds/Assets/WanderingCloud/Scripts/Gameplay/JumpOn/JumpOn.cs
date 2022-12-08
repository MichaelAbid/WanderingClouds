using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using WanderingCloud.Controller;

namespace WanderingCloud.Gameplay
{
    public class JumpOn : MonoBehaviour
    {
        public float bumpHeight = 10;
        [Range(0,1)] public float ratio = 0.75f;
        public BumperSource BumperSource;

        private void OnTriggerEnter(Collider other)
        {
            if (BumperSource.maxPullet*ratio <= BumperSource.currentPullet)
            {
                PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
                if (player != null)
                {
                    Debug.Log("Bumped");
                    player.ForcedJump(bumpHeight);
                }
            }
        }


    }
}
