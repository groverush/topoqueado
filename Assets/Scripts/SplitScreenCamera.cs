using UnityEngine;

public class SplitScreenCamera : MonoBehaviour
{
    [SerializeField] private Camera moleCamera;
    [SerializeField] private Camera hammerCamera;

    private void Start()
    {
        SetupSplitScreen();
    }

    private void SetupSplitScreen()
    {
        if (moleCamera != null)
        {
            moleCamera.rect = new Rect(0, 0, 0.5f, 1); // 👈 Left: x, y, width, height
        }

        if (hammerCamera != null)
        {
            hammerCamera.rect = new Rect(0.5f, 0, 0.5f, 1); // 👈 Right: x, y, width, height
        }
    }
}