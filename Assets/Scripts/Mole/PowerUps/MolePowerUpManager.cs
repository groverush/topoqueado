using System;
using System.Collections;
using UnityEngine;

public class MolePowerUpManager : MonoBehaviour
{
    [Header("Clone Settings")]
    [SerializeField] private GameObject moleClonePrefab;
    [SerializeField] private float cloneDuration = 5f;
    [SerializeField] private Vector3 cloneOffset = Vector3.zero;

    [Header("Mole Vision Settings")]
    [SerializeField] private Camera moleCamera;
    [SerializeField] private LayerMask hammerLayer;
    [SerializeField] private float visionDuration = 5f;

    [HideInInspector] public MoleCloneController MoleCloneInstance { get; private set; }

    private Coroutine cloneRoutine;
    private Coroutine visionRoutine;
    private int originalCullingMask;

    public bool IsCloneActive { get; private set; }
    public bool IsVisionActive { get; private set; }

    public event Action OnCloneEnd;
    public event Action OnVisionEnd;

    public void ActivateClone ( Vector3 molePosition )
    {
        if (cloneRoutine != null)
            StopCoroutine(cloneRoutine);

        if (MoleCloneInstance == null)
            CreateMoleClone();

        SyncMoleClone(molePosition);
        cloneRoutine = StartCoroutine(CloneCoroutine());
    }

    private IEnumerator CloneCoroutine ()
    {
        IsCloneActive = true;

        float remainingTime = cloneDuration;
        while (remainingTime > 0f)
        {
            Debug.Log($"[Clone] Tiempo restante: {remainingTime:F1} segundos");
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        DestroyMoleClone();
        IsCloneActive = false;
        OnCloneEnd?.Invoke();
    }

    private void CreateMoleClone ()
    {
        GameObject cloneObj = Instantiate(moleClonePrefab);
        MoleCloneInstance = cloneObj.GetComponent<MoleCloneController>();
        MoleCloneInstance.gameObject.SetActive(false);
    }

    public void SyncMoleClone ( Vector3 molePosition )
    {
        if (MoleCloneInstance != null)
        {
            MoleCloneInstance.transform.position = molePosition + cloneOffset;
            MoleCloneInstance.transform.rotation = Quaternion.identity;
            MoleCloneInstance.gameObject.SetActive(true);
        }
    }

    private void DestroyMoleClone ()
    {
        if (MoleCloneInstance != null)
        {
            MoleCloneInstance.DeactivateClone();
            Destroy(MoleCloneInstance.gameObject);
            MoleCloneInstance = null;
        }
    }

    public void ActivateMoleVision ()
    {
        if (visionRoutine != null)
            StopCoroutine(visionRoutine);

        visionRoutine = StartCoroutine(VisionCoroutine());
    }

    private IEnumerator VisionCoroutine ()
    {
        IsVisionActive = true;
        originalCullingMask = moleCamera.cullingMask;
        moleCamera.cullingMask |= hammerLayer.value;

        yield return new WaitForSeconds(visionDuration);

        moleCamera.cullingMask = originalCullingMask;
        IsVisionActive = false;
        OnVisionEnd?.Invoke();
    }
}
