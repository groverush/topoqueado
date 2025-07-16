using UnityEngine;

public class HammerDoubleHitPowerUp : BasePowerUp
{
    protected override void ApplyEffect ( HammerController hammer )
    {
        if (hammer != null && hammer.PowerUpManager != null)
        {
            hammer.PowerUpManager.ActivateDoubleHit(
                hammer.HammerBase,
                hammer.HammerRestPosition,
                hammer.InitialHammerAngle
            );
            Debug.Log("DoubleHitPowerUp: Activado desde el power-up.");
        }
    }
}