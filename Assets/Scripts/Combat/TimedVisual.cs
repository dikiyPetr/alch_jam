using System.Collections;
using UnityEngine;

// Визуальный компонент с управляемым временем жизни.
// StartVisual() — включить, через Duration секунд — выключить автоматически.
// StopVisual() — выключить немедленно.
public class TimedVisual : MonoBehaviour
{
    public float Duration { get; private set; }

    Coroutine _coroutine;

    public void StartVisual(float duration)
    {
        Duration = duration;
        StartVisual();
    }

    public virtual void StartVisual()
    {
        gameObject.SetActive(true);
        if (Duration > 0f)
            _coroutine = StartCoroutine(AutoStop());
    }

    public virtual void StopVisual()
    {
        if (_coroutine != null) { StopCoroutine(_coroutine); _coroutine = null; }
        gameObject.SetActive(false);
    }

    IEnumerator AutoStop()
    {
        yield return new WaitForSeconds(Duration);
        StopVisual();
    }
}
