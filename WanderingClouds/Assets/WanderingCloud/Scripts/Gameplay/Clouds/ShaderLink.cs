using UnityEngine;

namespace WanderingCloud
{
    public class ShaderLink : MonoBehaviour
    {
        [Header("reference"), SerializeField] protected Renderer render;
        [Header("Internal"), SerializeField] protected MaterialPropertyBlock propBlock;

        private void Awake()
        {
            if(render is null)render = GetComponent<Renderer>();
        }

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

        public void UpdateProperty(string key, float value) => UpdateProperty(render, new (string, float)[1] { new(key, value) });    
        public void UpdateProperty(string key, Vector3 value) => UpdateProperty(render, new (string, Vector3)[1] { new(key, value) });    
        public void UpdateProperty(string key, int value) => UpdateProperty(render, new (string, int)[1] { new(key, value) });    
        public void UpdateProperty(string key, float[] value) => UpdateProperty(render, new (string, float[])[1] {new (key, value)});    
        public void UpdateProperty(string key, Color value) => UpdateProperty(render, new (string, Color)[1] {new (key, value)});    
        public void UpdateProperty(Renderer render, (string key, int value)[] shadersData)
        {
            GetData(render, ref propBlock);
            foreach (var shaderData in shadersData)
            {
                propBlock.SetInt(shaderData.key, shaderData.value);
            }
            SendData(render, propBlock);
        }
        public void UpdateProperty(Renderer render, (string key, Color value)[] shadersData)
        {
            GetData(render, ref propBlock);
            foreach (var shaderData in shadersData)
            {
                propBlock.SetColor(shaderData.key, shaderData.value);
            }
            SendData(render, propBlock);
        }
        public void UpdateProperty(Renderer render, (string key, float value)[] shadersData)
        {
            GetData(render, ref propBlock);
            foreach (var shaderData in shadersData)
            {
                propBlock.SetFloat(shaderData.key, shaderData.value);
            }
            SendData(render, propBlock);
        }
        public void UpdateProperty(Renderer render, (string key, float[] value)[] shadersData)
        {
            GetData(render, ref propBlock);
            foreach (var shaderData in shadersData)
            {
                propBlock.SetFloatArray(shaderData.key, shaderData.value);
            }
            SendData(render, propBlock);
        }
        public void UpdateProperty(Renderer render, (string key, Vector3 value)[] shadersData)
        {
            GetData(render, ref propBlock);
            foreach (var shaderData in shadersData)
            {
                propBlock.SetVector(shaderData.key, shaderData.value);
            }
            SendData(render, propBlock);
        }
    }
}