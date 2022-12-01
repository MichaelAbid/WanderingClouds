using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class SimpleTimer_Drawer_ImageFill : MonoBehaviour
{
    [SerializeField, Required] private SimpleTimer timer;
    [Space(10)]
    [SerializeField] Image image;

    private void OnEnable() => timer.onValueUpdate.AddListener(Refresh);
    private void OnDisable() => timer.onValueUpdate.RemoveListener(Refresh);

    private void Refresh()    
    {
        image.fillAmount = timer.TimeLeft / timer.Duration;
    }

}
