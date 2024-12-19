using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // Do zarządzania scenami
using UnityEngine.UI; // Do obsługi UI

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI recordText; // Odniesienie do tekstu rekordu
    private int record;

    private void Start()
    {
        // Załaduj rekord z PlayerPrefs
        record = PlayerPrefs.GetInt("Highscore", 0);
        recordText.text = "Your highscore: " + (record == 0 ? "not set" : record.ToString());

    }

    public void StartGame()
    {
        // Przejdź do gry (Scena o indeksie 1)
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        // Zamknij grę
        Application.Quit();
        Debug.Log("Gra została zamknięta (działa tylko w buildzie)");
    }
}
