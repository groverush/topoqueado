using System;
using System.Collections;
using UnityEngine;

public class HammerPowerUpManager : MonoBehaviour
{
    [Header("Double Hit Settings")]
    [SerializeField] private GameObject hammerClonePrefab;
    [SerializeField] private float doubleHitDuration = 5f;
    [SerializeField] private Vector3 clonePositionOffset = Vector3.zero;

    [Header("Mole Vision Settings")]
    [SerializeField] private Camera hammerCamera;
    [SerializeField] private LayerMask moleLayer;
    [SerializeField] private float hammerVisionDuration = 5f;

    [HideInInspector] public HammerCloneController HammerCloneInstance { get; private set; }

    private Coroutine doubleHitRoutine;
    private Coroutine hammerVisionRoutine;
    private int originalCullingMask;

    public bool IsDoubleHitActive { get; private set; }
    public bool IsHammerVisionActive { get; private set; }

    public float DoubleHitTimeRemaining { get; private set; }
    public float HammerVisionTimeRemaining { get; private set; }

    public event Action OnDoubleHitEnd;
    public event Action OnHammerVisionEnd;

    public bool HasValidHammerClone ()
    {
        return HammerCloneInstance != null && HammerCloneInstance.gameObject != null;
    }

    public void ActivateDoubleHit ( Transform hammerBase, Vector3 hammerRestPosition, float initialHammerAngle )
    {
        if (doubleHitRoutine != null)
            StopCoroutine(doubleHitRoutine);

        if (HammerCloneInstance == null)
            CreateHammerClone();

        SyncHammerClone(hammerRestPosition, initialHammerAngle);

        doubleHitRoutine = StartCoroutine(AbilityTimer(doubleHitDuration,
            time => DoubleHitTimeRemaining = time,
            () =>
            {
                if (HammerCloneInstance != null)
                {
                    HammerCloneInstance.DeactivateClone();
                    Destroy(HammerCloneInstance.gameObject, 0.5f);
                    HammerCloneInstance = null;
                }
                IsDoubleHitActive = false;
                DoubleHitTimeRemaining = 0f;
                OnDoubleHitEnd?.Invoke();
            }));

        IsDoubleHitActive = true;
    }

    private void CreateHammerClone ()
    {
        GameObject cloneObj = Instantiate(hammerClonePrefab);
        HammerCloneInstance = cloneObj.GetComponent<HammerCloneController>();
        HammerCloneInstance.gameObject.SetActive(false);
    }

    public void SyncHammerClone ( Vector3 hammerRestPosition, float initialHammerAngle )
    {
        if (HasValidHammerClone())
        {
            HammerCloneInstance.transform.position = hammerRestPosition + clonePositionOffset;
            HammerCloneInstance.transform.rotation = Quaternion.identity;
            HammerCloneInstance.hammerBase.localPosition = Vector3.zero;
            HammerCloneInstance.hammerBase.localRotation = Quaternion.Euler(initialHammerAngle, 0f, 0f);
        }
    }

    public void ActivateCloneForHit ()
    {
        if (HasValidHammerClone() && IsDoubleHitActive)
        {
            HammerCloneInstance.gameObject.SetActive(true);
        }
    }

    public GameObject GetSecondHole ( HoleNavigation holeNavigation, GameObject currentHole )
    {
        if (holeNavigation == null || holeNavigation.Holes.Count <= 1) return null;

        GameObject randomHole = null;
        int attempts = 0;

        do
        {
            randomHole = holeNavigation.GetRandomHole();
            attempts++;
        }
        while (randomHole == currentHole && attempts < 10);

        return randomHole != currentHole ? randomHole : null;
    }

    public void ActivateHammerVision ()
    {
        if (hammerVisionRoutine != null)
            StopCoroutine(hammerVisionRoutine);

        hammerVisionRoutine = StartCoroutine(AbilityTimer(hammerVisionDuration,
            time => HammerVisionTimeRemaining = time,
            () =>
            {
                hammerCamera.cullingMask = originalCullingMask;
                IsHammerVisionActive = false;
                HammerVisionTimeRemaining = 0f;
                OnHammerVisionEnd?.Invoke();
            }));

        IsHammerVisionActive = true;
        originalCullingMask = hammerCamera.cullingMask;
        hammerCamera.cullingMask |= moleLayer.value;
    }

    private IEnumerator AbilityTimer ( float duration, Action<float> onTimeChange, Action onComplete )
    {
        float timeRemaining = duration;
        float lastReportedSeconds = Mathf.Ceil(timeRemaining);

        while (timeRemaining > 0f)
        {
            yield return null;
            timeRemaining -= Time.deltaTime;
            onTimeChange.Invoke(timeRemaining);

            float currentSeconds = Mathf.Ceil(timeRemaining);
            if (currentSeconds != lastReportedSeconds)
            {
                Debug.Log($"[PowerUp Timer] Tiempo restante: {currentSeconds} segundos");
                lastReportedSeconds = currentSeconds;
            }
        }

        onTimeChange.Invoke(0f);
        onComplete?.Invoke();
    }

    public bool IsAnyPowerUpActive ()
    {
        return IsDoubleHitActive || IsHammerVisionActive;
    }
}
