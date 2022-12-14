using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
        public bool isShow = false;
        public UnityEvent onShowEvents;
        public UnityEvent onHideEvents;

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
        public void HideAllMenu(string id ="")
        {
            foreach (Menu item in listOfAllMenu)
            {
                if ( item.isShow && item.listOfUiElementInMenu.Count > 0 && id!=item.menuID)
                {
                    item.isShow = false;
                    foreach (GameObject obj in item.listOfUiElementInMenu)
                    {
                        obj.SetActive(false);
                    }
                    item.onHideEvents.Invoke();
                }

            }
        }



        public Menu GetMenu(string id)
        {
            foreach (Menu item in listOfAllMenu)
            {
                if(item.menuID == id)
                {
                    return item;
                }
            }
            return null;
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
            HideAllMenu(id);

            foreach (Menu item in listOfAllMenu)
            {
                if (item.menuID == id && !item.isShow)
                {
                    item.isShow = true;
                    selectedMenu = item;
                    foreach (GameObject obj in item.listOfUiElementInMenu)
                    {
                        obj.SetActive(true);
                    }
                    item.onShowEvents.Invoke();
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
