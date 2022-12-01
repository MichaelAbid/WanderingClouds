using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class SimpleTimer : MonoBehaviour
{
    [Foldout("State"), SerializeField] private bool isPaused;

    [Foldout("Parameter"),SerializeField] private float duration = 5f;
    [ProgressBar("TimeLeft","duration", EColor.Red)] 
    [Foldout("Parameter"),SerializeField] private float timeLeft = 5f;

    public float Duration => duration;
    public float TimeLeft => timeLeft;
    public bool IsPaused => isPaused;

    [Foldout("Event")] public UnityEvent onValueUpdate;
    [Foldout("Event")] public UnityEvent onTimerEnd;
    
    private void OnEnable()
    {
        timeLeft = duration;
    }
    private void OnDisable()
    {
        timeLeft = 0;
    }
    private void Update()
    {
        if(isPaused) return;
        if(timeLeft <= 0) return;

        timeLeft -= Time.deltaTime;
        onValueUpdate?.Invoke();
        
        if(timeLeft <= 0)
        {
            timeLeft = 0;
            onValueUpdate?.Invoke();
            onTimerEnd?.Invoke();
        }
    }

    [Button()] public void Pause() => isPaused = true;
    [Button()]public void UnPause() => isPaused = false;
    [Button()]public void TogglePause() => isPaused = !isPaused;
    
    [Button()] public void ResetTimer()
    {
        timeLeft = duration;
        onValueUpdate?.Invoke();
    }
    public void SetDuration(float dur)
    {
        duration = dur;
        timeLeft = duration;

        onValueUpdate?.Invoke();
    }

}
