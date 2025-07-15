using UnityEngine;
using System;
using System.Collections;

public class TimerManager : MonoBehaviour
{
    // === Time ===
    [SerializeField] private int timeLimit;
    private int currentTimeRemaining;
    private bool isRunning = false;

    // === Events ===
    public event Action<int> OnTimeUpdated;
    public event Action OnTimerEnd;

    // === Properties ===
    public float CurrentTimeRemaining => currentTimeRemaining;

    public void StartTimer()
    {
        isRunning = true;
        StartCoroutine(CountdownCoroutine(timeLimit));
    }

    private IEnumerator CountdownCoroutine(int countdownValue)
    {
        currentTimeRemaining = countdownValue;

        while (currentTimeRemaining > 0 && isRunning)
        {
            OnTimeUpdated?.Invoke(currentTimeRemaining); // Notify UI
            yield return new WaitForSeconds(1.0f);
            currentTimeRemaining--;
        }

        EndTimer();
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    private void EndTimer()
    {
        isRunning = false;
        OnTimeUpdated?.Invoke(0); // Ensures that UI displays 0
        OnTimerEnd?.Invoke(); // Notify GameManager
    }
}