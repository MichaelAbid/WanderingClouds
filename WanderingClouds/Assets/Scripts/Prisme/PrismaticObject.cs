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

        GetComponent<MeshRenderer>().enabled = visible;

    }
}
