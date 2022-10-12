using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismaticObject : MonoBehaviour
{
    public GameObject urle;
    public GameObject giro;
    public string debug;

    private void Start()
    {
        if(urle == null)
        {
            urle = GameObject.FindGameObjectWithTag("Urle");
        }

        if (giro == null)
        {
            giro = GameObject.FindGameObjectWithTag("Giro");
        }
    }


    private void Update()
    {
        if (urle != null && giro != null)
        {
            
            CheckIfShouldBeVisible();
        }
    }

    private void CheckIfShouldBeVisible()
    {
        bool visible = false;
        if (giro.GetComponent<Player>().prismed)
        {
            float dot1 = Vector3.Dot((urle.GetComponent<Pawn>().Camera.transform.forward).normalized, (giro.transform.position - urle.GetComponent<Pawn>().Camera.transform.position).normalized);
            float dot2 = Vector3.Dot((giro.transform.position - urle.GetComponent<Pawn>().Camera.transform.position).normalized, (transform.position - giro.transform.position).normalized);
            debug = $"dot 1 : {dot1} | dot 2 : {dot2} ";
            if (dot1 > 0.85f && dot2 > 0.97f)
            {
                visible = true;
            }
        }
        GetComponent<MeshRenderer>().enabled = visible;

    }
}
