using UnityEngine;
using System.Collections;
using TMPro;

public class Timer : MonoBehaviour
{
    public bool isGameActive;

    float currCountdownValue;
    [Header("Countdown Elements")]
    [SerializeField] private GameObject timeoutPanel;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private int gameTime = 60;

    public IEnumerator StartCountdown(float countdownValue)
    {
        currCountdownValue = countdownValue;
        // timeLeftText.gameObject.SetActive(true);
        while (currCountdownValue >= 0 && isGameActive)
        {
            timeLeftText.text = "Time: " + currCountdownValue.ToString("0");
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
            if (currCountdownValue == 0)
            {
                GameOver();
            }
        }
    }

    public void StartCountdownGame()
    {
        isGameActive = true;
        timeLeftText.gameObject.SetActive(true);
        StartCoroutine("StartCountdown", gameTime);
    }

    public void GameOver()
    {
        timeLeftText.gameObject.SetActive(false);

        timeoutPanel.SetActive(true);
        isGameActive = false;
        Time.timeScale = 0f; // Game Paused
    }
}