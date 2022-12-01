using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WanderingCloud.Gameplay.AI
{
    public class AI_MovingAround : AI_Base
    {
        [Foldout("Idle")][SerializeField] private float idleTime;
        [Foldout("Idle")][SerializeField][ReadOnly] private float idleTimer;
        [Foldout("Wandering")][SerializeField] private float WanderingTime;
        [Foldout("Wandering")][SerializeField] private float WanderingRadius;
        [Foldout("Wandering")][SerializeField][ReadOnly] private Vector3 WanderingTarget;
        [Foldout("Wandering")][SerializeField][ReadOnly] private float WanderingTimer;

        protected override AI_STATE ChangeFromIdle()
        {
                if (idleTimer >= idleTime)
                {
                    idleTimer = 0;
                    WanderingTarget = GetRandomPositionOnNavMesh(WanderingRadius);
                    return AI_STATE.AI_WANDERING;
                }
            return base.ChangeFromIdle();
        }

        protected override AI_STATE ChangeFromWandering()
        {
            if (WanderingTimer >= WanderingTime)
            {
                WanderingTimer = 0;
                agent.SetDestination(transform.position);
                return AI_STATE.AI_IDLE;
            }
            return base.ChangeFromWandering();
        }


        protected override void WanderingBehavior()
        {
            if (Vector3.Distance(WanderingTarget, transform.position) <= 0.5)
            {
                WanderingTarget = GetRandomPositionOnNavMesh(WanderingRadius);
            }
            agent.SetDestination(WanderingTarget);

            base.WanderingBehavior();
        }
        protected override void TimerUpdate()
        {
            base.TimerUpdate();
            if (currentState == AI_STATE.AI_IDLE)
            {
                if (idleTimer < idleTime)
                {
                    idleTimer += Time.deltaTime;
                }
            }
            if (currentState == AI_STATE.AI_WANDERING)
            {
                if (WanderingTimer < WanderingTime)
                {
                    WanderingTimer += Time.deltaTime;
                }
            }
        }
    }
}
