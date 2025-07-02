using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyActor
{
    void TakeTurn();
    void ApplyDamage(int damage);
    bool IsDead { get; }
    EnemyType Type { get; }
}