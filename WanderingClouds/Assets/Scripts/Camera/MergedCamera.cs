using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergedCamera : MonoBehaviour
{

    public Pawn urle;
    public Pawn giro;

    public Camera selfCamera;





    public Vector3 testPos;
    [Button]
    public void Test()
    {
        MergeCamera(testPos);
    }


    public void MergeCamera(Vector3 camPos)
    {

        urle.allowCameraMovement = false;
        urle.allowMovement = false;
        giro.allowCameraMovement = false;
        giro.allowMovement = false; 


        StartCoroutine(MoveCameraToPoint(camPos));
    }

    IEnumerator MoveCameraToPoint(Vector3 camPos)
    {


        Vector3 urleStartPos = urle.transform.position;
        Vector3 giroStartPos = giro.transform.position;

        float time = 0;

        while ( time<=1)
        {
            urle.Camera.transform.position = Vector3.Lerp(urleStartPos, camPos, time);
            giro.Camera.transform.position = Vector3.Lerp(giroStartPos, camPos, time);

            time += Time.deltaTime/3;
            yield return new WaitForEndOfFrame();
        }
        urle.Camera.targetDisplay = 1;
        giro.Camera.targetDisplay = 1;
        selfCamera.targetDisplay = 0;



    }


}
