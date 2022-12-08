using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WanderingCloud.Gameplay
{
     public abstract class  Source : MonoBehaviour
    {
        public abstract bool Feed(CloudType cType);
    }
}
