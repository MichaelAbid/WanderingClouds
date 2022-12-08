using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WanderingCloud.Controller;

namespace WanderingCloud
{
    public class DoubleInteractElement_Drawer : MonoBehaviour
    {
        [SerializeField] public DoubleInteractElement doubleInteract;
        [Space(15)]
        [SerializeField] public Image leftPart;
        [SerializeField] public Image rightPart;

        private void Update()
        {
            leftPart.fillAmount = doubleInteract.urleCharge;
            rightPart.fillAmount = doubleInteract.giroCharge;
        }
    }
}