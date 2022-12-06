using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WanderingCloud
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);        
        }

    }
}
