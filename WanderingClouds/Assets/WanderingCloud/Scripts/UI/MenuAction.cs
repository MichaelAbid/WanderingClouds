using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using WanderingCloud.Controller;
using WanderingCloud.UI;

namespace WanderingCloud
{
    public class MenuAction : MonoBehaviour
    {



        public void LoadScene(MenuItem menu)
        {

            MenuManager.Instance.ShowMenu("IN_GAME");
            SceneManager.LoadScene(menu.GetProperties<string>("level_name"));

            var async = SceneManager.LoadSceneAsync(menu.GetProperties<string>("level_name"));
            StartCoroutine(WaitForSceneLoad(async));
            
        }

        IEnumerator WaitForSceneLoad(AsyncOperation async)
        {
            while (!async.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
            foreach (var item in FindObjectsOfType<PlayerController>())
            {
                item.ChangePawn();
            }
        }

        public void QuitApp()
        {
            Application.Quit();
        }


    }
}
