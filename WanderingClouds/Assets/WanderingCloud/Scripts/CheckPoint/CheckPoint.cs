using System.Collections.Generic;
using System.Collections;
using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEditor;
using WanderingCloud.Controller;

[Serializable]
public class PawnTimer
{
    public Pawn pawn;
    public float timer;
    public bool alreadyPassed;
}

public class CheckPoint : MonoBehaviour
{
    public int startingParticipant
    {
        get
        {
            if (isStartingPoint) return pawnTimers.Count;
            return previousCheckpoint.startingParticipant;
        }
    }
    public bool isStartingPoint {
        get
        {
            return previousCheckpoint == null;
        }
    }
    public bool isEndingPoint
    {
        get
        {
            return nextCheckpoint == null;
        }
    }
    public List<PawnTimer> pawnTimers;
    public CheckPoint nextCheckpoint;
    public CheckPoint previousCheckpoint;
    public float timer = 0;
    public float currentTimer
    {
        get
        {
            if(isStartingPoint) return timer;
            return previousCheckpoint.currentTimer;
        }
    }
    public bool chronoStarted = false;
    private void Update()
    {
        if (chronoStarted && isStartingPoint)
        {
            timer += Time.deltaTime;
        }
    }
    [Button]
    public void StartChrono()
    {
        if (isStartingPoint) { 
            timer = 0;
            chronoStarted = true;
            foreach (PawnTimer timer in pawnTimers) {
                timer.alreadyPassed = true;
            }
        }
        else
        {
            previousCheckpoint.StartChrono();
        }
    }
    public void StopChrono()
    {
        if (isStartingPoint)
        {
            chronoStarted = false;
        }
        else
        {
            previousCheckpoint.StopChrono();
        }
    }
#if UNITY_EDITOR
    [Button]
    public void CreateNewcheckPoint()
    {
        CheckPoint tempnextCheckpoint = GameObject.Instantiate(gameObject,transform.position,transform.rotation).GetComponent<CheckPoint>();
        if (!isEndingPoint)
        {
            nextCheckpoint.previousCheckpoint = tempnextCheckpoint;
        }
        nextCheckpoint = tempnextCheckpoint;
        nextCheckpoint.previousCheckpoint = this;

        Selection.activeGameObject = nextCheckpoint.gameObject;

    }
#endif
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (isStartingPoint)
        {
            Gizmos.color = new Color(0,1,0,0.5f);
            GUI.color = Color.green;
            Gizmos.DrawSphere(transform.position, 1);
        }else if (isEndingPoint)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            GUI.color = Color.red;
            Gizmos.DrawSphere(transform.position, 1);
        }
        else
        {
            Gizmos.color = new Color(1, 1, 0, 0.5f);
            GUI.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 1);
        }


        if (!isEndingPoint)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(this.transform.position, nextCheckpoint.transform.position);
        }

        if (pawnTimers.Count > 0)
        {
            int i = 0;
            foreach (PawnTimer item in pawnTimers)
            {
                
                Handles.Label(transform.position+(Vector3.up*2) +(Vector3.up*i),$"{item.pawn.name} : {item.timer}");
                i++;
            }
        }


    }
#endif
    private void OnTriggerEnter(Collider other)
    {
        Pawn pawn = other.gameObject.GetComponentInParent<Pawn>();
        if (pawnTimers.Count > 0)
        {
            foreach (PawnTimer item in pawnTimers)
            {
                if(item.pawn == pawn)
                {
                    return;
                }
            }
        }
        PawnTimer pt = new PawnTimer();
        pt.pawn = pawn;
        pt.alreadyPassed = true;
        pawnTimers.Add(pt);
        pt.timer = currentTimer;
        if (isEndingPoint)
        {
            if(startingParticipant == pawnTimers.Count)
            {
                StopChrono();
            }
        }
        

    }
}
