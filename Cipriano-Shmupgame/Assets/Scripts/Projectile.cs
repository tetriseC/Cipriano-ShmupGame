/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  Projectile.cs
- DESCRIPTION: Flies in a straight line after launch and removes itself
              when it hits something or runs out of lifetime.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/13 | David Cipriano | Created Script

************************************************************/

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 2f;

    private Rigidbody body;

    // ---------- Unity Events ----------

    // Find the Rigidbody once at startup
    private void Awake()
    {
        Debug.Log("Bullet layer: " + LayerMask.LayerToName(gameObject.layer)
    + " | ignores Default: " + Physics.GetIgnoreLayerCollision(gameObject.layer, 0));

        body = GetComponent<Rigidbody>();
    }

    // Disappear when touching anything this layer can hit
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bullet hit " + other.name);
        
        Destroy(gameObject);
    }

    // ---------- Launching ----------

    public void Launch(Vector3 direction)
    {
        body.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, lifetime);
    }
}
