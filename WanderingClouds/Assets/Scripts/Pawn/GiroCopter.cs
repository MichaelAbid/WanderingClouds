using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiroCopter : CopterBase
{

    //Rigidbody
    public Rigidbody rigbody;

    // Floating
    public float speedToClimb = 10;

    // SwitchMode
    public bool isFloating;
    public float timeBeforeAutomaticSwitch = 10;

    //Grabed
    public bool grabed;
    public float ropeHeight;

    // Cooldown
    private bool canSwitchAir = true;
    public float timeBeforeSwitchingMode = 1;
    

    public override void NorthButtonInput()
    {
        // Get out Air/ Get In
        if (canSwitchAir)
        {
            isFloating = !isFloating;
            rigbody.useGravity = !isFloating;
            if (isFloating)
            {
                StartCoroutine(TimerBeforeForcedToDropDown());
            }
            StartCoroutine(SwitchTimer());
        }
    }

    IEnumerator TimerBeforeForcedToDropDown()
    {
        float time = 0;
        while (time < timeBeforeAutomaticSwitch && isFloating)
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
        isFloating = false;
        rigbody.useGravity = !isFloating;

    }


    IEnumerator SwitchTimer()
    {
        canSwitchAir = false;
        yield return new WaitForSeconds(timeBeforeSwitchingMode);
        canSwitchAir = true;
    }

    protected override void MovementUpdate()
    {
        if (isFloating)
        {
            if (!grabed )
            {
                if(Physics.Raycast(transform.position, Vector3.down, 10))
                {
                    transform.position+=Vector3.up * Time.deltaTime * speedToClimb;
                }
                else
                {
                    base.MovementUpdate();
                }
            }
            else
            {
                if (Physics.Raycast(transform.position, Vector3.down, ropeHeight-0.1f))
                {
                    transform.position += Vector3.up * Time.deltaTime * speedToClimb;
                }
                else if (transform.position.y - ropeHeight >0)
                {
                    transform.position += Vector3.down * Time.deltaTime * speedToClimb;
                }
                else
                {
                    base.MovementUpdate();
                }
            }
        }
        else
        {
            
            base.MovementUpdate();
        }
    }



}

