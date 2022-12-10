using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using WanderingCloud.Controller;

namespace WanderingCloud.Gameplay.AI
{
    public class AI_BaseCreature : AI_Base
    {
        public bool isAgent = true;

        [Header("ListPoint"), SerializeField] private List<WaitPoint> wanderingPoints = new List<WaitPoint>();
        [SerializeField, ReadOnly] private int wanderingPointsIndex = 0;
        [SerializeField, ReadOnly] private float waitingTime = 0;


        private void Awake()
        {
            agent.transform.position = wanderingPoints[wanderingPointsIndex].transform.position;
        }

        override protected void IdleBehavior()
        {
            if (isAgent) return;

            transform.Translate((wanderingPoints[wanderingPointsIndex].transform.position - transform.position).normalized * agent.speed * Time.deltaTime);
        }
        override protected void WanderingBehavior()
        {
            waitingTime -= Time.deltaTime;
        }

        override protected AI_STATE ChangeFromIdle()
        {
            if (Vector3.Distance(wanderingPoints[wanderingPointsIndex].transform.position, transform.position) <= agent.radius + 0.5f)
            {
                waitingTime = wanderingPoints[wanderingPointsIndex].waitTime;
                return AI_STATE.AI_WANDERING;
            }
            else
            {
                return AI_STATE.AI_IDLE;
            }

        }

        override protected AI_STATE ChangeFromWandering()
        {
            if(waitingTime < 0)
            {
                do
                {
                    wanderingPointsIndex++;
                    wanderingPointsIndex %= wanderingPoints.Count;
                }
                while (!wanderingPoints[wanderingPointsIndex].isAvailable);

                if(isAgent) agent.SetDestination(wanderingPoints[wanderingPointsIndex].transform.position);
                return AI_STATE.AI_IDLE;
            }

            return AI_STATE.AI_WANDERING;
        }
        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < wanderingPoints.Count; i++)
            {
                Debug.DrawLine(wanderingPoints[i].transform.position, wanderingPoints[(i + 1) % wanderingPoints.Count].transform.position, Color.red);
            }
        }

    }
}
