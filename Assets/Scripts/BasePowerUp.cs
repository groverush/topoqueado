using UnityEngine;
using System;

public abstract class BasePowerUp : MonoBehaviour
{
    [SerializeField] private Vector3 offsetPosition = Vector3.up;
    [SerializeField] private float lifeDuration = 10f;

    [Header("Animation Settings")]
    [SerializeField] private float rotationSpeed = 100;
    [SerializeField] private float bounceAmplitude = 0.1f;
    [SerializeField] private float bounceFrequency = 3f;

    private bool collected = false;
    private Vector3 initialPosition;

    // === Events ===
    public Action OnCollected;
    public Action onEffectApplied;

    private void Start()
    {
        transform.position += offsetPosition;
        initialPosition = transform.position;

        if (AudioManager.instance != null)
        {
            OnCollected += () => AudioManager.instance.PlaySFX(AudioManager.SfxType.PowerUp, 0, AudioManager.instance.PowerUpVolume);
        }

        Invoke(nameof(DestroyIfNotCollected), lifeDuration);
    }

    private void Update()
    {
        PowerUpAnimation();
    }

    private void PowerUpAnimation()
    {
        if (collected) return;

        // Rotacion continua sobre el eje Y local
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // Rebote sobre el eje Y sin afectar X ni Z
        float bounce = Mathf.Sin(Time.time * bounceFrequency) * bounceAmplitude;
        Vector3 newPosition = initialPosition + new Vector3(0f, bounce, 0f);
        transform.position = newPosition;

    }

    private void OnTriggerEnter(Collider other)
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

    protected abstract void ApplyEffect(GameObject target);

    private void DestroyIfNotCollected()
    {
        if (!collected) Destroy(gameObject);
    }

    private void HandleCollected()
    {
        collected = true;
        OnCollected?.Invoke();
        Destroy(gameObject);
    }
}
