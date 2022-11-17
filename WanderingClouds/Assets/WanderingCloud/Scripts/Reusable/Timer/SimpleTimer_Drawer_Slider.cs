using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class SimpleTimer_Drawer_Slider : MonoBehaviour
{
    [SerializeField, Required] private SimpleTimer timer;
    [Space(10)]
    [SerializeField] Slider slider;

    private void OnEnable() => timer.onValueUpdate.AddListener(Refresh);
    private void OnDisable() => timer.onValueUpdate.RemoveListener(Refresh);

    private void Refresh()    {
        slider.maxValue = timer.Duration;
        slider.value = timer.TimeLeft;
    }

}
