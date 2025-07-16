using System.Collections;
using UnityEngine;

public class HammerPowerUps : MonoBehaviour
{
    [SerializeField] private float doubleHitDuration = 5f;
    private Coroutine powerUpTimer;
    private float remainingTime;
    private bool doubleHitActive = false;

    public bool IsDoubleHitActive => doubleHitActive;
    public float RemainingTime => remainingTime;

    public event System.Action OnDoubleHitEnd;

    public void ActivateDoubleHit ()
    {
        if (powerUpTimer != null) StopCoroutine(powerUpTimer);
        doubleHitActive = true;
        powerUpTimer = StartCoroutine(DoubleHitCountdown());
    }

    private IEnumerator DoubleHitCountdown ()
    {
        remainingTime = doubleHitDuration;
        while (remainingTime > 0f)
        {
            Debug.Log($"Double Hit activo: {remainingTime:F1}s");
            remainingTime -= 1f;
            yield return new WaitForSeconds(1f);
        }

        doubleHitActive = false;
        OnDoubleHitEnd?.Invoke();
    }
}
