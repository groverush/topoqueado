using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Timer timer;

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
