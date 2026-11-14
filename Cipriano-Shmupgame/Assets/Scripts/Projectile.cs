/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  Projectile.cs
- DESCRIPTION: Flies in a straight line after launch and goes back to
              its object pool when it hits something or runs out of lifetime.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/13 | David Cipriano | Created Script
- 2026/11/13 | David Cipriano | Returns to an object pool instead of being destroyed

************************************************************/

using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 1;

    private Rigidbody body;
    private IObjectPool<Projectile> pool;
    private float lifetimeRemaining;

    // ---------- Unity Events ----------

    // Find the Rigidbody once at startup
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Count the lifetime down and go back to the pool when it runs out
    private void Update()
    {
        lifetimeRemaining -= Time.deltaTime;

        if (lifetimeRemaining <= 0f)
        {
            ReturnToPool();
        }
    }

    // Go back to the pool when touching anything this layer can hit
    private void OnTriggerEnter(Collider other)
{
    Health targetHealth = other.GetComponentInParent<Health>();

    if (targetHealth != null)
    {
        targetHealth.TakeDamage(damage);
    }

    ReturnToPool();
}

    // ---------- Launching ----------

    public void SetPool(IObjectPool<Projectile> owningPool)
    {
        pool = owningPool;
    }

    public void Launch(Vector3 direction)
    {
        body.linearVelocity = direction.normalized * speed;
        lifetimeRemaining = lifetime;
    }

    // ---------- Returning ----------

    private void ReturnToPool()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        if (pool == null)
        {
            Destroy(gameObject);
            return;
        }

        pool.Release(this);
    }
}
