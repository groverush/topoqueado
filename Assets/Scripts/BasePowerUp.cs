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
        if (other.CompareTag("Hammer") && !collected)
        {
            HammerController hammer = other.GetComponent<HammerController>() ?? other.GetComponentInParent<HammerController>();

            if (hammer != null)
            {
                ApplyEffect(hammer);
                HandleCollected();
            }
        }
    }

    protected abstract void ApplyEffect ( HammerController hammer );

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
