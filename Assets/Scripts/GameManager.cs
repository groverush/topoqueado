using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Timer timer;
    private GameObject hammer;
    private GameObject mole;

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        hammer = GameObject.FindGameObjectWithTag("Hammer");
        mole = GameObject.FindGameObjectWithTag("Mole");
    }
    private void Start()
    {
        timer.StartCountdownGame();
    }
    public void RestartGame()
    {

        Time.timeScale = 1f; // Reset the game time scale
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
