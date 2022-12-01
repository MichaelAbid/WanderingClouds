using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WanderingCloud.Controller;
using WanderingCloud.Gameplay.AI;

namespace WanderingCloud.Gameplay
{
    public class AiGraber : MonoBehaviour
    {

        public PlayerBrain playerRef;

        public AI_Base aiGrabed;
        public Transform grabSocketPosition;

        public bool Grab()
        {
            var nearObject = Physics.OverlapSphere(playerRef.Avatar.position, 3f);
            if (nearObject.Length == 0) return false;
            var nearTarget = nearObject.
                Where(x => x.GetComponent<AI_Base>() && x.GetComponent<AI_Base>().isGrabbable == true).
                Select(x => x.GetComponent<AI_Base>()).ToArray();
            if (nearTarget.Length == 0) return false;
            var target = nearTarget.OrderBy(x => Vector3.Distance(x.transform.position, playerRef.Avatar.position)).First();
            aiGrabed = target;
            target.isAiActive = false;
            target.isGrabbable = false;
            target.isGrabbed = true;
            target.collider.enabled = false;
            target.agent.enabled = false;
            target.rigidBody.useGravity = false;
            return true;
        }
        public void UnGrab()
        {
            aiGrabed.isAiActive = true;
            aiGrabed.isGrabbable = true;
            aiGrabed.isGrabbed = false;
            aiGrabed.collider.enabled = true;
            aiGrabed.agent.enabled = true;
            aiGrabed.rigidBody.useGravity = true;
            aiGrabed.currentState = AI_STATE.AI_IDLE;
            aiGrabed = null;
        }

        private void Update()
        {

            if (aiGrabed != null)
            {
                aiGrabed.transform.position = grabSocketPosition.position;
                
            }

        }

    }

    

}
