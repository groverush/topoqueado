using UnityEngine;
using System;

public abstract class BasePowerUp : MonoBehaviour
{
    [SerializeField] private Vector3 offsetPosition = Vector3.up;
    [SerializeField] private float lifeDuration = 10f;

    public event Action OnCollected;

    private bool collected = false;

    private void Start ()
    {
        transform.position += offsetPosition;
        Invoke(nameof(DestroyIfNotCollected), lifeDuration);
    }

    private void OnTriggerEnter ( Collider other )
    {
        if (collected) return;

        if (other.CompareTag("Hammer"))
        {
            HammerController hammer = other.GetComponent<HammerController>() ?? other.GetComponentInParent<HammerController>();
            if (hammer != null)
            {
                ApplyEffect(hammer.gameObject);
                HandleCollected();
                return;
            }
        }

        if (other.CompareTag("Mole"))
        {
            MoleController mole = other.GetComponent<MoleController>() ?? other.GetComponentInParent<MoleController>();
            if (mole != null)
            {
                ApplyEffect(mole.gameObject);
                HandleCollected();
            }
        }
    }

    protected abstract void ApplyEffect ( GameObject target );

    private void DestroyIfNotCollected ()
    {
        if (!collected)
            HandleCollected();
    }

    private void HandleCollected ()
    {
        collected = true;
        OnCollected?.Invoke();
        Destroy(gameObject);
    }
}
