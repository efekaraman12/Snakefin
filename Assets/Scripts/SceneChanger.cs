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
            GameManager.Instance.InitializeGameScene(); // Sahne baþlatýlýyor
            GameManager.Instance.AdjustGameArea(); // Duvarlar ve UI ayarlanýyor
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game exited.");
    }
}
