using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // === Timer ===
    [Header("Timer UI")]
    [SerializeField] private GameObject timeoutPanel;
    [SerializeField] private TextMeshProUGUI timeLeftText;

    // Updates the visible countdown timer on screen
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
}