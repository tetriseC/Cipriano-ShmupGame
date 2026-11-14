/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  MovementBehavior.cs
- DESCRIPTION: Base for swappable enemy movement pieces. EnemyBrain asks
              its MovementBehavior which way to walk each frame.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/13 | David Cipriano | Created Script

************************************************************/

using UnityEngine;

public abstract class MovementBehavior : MonoBehaviour
{
    public abstract Vector3 ChooseMoveDirection(Vector3 targetPosition, float deltaTime);
}
