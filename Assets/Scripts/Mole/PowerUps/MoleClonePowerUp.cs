using UnityEngine;

public class MoleClonePowerUp : BasePowerUp
{
    protected override void ApplyEffect ( GameObject target )
    {
        MoleController moleController = target.GetComponent<MoleController>();

        if (moleController != null && moleController.MolePowerUpManager != null)
        {
            moleController.MolePowerUpManager.UnlockCloneAbility();
            Debug.Log("Habilidad de Clone desbloqueada para el topo.");
        }
        else
        {
            Debug.LogWarning("MolePowerUpManager no asignado en MoleController.");
        }
    }
}
