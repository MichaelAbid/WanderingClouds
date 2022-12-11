using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WanderingCloud.Controller;
using WanderingCloud.UI;

namespace WanderingCloud
{
    public class MenuAction : MonoBehaviour
    {


        #region Loading Level

        List<string> loadedScene = new List<string>();

        public void LoadScene(MenuItem menu)
        {
            
            MenuManager.Instance.ShowMenu("LOADING_SCENE");
            SceneManager.LoadScene(menu.GetProperties<string>("level_name"));
            loadedScene.Add(menu.GetProperties<string>("level_name"));
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
            var syncmenu = SceneManager.UnloadSceneAsync("MainMenu");
            while (!syncmenu.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
            MenuManager.Instance.ShowMenu("IN_GAME");
        }

        IEnumerator WaitForSceneLoad(List<AsyncOperation> async)
        {
            bool done = false;
            while (!done)
            {
                done = true;
                foreach (var item in async)
                {
                    if (!item.isDone)
                    {
                        done = false;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            foreach (var item in FindObjectsOfType<PlayerController>())
            {
                item.ChangePawn();
            }
            var syncmenu =SceneManager.UnloadSceneAsync("MainMenu");
            while (!syncmenu.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
            MenuManager.Instance.ShowMenu("IN_GAME");
        }

        public void LoadMultipleScene(MenuItem menu)
        {
            MenuManager.Instance.ShowMenu("LOADING_SCENE");
            List<AsyncOperation> asyncs = new List<AsyncOperation>();
            bool i = true;
            foreach (var item in menu.GetProperties<List<string>>("levels_name"))
            {
                loadedScene.Add(item);
                if (i)
                {
                    var async = SceneManager.LoadSceneAsync(item, LoadSceneMode.Additive);
                    asyncs.Add(async);
                    i = false;
                }
                else
                {
                    var async = SceneManager.LoadSceneAsync(item, LoadSceneMode.Additive);
                    asyncs.Add(async);
                }
                
            }
            StartCoroutine(WaitForSceneLoad(asyncs));
        }
        #endregion


        #region Pause Game
        public bool paused;
        public void Pause()
        {
            if (MenuManager.Instance.selectedMenuId == "IN_GAME")
            {
                paused = true;
                MenuManager.Instance.ShowMenu("PAUSE_MENU");
            }
        }

        public void Resume()
        {
            if (MenuManager.Instance.selectedMenuId == "PAUSE_MENU")
            {
                paused = false;
                MenuManager.Instance.ShowMenu("IN_GAME");
            }
        }

        #endregion



        #region Return to Menu

        public void ReturnToMainMenu()
        {
            paused = false;
            var async = SceneManager.LoadSceneAsync("MainMenu");
            StartCoroutine(WaitForReturnToMainMenuLoad(async));

        }
        IEnumerator WaitForReturnToMainMenuLoad(AsyncOperation async)
        {
            while (!async.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
            MenuManager.Instance.ShowMenu("MAIN_MENU");
        }

        #endregion


        public void QuitApp()
        {
            Application.Quit();
        }


        public void ShowMainMenuAfterStart(float time)
        {
            StartCoroutine(WaitShowMainMenuLoad(time));
        }

        IEnumerator WaitShowMainMenuLoad(float time)
        {
            float t = 0;
            while (t<time)
            {
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
            MenuManager.Instance.ShowMenu("MAIN_MENU");
        }


        public void FadeOutStartMenu(string id)
        {
            StartCoroutine(FadeOutStartMenuCoroutine(id,2));
        }

        IEnumerator FadeOutStartMenuCoroutine(string id,float time)
        {
            float t = 0;
            while (t < time)
            {
                
                float ratio = (1 - (t/time));
                foreach (var item in MenuManager.Instance.GetMenu(id).listOfUiElementInMenu)
                {
                    if (!item.activeSelf)
                    {
                        item.SetActive(true);
                    }
                    Image img = item.GetComponent<Image>();
                    if(img!=null) img.color = new Color(img.color.r, img.color.g, img.color.b, ratio);
                    TextMeshProUGUI text = item.GetComponent<TextMeshProUGUI>();
                    if(text!=null) text.color = new Color(text.color.r, text.color.g, text.color.b, ratio);
                    
                    if(t >= time)
                    {
                        item.SetActive(false);
                    }
                    

                }
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }

        }
    }
}
