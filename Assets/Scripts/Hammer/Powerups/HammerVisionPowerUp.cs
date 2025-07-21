using UnityEngine;

public class HammerVisionPowerUp : BasePowerUp
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
        var hammer = target.GetComponent<HammerController>();
        if (hammer != null && hammer.PowerUpManager != null)
        {
            hammer.PowerUpManager.ActivateHammerVision();
            onEffectApplied?.Invoke();
        }
    }
}
    