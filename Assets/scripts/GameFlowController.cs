using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFlowController : MonoBehaviour
{
    public static GameFlowController Instance { get; private set; }

    [Header("Gameplay")]
    [SerializeField] PlayerMove playerMove;
    [SerializeField] float gameOverDelay = 1.25f;

    [Header("Screens")]
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject gameplayHud;
    [SerializeField] Button startButton;
    [SerializeField] Button restartButton;

    [Header("Readouts")]
    [SerializeField] TMP_Text distanceText;
    [SerializeField] TMP_Text finalScoreText;

    bool isPlaying;
    bool gameOverRequested;

    public bool IsPlaying => isPlaying;

    void Awake()
    {
        Instance = this;
        StatControl.coinCount = 0;
        Time.timeScale = 0f;

        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        SetActive(startPanel, true);
        SetActive(gameOverPanel, false);
        SetActive(gameplayHud, false);
    }

    void Update()
    {
        if (playerMove == null)
        {
            return;
        }

        if (distanceText != null)
        {
            distanceText.text = Mathf.FloorToInt(playerMove.DistanceTravelled) + " m";
        }

    }

    public void StartGame()
    {
        if (isPlaying)
        {
            return;
        }

        StatControl.coinCount = 0;
        gameOverRequested = false;
        isPlaying = true;
        playerMove?.BeginRun();

        SetActive(startPanel, false);
        SetActive(gameOverPanel, false);
        SetActive(gameplayHud, true);
        Time.timeScale = 1f;
    }

    public void EndGame()
    {
        if (!isPlaying || gameOverRequested)
        {
            return;
        }

        gameOverRequested = true;
        isPlaying = false;
        StartCoroutine(ShowGameOverAfterDeath());
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator ShowGameOverAfterDeath()
    {
        yield return new WaitForSecondsRealtime(gameOverDelay);

        if (finalScoreText != null && playerMove != null)
        {
            finalScoreText.text = "Distance  " + Mathf.FloorToInt(playerMove.DistanceTravelled) +
                                  " m\nCoins  " + StatControl.coinCount;
        }

        SetActive(gameplayHud, false);
        SetActive(gameOverPanel, true);
        Time.timeScale = 0f;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        Time.timeScale = 1f;
    }

    static void SetActive(GameObject target, bool value)
    {
        if (target != null)
        {
            target.SetActive(value);
        }
    }
}
