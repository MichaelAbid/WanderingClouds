using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WanderingCloud
{
    public class CloudMove : MonoBehaviour
    {


        public Vector3 toGo;
        public bool shouldGo;
        public float moveSpeed;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        [Button]
        public void Go()
        {
            shouldGo = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (shouldGo)
            {
                transform.localPosition += (toGo - transform.localPosition ).normalized * Time.deltaTime * moveSpeed;
                if (Vector3.Distance(toGo, transform.localPosition) <= 0.1f)
                {
                    shouldGo = false;
                }
            }
        }
    }
}
