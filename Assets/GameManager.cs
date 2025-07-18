using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //these are deleted when reloading the scene
    [SerializeField] private Text TimerText;
    [SerializeField] private GameObject WinScreen;

    [SerializeField] private float countdownTime = 30f; // Initial countdown time in seconds
    public static GameManager Instance;
    [SerializeField] private float currentTime;
    [SerializeField] private bool isGameOver = false;

    void Awake()
    {
        // Make sure there is only one GameManager and persist it across scenes if necessary
        if (Instance == null)
        {
            Instance = this; // Set the singleton instance
            //DontDestroyOnLoad(gameObject); // Optional: Keep GameManager across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy any extra GameManager instances
        }
        //Time.timeScale = 1;
    }

    void Start()
    {
        Time.timeScale = 1f;
        currentTime = countdownTime;
        UpdateTimerText();
    }

    void Update()
    {
        if (isGameOver) return;

        // Decrease timer
        currentTime -= Time.deltaTime;

        // Check if timer hits zero
        if (currentTime <= 0)
        {
            currentTime = 0;
            isGameOver = true;
            Time.timeScale = 0;
            WinScreen.SetActive(true);
            if (RewardedAdsButton.Instance != null)
            {
                RewardedAdsButton.Instance.LoadAd();
            }
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        TimerText.text = $"{minutes:00}:{seconds:00}";
    }

    // Add time to the countdown timer
    public void AddTime(float timeToAdd)
    {
        currentTime += timeToAdd;
        Debug.Log($"Added {timeToAdd} seconds. Current time: {currentTime}");
        WinScreen.SetActive(false);
        Time.timeScale = 1;
        isGameOver = false;
    }

    // Reset the game when the timer reaches zero
    public void ResetGame()
    {
        Debug.Log("Game Over! Resetting...");
        // Reset logic (e.g., reload the scene, reset variables, etc.)
        Time.timeScale = 1;
        currentTime = countdownTime;
        isGameOver = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
