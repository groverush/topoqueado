using UnityEngine;

public class MoleVisionPowerUp : BasePowerUp
{
    protected override void ApplyEffect ( GameObject target )
    {
        MoleController moleController = target.GetComponent<MoleController>();

        if (moleController != null && moleController.MolePowerUpManager != null)
        {
            moleController.MolePowerUpManager.ActivateMoleVision();
            Debug.Log("Mole Vision PowerUp activado.");
        }
        else
        {
            Debug.LogWarning("MolePowerUpManager no asignado en MoleController.");
        }
    }
}
