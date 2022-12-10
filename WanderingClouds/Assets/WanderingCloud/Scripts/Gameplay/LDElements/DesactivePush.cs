using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WanderingCloud
{
    public class DesactivePush : MonoBehaviour
    {


        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PushPlayers>() != null && !other.GetComponent<PushPlayers>().isIA)
            {
                other.GetComponent<PushPlayers>().shouldPush = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<PushPlayers>() != null && !other.GetComponent<PushPlayers>().isIA)
            {
                other.GetComponent<PushPlayers>().shouldPush = true;
            }
        }
    }
}
