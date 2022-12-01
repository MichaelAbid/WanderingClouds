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
        public List<GameObject> wanderingPoints = new List<GameObject>();
        int wanderingPointCount = 0;

        protected override void WanderingBehavior()
        {
            agent.SetDestination(wanderingPoints[wanderingPointCount].transform.position);

            currentState = AI_STATE.AI_IDLE;
        }

        protected override AI_STATE ChangeFromIdle()
        {
            if (Vector3.Distance(wanderingPoints[wanderingPointCount].transform.position, transform.position) <= 2 )
            {
                wanderingPointCount++;
                if (wanderingPointCount == wanderingPoints.Count)
                {
                    wanderingPointCount = 0;
                }
                return AI_STATE.AI_WANDERING;
            }
            else
            {
                return base.ChangeFromIdle();
            }

        }
    }
}
