/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  Shooter.cs
- DESCRIPTION: Reusable shooting component. Any controller (player or
              enemy) calls TryShoot() to fire a projectile from the
              muzzle, limited by a cooldown between shots. Projectiles
              come from an object pool and are reused instead of destroyed.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/13 | David Cipriano | Created Script
- 2026/11/13 | David Cipriano | Fixed cooldown reset
- 2026/11/13 | David Cipriano | Projectiles reused through an object pool

************************************************************/

using UnityEngine;
using UnityEngine.Pool;

public class Shooter : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float cooldown = 0.15f;

    [SerializeField] private int poolStartSize = 20;
    [SerializeField] private int poolMaxSize = 100;

    private float cooldownRemaining;
    private ObjectPool<Projectile> projectilePool;

    // ---------- Unity Events ----------

    // Build the pool of reusable projectiles once at startup
    private void Awake()
    {
        projectilePool = new ObjectPool<Projectile>(
            CreateProjectile,
            TakeProjectileFromPool,
            ReturnProjectileToPool,
            DestroyProjectile,
            collectionCheck: false,
            defaultCapacity: poolStartSize,
            maxSize: poolMaxSize);
    }

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
        Projectile projectile = projectilePool.Get();
        projectile.transform.SetPositionAndRotation(muzzle.position, muzzle.rotation);
        projectile.Launch(muzzle.forward);
    }

    // ---------- Object Pool ----------

    private Projectile CreateProjectile()
    {
        Projectile projectile = Instantiate(projectilePrefab);
        projectile.SetPool(projectilePool);
        return projectile;
    }

    private static void TakeProjectileFromPool(Projectile projectile)
    {
        projectile.gameObject.SetActive(true);
    }

    private static void ReturnProjectileToPool(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    private static void DestroyProjectile(Projectile projectile)
    {
        Destroy(projectile.gameObject);
    }
}
