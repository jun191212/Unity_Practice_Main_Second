using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int totalItemsToCollect = 10;
    public float gameTimeLimit = 120f;
    public bool useTimeLimit = false;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public GameObject clearPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;

    private int currentScore = 0;
    private int itemsCollected = 0;
    private float remainingTime;
    private bool isGameOver = false;
    private int highScore = 0;

    private const string HIGH_SCORE_KEY = "HighScore";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        remainingTime = gameTimeLimit;
        LoadHighScore();
        UpdateUI();

        if (clearPanel != null)
            clearPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
     
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitGame();
            }
     

        if (useTimeLimit)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0)
            {
                remainingTime = 0;
                GameOver();
            }

            UpdateTimerUI();
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        itemsCollected++;
        UpdateUI();

        if (itemsCollected >= totalItemsToCollect)
        {
            GameClear();
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = itemsCollected + " / " + totalItemsToCollect;
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null && useTimeLimit)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (remainingTime <= 10f)
            {
                timerText.color = Color.red;
            }
        }
    }

    void GameClear()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
        }

        if (clearPanel != null)
        {
            clearPanel.SetActive(true);

            TextMeshProUGUI[] texts = clearPanel.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in texts)
            {
                if (text.name == "FinalScoreText")
                {
                    text.text = "Score: " + currentScore;
                }
                else if (text.name == "HighScoreText")
                {
                    text.text = "High Score: " + highScore;
                }
            }
        }
    }

    void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }

    void SaveHighScore()
    {
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
        PlayerPrefs.Save();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public int GetItemsCollected()
    {
        return itemsCollected;
    }

    public int GetTotalItems()
    {
        return totalItemsToCollect;
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}