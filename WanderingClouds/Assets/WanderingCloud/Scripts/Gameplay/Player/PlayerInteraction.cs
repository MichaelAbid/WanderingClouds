using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using WanderingCloud.Controller;

namespace WanderingCloud
{
    public class PlayerInteraction : MonoBehaviour
    {
        public  UnityEvent<Pawn> onInteractBegin;
        public  UnityEvent<Pawn> onInteractEnd;

        public void InteractBegin(Pawn pawn)
        {
            onInteractBegin?.Invoke(pawn);
        }
        public void InteractEnd(Pawn pawn)
        {
            onInteractEnd?.Invoke(pawn);
        }
    }
}