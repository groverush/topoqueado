using UnityEngine;

public class MoleVisionPowerUp : BasePowerUp
{
    void OnEnable()
    {
        if (AudioManager.instance != null)
        {
            onEffectApplied += () => AudioManager.instance.PlaySFX(AudioManager.SfxType.PowerUp, 1, AudioManager.instance.XRayEffectVolume);
        }
    }

    protected override void ApplyEffect(GameObject target)
    {
        MoleController moleController = target.GetComponent<MoleController>();

        if (moleController != null && moleController.MolePowerUpManager != null)
        {
            moleController.MolePowerUpManager.ActivateMoleVision();
            onEffectApplied?.Invoke();
        }
        else
        {
            Debug.LogWarning("MolePowerUpManager no asignado en MoleController.");
        }
    }
}
