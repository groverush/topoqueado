using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // === Managers ===
    [Header("Managers")]
    [SerializeField] private SplitScreenManager splitScreenManager;
    [SerializeField] private ScoreManager scoreManager;

    // === Timer ===
    [Header("Timer UI")]
    [SerializeField] private GameObject timeoutPanel;
    [SerializeField] private TextMeshProUGUI timeLeftText;

    // === Static score display ===
    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI hammerScoreText;
    [SerializeField] private TextMeshProUGUI moleScoreText;

    // === Floating score ===
    [Header("Floating Score UI")]
    [SerializeField] private TextMeshProUGUI floatingHammerScoreText;
    [SerializeField] private TextMeshProUGUI floatingMoleScoreText;
    [SerializeField] private float floatingTextDelay;

    // === Coroutines ===
    private Coroutine hammerScoreCoroutine;
    private Coroutine moleScoreCoroutine;

    void Awake()
    {
        if (scoreManager != null)
        {
            scoreManager.OnHammerScoreChanged += UpdateHammerScoreText;
            scoreManager.OnMoleScoreChanged += UpdateMoleScoreText;
        }
    }

    // Timer methods
    public void UpdateTimerText(int seconds)
    {
        timeLeftText.text = $"{seconds:00}";
    }

    public void ShowTimerText()
    {
        timeLeftText.gameObject.SetActive(true);
    }

    private void HideTimerText()
    {
        timeLeftText.gameObject.SetActive(false);
    }

    public void ShowTimeoutPanel()
    {
        HideTimerText();
        timeoutPanel.SetActive(true);
    }

    // Score methods
    private void UpdateHammerScoreText(int score, Vector3 holePosition)
    {
        hammerScoreText.text = score.ToString();
        DisplayFloatingScore(holePosition, splitScreenManager.HammerCamera, floatingHammerScoreText, ref hammerScoreCoroutine);
    }

    private void UpdateMoleScoreText(int score, Vector3 holePosition)
    {
        moleScoreText.text = score.ToString();
        DisplayFloatingScore(holePosition, splitScreenManager.MoleCamera, floatingMoleScoreText, ref moleScoreCoroutine);
    }

    private void DisplayFloatingScore(Vector3 holePosition, Camera camera, TextMeshProUGUI floatingText, ref Coroutine routine)
    {
        Vector3 screenPosition = camera.WorldToScreenPoint(holePosition);
        floatingText.transform.position = screenPosition;
        floatingText.gameObject.SetActive(true);

        if (routine != null)
        {
            StopCoroutine(routine);
        }

        routine = StartCoroutine(HideFloatingScoreAfterDelay(floatingText, floatingTextDelay));
    }

    private IEnumerator HideFloatingScoreAfterDelay(TextMeshProUGUI textElement, float delay)
    {
        yield return new WaitForSeconds(delay);
        textElement.gameObject.SetActive(false);
    }
}