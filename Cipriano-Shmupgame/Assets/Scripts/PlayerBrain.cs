/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  PlayerBrain.cs
- DESCRIPTION: The player's "brain". Reads WASD, arrow keys, fire and
              reload from the New Input System, turns the keys into floor
              directions, and passes them on to the CharacterBody (moving,
              facing), Shooter, and Ammo.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/13 | David Cipriano | Created Script
- 2026/11/13 | David Cipriano | Arrow-key aiming with diagonal settle time
- 2026/11/13 | David Cipriano | Firing through Shooter; aiming moved to Update
- 2026/11/13 | David Cipriano | Reload input
- 2026/11/13 | David Cipriano | Renamed to PlayerBrain; movement and turning moved into CharacterBody
- 2026/11/14 | David Cipriano | Removed aim settle time

************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterBody))]
public class PlayerBrain : MonoBehaviour
{
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference aimInput;
    [SerializeField] private InputActionReference fireInput;
    [SerializeField] private InputActionReference reloadInput;

    [SerializeField] private Shooter shooter;
    [SerializeField] private Ammo ammo;

    private CharacterBody body;

    // ---------- Unity Events ----------

    // Find the body once at startup
    private void Awake()
    {
        body = GetComponent<CharacterBody>();
    }

    // Start listening to input when the player turns on
    private void OnEnable()
    {
        moveInput.action.Enable();
        aimInput.action.Enable();
        fireInput.action.Enable();
        reloadInput.action.Enable();
    }

    // Stop listening to input when the player turns off
    private void OnDisable()
    {
        moveInput.action.Disable();
        aimInput.action.Disable();
        fireInput.action.Disable();
        reloadInput.action.Disable();
    }

    // Read input every frame and hand it to the body, shooter, and ammo
    private void Update()
    {
        // ---------- Moving ----------

        body.Move(ConvertKeysToFloorDirection(moveInput.action.ReadValue<Vector2>()));

        // ---------- Aiming ----------

        body.Face(ConvertKeysToFloorDirection(aimInput.action.ReadValue<Vector2>()));

        // ---------- Firing ----------

        if (fireInput.action.IsPressed())
        {
            shooter.TryShoot();
        }

        // ---------- Reloading ----------

        if (reloadInput.action.WasPressedThisFrame())
        {
            ammo.TryStartReload();
        }
    }

    // ---------- Direction Helpers ----------

    private static Vector3 ConvertKeysToFloorDirection(Vector2 keys)
    {
        return new Vector3(keys.x, 0f, keys.y);
    }
}
