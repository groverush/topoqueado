using UnityEngine;

public class MoleVisionPowerUp : BasePowerUp
{
    [SerializeField] private float visionDuration = 5f;
    [SerializeField] private LayerMask moleLayer;

    protected override void ApplyEffect ( HammerController hammer )
    {
        Debug.Log("Activando visi�n de topo desde MoleVisionPowerUp");
        hammer.EnableMoleVision(visionDuration, moleLayer);
    }
}
