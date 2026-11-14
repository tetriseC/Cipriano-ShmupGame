/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  HealthPickup.cs
- DESCRIPTION: Heals whoever touches it, but only if they are hurt.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/14 | David Cipriano | Created Script

************************************************************/

using UnityEngine;

public class HealthPickup : Pickup
{
    [SerializeField] private int healAmount = 1;

    // ---------- Effect ----------

    protected override bool TryApplyTo(GameObject collector)
    {
        Health health = collector.GetComponentInParent<Health>();

        if (health == null || !IsHurt(health))
        {
            return false;
        }

        health.Heal(healAmount);
        return true;
    }

    private static bool IsHurt(Health health)
    {
        return health.CurrentHealth < health.MaxHealth;
    }
}
