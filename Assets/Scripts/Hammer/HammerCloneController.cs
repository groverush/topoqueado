using System.Collections;
using UnityEngine;

public class HammerCloneController : MonoBehaviour
{
    public Transform hammerBase;

    public void DeactivateClone ()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    public IEnumerator PlayHitAnimation ( Vector3 holePosition, float initialAngle, float hitAngle, float downDuration, float pause, Vector3 hitOffset )
    {
        Vector3 targetPos = new Vector3(holePosition.x, transform.position.y, holePosition.z) + hitOffset;
        Vector3 startPos = transform.position;

        float moveToTime = 0.1f;
        float elapsed = 0f;
        while (elapsed < moveToTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveToTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;

        yield return RotateHammer(initialAngle, hitAngle, downDuration);
        yield return new WaitForSeconds(pause);
        yield return RotateHammer(hitAngle, initialAngle, downDuration);

        elapsed = 0f;
        while (elapsed < moveToTime)
        {
            transform.position = Vector3.Lerp(targetPos, startPos, elapsed / moveToTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;
    }

    private IEnumerator RotateHammer ( float fromAngle, float toAngle, float duration )
    {
        if (hammerBase == null)
            yield break;

        Quaternion startRot = Quaternion.Euler(fromAngle, 0f, 0f);
        Quaternion endRot = Quaternion.Euler(toAngle, 0f, 0f);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (hammerBase == null)
                yield break;

            hammerBase.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (hammerBase != null)
            hammerBase.localRotation = endRot;
    }

}
