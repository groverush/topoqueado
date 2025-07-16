using UnityEngine;

public class HammerDoubleHitPowerUp : BasePowerUp
{
    protected override void ApplyEffect ( GameObject target )
    {
        var hammer = target.GetComponent<HammerController>();
        if (hammer != null)
        {
            hammer.PowerUpManager.ActivateDoubleHit(hammer.HammerBase, hammer.HammerRestPosition, hammer.InitialHammerAngle);
            Debug.Log("Double Hit PowerUp activado.");
        }
    }
}