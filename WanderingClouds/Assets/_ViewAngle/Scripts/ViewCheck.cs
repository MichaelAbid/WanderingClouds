using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class ViewCheck : MonoBehaviour
{
    public ObjectAngleViewV3[] viewAngles;
    public ObjectMultipleAngleView doubleViewAngles;

    public bool allSynch;
    public bool AllSynch
    {
        get => allSynch;
        private set
        {
            if (allSynch != value)
            {
                if (value)
                {
                    Debug.Log("onFullAlign");
                    onFullAlign?.Invoke();
                }
                else
                {
                    Debug.Log("onNoneAlign");
                    onNoneAlign?.Invoke();
                }
            }
            allSynch = value;
        }
    }

    public UnityEvent onFullAlign;
    public UnityEvent onNoneAlign;
    
    private void Update()
    {
        bool allValid = true;
        for (int i = 0; i < viewAngles.Length; i++)
        {
            if (!viewAngles[i].isAlign)
            {
                allValid = false;
            }
        }
        if (!doubleViewAngles.isAlign)
        {
            allValid = false;
        }

        AllSynch = allValid;
    }
}
