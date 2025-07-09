using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Timer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public bool isGameActive;

    float currCountdownValue;
    [Header("Countdown Elements")]
    [SerializeField] private GameObject timeoutPanel;
    [SerializeField] private TextMeshProUGUI timeLeftText;




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
        StartCoroutine("StartCountdown", 3);

    }
    public void GameOver()
    {
        timeLeftText.gameObject.SetActive(false);

        timeoutPanel.SetActive(true);
        isGameActive = false;
        Time.timeScale = 0f; // Game Paused
    }



}
