using UnityEngine;
using System;

public abstract class BasePowerUp : MonoBehaviour
{
    [SerializeField] private Vector3 offsetPosition = Vector3.up;
    [SerializeField] private float lifeDuration = 10f;

    private bool collected = false;

    public event Action OnCollected; // Evento público para que otros scripts se suscriban

    private void Start ()
    {
        transform.position += offsetPosition;
        Invoke(nameof(DestroyIfNotCollected), lifeDuration);
    }

    private void OnTriggerEnter ( Collider other )
    {
        if (other.CompareTag("Hammer"))
        {
            HammerController hammerController = other.GetComponent<HammerController>();
            if (hammerController == null)
                hammerController = other.GetComponentInParent<HammerController>();

            if (hammerController != null)
            {
                ApplyEffect(hammerController);
                OnCollectedLogic();
            }
            else
            {
                Debug.LogWarning($"El objeto {other.name} tiene el tag Hammer pero no se encontró HammerController ni en el objeto ni en sus padres.");
            }
        }
    }

    protected abstract void ApplyEffect ( HammerController hammer );

    private void DestroyIfNotCollected ()
    {
        if (!collected)
        {
            OnCollectedLogic();
        }
    }

    private void OnCollectedLogic ()
    {
        if (collected) return;

        collected = true;
        OnCollected?.Invoke();
        Destroy(gameObject);
    }
}
