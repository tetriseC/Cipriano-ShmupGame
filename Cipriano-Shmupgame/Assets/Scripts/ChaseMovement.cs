/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  ChaseMovement.cs
- DESCRIPTION: Moves an enemy toward a target with a NavMeshAgent, which
              finds its own way around walls and stops at attack range.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/13 | David Cipriano | Created Script
- 2026/11/14 | David Cipriano | Agent moves the enemy directly (simplified)
- 2026/11/14 | David Cipriano | Agent assigned in Inspector so it exists before other scripts use it

************************************************************/

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ChaseMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float attackRange = 4f;

    // ---------- Unity Events ----------

    // Set up the agent once at startup; the brain handles turning
    private void Awake()
    {
        agent.updateRotation = false;
        agent.stoppingDistance = attackRange;
    }

    // ---------- Commands ----------

    public void ChaseTowards(Vector3 targetPosition)
    {
        agent.SetDestination(targetPosition);
    }

    public void SetSpeed(float speed)
    {
        agent.speed = speed;
    }
}
