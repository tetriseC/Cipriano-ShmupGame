/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  EnemyBrain.cs
- DESCRIPTION: The enemy's "brain". Each time it spawns it picks a random
              enemy type (Scout, Grunt, Brute, defined below) and a random
              firing piece (any FiringBehavior attached to the enemy),
              and applies the type's health, speed, size, and color. Each
              frame it chases the target, faces it (snapped to 45-degree
              steps, like the player), and asks the firing
              piece whether to shoot. When its Health runs out it announces
              that it was defeated and switches itself off.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/13 | David Cipriano | Created Script
- 2026/11/14 | David Cipriano | Single chase movement; random firing piece only
- 2026/11/14 | David Cipriano | Random enemy types applied in code on one prefab
- 2026/11/14 | David Cipriano | Aim snaps to 45-degree steps like the player
- 2026/11/14 | David Cipriano | Slower default enemy speeds
- 2026/11/14 | David Cipriano | Types defined in code; firing pieces found automatically
- 2026/11/14 | David Cipriano | Size only widens the enemy so it stays on the floor
- 2026/11/14 | David Cipriano | NavMeshAgent moves the enemy; no CharacterBody

************************************************************/

using System;
using UnityEngine;

[RequireComponent(typeof(Health), typeof(ChaseMovement))]
public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Shooter shooter;
    [SerializeField] private Renderer bodyRenderer;
    [SerializeField] private float aimStepDegrees = 45f;

    public event Action<EnemyBrain> Defeated;

    public int PointValue => currentType.pointValue;

    private static readonly EnemyType[] TypeOptions =
    {
        new EnemyType("Scout", 1, 2f, 50, 0.7f, Color.yellow),
        new EnemyType("Grunt", 2, 1.4f, 100, 1f, Color.orange),
        new EnemyType("Brute", 3, 0.9f, 200, 1.4f, Color.red),
    };

    private const float MinimumDirectionAmount = 0.01f;

    private Health health;
    private ChaseMovement chase;
    private FiringBehavior[] firingOptions;
    private EnemyType currentType;
    private FiringBehavior firing;

    // ---------- Unity Events ----------

    // Find Health, chase movement, and every attached firing piece once at startup
    private void Awake()
    {
        health = GetComponent<Health>();
        chase = GetComponent<ChaseMovement>();
        firingOptions = GetComponents<FiringBehavior>();
    }

    // Roll a random type and firing piece, and start listening for death, each time the enemy turns on
    private void OnEnable()
    {
        Configure(PickRandom(TypeOptions), PickRandom(firingOptions));
        health.Died += HandleDied;
    }

    // Stop listening for death when the enemy turns off
    private void OnDisable()
    {
        health.Died -= HandleDied;
    }

    // Chase, face, and fire every frame
    private void Update()
    {
        if (target == null)
        {
            return;
        }

        // ---------- Moving ----------

        chase.ChaseTowards(target.position);

        // ---------- Aiming ----------

        Face(DirectionToTarget());

        // ---------- Firing ----------

        if (firing.WantsToFire(Time.deltaTime) && shooter.TryShoot())
        {
            firing.NotifyShotFired();
        }
    }

    // ---------- Choosing Type and Firing ----------

    public void Configure(EnemyType chosenType, FiringBehavior chosenFiring)
    {
        currentType = chosenType;
        firing = chosenFiring;
        ApplyType(chosenType);
    }

    private void ApplyType(EnemyType type)
    {
        health.SetMaxHealthAndRefill(type.maxHealth);
        chase.SetSpeed(type.moveSpeed);
        transform.localScale = new Vector3(type.size, 1f, type.size);
        bodyRenderer.material.color = type.color;
    }

    private static T PickRandom<T>(T[] options)
    {
        return options[UnityEngine.Random.Range(0, options.Length)];
    }

    // ---------- Targeting ----------

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private Vector3 DirectionToTarget()
    {
        return SnapToAimStep(target.position - transform.position);
    }

    private Vector3 SnapToAimStep(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(angle / aimStepDegrees) * aimStepDegrees;
        return Quaternion.Euler(0f, snappedAngle, 0f) * Vector3.forward;
    }

    private void Face(Vector3 direction)
    {
        if (direction.sqrMagnitude > MinimumDirectionAmount)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    // ---------- Dying ----------

    private void HandleDied()
    {
        Defeated?.Invoke(this);
        gameObject.SetActive(false);
    }
}
