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
        if (scene.buildIndex == 1) // Oyun sahnesi
        {
            if (GameManager.Instance != null)
            {
                // Kýsa bir gecikme ile initialize et
                Invoke("InitializeGameScene", 0.2f);
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void InitializeGameScene()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.InitializeGameScene();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}