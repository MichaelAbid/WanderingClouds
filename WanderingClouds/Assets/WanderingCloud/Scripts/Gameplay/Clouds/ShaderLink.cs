﻿using UnityEngine;

namespace WanderingCloud
{
    public class ShaderLink : MonoBehaviour
    {
        [Header("reference"), SerializeField] protected Renderer render;
        [Header("Internal"), SerializeField] protected MaterialPropertyBlock propBlock;

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

        public void UpdateProperty(Renderer render, (string key, float value) shaderData) => UpdateProperty(render, new (string, float)[1] { shaderData });    
        public void UpdateProperty(Renderer render, (string key, Vector3 value) shaderData) => UpdateProperty(render, new (string, Vector3)[1] { shaderData });    
        public void UpdateProperty(Renderer render, (string key, int value) shaderData) => UpdateProperty(render, new (string, int)[1] { shaderData });    
        public void UpdateProperty(Renderer render, (string key, int value)[] shadersData)
        {
            GetData(render, ref propBlock);
            foreach (var shaderData in shadersData)
            {
                propBlock.SetInt(shaderData.key, shaderData.value);
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