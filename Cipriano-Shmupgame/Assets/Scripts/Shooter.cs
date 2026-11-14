/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  Shooter.cs
- DESCRIPTION: Reusable shooting component. Any controller (player or
              enemy) calls TryShoot() to fire a projectile from the
              muzzle, limited by a cooldown between shots.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/13 | David Cipriano | Created Script
- 2026/11/13 | David Cipriano | Fixed cooldown reset

************************************************************/

using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float cooldown = 0.15f;

    private float cooldownRemaining;

    // ---------- Unity Events ----------

    // Count the cooldown down every frame
    private void Update()
    {
        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -= Time.deltaTime;
        }
    }

    // ---------- Shooting ----------

    public bool TryShoot()
    {
        if (!IsReadyToShoot())
        {
            return false;
        }

        SpawnProjectileAtMuzzle();
        cooldownRemaining = cooldown;
        return true;
    }

    private bool IsReadyToShoot()
    {
        return cooldownRemaining <= 0f;
    }

    private void SpawnProjectileAtMuzzle()
    {
        Projectile projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        projectile.Launch(muzzle.forward);
    }
}
