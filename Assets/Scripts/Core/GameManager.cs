using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Referensi")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI highScoreText;
    
    [Header("UI Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI resultTimeText; 

    [Header("Status Permainan")]
    public bool isGameOver = false;
    private float survivalTime = 0f;
    private float highScore = 0f;

    void Start()
    {
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f; 

        highScore = PlayerPrefs.GetFloat("HighScore_PinkEmber", 0f);
        UpdateHighScoreUI();
    }

    void Update()
    {
        if (!isGameOver)
        {
            survivalTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(survivalTime / 60F);
        int seconds = Mathf.FloorToInt(survivalTime % 60F);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void UpdateHighScoreUI()
    {
        int minutes = Mathf.FloorToInt(highScore / 60F);
        int seconds = Mathf.FloorToInt(highScore % 60F);
        highScoreText.text = "Best: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void GameOver()
    {
        isGameOver = true;

        if (survivalTime > highScore)
        {
            highScore = survivalTime;
            PlayerPrefs.SetFloat("HighScore_PinkEmber", highScore);
            PlayerPrefs.Save();
            UpdateHighScoreUI();
            Debug.Log("Rekor Baru Tercipta!");
        }
        gameOverPanel.SetActive(true);

        if (resultTimeText != null)
        {
             int minutes = Mathf.FloorToInt(survivalTime / 60F);
             int seconds = Mathf.FloorToInt(survivalTime % 60F);
             resultTimeText.text = "Waktu Bertahan:\n" + string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        Time.timeScale = 0f; 
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}