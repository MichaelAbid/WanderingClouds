using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WanderingCloud
{

    public enum PropertiesType
    {
        FLOAT,
        STRING,
        INTEGER,
        BOOLEAN,
        LIST
    }

    [Serializable]
    public class MenuItemData
    {


        public string name;
        public PropertiesType type;
        [ShowIf("type", PropertiesType.STRING)][AllowNesting] public string StringValue;
        [ShowIf("type", PropertiesType.BOOLEAN)][AllowNesting] public bool BoolValue;
        [ShowIf("type", PropertiesType.INTEGER)][AllowNesting] public int IntegerValue;
        [ShowIf("type", PropertiesType.FLOAT)][AllowNesting] public float FloatValue;
        [ShowIf("type", PropertiesType.LIST)][AllowNesting] public List<string> ListValue;

        public string GetStringValue()
        {
            return StringValue;
        }
        public bool GetBoolValue()
        {
            return BoolValue;
        }
        public int GetIntegerValue()
        {
            return IntegerValue;
        }
        public float GetFloatValue()
        {
            return FloatValue;
        }
        public List<string> GetListValue()
        {
            return ListValue;
        }


    }

    [System.Serializable]
    public class MenuItemEvent : UnityEvent<MenuItem>
    {
    }


    public class MenuItem : MonoBehaviour
    {

        public bool isActive = false;

        public Image backgroundRef;
        public Sprite backgroundUnactive;
        public Sprite backgroundActive;

        public Text textRef;
        public TextMeshProUGUI textMeshProRef;
        public Color textColorUnactive;
        public Color textColorActive;


        public List<MenuItemData> listProperties;



        public void AddProperties<T>(string name, T variable)
        {
            MenuItemData mid = new MenuItemData();
            mid.name = name;
            if (variable is string)
            {
                mid.StringValue = variable as string;
            }
            if (variable is bool)
            {
                mid.BoolValue = (variable as bool? != null) ? (bool)(variable as bool?) : mid.BoolValue;
            }
            if (variable is int)
            {
                mid.IntegerValue = (variable as int? != null) ? (int)(variable as int?) : mid.IntegerValue;
            }
            if (variable is float)
            {
                mid.FloatValue = (variable as float? != null) ? (float)(variable as float?) : mid.FloatValue;
            }
            listProperties.Add(mid);
        }

        public T GetProperties<T>(string name)
        {
            foreach (MenuItemData item in listProperties)
            {
                if (item.name == name)
                {
                    switch (item.type)
                    {
                        case PropertiesType.STRING:
                            return (T)Convert.ChangeType(item.StringValue, typeof(T));
                        case PropertiesType.BOOLEAN:
                            return (T)Convert.ChangeType(item.BoolValue, typeof(T));
                        case PropertiesType.INTEGER:
                            return (T)Convert.ChangeType(item.IntegerValue, typeof(T));
                        case PropertiesType.FLOAT:
                            return (T)Convert.ChangeType(item.FloatValue, typeof(T));
                        case PropertiesType.LIST:
                            return (T)Convert.ChangeType(item.ListValue, typeof(T));
                        default:
                            break;
                    }
                }
            }
            return (T)Convert.ChangeType(null, typeof(T)); ;
        }


        public MenuItemEvent events;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void setText(string text)
        {
            if (textRef != null)
            {
                textRef.text = text;
            }
            if (textMeshProRef != null)
            {
                textMeshProRef.text = text;
            }
        }

        internal void Active(bool v)
        {
            isActive = v;
            backgroundRef.sprite = isActive ? backgroundActive : backgroundUnactive;
            if (textRef != null)
            {
                textRef.color = isActive ? textColorActive : textColorUnactive;
            }
            if (textMeshProRef != null)
            {
                textMeshProRef.color = isActive ? textColorActive : textColorUnactive;
            }

        }

        public void Press()
        {
            events.Invoke(this);
        }
    }
}