using NaughtyAttributes;
using UnityEngine;

namespace WanderingCloud.Controller
{
// A Script that can be controlled by a Controller
// Class that derived from this one will be able to be controlled as well
    public abstract class Pawn : MonoBehaviour
    {
        [Foldout("Controller")] public bool controlled;
        [Foldout("Controller")] public Camera Camera;
        [Foldout("Controller")] public bool isGyro = true;
        [Foldout("Controller")] public bool isPlayablePawn = true;

        [Foldout("Controller"), Space(10)] public bool allowBodyMovement = true;
        [Foldout("Controller")] public bool allowCameraMovement = true;

        public virtual void PlayerConnect(int playerIndex) { }
        public virtual void PlayerDisconnect(int playerIndex) { }
        
        public virtual void MovementInput(Vector2 input)
        {
        }
        public virtual void CameraMovementInput(Vector2 input)
        {
        }

        #region FaceButton

        public virtual void SouthButtonInput()
        {
        }

        public virtual void SouthButtonInputReleased()
        {
        }

        public virtual void NorthButtonInput()
        {
        }

        public virtual void NorthButtonInputReleased()
        {
        }

        public virtual void EstButtonInput()
        {
        }

        public virtual void EstButtonInputReleased()
        {
        }

        public virtual void WestButtonInput()
        {
        }

        public virtual void WestButtonInputReleased()
        {
        }

        #endregion

        #region Trigger&Bumper

        public virtual void RightTriggerInput()
        {
        }

        public virtual void RightTriggerInputReleased()
        {
        }

        public virtual void LeftTriggerInput()
        {
        }

        public virtual void LeftTriggerInputReleased()
        {
        }


        public virtual void RightBumperInput()
        {
        }

        public virtual void RightBumperInputReleased()
        {
        }

        public virtual void LeftBumperInput()
        {
        }

        public virtual void LeftBumperInputReleased()
        {
        }

        #endregion
    }
}