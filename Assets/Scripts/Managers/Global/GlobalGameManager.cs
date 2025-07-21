using UnityEngine;

public class GlobalGameManager : MonoBehaviour
{
    // === Managers ===
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private SceneSwitchingManager sceneSwitchingManager;

    // === Singleton ===
    public static GlobalGameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioManager.enabled = true;
        sceneSwitchingManager.enabled = true;
    }
}
