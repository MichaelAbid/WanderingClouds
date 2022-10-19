using NaughtyAttributes;
using UnityEngine.Events;
using UnityEngine;
using UnityEditor;

public class ObjectAngleViewV2 : MonoBehaviour
{
    public Pawn[] players;

    [Range(0f, 1f)] public float threshold = 0.05f;

    [Header("Info")] public Vector3 pointOfView;
    [MinMaxSlider(-50, 50)] public Vector2 minMaxDist = new Vector2(1, 10);
    public bool done;

    public Vector3 alignDir
    {
        get => (transform.position - pointOfView).normalized;
    }
    public float viewPointDist
    {
        get => (transform.position - pointOfView).magnitude;
    }

    [Header("Info")] public Transform[] composite;
    public Vector2[] screenPos;
    public TransformData[] shufleData;
    public TransformData[] clearData;

    [Button()] public void SetUp()
    {
        int childCount = transform.childCount;
        composite = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            composite[i] = transform.GetChild(i);
        }

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

    [Button()] public void SaveView()
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

    [Button("Reset")]
    private void Start()
    {
        done = false;
        ToShufle();
    }
    
    public bool PawnInAlignement(Pawn player)
    {
        Vector2[] actualScreenPos = new Vector2[screenPos.Length];
        for (int i = 0; i < composite.Length; i++)
        {
            actualScreenPos[i] = player.Camera.GetScreenRatioPosOfGameObject(composite[i].position) * player.Camera.rect.size;
        }

        float distance = 0f;
        var refPos = player.Camera.transform.position + player.Camera.transform.forward * 3f;
        for (int i = 0; i < actualScreenPos.Length - 1; i++)
        {
            distance += Vector2.Distance(screenPos[i] - screenPos[i + 1], actualScreenPos[i] - actualScreenPos[i + 1]);

            Debug.DrawLine(refPos + (Vector3)actualScreenPos[i], refPos + (Vector3)actualScreenPos[i+1], Color.red);
            Debug.DrawLine(refPos + (Vector3)screenPos[i], refPos + (Vector3)screenPos[i + 1], Color.green);

        }

        return distance < threshold;
    }
    void Update()
    {
        foreach (var player in players)
        {
            if (PawnInAlignement(player))
            {
                if (done) return;
                done = true;
                for (int i = 0; i < composite.Length; i++)
                {
                    composite[i].transform.DoTransform(clearData[i], 0.05f);

                }            
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < screenPos.Length - 1; i++)
        {
            Debug.DrawLine(transform.position + (Vector3)screenPos[i], transform.position + (Vector3)screenPos[i + 1], Color.green);
        }

        Debug.DrawLine(transform.position, transform.position + alignDir * viewPointDist, Color.blue);
    }
}