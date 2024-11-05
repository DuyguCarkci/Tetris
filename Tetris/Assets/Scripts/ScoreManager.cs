using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    public Text highScoreText; 
    private int score = 0;

    private void Start()
    {
        LoadScore(); 
    }

    public int CurrentScore 
    {
        get { return score; }
    }

    
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
        SaveScore(); 
    }

    
    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }

    
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        else
        {
            Debug.LogWarning("Score Text component is not assigned!");
        }

        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0);
        }
    }

    
    public void SaveScore()
    {
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }

   
    public void LoadScore()
    {
        score = PlayerPrefs.GetInt("HighScore", 0);
        UpdateScoreUI();
    }
}
