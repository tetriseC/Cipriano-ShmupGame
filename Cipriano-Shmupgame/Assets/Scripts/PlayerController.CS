/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  PlayerController.cs
- DESCRIPTION: Moves the player around the arena with WASD and aims the
              player with the arrow keys using the New Input System.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/13 | David Cipriano | Created Script
- 2026/11/13 | David Cipriano | Arrow-key aiming with diagonal settle time

************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private const float MinimumDirectionAmount = 0.01f;
    private const float SameDirectionDegrees = 5f;

    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference aimInput;

    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float aimSettleSeconds = 0.05f;

    private Rigidbody body;
    private Vector3 heldAimDirection;
    private float secondsAimHeld;

    // ---------- Unity Events ----------

    // Find the Rigidbody once at startup
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Start listening to input when the player turns on
    private void OnEnable()
    {
        moveInput.action.Enable();
        aimInput.action.Enable();
    }

    // Stop listening to input when the player turns off
    private void OnDisable()
    {
        moveInput.action.Disable();
        aimInput.action.Disable();
    }

    // Move every physics step, then turn once the aim has been held long enough
    private void FixedUpdate()
    {
        Vector2 movement = moveInput.action.ReadValue<Vector2>();
        body.linearVelocity = ConvertInputToFloorDirection(movement) * moveSpeed;

        
    }

    void Update()
    {
        Vector2 aim = aimInput.action.ReadValue<Vector2>();
        TrackHowLongAimIsHeld(ConvertInputToFloorDirection(aim));

        if (HasAimBeenHeldLongEnough())
        {
            body.rotation = Quaternion.LookRotation(heldAimDirection);
        }
    }

    // ---------- Aiming ----------

    private void TrackHowLongAimIsHeld(Vector3 aimDirection)
    {
        if (ArePointingSameWay(aimDirection, heldAimDirection))
        {
            secondsAimHeld += Time.fixedDeltaTime;
        }
        else
        {
            heldAimDirection = aimDirection;
            secondsAimHeld = 0f;
        }
    }

    private bool HasAimBeenHeldLongEnough()
    {
        return IsLongEnoughToAim(heldAimDirection) && secondsAimHeld >= aimSettleSeconds;
    }

    // ---------- Direction Helpers ----------

    private static Vector3 ConvertInputToFloorDirection(Vector2 input)
    {
        return new Vector3(input.x, 0f, input.y);
    }

    private static bool IsLongEnoughToAim(Vector3 direction)
    {
        return direction.sqrMagnitude > MinimumDirectionAmount;
    }

    private static bool ArePointingSameWay(Vector3 first, Vector3 second)
    {
        if (!IsLongEnoughToAim(first) || !IsLongEnoughToAim(second))
        {
            return IsLongEnoughToAim(first) == IsLongEnoughToAim(second);
        }

        return Vector3.Angle(first, second) < SameDirectionDegrees;
    }
}