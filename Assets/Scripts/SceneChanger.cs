using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            GameManager.Instance.InitializeGameScene(); // Yeni sahne ba�lat�l�yor
            GameManager.Instance.AdjustGameArea(); // Duvarlar ve UI g�ncelleniyor

            Debug.Log("Oyun sahnesi y�klendi.");
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game exited.");
    }


    public void ReturnToMainMenu()
    {
        int currentScore = ScoreManager.Instance.GetScore();

        // Highscore'u g�ncelle
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
        }

        SceneManager.LoadScene(0); // Ana men� sahnesi
    }
}
