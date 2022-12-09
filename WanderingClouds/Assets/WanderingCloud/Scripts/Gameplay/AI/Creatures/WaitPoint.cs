using UnityEngine;

namespace WanderingCloud.Gameplay.AI
{
    public class WaitPoint : MonoBehaviour
    {
        public float waitTime;
        public bool isAvailable = true;

        public void SetAvailable(bool gfbc)
        {
            isAvailable = gfbc;
        }
    }
}
