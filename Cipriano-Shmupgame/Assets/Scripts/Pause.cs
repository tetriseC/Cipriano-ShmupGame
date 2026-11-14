/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  Pause.cs
- DESCRIPTION: Freezes and unfreezes the game with the pause key, and turns
              off player control while paused.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/14 | David Cipriano | Created Script

************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    [SerializeField] private InputActionReference pauseInput;
    [SerializeField] private PlayerBrain playerBrain;

    private bool isPaused;

    // ---------- Unity Events ----------

    // Start listening for the pause key
    private void OnEnable()
    {
        pauseInput.action.Enable();
    }

    // Stop listening for the pause key
    private void OnDisable()
    {
        pauseInput.action.Disable();
    }

    // Flip between paused and playing whenever the pause key is pressed
    private void Update()
    {
        if (pauseInput.action.WasPressedThisFrame())
        {
            TogglePause();
        }
    }

    // ---------- Pausing ----------

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        playerBrain.enabled = !isPaused;
    }
}
