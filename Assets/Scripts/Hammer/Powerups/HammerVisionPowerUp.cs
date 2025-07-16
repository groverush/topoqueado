using UnityEngine;

public class HammerVisionPowerUp : BasePowerUp
{
    protected override void ApplyEffect ( HammerController hammer )
    {
        if (hammer != null && hammer.PowerUpManager != null)
        {
            hammer.PowerUpManager.ActivateMoleVision();
            Debug.Log("Mole Vision PowerUp activado.");
        }
    }
}
