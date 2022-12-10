using Codice.CM.ConfigureHelper;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using WanderingCloud.Controller;

namespace WanderingCloud.UI
{
    [System.Serializable]
    public class Menu
    {
        /// <summary>
        /// The menu identifier
        /// </summary>
        public string menuID;
        /// <summary>
        /// The list of UI element in menu
        /// </summary>
        [Header("Drag UI Element which belong to this menu")]
        public MenuNavigator navigator;
        public List<GameObject> listOfUiElementInMenu;
        public bool consumeInput;

        
    }
    public class MenuManager : SingletonMonoBehaviour<MenuManager>
    {



        /// <summary>The canvas</summary>
        public GameObject canvas;

        /// <summary>
        /// The starting menu identifier
        /// </summary>
        [Foldout("Menu Managing")]
        public string startingMenuId = "START_MENU";
        /// <summary>
        /// The selected menu identifier
        /// </summary>
        [Foldout("Menu Managing")]
        public string selectedMenuId;

        [Foldout("Menu Managing")]
        public Menu selectedMenu;

        /// <summary>
        /// The list of all menu
        /// </summary>
        [Foldout("Menu Managing")]
        [NonReorderable]
        public List<Menu> listOfAllMenu;



        private void Awake()
        {
            MakeSingleton();
        }

        /// <summary>
        /// Adds a new menu.
        /// </summary>
        [Button]
        public void AddANewMenu()
        {
            listOfAllMenu.Add(new Menu());
        }
        /// <summary>
        /// Removes the last menu.
        /// </summary>
        [Button]
        public void RemoveLastMenu()
        {
            if (listOfAllMenu.Count > 0)
            {
                listOfAllMenu.RemoveAt(listOfAllMenu.Count - 1);
            }
        }
        /// <summary>
        /// Hides all menu.
        /// </summary>
        [Button]
        public void HideAllMenu()
        {
            foreach (Menu item in listOfAllMenu)
            {
                if (item.listOfUiElementInMenu.Count > 0)
                {
                    foreach (GameObject obj in item.listOfUiElementInMenu)
                    {
                        obj.SetActive(false);
                    }
                }
            }
        }








        /// <summary>
        /// Shows all menu.
        /// </summary>
        [Button]
        public void ShowAllMenu()
        {
            foreach (Menu item in listOfAllMenu)
            {
                foreach (GameObject obj in item.listOfUiElementInMenu)
                {
                    obj.SetActive(true);
                }
            }
        }
        /// <summary>
        /// Shows the menu.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void ShowMenu(string id)
        {
            HideAllMenu();

            foreach (Menu item in listOfAllMenu)
            {
                if (item.menuID == id)
                {
                    selectedMenu = item;
                    foreach (GameObject obj in item.listOfUiElementInMenu)
                    {
                        obj.SetActive(true);
                    }
                }
            }
            selectedMenuId = id;

            

        }



        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            ShowMenu(startingMenuId);
        }
        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {

        }


        [Foldout("Menu Managing")]
        [Header("Menu Preview (Dev Only)")]
        public string selectAMenuToPreview;


        [Button]
        public void PreviewMenu()
        {
            ShowMenu(selectAMenuToPreview);
        }


        public void Press()
        {
            if (selectedMenu.navigator != null)
            {
                selectedMenu.navigator.Press();
            }
        }

        public void Navigate(Vector2 direction)
        {
            if (selectedMenu.navigator != null)
            {
                selectedMenu.navigator.Navigate(direction);
            }
        }






    }
}
