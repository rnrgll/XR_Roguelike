using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IEnemyActor
{
    private void Start()
    {
        //Managers.Manager.Turn.RegisterEnemy(this);
    }

    public void TakeTurn()
    {
        Debug.Log($"{gameObject.name}의 적 행동 발동!");
        // 적 행동 로직
    }
}