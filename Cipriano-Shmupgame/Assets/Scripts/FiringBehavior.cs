/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  FiringBehavior.cs
- DESCRIPTION: Base for swappable enemy firing pieces. EnemyBrain asks
              its FiringBehavior whether it wants to shoot this frame, and
              tells it when a shot actually fired.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/13 | David Cipriano | Created Script

************************************************************/

using UnityEngine;

public abstract class FiringBehavior : MonoBehaviour
{
    public abstract bool WantsToFire(float deltaTime);

    public virtual void NotifyShotFired()
    {
    }
}
