using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float resetTime;

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void InvokeResetLevel(float delay)
    {
        Invoke("ResetLevel", delay);
    }
}
