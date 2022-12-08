using System;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using WanderingCloud.Controller;

namespace WanderingCloud
{
    public class BillBoardPawn : MonoBehaviour
    {
        [SerializeField] private Pawn player;

        private void Awake()
        {
            if(GetComponent<Canvas>())GetComponent<Canvas>().worldCamera = player.Camera;
        }

        private void LateUpdate()
        {
            transform.rotation = Quaternion.LookRotation(player.Camera.transform.position - transform.position, player.Camera.transform.up);
        }
    }
}