using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WanderingCloud.Controller;

namespace WanderingCloud.Gameplay
{
    public class JumpOn : MonoBehaviour
    {
        public float bumpHeight = 10;

        private void OnTriggerEnter(Collider other)
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
