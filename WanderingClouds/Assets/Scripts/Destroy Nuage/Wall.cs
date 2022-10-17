using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public MeshRenderer visual;
    public Collider colider;

    public bool disolved = false;

    public void Disolve(float time)
    {
        StartCoroutine(DisolveOverTime(time));
    }

    IEnumerator DisolveOverTime(float time)
    {
        disolved = true;
        colider.enabled = false;
        Color col = visual.material.color;
        col.a = 0.5f;
        visual.material.color = col;
        yield return new WaitForSeconds(time);
        disolved = false;
        col.a = 1;
        visual.material.color = col;
        colider.enabled = true;
    }
}
