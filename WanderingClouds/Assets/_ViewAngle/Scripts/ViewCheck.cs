using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class ViewCheck : MonoBehaviour
{
    public AngleViewObject[] viewAngles;
    public MultipleViewObject doubleViewAngles;

    public bool allSynch;
    public bool AllSynch
    {
        get => allSynch;
        private set
        {
            if (allSynch != value)
            {
                if (value) onFullAlign?.Invoke();
                else onNoneAlign?.Invoke();
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
        if (doubleViewAngles is not null && !doubleViewAngles.isAlign)
        {
            allValid = false;
        }

        AllSynch = allValid;
    }
}
