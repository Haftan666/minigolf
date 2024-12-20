using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform ballTransform;
    public GameObject[] levelPassedTriggers;
    private Vector3 initialBallPosition;
    private int currentLevel = 0;
    private const float levelOffsetX = 70f;
    public float resetTime;
    public bool hasAppliedForce = false;
    public BallController ballController;
    public LastAttemptArrowController lastAttemptArrowController;
    public TextMeshProUGUI attemptsText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI totalAttemptsText;
    public GameObject congratulationsPanel;
    private int attempts = 0;
    private int totalAttempts = 0;
    private bool gameEnded = false;
    public GameObject backgroundMusic;
    public GameObject congratulationsMusic;

    void Start()
    {
        initialBallPosition = ballTransform.position;
        UpdateAttemptsText();
        UpdateLevelText();
        congratulationsPanel.SetActive(false);
        lastAttemptArrowController.HideLastAttemptArrow();
    }

    public void ResetLevel(bool isNext)
    {
        ballTransform.position = initialBallPosition + new Vector3(currentLevel * levelOffsetX, 0, 0);
        ballTransform.rotation = Quaternion.identity;
        ballTransform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        ballTransform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        hasAppliedForce = false;
        ballController.ResetTimeSinceLastMove();
        if(!isNext)
        {
            attempts++;
        }
 
        UpdateAttemptsText();
    }

    public void NextLevel()
    {
        totalAttempts += attempts + 1;
        attempts = 0;

        int nextLevel = currentLevel + 1;
        GameObject nextLevelObject = GameObject.Find($"level{nextLevel + 1}");
        if (nextLevelObject != null)
        {
            currentLevel = nextLevel;
            InvokeResetLevel(resetTime, true);
            Invoke("UpdateLevelText", resetTime);
        }
        else
        {
            gameEnded = true;
            ShowCongratulations();
        }
    }

    public void InvokeResetLevel(float delay, bool isNext)
    {
        // Używamy pomocniczej metody, która wywoła ResetLevel
        StartCoroutine(InvokeWithDelay(delay, isNext));
    }

    private IEnumerator InvokeWithDelay(float delay, bool isNext)
    {
        yield return new WaitForSeconds(delay);
        ResetLevel(isNext);
    }

    public void SetHasAppliedForce(bool value)
    {
        hasAppliedForce = value;
    }

    public bool GetHasAppliedForce()
    {
        return hasAppliedForce;
    }

    public GameObject GetCurrentLevelPassedTrigger()
    {
        if (currentLevel < levelPassedTriggers.Length)
        {
            return levelPassedTriggers[currentLevel];
        }
        return null;
    }

    private void UpdateAttemptsText()
    {
        attemptsText.text = $"Attempt {attempts +1}";
    }

    private void UpdateLevelText()
    {
        levelText.text = $"Level {currentLevel + 1}";
    }

    private void ShowCongratulations()
    {
        backgroundMusic.SetActive(false);
        congratulationsMusic.SetActive(true);
        congratulationsMusic.GetComponent<AudioSource>().loop = false;
        congratulationsMusic.GetComponent<AudioSource>().Play();
        totalAttempts += attempts;
        totalAttemptsText.text = $"Total Attempts: {totalAttempts}";
        if (totalAttempts < PlayerPrefs.GetInt("Highscore", 0) || PlayerPrefs.GetInt("Highscore", 0) == 0)
        {
            PlayerPrefs.SetInt("Highscore", totalAttempts);
            PlayerPrefs.Save();
        }
        StartCoroutine(FadeInPanel(congratulationsPanel, 2f));
    }

    private IEnumerator FadeInPanel(GameObject panel, float duration)
    {
        yield return new WaitForSeconds(resetTime);

        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }

        float elapsedTime = 0f;
        panel.SetActive(true);
        canvasGroup.alpha = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public bool getGameEnded()
    {
        return gameEnded;
    }

    public int GetAttempts()
    {
        return attempts;
    }

    public void MainMenu()
    {
        // Przejdź do menu
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        // Zamknij grę
        Application.Quit();
        Debug.Log("Gra została zamknięta (działa tylko w buildzie)");
    }
}
