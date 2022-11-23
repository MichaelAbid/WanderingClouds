using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace WanderingCloud.Gameplay.AI
{
    public class AdaptativeCarving : MonoBehaviour
    {

        public NavMeshObstacle obstacle;

        public AnimationCurve carveSizeByDistance;

        
        public List<AI_Base> aiList;

        [SerializeField]
        [ReadOnly]
        private float debugDistance;

        // Start is called before the first frame update
        void Start()
        {
            aiList = FindObjectsOfType<AI_Base>().ToList();
        }

        // Update is called once per frame
        void Update()
        {
            CheckForAI();
        }


        private void CheckForAI()
        {
            float clostest = -1;
            foreach (AI_Base ai in aiList)
            {
                float distance = Vector3.Distance(transform.position, ai.transform.position);
                if (clostest < 0 || distance< clostest)
                {
                    clostest = distance;
                }
            }
            debugDistance = clostest;
            if (clostest>=0 && clostest <= carveSizeByDistance.keys[carveSizeByDistance.length-1].time)
            {
                obstacle.radius = carveSizeByDistance.Evaluate(clostest);
            }

        }

    }
}
