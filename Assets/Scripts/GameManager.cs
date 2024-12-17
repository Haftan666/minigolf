using TMPro;
using UnityEngine;
using System.Collections;

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
    public TextMeshProUGUI attemptsText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI totalAttemptsText;
    public GameObject congratulationsPanel;
    private int attempts = 1;
    private int totalAttempts = 0;
    private bool gameEnded = false;

    void Start()
    {
        initialBallPosition = ballTransform.position; // Zapisz początkową pozycję piłki
        UpdateAttemptsText();
        UpdateLevelText();
        congratulationsPanel.SetActive(false);
    }

    public void ResetLevel()
    {
        //Debug.Log($"Attempts: {attempts}, Total Attempts: {totalAttempts}");
        ballTransform.position = initialBallPosition + new Vector3(currentLevel * levelOffsetX, 0, 0);
        ballTransform.rotation = Quaternion.identity;
        ballTransform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero; // Zresetuj prędkość piłki
        ballTransform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // Zresetuj rotację piłki
        hasAppliedForce = false;
        ballController.ResetTimeSinceLastMove();
        attempts++;
        UpdateAttemptsText();
    }

    public void NextLevel()
    {
        totalAttempts += attempts;
        attempts = 0;

        int nextLevel = currentLevel + 1;
        GameObject nextLevelObject = GameObject.Find($"level{nextLevel + 1}");
        if (nextLevelObject != null)
        {
            currentLevel = nextLevel;
            InvokeResetLevel(resetTime);
            Invoke("UpdateLevelText", resetTime);
        }
        else
        {
            gameEnded = true;
            ShowCongratulations();
        }
    }

    public void InvokeResetLevel(float delay)
    {
        Invoke("ResetLevel", delay);
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
        attemptsText.text = $"Attempt {attempts}";
    }

    private void UpdateLevelText()
    {
        levelText.text = $"Level {currentLevel + 1}";
    }

    private void ShowCongratulations()
    {
        totalAttempts += attempts;
        totalAttemptsText.text = $"Total Attempts: {totalAttempts}";
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
}
