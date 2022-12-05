using UnityEngine;
using UnityEngine.Animations.Rigging;
using WanderingCloud.Controller;

namespace WanderingCloud
{
    public class HeadTargeting : MonoBehaviour
    {
        [SerializeField] private PlayerBrain player;
        [SerializeField] private PlayerAim aim;

        private void Update()
        {
            var camDir = player.Camera.transform.forward;
            if (aim.isAiming)
            {
                transform.position = player.Avatar.position + player.Avatar.forward + Vector3.up * (camDir.y * 2f);
            }
            else
            {
                transform.position = player.Avatar.position + camDir * 4f;
            }
        }
    }
}