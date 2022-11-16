using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WanderingCloud.Controller;
using WanderingCloud.Gameplay;

public class CloudExploder : MonoBehaviour
{


    public List<CloudSource> BouletteList = new List<CloudSource>();

    public Player playerComponent;

    internal bool ExplodeNearest()
    {
        if (BouletteList.Count > 0)
        {
            CloudSource nearest = BouletteList[0];
            for (int i = 1; i < BouletteList.Count; i++)
            {
                if (Vector3.Distance(BouletteList[i].transform.position, transform.position) < Vector3.Distance(nearest.transform.position, transform.position))
                {
                    nearest = BouletteList[i];
                }
            }
            if (nearest.cgUrle != null)
            {
                nearest.cgUrle.BouletteList.Remove(nearest);
            }
            if (nearest.cgGiro != null)
            {
                nearest.cgGiro.BouletteList.Remove(nearest);
            }
            return true;
        }
        return false;
    }
    
    private void SourceUiManager()
    {
        if (BouletteList.Count > 0)
        {
            CloudSource nearest = BouletteList[0];
            for (int i = 1; i < BouletteList.Count; i++)
            {
                BouletteList[i].UnShowGrabUI(playerComponent.isGyro);
                if (Vector3.Distance(BouletteList[i].transform.position, transform.position) < Vector3.Distance(nearest.transform.position, transform.position))
                {
                    nearest = BouletteList[i];
                }
            }
            nearest.ShowGrabUI(playerComponent.isGyro);
        }
    }

}
}
