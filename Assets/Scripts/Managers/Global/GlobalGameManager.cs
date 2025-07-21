using UnityEngine;

public class GlobalGameManager : MonoBehaviour
{
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
    }
}
