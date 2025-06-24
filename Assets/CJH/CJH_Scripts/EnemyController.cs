using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IEnemyActor
{
    private void Start()
    {
        Managers.Manager.turnManager.RegisterEnemy(this);
    }

    public void TakeTurn()
    {
        Debug.Log($"{gameObject.name}의 적 행동 발동!");
        // 적의 스킬이나 상태 처리 등 추가 가능
    }
}