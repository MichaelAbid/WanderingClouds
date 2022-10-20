using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PouffObject_ShaderLink : MonoBehaviour
{
    [Header("reference")] public Renderer render;

    [Header("Parameter")] public float opacity = 1f;

    [Header("Internal")] private MaterialPropertyBlock propBlock;

    private void GetData(Renderer render, ref MaterialPropertyBlock propBlock)
    {
        //permet d'overide les param sans modif le mat ou cr�er d'instance
        propBlock = new MaterialPropertyBlock();
        //Recup Data
        render.GetPropertyBlock(propBlock, 0);
    }

    private void SendData(Renderer render, MaterialPropertyBlock propBlock)
    {
        //Push Data
        render.SetPropertyBlock(propBlock, 0);
    }

    public void UpdateProperty((string key, float value) shaderData) =>
        UpdateProperty(render, propBlock, new (string, float)[1] { shaderData });

    public void UpdateProperty(Renderer render, MaterialPropertyBlock propBlock,
        (string key, float value)[] shadersData)
    {
        GetData(render, ref propBlock);
        foreach (var shaderData in shadersData)
        {
            propBlock.SetFloat(shaderData.key, shaderData.value);
        }

        SendData(render, propBlock);
    }

    void Start()
    {
        UpdateProperty(("_Alpha", opacity));
    }

    void Update()
    {
        UpdateProperty(("_Alpha", opacity));
    }
}