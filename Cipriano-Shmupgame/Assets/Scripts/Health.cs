/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  Health.cs
- DESCRIPTION: Reusable health component for anything that can be hurt
              (player or enemy). Announces changes and death through
              events so other scripts (HUD, Game Manager, enemy logic)
              can react without Health knowing about them.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/13 | David Cipriano | Created Script

************************************************************/

using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    public event Action<int, int> HealthChanged;
    public event Action Died;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;
    public bool IsDead => CurrentHealth <= 0;

    // ---------- Unity Events ----------

    // Start at full health whenever this object turns on (also resets pooled enemies)
    private void OnEnable()
    {
        CurrentHealth = maxHealth;
        HealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    // ---------- Changing Health ----------

    public void TakeDamage(int amount)
    {
        if (IsDead)
        {
            return;
        }

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
        HealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (IsDead)
        {
            Died?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead)
        {
            return;
        }

        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        HealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}
