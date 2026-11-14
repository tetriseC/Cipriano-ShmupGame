/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  CharacterBody.cs
- DESCRIPTION: Reusable "body" for anything that walks and turns on the
              arena floor. A brain (player input or enemy AI) tells it
              which way to move and which way to face as floor directions;
              the body does the actual Rigidbody movement and turning.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/13 | David Cipriano | Created Script (split out of PlayerBrain)
- 2026/11/13 | David Cipriano | Commands take floor directions directly
- 2026/11/14 | David Cipriano | SetMoveSpeed command; slower default speed

************************************************************/

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterBody : MonoBehaviour
{
    private const float MinimumDirectionAmount = 0.01f;

    [SerializeField] private float moveSpeed = 2.5f;

    private Rigidbody rigidBody;
    private Vector3 moveDirection;

    // ---------- Unity Events ----------

    // Find the Rigidbody once at startup
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Stop moving when switched off so a reused character doesn't drift
    private void OnDisable()
    {
        moveDirection = Vector3.zero;
    }

    // Move every physics step in the last direction the brain asked for
    private void FixedUpdate()
    {
        rigidBody.linearVelocity = moveDirection * moveSpeed;
    }

    // ---------- Commands ----------

    public void Move(Vector3 direction)
    {
        Vector3 flatDirection = Flatten(direction);
        moveDirection = IsLongEnoughToUse(flatDirection) ? flatDirection.normalized : Vector3.zero;
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        moveSpeed = newMoveSpeed;
    }

    public void Face(Vector3 direction)
    {
        Vector3 flatDirection = Flatten(direction);

        if (IsLongEnoughToUse(flatDirection))
        {
            transform.rotation = Quaternion.LookRotation(flatDirection);
        }
    }

    // ---------- Direction Helpers ----------

    public static bool IsLongEnoughToUse(Vector3 direction)
    {
        return direction.sqrMagnitude > MinimumDirectionAmount;
    }

    private static Vector3 Flatten(Vector3 direction)
    {
        return new Vector3(direction.x, 0f, direction.z);
    }
}
