using UnityEngine;
using System.Collections;

public class HammerCloneController : MonoBehaviour
{
    [SerializeField] private Transform hammerBase;

    private Coroutine currentRoutine;

    private void Awake ()
    {
        gameObject.SetActive(false); // Empieza desactivado
    }

    public void ActivateClone ( Vector3 position, float initialAngle )
    {
        transform.position = position;
        hammerBase.localRotation = Quaternion.Euler(initialAngle, 0f, 0f);
        gameObject.SetActive(true);
    }

    public IEnumerator PlayHitAnimation ( Vector3 holePosition, float initialAngle, float hitAngle, float hitDuration, float pause, Vector3 hitOffset )
    {
        // Si ya había una rutina activa, se cancela antes de arrancar otra
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(HitAnimationSequence(holePosition, initialAngle, hitAngle, hitDuration, pause, hitOffset));

        yield return currentRoutine;
    }

    private IEnumerator HitAnimationSequence ( Vector3 holePosition, float initialAngle, float hitAngle, float hitDuration, float pause, Vector3 hitOffset )
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(holePosition.x, startPos.y, holePosition.z) + hitOffset;

        float moveToTime = 0.1f;
        float elapsed = 0f;
        while (elapsed < moveToTime)
        {
            if (hammerBase == null) yield break; // Prevención
            hammerBase.position = Vector3.Lerp(startPos, targetPos, elapsed / moveToTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (hammerBase == null) yield break;
        hammerBase.position = targetPos;

        yield return RotateHammer(initialAngle, hitAngle, hitDuration);
        yield return new WaitForSeconds(pause);
        yield return RotateHammer(hitAngle, initialAngle, hitDuration);

        elapsed = 0f;
        while (elapsed < moveToTime)
        {
            if (hammerBase == null) yield break;
            hammerBase.position = Vector3.Lerp(targetPos, startPos, elapsed / moveToTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (hammerBase != null)
            hammerBase.position = startPos;
    }

    private IEnumerator RotateHammer ( float fromAngle, float toAngle, float duration )
    {
        Quaternion startRot = Quaternion.Euler(fromAngle, 0f, 0f);
        Quaternion endRot = Quaternion.Euler(toAngle, 0f, 0f);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (hammerBase == null) yield break;
            hammerBase.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (hammerBase != null)
            hammerBase.localRotation = endRot;
    }

    public void DeactivateClone ()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        gameObject.SetActive(false);
    }
}
