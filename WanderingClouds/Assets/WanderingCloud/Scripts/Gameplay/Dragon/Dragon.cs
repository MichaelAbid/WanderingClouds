using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WanderingCloud
{
    public class Dragon : MonoBehaviour
    {

        public GameObject objectToBurn;
        

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void BurnTarget()
        {
            if(objectToBurn != null)
            {
                Destroy(objectToBurn);
            }
        }
    }
}
