using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WanderingCloud.Controller;

namespace WanderingCloud.Gameplay.AI
{
    public class HideOut : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }



#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1,1,0,0.5f);
            Gizmos.DrawCube(transform.position + (Vector3.up*0.5f), transform.lossyScale);
        }
#endif
    }
}
