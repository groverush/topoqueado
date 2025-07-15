using UnityEngine;
using System.Collections;

public class HammerPowerUps : MonoBehaviour
{
    [SerializeField] private float doubleHitDuration = 5f;

    private bool doubleHitActive = false;
    private Coroutine powerUpTimer;
    public event System.Action OnDoubleHitEnd;


    public void ActivateDoubleHit ()
    {
        Debug.Log("Activating Double Hit Power-Up");
        if (powerUpTimer != null)
            StopCoroutine(powerUpTimer);

        doubleHitActive = true;
        powerUpTimer = StartCoroutine(DoubleHitCountdown());
    }

    public bool IsDoubleHitActive ()
    {
        return doubleHitActive;
    }

    public GameObject GetSecondHole ( HoleNavigation holeNavigation )
    {
        return holeNavigation.GetRandomHoleExcluding(holeNavigation.CurrentHole);
    }
    public float GetRemainingDoubleHitTime ()
    {
        if (doubleHitActive && powerUpTimer != null)
        {
            return remainingTime;
        }
        return 0f;
    }

    private float remainingTime;

    private IEnumerator DoubleHitCountdown ()
    {
        remainingTime = doubleHitDuration;
        while (remainingTime > 0f)
        {
            Debug.Log($"DoubleHit activo: {remainingTime:F1} segundos restantes");
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }
        OnDoubleHitEnd?.Invoke();
        doubleHitActive = false;
    }

}
