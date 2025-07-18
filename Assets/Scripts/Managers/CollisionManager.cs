using System;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    // === Controllers ===
    [SerializeField] private HammerController hammerController;
    [SerializeField] private MoleController moleController;

    // === Hit validation ===
    private bool isHitValidate = false;

    // === Events ===
    public event Action<HammerController> OnHitSuccess;
    public event Action<MoleController> OnHitMiss;

    void Awake()
    {
        if (hammerController != null)
        {
            hammerController.OnHammerHitAttempt += ValidateHit;
        }

        if (moleController != null)
        {
            moleController.OnMoleHit += RegisterHitSuccess;
        }
    }

    private void ResetState()
    {
        isHitValidate = true;
        CancelInvoke(nameof(RegisterHitFailure));
    }

    // Called when the hammer attempts a hit
    private void ValidateHit()
    {
        if (moleController.CurrentPopState == MoleController.PopStates.Visible)
        {
            isHitValidate = false;

            // Allow time for a collision to occur
            Invoke(nameof(RegisterHitFailure), 0.3f);
        }
    }

    // Called when the mole reports a collision after a hit attempt
    private void RegisterHitSuccess()
    {
        if (!isHitValidate)
        {
            OnHitSuccess?.Invoke(hammerController);
            isHitValidate = true;
        }
    }

    // Called if no collision is reported within a short window after hit attempt
    private void RegisterHitFailure()
    {
        if (!isHitValidate)
        {
            OnHitMiss?.Invoke(moleController);
            isHitValidate = true;
        }
    }
}