using UnityEngine;

public class MoleClonePowerUp : BasePowerUp
{
    protected override void ApplyEffect ( GameObject target )
    {
        MoleController moleController = target.GetComponent<MoleController>();

        if (moleController != null && moleController.MolePowerUpManager != null)
        {
            moleController.MolePowerUpManager.ActivateClone();
            Debug.Log("Clone PowerUp activado.");
        }
        else
        {
            Debug.LogWarning("MolePowerUpManager no asignado en MoleController.");
        }
    }

}
