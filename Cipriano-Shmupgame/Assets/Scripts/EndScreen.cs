/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  EndScreen.cs
- DESCRIPTION: Shows how the last round ended, the score, and the best
              score, and lets the player replay or go back to the menu.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/14 | David Cipriano | Created Script

************************************************************/

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;

    [SerializeField] private string levelSceneName = "L_Level01";
    [SerializeField] private string menuSceneName = "L_MainMenu_UI";

    // ---------- Unity Events ----------

    // Fill in the results from the round that just ended
    private void Start()
    {
        ShowResult(GameManager.LastResult);
        ShowScores(GameManager.LastScore, GameManager.LastHighScore);
    }

    // ---------- Displaying ----------

    private void ShowResult(GameResult result)
    {
        resultText.text = result == GameResult.Victory ? "Victory!" : "Defeat";
    }

    private void ShowScores(int score, int highScore)
    {
        scoreText.text = $"Score {score}";
        highScoreText.text = $"Best {highScore}";
    }

    // ---------- Buttons ----------

    public void PlayAgain()
    {
        SceneManager.LoadScene(levelSceneName);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
