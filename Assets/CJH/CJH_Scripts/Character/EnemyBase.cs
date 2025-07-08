using System;
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
    public Action OnMonsterDied;


    private void Awake()
    {
        currentHP = maxHP;  // 스폰 직후 현재체력을 최대치로 셋팅 
    }

    /// <summary>
    /// 전투 시작 전 체력·상태를 초기화한다.
    /// </summary>
    public void InitForBattle()
    {
        ResetStatus();
        // 추가 상태(버프·디버프 등) 초기화 시 여기에 작성하거라
    }

    private void ResetStatus()
    {
        currentHP = maxHP;
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
            OnMonsterDied?.Invoke();
        }
    }

    public abstract void TakeTurn();

    protected virtual void OnDestroy()
    {
        // 몬스터가 죽으면 등록 해제
        if (TurnManager.HasInstance)
            TurnManager.Instance.UnregisterEnemy(this);
    }

}