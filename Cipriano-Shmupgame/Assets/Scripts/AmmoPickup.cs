/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  AmmoPickup.cs
- DESCRIPTION: Gives spare magazines to whoever touches it.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/14 | David Cipriano | Created Script

************************************************************/

using UnityEngine;

public class AmmoPickup : Pickup
{
    [SerializeField] private int magazineCount = 1;

    // ---------- Effect ----------

    protected override bool TryApplyTo(GameObject collector)
    {
        Ammo ammo = collector.GetComponentInChildren<Ammo>();

        if (ammo == null)
        {
            return false;
        }

        ammo.AddMagazines(magazineCount);
        return true;
    }
}
