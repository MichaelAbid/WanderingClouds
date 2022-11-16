using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudBoulette : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    


    private void OnTriggerEnter(Collider other)
    {
        CloudGrabber cg = other.GetComponentInParent<CloudGrabber>();
        if (cg != null)
        {
            cg.BouletteList.Add(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CloudGrabber cg = other.GetComponentInParent<CloudGrabber>();
        if (cg != null)
        {
            cg.BouletteList.Remove(this);
        }
    }

    public void ShowGrabUI()
    {
        
    }
    
    public void UnShowGrabUI()
    {

    }
}
