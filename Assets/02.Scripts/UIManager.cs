using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameplayPanel;
    public GameObject pausePanel;

    [Header("Gameplay UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public Slider progressBar;

    [Header("Effects")]
    public Animator scoreAnimator;
    public AudioClip uiClickSound;

    private AudioSource audioSource;
    private bool isPaused = false;

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

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        ShowGameplayUI();

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        UpdateProgressBar();
    }

    public void ShowGameplayUI()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        PlayUISound();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        PlayUISound();
    }

    public void UpdateScoreDisplay(int current, int total)
    {
        if (scoreText != null)
        {
            scoreText.text = current + " / " + total;
        }

        if (scoreAnimator != null)
        {
            scoreAnimator.SetTrigger("Update");
        }
    }

    void UpdateProgressBar()
    {
        if (progressBar != null && GameManager.Instance != null)
        {
            float progress = (float)GameManager.Instance.GetItemsCollected() / GameManager.Instance.GetTotalItems();
            progressBar.value = progress;
        }
    }

    void PlayUISound()
    {
        if (audioSource != null && uiClickSound != null)
        {
            audioSource.PlayOneShot(uiClickSound);
        }
    }

    public void OnButtonClick()
    {
        PlayUISound();
    }
}