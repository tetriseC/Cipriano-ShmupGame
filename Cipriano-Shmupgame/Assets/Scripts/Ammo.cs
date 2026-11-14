/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  Ammo.cs
- DESCRIPTION: Reusable magazine-based ammunition. Shots come out of the
              current magazine; reloading swaps in a spare magazine after
              a short delay (leftover bullets are lost). Pickups add spare
              magazines. Announces changes through an event for the HUD.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/13 | David Cipriano | Created Script
- 2026/11/13 | David Cipriano | Magazines with timed reload

************************************************************/

using System;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] private int magazineSize = 10;
    [SerializeField] private int startingSpareMagazines = 2;
    [SerializeField] private int maxSpareMagazines = 5;
    [SerializeField] private float reloadSeconds = 1f;

    public event Action<int, int> AmmoChanged;

    public int BulletsInMagazine { get; private set; }
    public int SpareMagazines { get; private set; }
    public bool IsReloading => reloadRemaining > 0f;

    private float reloadRemaining;

    // ---------- Unity Events ----------

    // Start with a full magazine and the starting spares whenever this object turns on
    private void OnEnable()
    {
        BulletsInMagazine = magazineSize;
        SpareMagazines = startingSpareMagazines;
        reloadRemaining = 0f;
        AnnounceAmmo();
    }

    // Count the reload down and swap the magazine when it finishes
    private void Update()
    {
        if (!IsReloading)
        {
            return;
        }

        reloadRemaining -= Time.deltaTime;

        if (!IsReloading)
        {
            SwapInSpareMagazine();
        }
    }

    // ---------- Shooting ----------

    public bool TrySpend(int amount)
    {
        if (IsReloading || BulletsInMagazine < amount)
        {
            return false;
        }

        BulletsInMagazine -= amount;
        AnnounceAmmo();

        if (BulletsInMagazine == 0)
        {
            TryStartReload();
        }

        return true;
    }

    // ---------- Reloading ----------

    public bool TryStartReload()
    {
        if (!CanReload())
        {
            return false;
        }

        reloadRemaining = reloadSeconds;
        return true;
    }

    private bool CanReload()
    {
        return !IsReloading && SpareMagazines > 0 && BulletsInMagazine < magazineSize;
    }

    private void SwapInSpareMagazine()
    {
        SpareMagazines -= 1;
        BulletsInMagazine = magazineSize;
        AnnounceAmmo();
    }

    // ---------- Pickups ----------

    public void AddMagazines(int count)
    {
        SpareMagazines = Mathf.Min(SpareMagazines + count, maxSpareMagazines);
        AnnounceAmmo();

        if (BulletsInMagazine == 0)
        {
            TryStartReload();
        }
    }

    // ---------- Announcing ----------

    private void AnnounceAmmo()
    {
        AmmoChanged?.Invoke(BulletsInMagazine, SpareMagazines);
    }
}
