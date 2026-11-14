/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  EnemySpawner.cs
- DESCRIPTION: Spawns enemies from an object pool along the top or bottom
              wall, whichever is farther from the player, every few seconds,
              with only a limited number alive at once. Defeated enemies go back
              into the pool, and the spawner announces each defeat so the
              Game Manager can add score.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/14 | David Cipriano | Created Script
- 2026/11/14 | David Cipriano | Spawn along the top or bottom wall instead of spawn points
- 2026/11/14 | David Cipriano | Fixed spawn rate (simplified)

************************************************************/

using System;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyBrain enemyPrefab;
    [SerializeField] private Transform target;
    [SerializeField] private Transform topSpawnLine;
    [SerializeField] private Transform bottomSpawnLine;
    [SerializeField] private float spawnLineHalfWidth = 8f;

    [SerializeField] private float secondsBetweenSpawns = 2f;
    [SerializeField] private int maxAliveEnemies = 8;

    public event Action<EnemyBrain> EnemyDefeated;

    private ObjectPool<EnemyBrain> enemyPool;
    private float secondsUntilNextSpawn;
    private Vector3 nextSpawnPosition;

    // ---------- Unity Events ----------

    // Build the pool of reusable enemies once at startup
    private void Awake()
    {
        enemyPool = new ObjectPool<EnemyBrain>(
            CreateEnemy,
            null,
            ReturnEnemyToPool,
            DestroyEnemy,
            collectionCheck: false,
            defaultCapacity: maxAliveEnemies,
            maxSize: maxAliveEnemies * 2);
    }

    // Spawn an enemy whenever the timer runs out and there's room
    private void Update()
    {
        secondsUntilNextSpawn -= Time.deltaTime;

        if (secondsUntilNextSpawn <= 0f && enemyPool.CountActive < maxAliveEnemies)
        {
            SpawnEnemy();
            secondsUntilNextSpawn = secondsBetweenSpawns;
        }
    }

    // ---------- Spawning ----------

    private void SpawnEnemy()
    {
        nextSpawnPosition = ChooseSpawnPosition();

        EnemyBrain enemy = enemyPool.Get();
        enemy.transform.position = nextSpawnPosition;
        enemy.SetTarget(target);
        enemy.gameObject.SetActive(true);
    }

    private Vector3 ChooseSpawnPosition()
    {
        Transform spawnLine = FartherFromTarget(topSpawnLine, bottomSpawnLine);
        float offsetAlongLine = UnityEngine.Random.Range(-spawnLineHalfWidth, spawnLineHalfWidth);
        return spawnLine.position + spawnLine.right * offsetAlongLine;
    }

    private Transform FartherFromTarget(Transform first, Transform second)
    {
        float firstDistance = Vector3.Distance(first.position, target.position);
        float secondDistance = Vector3.Distance(second.position, target.position);
        return firstDistance >= secondDistance ? first : second;
    }

    // ---------- Defeat ----------

    private void HandleEnemyDefeated(EnemyBrain enemy)
    {
        EnemyDefeated?.Invoke(enemy);
        enemyPool.Release(enemy);
    }

    // ---------- Object Pool ----------

    private EnemyBrain CreateEnemy()
    {
        EnemyBrain enemy = Instantiate(enemyPrefab, nextSpawnPosition, Quaternion.identity, transform);
        enemy.Defeated += HandleEnemyDefeated;
        return enemy;
    }

    private static void ReturnEnemyToPool(EnemyBrain enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private static void DestroyEnemy(EnemyBrain enemy)
    {
        if (enemy != null)
        {
            Destroy(enemy.gameObject);
        }
    }
}
