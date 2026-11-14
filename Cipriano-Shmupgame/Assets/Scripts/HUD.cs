/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  HUD.cs
- DESCRIPTION: Shows score, high score, time, health, and ammo on screen.
              Listens to events instead of checking every frame.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/14 | David Cipriano | Created Script

************************************************************/

using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Health playerHealth;
    [SerializeField] private Ammo playerAmmo;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text ammoText;

    // ---------- Unity Events ----------

    // Start listening for every value the HUD shows
    private void OnEnable()
    {
        gameManager.ScoreChanged += ShowScore;
        gameManager.HighScoreChanged += ShowHighScore;
        gameManager.TimeChanged += ShowTime;
        playerHealth.HealthChanged += ShowHealth;
        playerAmmo.AmmoChanged += ShowAmmo;
    }

    // Stop listening when the HUD turns off
    private void OnDisable()
    {
        gameManager.ScoreChanged -= ShowScore;
        gameManager.HighScoreChanged -= ShowHighScore;
        gameManager.TimeChanged -= ShowTime;
        playerHealth.HealthChanged -= ShowHealth;
        playerAmmo.AmmoChanged -= ShowAmmo;
    }

    // Fill in the starting values before anything has changed
    private void Start()
    {
        ShowScore(gameManager.Score);
        ShowHighScore(gameManager.HighScore);
        ShowTime(gameManager.SecondsRemaining);
        ShowHealth(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        ShowAmmo(playerAmmo.BulletsInMagazine, playerAmmo.SpareMagazines);
    }

    // ---------- Displaying ----------

    private void ShowScore(int score)
    {
        scoreText.text = $"Score {score}";
    }

    private void ShowHighScore(int highScore)
    {
        highScoreText.text = $"Best {highScore}";
    }

    private void ShowTime(float secondsRemaining)
    {
        int wholeSeconds = Mathf.CeilToInt(secondsRemaining);
        timeText.text = $"{wholeSeconds / 60}:{wholeSeconds % 60:00}";
    }

    private void ShowHealth(int currentHealth, int maxHealth)
    {
        healthText.text = $"Health {currentHealth}/{maxHealth}";
    }

    private void ShowAmmo(int bulletsInMagazine, int spareMagazines)
    {
        ammoText.text = $"Ammo {bulletsInMagazine}  Mags {spareMagazines}";
    }
}
