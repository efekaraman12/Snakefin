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
            GameManager.Instance.InitializeGameScene(); // Sahne ba�lat�l�yor
            GameManager.Instance.AdjustGameArea(); // Duvarlar ve UI ayarlan�yor
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game exited.");
    }
}
