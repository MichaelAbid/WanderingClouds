using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WanderingCloud.Controller
{
    public class PlayerController : MonoBehaviour
    {
        private int ControllerNumber;
        public Pawn cPawn;
        private int pawnIndex = 0;
        public List<Pawn> listOfAllPawn;

        private void Start()
        {
            ControllerNumber = FindObjectsOfType<PlayerController>().Length -1;
            ControllerJoin();
            listOfAllPawn = FindObjectsOfType<Pawn>().ToList();
            pawnIndex = -1;
            ChangePawn();

        }

        public void ChangePawn()
        {
            pawnIndex = pawnIndex + 1 < listOfAllPawn.Count ? pawnIndex + 1 : 0;
            int i = 0;
            foreach (Pawn pawn in listOfAllPawn)
            {
                if (!pawn.controlled && i >= pawnIndex)
                {
                    if (cPawn != null)
                    {
                        cPawn.controlled = false;
                        cPawn.PlayerDisconnect(ControllerNumber);
                    }

                    pawn.controlled = true;
                    this.cPawn = pawn;
                    cPawn.PlayerConnect(ControllerNumber);
                    GetComponent<PlayerInput>().camera = cPawn.Camera;
                    break;
                }

                i++;
            }
        }

        public void ControllerJoin()
        {
            Debug.Log($"Controller Joined on number {ControllerNumber}");

        }

        public void LeftJoyStick(InputAction.CallbackContext callback)
        {
            if (cPawn != null)
            {
                cPawn.MovementInput(callback.ReadValue<Vector2>());
            }
        }

        public void RightJoyStick(InputAction.CallbackContext callback)
        {
            if (cPawn != null)
            {
                cPawn.CameraMovementInput(callback.ReadValue<Vector2>());
            }
        }


        public void ButtonEst(InputAction.CallbackContext callback)
        {

            if (cPawn != null)
            {
                if (callback.performed)
                {
                    cPawn.EstButtonInput();
                }

                if (callback.canceled)
                {
                    cPawn.EstButtonInputReleased();
                }
            }
        }

        public void ButtonWest(InputAction.CallbackContext callback)
        {
            if (cPawn != null)
            {
                if (callback.performed)
                {
                    cPawn.WestButtonInput();
                }

                if (callback.canceled)
                {
                    cPawn.WestButtonInputReleased();
                }
            }
        }

        public void ButtonSouth(InputAction.CallbackContext callback)
        {

            if (cPawn != null)
            {
                if (callback.performed)
                {
                    cPawn.SouthButtonInput();
                }

                if (callback.canceled)
                {
                    cPawn.SouthButtonInputReleased();
                }
            }
        }

        public void ButtonNorth(InputAction.CallbackContext callback)
        {
            if (cPawn != null)
            {
                if (callback.performed)
                {
                    cPawn.NorthButtonInput();
                }

                if (callback.canceled)
                {
                    cPawn.NorthButtonInputReleased();
                }
            }
        }

        public void ButtonSelect(InputAction.CallbackContext callback)
        {
            if (callback.performed)
            {
                ChangePawn();
            }
        }



        public void RightTrigger(InputAction.CallbackContext callback)
        {
            if (cPawn != null)
            {
                if (callback.performed)
                {
                    cPawn.RightTriggerInput();
                }

                if (callback.canceled)
                {
                    cPawn.RightTriggerInputReleased();
                }
            }
        }

        public void LeftTrigger(InputAction.CallbackContext callback)
        {
            if (cPawn != null)
            {
                if (callback.performed)
                {
                    cPawn.LeftTriggerInput();
                }

                if (callback.canceled)
                {
                    cPawn.LeftTriggerInputReleased();
                }
            }
        }


        public void RightBumper(InputAction.CallbackContext callback)
        {
            if (cPawn != null)
            {
                if (callback.performed)
                {
                    cPawn.RightBumperInput();
                }

                if (callback.canceled)
                {
                    cPawn.RightBumperInputReleased();
                }
            }
        }

        public void LeftBumper(InputAction.CallbackContext callback)
        {
            if (cPawn != null)
            {
                if (callback.performed)
                {
                    cPawn.LeftBumperInput();
                }

                if (callback.canceled)
                {
                    cPawn.LeftBumperInputReleased();
                }
            }
        }


    }
}
