using UnityEngine;

public class MolePowerUpClone : BasePowerUp
{
    protected override void ApplyEffect ( GameObject target )
    {
        MoleController moleController = target.GetComponent<MoleController>();

        if (moleController != null && moleController.MolePowerUpManager != null)
        {
            moleController.MolePowerUpManager.UnlockCloneAbility();
        }
        else
        {
            Debug.LogWarning("MolePowerUpManager no asignado en MoleController.");
        }
    }
}
