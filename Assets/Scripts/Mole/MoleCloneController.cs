using System.Collections;
using UnityEngine;

public class MoleCloneController : MonoBehaviour
{
    private bool isAbilityUnlocked = false;
    private bool isVisible = false;

    // === Pop in/out ===
    [SerializeField] private Vector3 popOutOffset = new(0, 0.25f, -0.1f);
    [SerializeField] private float popInDelay;
    [SerializeField] private float popAnimationDuration;
    private Vector3 originalPosition;

    // === Coroutines ===
    private Coroutine currentRoutine;

    // === Properties ===
    public bool IsAbilityUnlocked => isAbilityUnlocked;
    public bool IsVisible => isVisible;

    private void Awake()
    {
        originalPosition = transform.position;
    }

    public void UnlockCloneAbility()
    {
        isAbilityUnlocked = true;
        gameObject.SetActive(false);
        isVisible = false;
    }

    public void ShowAtPosition(Vector3 holePosition)
    {
        if (!isAbilityUnlocked || isVisible) return;

        if (currentRoutine != null) StopCoroutine(currentRoutine);

        gameObject.SetActive(true);
        transform.position = holePosition;

        Vector3 targetPosition = holePosition + popOutOffset;
        currentRoutine = StartCoroutine(MoveToPosition(transform.position, targetPosition, popAnimationDuration));
        isVisible = true;
    }

    public void HideClone()
    {
        if (!isAbilityUnlocked || !isVisible) return;

        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(PopInRoutine());
    }

    public void ForceHideCloneImmediately()
    {
        if (!isAbilityUnlocked) return;

        if (currentRoutine != null) StopCoroutine(currentRoutine);

        gameObject.SetActive(false);
        isVisible = false;
    }

    // Pop in/out animation
    private IEnumerator PopInRoutine()
    {
        yield return new WaitForSeconds(popInDelay);

        StartCoroutine(MoveToPosition(transform.position, originalPosition, popAnimationDuration));

        gameObject.SetActive(false);
        isVisible = false;
        currentRoutine = null;
    }

    private IEnumerator MoveToPosition(Vector3 previous, Vector3 newPosition, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(previous, newPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = newPosition;
        currentRoutine = null;
    }
}