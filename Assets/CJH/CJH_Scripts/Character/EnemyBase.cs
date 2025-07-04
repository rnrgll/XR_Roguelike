using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IEnemyActor
{
    [SerializeField] protected int maxHP = 500;
    protected int currentHP;

    [SerializeField] protected EnemyType type;
    public EnemyType Type => type;

    public bool IsDead => currentHP <= 0;

    protected virtual void Start()
    {
        currentHP = maxHP;
        TurnManager.Instance.RegisterEnemy(this);
    }

    public virtual void ApplyDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);
        Debug.Log($"[{this.GetType().Name}] 피해 {damage}, 남은 체력: {currentHP}");

        if (IsDead)
        {
            Debug.Log($"[{this.GetType().Name}] 사망 처리됨");
        }
    }

    public abstract void TakeTurn();
}