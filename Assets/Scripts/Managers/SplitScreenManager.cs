using UnityEngine;

public class SplitScreenManager : MonoBehaviour
{
    // === Cameras ===
    [SerializeField] private Camera moleCamera;
    [SerializeField] private Camera hammerCamera;

    // === Properties ===
    public Camera MoleCamera => moleCamera;
    public Camera HammerCamera => hammerCamera;

    public void SetupSplitScreen()
    {
        if (moleCamera != null)
        {
            moleCamera.rect = new Rect(0, 0, 0.5f, 1); // Left: x, y, width, height
        }

        if (hammerCamera != null)
        {
            hammerCamera.rect = new Rect(0.5f, 0, 0.5f, 1); // Right: x, y, width, height
        }
    }
}