using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : IEnemyBehavior
{
    public void Execute(PlayerController player)
    {
        int roll = Random.Range(0, 2);
        if (roll == 0)
        {
            Debug.Log("보스가 화염을 내뿜는다!");
            player.TakeDamage(20);
        }
        else
        {
            Debug.Log("보스가 어둠의 저주를 속삭인다.");
            player.TakeDamage(5);
        }
    }
}