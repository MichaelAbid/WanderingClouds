using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WanderingCloud.Gameplay
{
    public class CloudSourceZone : MonoBehaviour
    {

        [SerializeField] private Object sourceObjectPrefab;
        [SerializeField] private GameObject targetObjectPrefab;

        public float timeBeforeRespawn = 60;
        private float timer;
        // Start is called before the first frame update
        void Start()
        {
            timer = timeBeforeRespawn;
        }

        // Update is called once per frame
        void Update()
        {
            if (targetObjectPrefab == null)
            { 
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    RespawnSource();
                }
            }
        }

        [Button("Force Respawn")]
        public void RespawnSource()
        {
            targetObjectPrefab = (GameObject)Instantiate(sourceObjectPrefab, transform.position, Quaternion.identity);
            timer = timeBeforeRespawn;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
#endif

    }
}