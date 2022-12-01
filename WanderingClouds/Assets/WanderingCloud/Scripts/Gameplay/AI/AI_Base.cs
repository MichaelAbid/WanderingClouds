using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using WanderingCloud.Controller;

namespace WanderingCloud.Gameplay.AI
{
    public enum AI_STATE
    {
        AI_IDLE,
        AI_WANDERING,
        AI_HIDE,
        AI_STUN
    }

    public class AI_Base : MonoBehaviour
    {
        [Foldout("Players")][SerializeField] protected List<Player> playerList = new List<Player>();
        [Foldout("Ref")][SerializeField] public NavMeshAgent agent;
        [Foldout("Ref")][SerializeField] public Rigidbody rigidBody;
         
        [Foldout("State")][SerializeField] public AI_STATE currentState;
        [Foldout("State")][SerializeField] public bool isAiActive = true;
        [Foldout("State")][SerializeField] public bool isGrabbable = true;


        private void Start()
        {
            GetAllRef();
        }

        [Button]
        virtual protected void GetAllRef()
        {
            playerList = FindObjectsOfType<Player>().ToList();
        }

        private void Update()
        {
            if (isAiActive)
            {
                StateUpdate();
                TimerUpdate();
                Behavior();
            }
        }

        virtual protected void TimerUpdate()
        {

        }

        protected void StateUpdate()
        {
            currentState = CheckNewState();
        }

        protected AI_STATE CheckNewState()
        {
            switch (currentState)
            {
                case AI_STATE.AI_IDLE:
                    return ChangeFromIdle();
                case AI_STATE.AI_WANDERING:
                    return ChangeFromWandering();
                case AI_STATE.AI_HIDE:
                    return ChangeFromHide();
                case AI_STATE.AI_STUN:
                    return ChangeFromStun();
                default:
                    return AI_STATE.AI_IDLE;
            }
        }
        virtual protected AI_STATE ChangeFromIdle()
        {
            return AI_STATE.AI_IDLE;
        }
        virtual protected AI_STATE ChangeFromWandering()
        {
            return AI_STATE.AI_WANDERING;
        }
        virtual protected AI_STATE ChangeFromHide()
        {
            return AI_STATE.AI_HIDE;
        }
        virtual protected AI_STATE ChangeFromStun()
        {
            return AI_STATE.AI_STUN;
        }
        protected void Behavior()
        {
            switch (currentState)
            {
                case AI_STATE.AI_IDLE:
                    IdleBehavior();
                    break;
                case AI_STATE.AI_WANDERING:
                    WanderingBehavior();
                    break;
                case AI_STATE.AI_HIDE:
                    HideBehavior();
                    break;
                case AI_STATE.AI_STUN:
                    StunBehavior();
                    break;
                default:
                    break;
            }
        }

        virtual protected void IdleBehavior()
        {

        }

        virtual protected void WanderingBehavior()
        {

        }

        virtual protected void HideBehavior()
        {

        }

        virtual protected void StunBehavior()
        {

        }


        public Vector3 GetRandomPositionOnNavMesh(float radius)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, radius, 1);
            return hit.position;
        }

    }
}
