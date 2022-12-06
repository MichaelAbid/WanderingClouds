using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WanderingCloud.UI
{
    [System.Serializable]
    public struct MenuNavigation
    {
        public string id;
        public MenuItem item;
        public string rightItem;
        public string leftItem;
        public string upItem;
        public string downItem;



    }
    public class MenuNavigator : MonoBehaviour
    {
        public List<MenuNavigation> listOfMenuItem;
        public string startingId;
        public MenuNavigation actualItem;

        public float delayBeforeNextNavigate = 1;
        private float durationNavigate = 0;



        [Button]
        private void Start()
        {
            ChangeActiveItem(startingId);
        }

        public void Build()
        {
            ChangeActiveItem(startingId);
        }


        public void ChangeActiveItem(string id)
        {
            if (id != "")
            {
                foreach (MenuNavigation item in listOfMenuItem)
                {
                    if (item.id == id)
                    {
                        item.item.Active(true);
                        actualItem = item;
                    }
                    else
                    {
                        if (item.id != "")
                        {
                            item.item.Active(false);
                        }
                    }
                }
            }
        }

        [Button("Simulate Left Input")]
        public void Left()
        {
            ChangeActiveItem(actualItem.leftItem);
        }
        [Button("Simulate Right Input")]
        public void Right()
        {
            ChangeActiveItem(actualItem.rightItem);
        }
        [Button("Simulate Up Input")]
        public void Up()
        {
            ChangeActiveItem(actualItem.upItem);
        }
        [Button("Simulate Down Input")]
        public void Down()
        {
            ChangeActiveItem(actualItem.downItem);
        }

        private void Update()
        {
            if (durationNavigate < delayBeforeNextNavigate)
            {
                durationNavigate += Time.deltaTime;
            }
        }

        public void Navigate(Vector2 direction)
        {

            if (durationNavigate >= delayBeforeNextNavigate)
            {

                int x = Mathf.RoundToInt(direction.x);
                int y = Mathf.RoundToInt(direction.y);

                if (x > 0)
                {
                    Right();
                    durationNavigate = 0;
                    return;
                }
                else if (x < 0)
                {
                    Left();
                    durationNavigate = 0;
                    return;
                }

                if (y > 0)
                {
                    Up();
                    durationNavigate = 0;
                    return;
                }
                else if (y < 0)
                {
                    Down();
                    durationNavigate = 0;
                    return;
                }
            }


        }
        [Button]
        public void Press()
        {
            actualItem.item.Press();

        }
    }
}
