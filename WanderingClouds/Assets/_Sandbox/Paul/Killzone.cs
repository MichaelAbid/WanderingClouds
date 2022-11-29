using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
    [SerializeField] private Transform resetTransform;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            collision.transform.parent.position = resetTransform.position;
    }
}
