using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using WanderingCloud.Controller;

namespace WanderingCloud.Gameplay.AI
{
    public class AI_HideFromPlayer : AI_Base
    {
        [Foldout("Ref")][SerializeField] private BumperSource source;

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
        [Foldout("Hide")][SerializeField] private float hideOutMinDist = 10;
        [Foldout("Hide")][SerializeField] private float hideOutMedDist = 40;
        [Foldout("Hide")][SerializeField] private float hideOutMaxDist = 50;
        [Foldout("HideOut")][SerializeField] protected List<HideOut> hideOuts = new List<HideOut>();

        

        [Foldout("Debug")] public bool debugHideOut = false;
        [Foldout("Debug")] public bool debugHideOutPlayers = false;
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
                WanderingTarget = GetRandomPositionOnNavMesh(WanderingRadius);
                return AI_STATE.AI_WANDERING;
            }
            return base.ChangeFromIdle();
        }

        protected override void GetAllRef()
        {
            base.GetAllRef();
            hideOuts = FindObjectsOfType<HideOut>().ToList();

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
            if (source.currentPullet / source.maxPullet <= 0.9)
            {
                if (Vector3.Distance(WanderingTarget, transform.position) <= 0.5)
                {
                    WanderingTarget = GetRandomPositionOnNavMesh(WanderingRadius);
                }
                agent.SetDestination(WanderingTarget);

                base.WanderingBehavior();
            }
            else
            {
                WanderingTarget = transform.position;
            }
        }

        

        protected override void HideBehavior()
        {
            if (Vector3.Distance(WanderingTarget, transform.position) <= 2)
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
                float distanceHideOutFromIA = Vector3.Distance(hideOut.transform.position, transform.position);

                if(distanceHideOutFromIA < hideOutMinDist)
                {
                    point += 1;
                }else if(distanceHideOutFromIA < hideOutMedDist)
                {
                    point += 3;
                }else if(distanceHideOutFromIA < hideOutMaxDist)
                {
                    point += 2;
                }

                if (point != 0)
                {

                    foreach (Player player in playerList)
                    {
                        float distanceHideOutFromPlayer = Vector3.Distance(hideOut.transform.position, player.transform.position);
                        float distanceIAFromPlayer = Vector3.Distance(transform.position, player.transform.position);
                        if (distanceHideOutFromPlayer > distancePlayerMed)
                        {
                            point += 2;
                        }
                        else if (distanceHideOutFromPlayer > distancePlayerMin)
                        {
                            point += 1;
                        }




                    }
                }

                if (point > maxPoint)
                {
                    hide = hideOut;
                    maxPoint = point;
                }
                else if (point == maxPoint && point != 0)
                {
                    if (distanceHideOutFromIA <= Vector3.Distance(hide.transform.position, transform.position))
                    {
                        hide = hideOut;
                    }
                }



            }
            if (hide != null) return hide.transform.position;
            return transform.position;
        }

        private void OnDrawGizmos()
        {

            Handles.color = Color.green;
            Handles.CircleHandleCap(0, transform.position, Quaternion.LookRotation(transform.up), distanceFromPlayerToHide, EventType.Repaint);

            foreach (HideOut hideOut in hideOuts)
            {
                if(hideOut == null)
                {
                    GetAllRef();
                    break;
                }
                int point = 0;
                float distanceHideOutFromIA = Vector3.Distance(hideOut.transform.position, transform.position);
                Gizmos.color = Color.red;
                if (distanceHideOutFromIA < hideOutMinDist)
                {
                    Gizmos.color = Color.blue;
                    point += 1;
                }
                else if (distanceHideOutFromIA < hideOutMedDist)
                {
                    Gizmos.color = Color.green;
                    point += 3;
                }
                else if (distanceHideOutFromIA < hideOutMaxDist)
                {
                    Gizmos.color = Color.yellow;
                    point += 2;
                }
                if (debugHideOut) { 
                    Gizmos.DrawLine(transform.position, hideOut.transform.position);
                    Handles.Label(transform.position + ((hideOut.transform.position - transform.position).normalized * distanceHideOutFromIA / 2) + Vector3.up, $"Distance : {distanceHideOutFromIA}");
                }

                if (point != 0)
                {

                    foreach (Player player in playerList)
                    {
                        if (player == null)
                        {
                            GetAllRef();
                            break;
                        }
                        float distanceHideOutFromPlayer = Vector3.Distance(hideOut.transform.position, player.transform.position);
                        float distanceIAFromPlayer = Vector3.Distance(transform.position, player.transform.position);
                        Gizmos.color = Color.red;
                        if (distanceHideOutFromPlayer > distancePlayerMed)
                        {
                            Gizmos.color = Color.green;
                            point += 2;
                            if (distanceHideOutFromPlayer < distanceHideOutFromIA)
                            {
                                point -= 1;
                            }
                        }
                        else if(distanceHideOutFromPlayer > distancePlayerMin)
                        {
                            Gizmos.color = Color.yellow;
                            point += 1;
                            if (distanceHideOutFromPlayer < distanceHideOutFromIA)
                            {
                                point -= 1;
                            }
                        }
                        else
                        {
                            if (distanceHideOutFromPlayer > distanceHideOutFromIA)
                            {
                                point += 1;
                            }
                        }

                        

                        if (debugHideOutPlayers)
                        {
                            Gizmos.DrawLine(player.transform.position, hideOut.transform.position);
                            Handles.Label(player.transform.position + ((hideOut.transform.position - player.transform.position).normalized * distanceHideOutFromPlayer / 2) + Vector3.up, $"Distance : {distanceHideOutFromPlayer}");
                        }

                    }
                }
                Handles.Label(hideOut.transform.position + Vector3.up, $"Point : {point}");
            }


            NavMeshPath path = agent.path;
            Color startCol = Color.blue;
            Color endCol = Color.red;
            Vector3[] corners = path.corners;
            if (corners.Length > 1)
            {
                Vector3 previous = corners[0];
                float i = 0;
                foreach (Vector3 corr in corners)
                {
                    if (previous != corr)
                    {
                        Gizmos.color = Color.LerpUnclamped(startCol,endCol,i/corners.Length);
                        Gizmos.DrawLine(corr, previous);
                        previous = corr;
                    }
                    i++;
                }
            }


        }
    }


}
