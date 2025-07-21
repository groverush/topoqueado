using UnityEngine;

public class MoleVisionPowerUp : BasePowerUp
{
    protected override void ApplyEffect ( GameObject target )
    {
        MoleController moleController = target.GetComponent<MoleController>();

        if (moleController != null && moleController.MolePowerUpManager != null)
        {
            moleController.MolePowerUpManager.ActivateMoleVision();
        }
        else
        {
            Debug.LogWarning("MolePowerUpManager no asignado en MoleController.");
        }
    }
}
