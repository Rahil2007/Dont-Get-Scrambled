using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    private float maxHealth,currentHealth, targetHealth;
    [SerializeField] private float width, height;
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Money")]
    private MoneyManager moneyManager;
    [SerializeField] private TextMeshProUGUI moneyText;

    [Header("PauseScreen")]
    [SerializeField]GameObject pauseScreen;
    private bool isPaused = false;

    [Header("StartScreen")]
    [SerializeField] GameObject startScreen;
    [SerializeField] GameObject HUD;
    [SerializeField] GameObject obstacles;
    [SerializeField] Button startButton;

    [Header("GameOverScreen")]
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] TextMeshProUGUI wavesSurvivedText;


    private WaveManager waveManager;
    void Awake()
    {
        if(obstacles != null)
            obstacles.SetActive(false);
        moneyManager = FindAnyObjectByType<MoneyManager>();
        waveManager = FindAnyObjectByType<WaveManager>();
        if(HUD != null)
            HUD.SetActive(false);
        if (startScreen != null)
            startScreen.SetActive(true);
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += SetHealth;
            playerHealth.OnMaxHealthChanged += UpdateHealthBar;
        }
        if (moneyManager != null)   
            moneyText.SetText($"${moneyManager.Money}");
    }
    void Start()
    {
        if (playerHealth != null)
        {
            maxHealth = playerHealth.MaxHealth;
            currentHealth = playerHealth.CurrentHealth;
            targetHealth = currentHealth;
        }
    }

    void Update()
    {
        if (!Mathf.Approximately(currentHealth, targetHealth))
        {    
            currentHealth = Mathf.Max(0, Mathf.MoveTowards(currentHealth, targetHealth, smoothSpeed * Time.deltaTime));
            float newWidth = (currentHealth / maxHealth) * width;
            newWidth = Mathf.Max(0f, newWidth);
            healthBar.sizeDelta = new Vector2(newWidth, height);
            healthText.SetText($"{Mathf.CeilToInt(currentHealth):0}/{maxHealth:0}");
        }
        if(Input.GetKeyDown(KeyCode.Escape))
            Pause();
    }

    void SetHealth()
    {
        targetHealth = playerHealth.CurrentHealth;
    }

    void UpdateHealthBar()
    {
        maxHealth = playerHealth.MaxHealth;
        currentHealth = maxHealth;
        targetHealth = maxHealth;
        float newWidth = (currentHealth / maxHealth) * width;
        newWidth = Mathf.Max(0f, newWidth);
        healthBar.sizeDelta = new Vector2(newWidth, height);
        healthText.SetText($"{currentHealth:0}/{maxHealth:0}");
    }

    public void UpdateMoneyText()
    {
        if (moneyManager != null)
            moneyText.SetText($"${moneyManager.Money}");
    }

    void OnDisable()
    {
        if (playerHealth != null)
        { 
            playerHealth.OnHealthChanged -= SetHealth;
            playerHealth.OnMaxHealthChanged -= UpdateHealthBar;
        }
    }

    void Pause()
    {
        if(startScreen.activeSelf || gameOverScreen.activeSelf)
            return;
        isPaused = !isPaused;
        pauseScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void StartGame()
    {
        startButton.interactable = false;
        startScreen.GetComponent<Animator>().SetTrigger("Fade");
        obstacles.SetActive(true);
        HUD.SetActive(true);
        obstacles.GetComponent<Animator>().SetTrigger("Slide");
        HUD.GetComponent<Animator>().SetTrigger("Fade");
        Invoke("DisableStartScreen", 1f);
    }
    
    void DisableStartScreen()
    {
        startScreen.SetActive(false);
        waveManager.StartGame();
    }

    public void GameOver()
    {
        waveManager.CalculateScore();
        gameOverScreen.SetActive(true);
        gameOverScreen.GetComponent<Animator>().SetTrigger("Slide");
        finalScoreText.SetText($"{waveManager.Score}");
        wavesSurvivedText.SetText($"{waveManager.WaveNumber - 1}");
        Invoke("PauseTime", 1f);           
    }

    void PauseTime()
    {
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
