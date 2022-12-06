using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WanderingCloud
{
    public class MenuAction : MonoBehaviour
    {

        public void LoadScene(MenuItem menu)
        {

            SceneManager.LoadScene(menu.GetProperties<string>("level_name"));
        }


    }
}
