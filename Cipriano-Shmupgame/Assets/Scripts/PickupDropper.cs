/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  PickupDropper.cs
- DESCRIPTION: Has a chance to drop a random pickup where this object dies.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/14 | David Cipriano | Created Script

************************************************************/

using UnityEngine;

[RequireComponent(typeof(Health))]
public class PickupDropper : MonoBehaviour
{
    [SerializeField] private Pickup[] pickupPrefabs;
    [SerializeField, Range(0f, 1f)] private float dropChance = 0.3f;

    private Health health;

    // ---------- Unity Events ----------

    // Find Health once at startup
    private void Awake()
    {
        health = GetComponent<Health>();
    }

    // Start listening for death when this turns on
    private void OnEnable()
    {
        health.Died += HandleDied;
    }

    // Stop listening when this turns off
    private void OnDisable()
    {
        health.Died -= HandleDied;
    }

    // ---------- Dropping ----------

    private void HandleDied()
    {
        if (Random.value < dropChance)
        {
            Instantiate(PickRandomPrefab(), transform.position, Quaternion.identity);
        }
    }

    private Pickup PickRandomPrefab()
    {
        return pickupPrefabs[Random.Range(0, pickupPrefabs.Length)];
    }
}
