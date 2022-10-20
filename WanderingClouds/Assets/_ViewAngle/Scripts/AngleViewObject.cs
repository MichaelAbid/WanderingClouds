using NaughtyAttributes;
using UnityEngine.Events;
using UnityEngine;
using UnityEditor;

public class AngleViewObject : MonoBehaviour
{
    public Pawn[] players;

    [Range(0f, 1f)] public float threshold = 0.05f;

    [Header("Info")] public Vector3 pointOfView;
    public Quaternion viewOrientation = Quaternion.identity;

    public Vector3 alignDir
    {
        get => viewOrientation * Vector3.forward;
    }

    public bool isAlign;


    [Header("Elements")] public Transform[] composite;
    public Vector2[] screenPos;
    public TransformData[] shufleData;
    public TransformData[] clearData;

    [Header("Editor")] [MinMaxSlider(1, 50)]
    public Vector2 minMaxDist = new Vector2(1, 10);

    [Button()]
    public void SetUpArraySizes()
    {
        int childCount = composite.Length;
        shufleData = new TransformData[childCount];
        clearData = new TransformData[childCount];
        screenPos = new Vector2[childCount];
    }

    [Button()] public void ShufleElements()
    {
        for (int i = 0; i < composite.Length; i++)
        {
            var newRange = Random.Range(minMaxDist.x, minMaxDist.y);
            var toElement = composite[i].position - pointOfView;

            var factor = newRange / toElement.magnitude;

            composite[i].position = pointOfView + toElement * factor;
            composite[i].localScale *= factor;

            shufleData[i] = composite[i];
        }
    }

    [Button()]  public void SaveView()
    {
        pointOfView = SceneView.lastActiveSceneView.camera.transform.position;

        for (int i = 0; i < composite.Length; i++)
        {
            screenPos[i] = SceneView.lastActiveSceneView.camera.GetScreenRatioPosOfGameObject(composite[i].position);
        }
    }

    [Button()] public void SaveClear()
    {
        for (int i = 0; i < composite.Length; i++)
        {
            clearData[i] = composite[i];
        }
    }

    [Button()] public void ToShufle()
    {
        for (int i = 0; i < composite.Length; i++)
        {
            composite[i].transform.SetTransformData(shufleData[i]);
        }
    }

    [Button()] public void ToClear()
    {
        for (int i = 0; i < composite.Length; i++)
        {
            composite[i].transform.SetTransformData(clearData[i]);
        }
    }

    private void Start()
    {
        ToShufle();
    }

    public bool PawnInAlignement(Pawn player)
    {
        var playerCam = player.Camera.transform;
        Vector2[] actualScreenPos = new Vector2[screenPos.Length];
        for (int i = 0; i < composite.Length; i++)
        {
            var camToComp = composite[i].position - playerCam.position;
            if(!Physics.Raycast(playerCam.position,camToComp.normalized, camToComp.magnitude))continue;
            
            actualScreenPos[i] = player.Camera.GetScreenRatioPosOfGameObject(composite[i].position) * player.Camera.rect.size;
        }

        float distance = 0f;
        var refPos = playerCam.position + playerCam.forward * 3f;
        for (int i = 0; i < actualScreenPos.Length - 1; i++)
        {
            distance += Vector2.Distance(screenPos[i] - screenPos[i + 1], actualScreenPos[i] - actualScreenPos[i + 1]);

            Debug.DrawLine(refPos + (Vector3)actualScreenPos[i], refPos + (Vector3)actualScreenPos[i + 1], Color.red);
            Debug.DrawLine(refPos + (Vector3)screenPos[i], refPos + (Vector3)screenPos[i + 1], Color.green);
        }

        return distance < threshold;
    }

    void Update()
    {
        isAlign = false;
        foreach (var player in players)
        {
            isAlign = isAlign || PawnInAlignement(player);
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < screenPos.Length - 1; i++)
        {
            Debug.DrawLine(transform.position + (Vector3)screenPos[i], transform.position + (Vector3)screenPos[i + 1],
                Color.green);
            Debug.DrawLine(transform.position, transform.position + alignDir, Color.blue);
        }
    }
}