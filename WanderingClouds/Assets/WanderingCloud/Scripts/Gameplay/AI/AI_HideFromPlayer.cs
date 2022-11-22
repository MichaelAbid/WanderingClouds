using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using WanderingCloud.Controller;

namespace WanderingCloud.Gameplay.AI
{
    public class AI_HideFromPlayer : AI_Base
    {

        [Foldout("Idle")][SerializeField] private float idleTime;
        [Foldout("Idle")][SerializeField][ReadOnly] private float idleTimer;
        [Foldout("Wandering")][SerializeField] private float WanderingTime;
        [Foldout("Wandering")][SerializeField] private float WanderingRadius;
        [Foldout("Wandering")][SerializeField][ReadOnly] private Vector3 WanderingTarget;
        [Foldout("Wandering")][SerializeField][ReadOnly] private float WanderingTimer;
        [Foldout("Hide")][SerializeField] private float distanceFromPlayerToHide;
        [Foldout("Hide")][SerializeField] private float distancePlayerMin = 10;
        [Foldout("Hide")][SerializeField] private float distancePlayerMed = 20;
        [Foldout("Stun")][SerializeField] private float stunDuration;

        protected override AI_STATE ChangeFromIdle()
        {
            if (CheckIfPlayerNear())
            {
                WanderingTarget = GetSafestHideOut();
                idleTimer = 0;
                return AI_STATE.AI_HIDE;
            }
            if (idleTimer >= idleTime)
            if (idleTimer >= idleTime)
            {
                idleTimer = 0;
                WanderingTarget = GetRandomPositionOnNavMesh();
                return AI_STATE.AI_WANDERING;
            }
            return base.ChangeFromIdle();
        }

        protected override AI_STATE ChangeFromWandering()
        {
            if (CheckIfPlayerNear())
            {
                WanderingTarget = GetSafestHideOut();
                WanderingTimer = 0;
                return AI_STATE.AI_HIDE;
            }
            if (WanderingTimer >= WanderingTime)
            {
                WanderingTimer = 0;
                return AI_STATE.AI_IDLE;
            }
            return base.ChangeFromWandering();
        }

        protected override AI_STATE ChangeFromHide()
        {
            if (!CheckIfPlayerNear())
            {
                return AI_STATE.AI_IDLE;
            }

            return base.ChangeFromHide();
        }

        protected bool CheckIfPlayerNear()
        {
            foreach (Player player in playerList)
            {
                if (Vector3.Distance(transform.position, player.transform.position)<=distanceFromPlayerToHide)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void TimerUpdate()
        {
            base.TimerUpdate();
            if(currentState == AI_STATE.AI_IDLE)
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

        protected override void WanderingBehavior()
        {
            
            if (Vector3.Distance(WanderingTarget, transform.position) <= 0.5)
            {
                WanderingTarget = GetRandomPositionOnNavMesh();
            }
            agent.SetDestination(WanderingTarget);

            base.WanderingBehavior();
        }

        protected Vector3 GetRandomPositionOnNavMesh()
        {
            Vector3 randomDirection = Random.insideUnitSphere * WanderingRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, WanderingRadius, 1);
            return hit.position;
        }

        protected override void HideBehavior()
        {
            if (Vector3.Distance(WanderingTarget, transform.position) <= 0.5)
            {
                WanderingTarget = GetSafestHideOut();
            }
            agent.SetDestination(WanderingTarget);

            base.HideBehavior();
        }

        protected Vector3 GetSafestHideOut()
        {
            HideOut hide = null;
            int maxPoint = 0;

            foreach (HideOut hideOut in hideOuts)
            {
                int point = 0;
                float distanceFromIA = Vector3.Distance(hideOut.transform.position, transform.position);
                if (distanceFromIA <= 30)
                {
                    point +=2;
                }else 
                if (distanceFromIA <= 50)
                {
                    point += 3;
                }
                else
                {
                    point += 1;
                }

                foreach (Player player in playerList)
                {
                    float distanceFromPlayer = Vector3.Distance(hideOut.transform.position, player.transform.position);
                    if (distanceFromPlayer <= distancePlayerMin)
                    {
                        point += 1;
                        if (Vector3.Dot((hideOut.transform.position - transform.position).normalized, (player.transform.position - transform.position).normalized) >= 0f)
                        {
                            point = 0;
                            break;
                        }
                    }
                    else
                    if (distanceFromPlayer <= distancePlayerMed)
                    {
                        point += 2;
                        if (Vector3.Dot((hideOut.transform.position - transform.position).normalized, (player.transform.position - transform.position).normalized) >= 0.5f)
                        {
                            point = 0;
                            break;
                        }

                    }
                    else
                    {
                        point += 3;
                    }

                }

                if(point> maxPoint)
                {
                    hide = hideOut;
                    maxPoint = point;
                }
                else if (point == maxPoint && point!=0)
                {
                    if(distanceFromIA <= Vector3.Distance(hide.transform.position, transform.position))
                    {
                        hide = hideOut;
                    }
                }

            }
            if(hide != null) return hide.transform.position;
            return transform.position;
        }

        private void OnDrawGizmos()
        {

            Handles.color = Color.green;
            Handles.CircleHandleCap(0, transform.position, Quaternion.LookRotation(transform.up), distanceFromPlayerToHide, EventType.Repaint);

            foreach (HideOut hideOut in hideOuts)
            {
                int point = 0;
                float distanceFromIA = Vector3.Distance(hideOut.transform.position, transform.position);
                if (distanceFromIA <= 30)
                {
                    point += 2;
                }
                else
                if (distanceFromIA <= 50)
                {
                    point += 3;
                }
                else
                {
                    point += 1;
                }

                foreach (Player player in playerList)
                {
                    float distanceFromPlayer = Vector3.Distance(hideOut.transform.position, player.transform.position);
                    if (distanceFromPlayer <= distancePlayerMin)
                    {
                        point += 1;
                        if (Vector3.Dot((hideOut.transform.position - transform.position).normalized, (player.transform.position - transform.position).normalized) >= 0f)
                        {
                            point = 0;
                            break;
                        }
                    }
                    else
                    if (distanceFromPlayer <= distancePlayerMed)
                    {
                        point += 2;
                        if (Vector3.Dot((hideOut.transform.position - transform.position).normalized, (player.transform.position - transform.position).normalized) >= 0.5f)
                        {
                            point = 0;
                            break;
                        }

                    }
                    else
                    {
                        point += 3;
                    }
                    

                }

                Handles.color = Color.yellow;
                Handles.Label(hideOut.transform.position + Vector3.up, $"Point : {point}");
                



            }
        }
    }

    
}
