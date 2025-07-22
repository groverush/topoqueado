using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SceneSwitchingManager : MonoBehaviour
{
    // Constants for scene names
    private const string mainMenuScene = "MainMenu";
    private const string mainScene = "MainScene";
    private const string tutorialScene = "TutorialScene";
    private const string creditsScene = "CreditsScene";
    [SerializeField] private Sprite[] slides;
    [Header("Tutorial Presentation")]
    [SerializeField] private Image displayImage; // UI Image to show slides
    private int currentSlide = 0;
    [Header("Pause Panel")]
    [SerializeField] private GameObject pausePanel; // Pause panel
    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel; // Credits panel

    private PlayerInput playerInput; // Player input for pause functionality
    private InputAction pauseAction; // Input action for pause

    // === Singleton ===
    public static SceneSwitchingManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        playerInput = GetComponent<PlayerInput>();

        pauseAction = playerInput.actions["Pause"];
        // pauseAction.performed += ctx => OnPause();
    }
    void Start()
    {
        if (SceneManager.GetActiveScene().name == tutorialScene)
        {
            currentSlide = 0;
            if (slides != null && slides.Length > 0 && displayImage != null)
            {
                ShowSlide();
            }
        }
    }

    private void OnEnable()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic(AudioManager.MusicType.MainMenu, 0, AudioManager.instance.MainMenuSongVolume);
        }
        
        pauseAction.Enable(); // Habilita la acción
        pauseAction.performed += OnPause; // Suscribe el método al evento
    }

    private void OnDisable()
    {
        pauseAction.performed -= OnPause; // Desuscribe
        pauseAction.Disable(); // Deshabilita la acción
    }

    public void OnStartGameButtonClicked()
    {
        // Load the main scene
        SceneManager.LoadScene(mainScene);
        AudioManager.instance.PlayMusic(AudioManager.MusicType.InGame, 1, AudioManager.instance.MainThemeSongVolume);
    }

    public void OnTutorialButtonClicked()
    {
        // Load the tutorial scene
        SceneManager.LoadScene(tutorialScene);
    }

    public void OnContinueButtonClicked()
    {
        if (currentSlide < slides.Length)
        {

            ShowSlide();
        }
        else
        {
            SceneManager.LoadScene(mainScene);
        }
    }

    private void ShowSlide()
    {
        displayImage.sprite = slides[currentSlide];
        currentSlide++;
    }

    public void OnCreditsButtonClicked()
    {
        // Load the credits scene
        SceneManager.LoadScene(creditsScene);
    }

    public void OnMainMenuButtonClicked()
    {
        // Load the main menu scene
        SceneManager.LoadScene(mainMenuScene);
        AudioManager.instance.PlayMusic(AudioManager.MusicType.MainMenu, 0, AudioManager.instance.MainMenuSongVolume);
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        // Toggle pause state
        if (Time.timeScale == 1f)
        {
            Time.timeScale = 0f; // Pausa el juego
            pausePanel.SetActive(true); // Muestra el panel de pausa
        }
        else
        {
            Time.timeScale = 1f; // Reanuda el juego
            pausePanel.SetActive(false); // Oculta el panel de pausa
        }
    }

    public void OnResumeButtonClicked()
    {
        Time.timeScale = 1f; // Reanuda el juego
        pausePanel.SetActive(false); // Oculta el panel de pausa
    }
}