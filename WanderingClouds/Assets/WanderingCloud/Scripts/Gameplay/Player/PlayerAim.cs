using UnityEngine;

namespace WanderingCloud.Controller
{
    public class PlayerAim : MonoBehaviour
    {
        [HideInInspector] private PlayerBrain player;

        [SerializeField] private bool isAiming = false;

    }
}