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
                nearest.UnShowExplodeUI(false);
                nearest.cgUrle.BouletteList.Remove(nearest);
            }
            if (nearest.cgGiro != null)
            {
                nearest.UnShowExplodeUI(true);
                nearest.cgGiro.BouletteList.Remove(nearest);
            }
            nearest.ExplodeSource();
            return true;
        }
        return false;
    }

    private void Update()
    {
        SourceUiManager();
    }

    private void SourceUiManager()
    {
        if (BouletteList.Count > 0)
        {
            CloudSource nearest = null;
            for (int i = 1; i < BouletteList.Count; i++)
            {
                if (BouletteList[i].isActive) { 
                    BouletteList[i].UnShowExplodeUI(playerComponent.isGyro);
                    if (nearest == null)
                    {
                        nearest = BouletteList[i];
                    }
                    else if (Vector3.Distance(BouletteList[i].transform.position, transform.position) < Vector3.Distance(nearest.transform.position, transform.position))
                    {
                        nearest = BouletteList[i];
                    }
                }
            }
            if (nearest != null)
            {
                nearest.ShowExplodeUI(playerComponent.isGyro);
            }
        }
    }

}

