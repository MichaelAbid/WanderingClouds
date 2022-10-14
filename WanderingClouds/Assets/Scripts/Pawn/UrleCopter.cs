using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrleCopter : CopterBase
{


    private bool shouldScaleUpRope;
    private bool shouldScaleDownRope;

    public float ropeHeight = 5;
    public float maxHeight = 10;
    public float minHeight = 2;

    public GiroCopter giro;


    private void Start()
    {
        giro = GameObject.FindGameObjectWithTag("Giro").GetComponent<GiroCopter>();
    }

    public override void NorthButtonInput()
    {
        
        giro.grabed = !giro.grabed;

    }


    protected override void Update()
    {
        base.Update();

        RopeUpdate();
    }

    private void RopeUpdate()
    {
        if (giro!=null && giro.grabed)
        {
            if (shouldScaleUpRope)
            {
                ropeHeight += Time.deltaTime*5;
            }
            if (shouldScaleDownRope)
            {
                ropeHeight -= Time.deltaTime * 5;
            }
            ropeHeight = Mathf.Clamp(ropeHeight, minHeight, maxHeight);
            giro.ropeHeight = ropeHeight;

            Debug.DrawLine(transform.position, giro.transform.position,Color.red);

        }
        
    }

    public override void EstButtonInput()
    {
        shouldScaleUpRope = true;
    }

    public override void EstButtonInputReleased()
    {
        shouldScaleUpRope = false;
    }

    public override void WestButtonInput()
    {
        shouldScaleDownRope = true;
    }

    public override void WestButtonInputReleased()
    {
        shouldScaleDownRope = false;
    }


}
