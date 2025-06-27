using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IEnemyActor
{
    [SerializeField] private EnemyType enemyType;
    public EnemyType Type => enemyType;

    [SerializeField] private int maxHP = 100;
    private int currentHP;
    public bool IsDead { get; private set; }

    private IEnemyBehavior behavior;



    private void Start()
    {
        currentHP = maxHP;
        AssignBehavior(); // 타입에 따라 행동 클래스 결정
        Managers.Manager.turnManager.RegisterEnemy(this);
    }

    //ToDo 추후 기획의 몹 컨셉이 잡히게 되면 진행
    private void AssignBehavior()
    {
        behavior = enemyType switch
        {

            EnemyType.Boss => new BossBehavior(),
           _ => null
        };
    }

    public void TakeTurn()
    {
        var player = FindAnyObjectByType<PlayerController>();
        if (player == null || player.IsDead || behavior == null) return;

        Debug.Log($"{gameObject.name}({enemyType})의 턴 발동!");
        behavior.Execute(player);
    }

    public void ApplyDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"{gameObject.name} 피해: {damage}, 남은 체력: {currentHP}");

        if (currentHP <= 0)
        {
            Debug.Log($"{gameObject.name} 사망!");
            IsDead = true;
            Destroy(gameObject);
        }
    }
}