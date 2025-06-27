using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnButton : MonoBehaviour
{
    public void OnClickEndTurn()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        player.EndTurn();
    }
    public void OnClickAttack()
    {
        EnemyController enemy = FindAnyObjectByType<EnemyController>();
        if (enemy != null && !enemy.IsDead)
        {
            enemy.ApplyDamage(100);
        }
        else
        {
            Debug.Log("이미 죽은 적입니다.");
        }
    }

}