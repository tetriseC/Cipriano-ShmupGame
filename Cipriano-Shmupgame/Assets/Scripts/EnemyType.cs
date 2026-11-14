/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  EnemyType.cs
- DESCRIPTION: The settings that make one kind of enemy different from
              another (Scout, Grunt, Brute). EnemyBrain applies one of
              these to the single enemy prefab when it spawns.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/14 | David Cipriano | Created Script

************************************************************/

using System;
using UnityEngine;

[Serializable]
public struct EnemyType
{
    public string name;
    public int maxHealth;
    public float moveSpeed;
    public int pointValue;
    public float size;
    public Color color;

    public EnemyType(string name, int maxHealth, float moveSpeed, int pointValue, float size, Color color)
    {
        this.name = name;
        this.maxHealth = maxHealth;
        this.moveSpeed = moveSpeed;
        this.pointValue = pointValue;
        this.size = size;
        this.color = color;
    }
}
