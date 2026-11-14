/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  Pickup.cs
- DESCRIPTION: Base for anything the player can pick up. Applies its effect
              when touched and disappears, but only if the effect was used.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/14 | David Cipriano | Created Script

************************************************************/

using System;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    public event Action<Pickup> Collected;

    // ---------- Unity Events ----------

    // Try to give the effect to whatever touched it, and disappear only if it worked
    private void OnTriggerEnter(Collider other)
    {
        if (!TryApplyTo(other.gameObject))
        {
            return;
        }

        Collected?.Invoke(this);
        Destroy(gameObject);
    }

    // ---------- Effect ----------

    protected abstract bool TryApplyTo(GameObject collector);
}
