using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathMarker : MonoBehaviour
{
    Vector3 lastPos;
    float threhold = 2f;

    private void Awake()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        if(Vector3.Distance(lastPos, transform.position) > threhold)
        {
            Instantiate(gameObject, transform.position, transform.rotation, null);
        }
    }
}
