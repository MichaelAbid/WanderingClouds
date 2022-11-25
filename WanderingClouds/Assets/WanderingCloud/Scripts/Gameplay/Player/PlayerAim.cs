using UnityEngine;
using NaughtyAttributes;
using Cinemachine;

namespace WanderingCloud.Controller
{
    public class PlayerAim : MonoBehaviour
    {
        [SerializeField] private bool isAiming = false;

        [field: SerializeField, Foldout("References")] public PlayerBrain player;

        //On Payer Aim Input, affect -> Camera, Caracter Speed, Character Animations, Crosshair activation

        //Crosshair and autoaim 

        public void BeginAim()
        {
            player.CinemachineAim.m_YAxis = player.CinemachineExplo.m_YAxis;
            player.CinemachineAim.m_XAxis = player.CinemachineExplo.m_XAxis;

            //Aim VCam takes priority
            player.CinemachineAim.Priority = player.CinemachineExplo.Priority + 1;
        }

        public void EndAim()
        {
            player.CinemachineExplo.m_YAxis = player.CinemachineAim.m_YAxis;
            player.CinemachineExplo.m_XAxis = player.CinemachineAim.m_XAxis;

            //Aim VCam takes priority
            player.CinemachineAim.Priority = player.CinemachineExplo.Priority - 1;
        }
    }
}