using System.Collections;
using UnityEngine;

public class MoleCloneController : MonoBehaviour
{
    [SerializeField] private Transform moleBase;
    [SerializeField] private float posY;
    [SerializeField] private Vector3 offset;

    private Coroutine currentAnimationRoutine;
    private bool isAbilityUnlocked = false;
    private bool isVisible = false;

    public void UnlockCloneAbility ()
    {
        isAbilityUnlocked = true;
        gameObject.SetActive(false);
        isVisible = false;
    }

    public void ShowAtPosition ( Vector3 targetPosition )
    {
        if (!isAbilityUnlocked || isVisible) return;
        offset = new Vector3(0f, posY, 0f);
        transform.position = targetPosition + offset;

        Debug.Log("Clone in position" + transform.position);
        gameObject.SetActive(true);

        if (currentAnimationRoutine != null)
            StopCoroutine(currentAnimationRoutine);

        currentAnimationRoutine = StartCoroutine(RotateMole(-20f, 20f, 0.2f));
        isVisible = true;
    }

    public void HideClone ()
    {
        if (!isAbilityUnlocked || !isVisible) return;

        if (currentAnimationRoutine != null)
            StopCoroutine(currentAnimationRoutine);

        currentAnimationRoutine = StartCoroutine(HideRoutine());
    }

    public void ForceHideCloneImmediately ()
    {
        if (!isAbilityUnlocked) return;

        if (currentAnimationRoutine != null)
            StopCoroutine(currentAnimationRoutine);

        moleBase.localRotation = Quaternion.Euler(-20f, 0f, 0f);
        gameObject.SetActive(false);
        isVisible = false;
    }

    private IEnumerator HideRoutine ()
    {
        yield return RotateMole(20f, -20f, 0f);
        gameObject.SetActive(false);
        isVisible = false;
    }

    private IEnumerator RotateMole ( float fromAngle, float toAngle, float duration )
    {
        Quaternion startRot = Quaternion.Euler(fromAngle, 0f, 0f);
        Quaternion endRot = Quaternion.Euler(toAngle, 0f, 0f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            moleBase.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        moleBase.localRotation = endRot;
    }

    public bool IsAbilityUnlocked => isAbilityUnlocked;
    public bool IsVisible => isVisible;
}
