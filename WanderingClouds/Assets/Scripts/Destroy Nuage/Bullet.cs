using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speed;
    public float timeDisapear;

    private void Update()
    {
        MovementUpdate();
    }

    private void MovementUpdate()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log($"{gameObject.name} Collide {other.gameObject.name}");
        if (other.gameObject.GetComponent<Wall>() != null)
        {
            other.gameObject.GetComponent<Wall>().Disolve(timeDisapear);
        }
        Destroy(this.gameObject);
        
    }

    
}
