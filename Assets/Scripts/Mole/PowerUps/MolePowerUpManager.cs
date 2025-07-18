using System;
using System.Collections;
using UnityEngine;

public class MolePowerUpManager : MonoBehaviour
{
    [Header("Clone Settings")]
    [SerializeField] private GameObject moleClonePrefab;
    [SerializeField] private Vector3 cloneOffset = Vector3.zero;
    [SerializeField] private HoleNavigation holeNavigation;
    [SerializeField] private float moleCloneDuration = 5f;
    private MoleCloneController moleCloneInstance;
    private Vector3 lastClonePosition;
    private Coroutine cloneTimerRoutine;

    [Header("Mole Vision Settings")]
    [SerializeField] private Camera moleCamera;
    [SerializeField] private LayerMask hammerLayer;
    [SerializeField] private float moleVisionDuration = 5f;
    private Coroutine moleVisionRoutine;
    private int originalCullingMask;

    public bool IsMoleVisionActive { get; private set; }
    public bool IsCloneAbilityUnlocked { get; private set; } = false;

    public float MoleVisionTimeRemaining { get; private set; } = 0f;
    public float CloneAbilityTimeRemaining { get; private set; } = 0f;

    public event Action OnMoleVisionEnd;
    public event Action OnCloneEnd;

    public void UnlockCloneAbility ()
    {
        if (moleCloneInstance == null)
            CreateMoleClone();

        moleCloneInstance.UnlockCloneAbility();
        IsCloneAbilityUnlocked = true;

        if (cloneTimerRoutine != null) StopCoroutine(cloneTimerRoutine);
        cloneTimerRoutine = StartCoroutine(AbilityTimer(moleCloneDuration, 
            time => CloneAbilityTimeRemaining = time,
            () =>
            {
                IsCloneAbilityUnlocked = false;
                CloneAbilityTimeRemaining = 0f;
                HideClone();
                OnCloneEnd?.Invoke();
            }));
    }

    private void CreateMoleClone ()
    {
        GameObject cloneObj = Instantiate(moleClonePrefab);
        moleCloneInstance = cloneObj.GetComponent<MoleCloneController>();
        moleCloneInstance.gameObject.SetActive(false);
    }

    public void TryShowClone ( Vector3 moleCurrentPosition )
    {
        if (!IsCloneAbilityUnlocked || moleCloneInstance == null || holeNavigation == null) return;
        if (moleCloneInstance.IsVisible) return;

        GameObject randomHole = null;
        int attempts = 10;

        while (attempts > 0)
        {
            randomHole = holeNavigation.GetRandomHole();
            if (randomHole != null && Vector3.Distance(randomHole.transform.position, moleCurrentPosition) > 0.1f)
                break;
            attempts--;
        }

        if (randomHole != null)
        {
            lastClonePosition = randomHole.transform.position + cloneOffset;
            moleCloneInstance.ShowAtPosition(lastClonePosition);
        }
        else
        {
            Debug.LogWarning("No hay agujeros validos para el clon.");
        }
    }

    public void HideClone ()
    {
        if (IsCloneAbilityUnlocked && moleCloneInstance != null)
        {
            moleCloneInstance.HideClone();
        }
    }

    public void ActivateMoleVision ()
    {
        if (moleVisionRoutine != null) StopCoroutine(moleVisionRoutine);
        moleVisionRoutine = StartCoroutine(MoleVisionCoroutine());
    }

    private IEnumerator MoleVisionCoroutine ()
    {
        IsMoleVisionActive = true;
        MoleVisionTimeRemaining = moleVisionDuration;
        originalCullingMask = moleCamera.cullingMask;
        moleCamera.cullingMask |= hammerLayer.value;

        yield return AbilityTimer(moleVisionDuration,
            time => MoleVisionTimeRemaining = time,
            () =>
            {
                moleCamera.cullingMask = originalCullingMask;
                IsMoleVisionActive = false;
                MoleVisionTimeRemaining = 0f;
                OnMoleVisionEnd?.Invoke();
            });
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
        return IsMoleVisionActive || IsCloneAbilityUnlocked;
    }
}