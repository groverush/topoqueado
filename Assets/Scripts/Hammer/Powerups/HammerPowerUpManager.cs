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
    [SerializeField] private float moleVisionDuration = 5f;

    [HideInInspector] public HammerCloneController HammerCloneInstance { get; private set; }

    private Coroutine doubleHitRoutine;
    private Coroutine moleVisionRoutine;
    private int originalCullingMask;

    public bool IsDoubleHitActive { get; private set; }
    public bool IsMoleVisionActive { get; private set; }

    public event Action OnDoubleHitEnd;
    public event Action OnMoleVisionEnd;

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
        doubleHitRoutine = StartCoroutine(DoubleHitCoroutine());
    }

    private IEnumerator DoubleHitCoroutine ()
    {
        IsDoubleHitActive = true;

        float remainingTime = doubleHitDuration;
        while (remainingTime > 0f)
        {
            Debug.Log($"[DoubleHit] Tiempo restante: {remainingTime:F1} segundos");
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        if (HammerCloneInstance != null)
        {
            HammerCloneInstance.DeactivateClone();
            yield return new WaitForSeconds(0.5f);
            if (HammerCloneInstance != null)
                Destroy(HammerCloneInstance.gameObject);
            HammerCloneInstance = null;
        }

        IsDoubleHitActive = false;
        OnDoubleHitEnd?.Invoke();
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

    public void ActivateMoleVision ()
    {
        if (moleVisionRoutine != null)
            StopCoroutine(moleVisionRoutine);

        moleVisionRoutine = StartCoroutine(MoleVisionCoroutine());
    }

    private IEnumerator MoleVisionCoroutine ()
    {
        IsMoleVisionActive = true;
        originalCullingMask = hammerCamera.cullingMask;
        hammerCamera.cullingMask |= moleLayer.value;

        yield return new WaitForSeconds(moleVisionDuration);

        hammerCamera.cullingMask = originalCullingMask;
        IsMoleVisionActive = false;
        OnMoleVisionEnd?.Invoke();
    }
}
