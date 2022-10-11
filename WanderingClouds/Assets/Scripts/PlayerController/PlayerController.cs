using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int ControllerNumber;

    private void Start()
    {
        ControllerNumber = FindObjectsOfType<PlayerController>().Length;
        ControllerJoin();
    }

    public void ControllerJoin()
    {
        Debug.Log($"Controller Joined on number {ControllerNumber}");
        
    }
}
