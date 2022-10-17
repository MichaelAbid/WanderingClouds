using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System;
using TMPro;

public class WebGLKeyboard : MonoBehaviour, ISelectHandler
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void focusHandleAction(string title, string text, string gameObject);
#endif

    public void ReceiveInputData(string value)
    {
        gameObject.GetComponent<TMP_InputField>().text = value;
    }

    public void OnSelect(BaseEventData data)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        focusHandleAction("Title:", gameObject.GetComponent<TMP_InputField>().text, gameObject.name);
#endif
    }
}