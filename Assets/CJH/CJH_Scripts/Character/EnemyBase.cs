using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IEnemyActor
{
    public int maxHP = 1000;
    public int currentHP;

    [SerializeField] protected EnemyType type;
    public EnemyType Type => type;

    public bool IsDead => currentHP <= 0;

    private void Awake()
    {
        currentHP = maxHP;  // 스폰 직후 현재체력을 최대치로 셋팅 
    }

    protected virtual void Start()
    {
        TurnManager.Instance.RegisterEnemy(this);

        // 씬 내에 UI가 준비되어 있다면, 바로 초기화
        if (GameStatusUI.Instance != null)
            GameStatusUI.Instance.SetTarget(this);
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