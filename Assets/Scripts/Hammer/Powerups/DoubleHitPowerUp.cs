using UnityEngine;

public class DoubleHitPowerUp : BasePowerUp
{
    protected override void ApplyEffect ( HammerController hammer )
    {
        if (hammer != null)
        {
            hammer.HammerPowerUps.ActivateDoubleHit();
            Debug.Log("DoubleHitPowerUp: ActivateDoubleHit llamado ");
        }
        else
        {
            Debug.LogWarning("DoubleHitPowerUp: Hammer o HammerPowerUps es null");
        }
    }

}
