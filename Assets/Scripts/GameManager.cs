using UnityEngine;

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

    void Start()
    {
        initialBallPosition = ballTransform.position; // Zapisz początkową pozycję piłki
    }

    public void ResetLevel()
    {
        // Teleportuj piłkę do początkowej pozycji bieżącego poziomu
        ballTransform.position = initialBallPosition + new Vector3(currentLevel * levelOffsetX, 0, 0);
        ballTransform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero; // Zresetuj prędkość piłki
        ballTransform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // Zresetuj rotację piłki
        hasAppliedForce = false;
        ballController.ResetTimeSinceLastMove();
    }

    public void NextLevel()
    {
        int nextLevel = currentLevel + 1;
        GameObject nextLevelObject = GameObject.Find($"level{nextLevel + 1}");
        if (nextLevelObject != null)
        {
            currentLevel = nextLevel;
            InvokeResetLevel(resetTime);
        }
        else
        {
            Debug.LogWarning($"Level {nextLevel + 1} does not exist.");
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
}
