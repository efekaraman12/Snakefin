using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class SceneChanger : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SendGameSessionEvent(string eventName, string payload);

    public void StartGame()
    {
        SendGameSessionEvent("game_start", "{}");
        Debug.Log("Game started!");

        SceneManager.LoadScene("Snake");
    }

    public void ExitGame()
    {
        SendGameSessionEvent("game_exit", "{\"reason\": \"user_quit\"}");
        Debug.Log("Game exited!");

#if UNITY_WEBGL
        Application.ExternalEval("console.log('Game Quit Triggered');");
#else
        Application.Quit();
#endif
    }

    public void ReturnMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }



}