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
            
            Player player = other.GetComponentInParent<Player>();
            if (player != null && !player.isGrounded)
            {
                Debug.Log("Bumped");
                player.Jump(bumpHeight);
            }
        }


    }
}
