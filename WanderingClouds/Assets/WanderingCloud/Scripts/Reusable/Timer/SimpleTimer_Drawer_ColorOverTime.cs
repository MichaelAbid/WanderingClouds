using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using NaughtyAttributes;

public class SimpleTimer_Drawer_ColorOverTime : MonoBehaviour
{
    [SerializeField, Required] private SimpleTimer timer;
    [Space(10)]
    [SerializeField] private Image[] images;
    [SerializeField] private ColorTime[] paliers = 
        new []{new ColorTime(0,Color.red), new ColorTime(3,Color.yellow), new ColorTime(5,Color.green)};
    
    private void OnEnable() => timer.onValueUpdate.AddListener(Refresh);
    private void OnDisable() => timer.onValueUpdate.RemoveListener(Refresh);

    private void Refresh()
    {
        var order = paliers.OrderBy(x => x.time).ToArray();
        for (int i = 0; i < paliers.Length; i++)
        {
            if (timer.TimeLeft > order[i].time) continue;

            foreach (var image in images)
            {
                image.color = order[i].color;
            }
            return;
        }
        
        //No Color Below time found
        foreach (var image in images)
        {
            image.color = Color.black;
        }
    }
    
    [System.Serializable]
    private class ColorTime
    {
        public float time;
        public Color color;

        public ColorTime(float time, Color color)
        {
            this.time = time;
            this.color = color;
        }
    }
}
