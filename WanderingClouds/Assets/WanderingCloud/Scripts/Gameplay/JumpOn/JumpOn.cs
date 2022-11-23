using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WanderingCloud.Controller;

namespace WanderingCloud.Gameplay
{
    public class JumpOn : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                if (!player.isGrounded)
                {
                    player.Jump();
                }
            }
        }


    }
}
