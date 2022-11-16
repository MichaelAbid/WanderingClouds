using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGrabber : MonoBehaviour
{


    public List<CloudBoulette> BouletteList = new List<CloudBoulette>();



    private void Start()
    {
        

    }

    private void Update()
    {
        BouletteUiManager();
    }

    private void BouletteUiManager()
    {
        if (BouletteList.Count > 0)
        {
            CloudBoulette nearest = BouletteList[0];
            for (int i = 1; i < BouletteList.Count; i++)
            {
                if (Vector3.Distance(BouletteList[i].transform.position,transform.position)< Vector3.Distance(nearest.transform.position, transform.position))
                {
                    nearest = BouletteList[i];
                }
            }
            nearest.ShowGrabUI();
        }
    }

}
