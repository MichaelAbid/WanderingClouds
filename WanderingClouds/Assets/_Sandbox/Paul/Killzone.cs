using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WanderingCloud.Controller;

public class Killzone : MonoBehaviour
{
    [SerializeField] private Transform resetTransform;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision);
        if (collision.transform.parent.TryGetComponent<PlayerBrain>(out PlayerBrain player))
            collision.transform.parent.position = resetTransform.position;
    }
}
