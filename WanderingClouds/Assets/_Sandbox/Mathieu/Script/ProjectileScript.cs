using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [Range (1,10)]
    public int multiplicator;

    void Update()
    {
        transform.position = transform.position + new Vector3(0f, -1f, 0f) * Time.deltaTime * multiplicator;
    }
}
