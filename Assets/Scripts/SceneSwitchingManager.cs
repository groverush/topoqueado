using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneSwitchingManager : MonoBehaviour
{
    private const string mainMenuScene = "MainMenu";
    private const string mainScene = "MainScene";
    private const string tutorialScene = "TutorialScene";
    private const string creditsScene = "CreditsScene";
    [SerializeField] private Sprite[] slides; //
    [SerializeField] private Image displayImage; // UI Image to show slides
    private int currentSlide = 0;

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
    public void OnStartGameButtonClicked()
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

    public void OnGoBackButtonClicked()
    {
        // Load the main menu scene
        SceneManager.LoadScene(mainMenuScene);
    }
}
