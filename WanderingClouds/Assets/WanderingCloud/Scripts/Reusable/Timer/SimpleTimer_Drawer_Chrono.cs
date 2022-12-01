using System;
using TMPro;
using UnityEngine;
using NaughtyAttributes;

public class SimpleTimer_Drawer_Chrono : MonoBehaviour
{
    [SerializeField, Required] private SimpleTimer timer;
    [Space(10)] 
    [SerializeField] private TextMeshProUGUI textSlotMinutes;
    [SerializeField] private TextMeshProUGUI textSlotSeconds;

    private void OnEnable() => timer.onValueUpdate.AddListener(Refresh);
    private void OnDisable() => timer.onValueUpdate.RemoveListener(Refresh);
    
    public void Refresh()
    {
        if (textSlotSeconds is not null)
            textSlotSeconds.text = string.Format("{0:00}", Mathf.FloorToInt(timer.TimeLeft % 60));
        if (textSlotMinutes is not null)
            textSlotMinutes.text = string.Format("{0:00}", Mathf.FloorToInt(timer.TimeLeft / 60));
    }
}