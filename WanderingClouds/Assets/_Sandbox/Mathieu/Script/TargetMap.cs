using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMap : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint cp in collision.contacts)
        {
            //Debug.Log("Contact with object" + cp.otherCollider.gameObject.name);

            

            if (cp.otherCollider.gameObject.name == "Projectile")
            {
                Vector3 startOfRay = cp.point - cp.normal;
                Vector3 rayDir = cp.normal;

                Ray ray = new Ray(startOfRay, rayDir);
                RaycastHit hit;

                bool hitit = Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("HeatMapLayer"));

                if (hitit)
                {
                    Debug.Log("Hit Object" + hit.collider.gameObject.name);
                    Debug.Log("Hit Texture coordinates = " + hit.textureCoord.x + "," + hit.textureCoord.y);
                }

                Debug.Log(collision.gameObject.name + " is destroy");
                Destroy(cp.otherCollider.gameObject);
            }
        }
    }
}
