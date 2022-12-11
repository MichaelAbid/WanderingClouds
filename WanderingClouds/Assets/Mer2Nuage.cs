using System;
using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace WanderingCloud
{
    public class Mer2Nuage : MonoBehaviour
    {
        [SerializeField] private Transform[] players;
        [SerializeField] private Vector3 returnPosition;

        private void Update()
        {
            foreach (var player in players)
            {
                if (player.position.y < transform.position.y)
                {
                    player.position = returnPosition;
                }
            }
            
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            returnPosition = Handles.DoPositionHandle(returnPosition, Quaternion.identity);
        }
        #endif
    }
}