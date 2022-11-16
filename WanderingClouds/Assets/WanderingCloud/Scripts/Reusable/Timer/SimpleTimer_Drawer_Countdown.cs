using TMPro;
using UnityEngine;
using NaughtyAttributes;

public class SimpleTimer_Drawer_Countdown : MonoBehaviour
{
    [SerializeField, Required] private SimpleTimer timer;
    [Space(10)]
    [SerializeField] private string beforeTime;
    [SerializeField] private string afterTime;
    [SerializeField] private TextMeshProUGUI chronoText;

    private void OnEnable() => timer.onValueUpdate.AddListener(Refresh);
    private void OnDisable() => timer.onValueUpdate.RemoveListener(Refresh);

    private void Refresh()
    {
        if (chronoText is not null) chronoText.text = beforeTime + string.Format("{0:0}", timer.TimeLeft) + afterTime;
    }

}
