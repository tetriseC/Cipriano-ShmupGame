/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  GameManager.cs
- DESCRIPTION: Runs the survival timer, keeps the score and high score,
              and ends the round in Victory or Defeat.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/14 | David Cipriano | Created Script

************************************************************/

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string HighScoreKey = "HighScore";

    [SerializeField] private float roundSeconds = 120f;
    [SerializeField] private Health playerHealth;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private string endSceneName = "L_End";

    public event Action<float> TimeChanged;
    public event Action<int> ScoreChanged;
    public event Action<int> HighScoreChanged;
    public event Action<GameResult> RoundEnded;

    public static GameResult LastResult { get; private set; }
    public static int LastScore { get; private set; }

    public float SecondsRemaining { get; private set; }
    public int Score { get; private set; }
    public int HighScore { get; private set; }

    private bool roundIsOver;

    // ---------- Unity Events ----------

    // Start a fresh round with the saved high score
    private void Awake()
    {
        SecondsRemaining = roundSeconds;
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    // Listen for the player dying and enemies being defeated
    private void OnEnable()
    {
        playerHealth.Died += HandlePlayerDied;
        enemySpawner.EnemyDefeated += HandleEnemyDefeated;
    }

    // Stop listening when this turns off
    private void OnDisable()
    {
        playerHealth.Died -= HandlePlayerDied;
        enemySpawner.EnemyDefeated -= HandleEnemyDefeated;
    }

    // Tell the HUD the starting values once everything has subscribed
    private void Start()
    {
        TimeChanged?.Invoke(SecondsRemaining);
        ScoreChanged?.Invoke(Score);
        HighScoreChanged?.Invoke(HighScore);
    }

    // Count the timer down and win when it runs out
    private void Update()
    {
        if (roundIsOver)
        {
            return;
        }

        SecondsRemaining = Mathf.Max(0f, SecondsRemaining - Time.deltaTime);
        TimeChanged?.Invoke(SecondsRemaining);

        if (SecondsRemaining <= 0f)
        {
            EndRound(GameResult.Victory);
        }
    }

    // ---------- Scoring ----------

    private void HandleEnemyDefeated(EnemyBrain enemy)
    {
        AddScore(enemy.PointValue);
    }

    private void AddScore(int points)
    {
        if (roundIsOver)
        {
            return;
        }

        Score += points;
        ScoreChanged?.Invoke(Score);

        if (Score > HighScore)
        {
            HighScore = Score;
            HighScoreChanged?.Invoke(HighScore);
        }
    }

    // ---------- Ending ----------

    private void HandlePlayerDied()
    {
        EndRound(GameResult.Defeat);
    }

    private void EndRound(GameResult result)
    {
        if (roundIsOver)
        {
            return;
        }

        roundIsOver = true;
        SaveHighScore();
        RememberRound(result);
        RoundEnded?.Invoke(result);
        SceneManager.LoadScene(endSceneName);
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HighScoreKey, HighScore);
        PlayerPrefs.Save();
    }

    private void RememberRound(GameResult result)
    {
        LastResult = result;
        LastScore = Score;
    }
}
