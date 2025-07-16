using System.Collections;
using UnityEngine;

public class MoleCloneController : MonoBehaviour
{
    [SerializeField] public Transform moleBase;

    public IEnumerator PlayPopOutAnimation ( Vector3 targetPosition, float initialAngle, float popOutAngle, float moveDuration, float rotationDuration )
    {
        // Movimiento hacia el agujero
        yield return MoveToPosition(targetPosition, moveDuration);

        // Rotación simulando aparición
        yield return RotateMole(initialAngle, popOutAngle, rotationDuration);

        // Pequeña pausa si se quiere
        yield return new WaitForSeconds(0.1f);

        // Rotación simulando esconderse
        yield return RotateMole(popOutAngle, initialAngle, rotationDuration);

        // Regreso a posición de descanso (opcional)
        yield return MoveToPosition(targetPosition, moveDuration);

        gameObject.SetActive(false);
    }

    private IEnumerator MoveToPosition ( Vector3 targetPosition, float duration )
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
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

    public void DeactivateClone ()
    {
        if (gameObject != null)
            gameObject.SetActive(false);
    }
}
