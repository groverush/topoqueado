using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // === Manager references ===
    [Header("Managers")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SplitScreenManager splitScreenManager;
    [SerializeField] private TimerManager timerManager;

    // === Inputs ===
    [Header("Inputs")]
    [SerializeField] private PlayerInput hammerInput;
    [SerializeField] private PlayerInput moleInput;

    private void Awake()
    {
        InitializeInputs();

        uiManager.ShowTimerText();

        splitScreenManager.SetupSplitScreen();

        timerManager.OnTimeUpdated += uiManager.UpdateTimerText;
        timerManager.OnTimerEnd += EndGame;

        timerManager.StartTimer();
    }

    private void InitializeInputs()
    {
        var keyboard = Keyboard.current;

        if (moleInput != null)
        {
            moleInput.SwitchCurrentControlScheme("Keyboard", keyboard);
        }

        if (hammerInput != null)
        {
            hammerInput.SwitchCurrentControlScheme("Keyboard", keyboard);
        }
    }

    private void EndGame()
    {
        Time.timeScale = 0f;

        uiManager.ShowTimeoutPanel();

        moleInput.enabled = false;
        hammerInput.enabled = false;
    }

    public void RestartGame()
    {
        //timerManager.ResetTimer();
        //scoreManager.ResetScores();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}