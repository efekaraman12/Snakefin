using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshPro kütüphanesi
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score;

    [SerializeField] private TextMeshProUGUI scoreText; // TextMeshPro UI öðesi

    private void Awake()
    {
        // Ensure there is only one instance of ScoreManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
        Debug.Log("Score: " + score);
    }

    public int GetScore()
    {
        return score;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}

