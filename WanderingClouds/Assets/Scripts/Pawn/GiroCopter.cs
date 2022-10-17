using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public UrleCopter urle;

    // Cooldown
    private bool canSwitchAir = true;
    public float timeBeforeSwitchingMode = 1;

    [ReadOnly]
    [ResizableTextArea]
    public string debug;

    private void Start()
    {
        urle = GameObject.FindGameObjectWithTag("Urle").GetComponent<UrleCopter>();
    }

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
        float distance = Vector2.Distance(new Vector2(urle.transform.position.x, urle.transform.position.z), new Vector2(this.transform.position.x, this.transform.position.z));
        Vector2 directionToUrle = (new Vector2(urle.transform.position.x, urle.transform.position.z) - new Vector2(this.transform.position.x, this.transform.position.z)).normalized;
        float giroHeightCalc = Mathf.Sqrt(Mathf.Pow(ropeHeight, 2) - Mathf.Pow(distance, 2));
        debug = $"Rope Height : {ropeHeight} | Distance Between Urle & Giro : {distance} | Giro suposed Height With current Rope Height : {giroHeightCalc}";

        if (isFloating)
        {
            if (grabed )
            {

                // Movement X/Z
                // Check if We are in Urle Rop Distance 

                if(distance > ropeHeight)
                {
                    transform.position += new Vector3(directionToUrle.x, 0, directionToUrle.y) * Time.deltaTime * pawnSpeed;

                }


                // Height Adjustement
                if((transform.position.y - urle.transform.position.y) < giroHeightCalc - 0.25f)
                {
                    transform.position += Vector3.up * Time.deltaTime * pawnSpeed;
                }   
                else if ((transform.position.y - urle.transform.position.y) > giroHeightCalc + 0.25f)
                {
                    transform.position += Vector3.down * Time.deltaTime * pawnSpeed;
                }
                
            }
            else
            {
                if (Physics.Raycast(transform.position, Vector3.down, 10))
                {
                    transform.position += Vector3.up * Time.deltaTime * speedToClimb;
                }
                else
                {
                    base.MovementUpdate();
                }
            }
        }
        else
        {
            if (grabed)
            {
                if (pawnCurMovement != Vector2.zero)
                {
                    Vector3 newPos = transform.position + (pivotX.transform.forward * pawnCurMovement.y * pawnSpeed * Time.deltaTime) + (pivotX.transform.right * pawnCurMovement.x * pawnSpeed * Time.deltaTime);
                    if (Vector3.Distance(newPos, urle.transform.position) <= ropeHeight)
                    {
                        transform.position = newPos;
                    }

                    visual.transform.rotation = Quaternion.Euler(0, pivotX.transform.rotation.eulerAngles.y, 0);
                }
            }
            else
            {
                base.MovementUpdate();
            }
        }
    }



}

