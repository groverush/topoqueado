using UnityEngine;

public class HammerVisionPowerUp : BasePowerUp
{
    protected override void ApplyEffect ( GameObject target )
    {
        var hammer = target.GetComponent<HammerController>();
        if (hammer != null && hammer.PowerUpManager != null)
        {
            hammer.PowerUpManager.ActivateMoleVision();
            Debug.Log("Hammer Vision PowerUp activado.");
        }
    }
}
    