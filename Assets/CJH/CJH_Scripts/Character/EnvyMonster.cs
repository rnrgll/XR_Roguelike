using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvyMonster : EnemyBase
{
    private int turn = 1;
    private bool isCharging = false;
    private int chargeLevel = 0;
    private int chargeDamageReceived = 0;

    public override void TakeTurn()
    {
        StartCoroutine(TurnRoutine());
    }

    private IEnumerator TurnRoutine()
    {
        if (isCharging)
        {
            yield return ExecuteCharge();
            turn++;
            yield break;
        }

        if (turn == 3)
        {
            isCharging = true;
            chargeLevel = 1;
            chargeDamageReceived = 0;
            Debug.Log("[엔비] 차지 공격 준비!");
            yield break;
        }

        if (turn == 5)
        {
            isCharging = true;
            chargeLevel = 2;
            chargeDamageReceived = 0;
            Debug.Log("[엔비] 차지 공격2 준비!");
            yield break;
        }

        // 기본 공격
        Debug.Log("[엔비] 기본 공격!");
        TurnManager.Instance.GetPlayerController().TakeDamage(Mathf.RoundToInt(0.3f * maxHP));
        turn++;
        yield return null;
    }

    private IEnumerator ExecuteCharge()
    {
        var player = TurnManager.Instance.GetPlayerController();
        if (chargeLevel == 1)
        {
            if (chargeDamageReceived >= 100)
            {
                Debug.Log("[엔비] 차지1 저지 성공 → 15% 피해");
                player.TakeDamage(Mathf.RoundToInt(0.15f * maxHP));
            }
            else
            {
                Debug.Log("[엔비] 차지1 실패 → 60% 피해");
                player.TakeDamage(Mathf.RoundToInt(0.6f * maxHP));
            }
        }
        else if (chargeLevel == 2)
        {
            if (chargeDamageReceived >= 150)
            {
                Debug.Log("[엔비] 차지2 저지 성공 → 20% 피해");
                player.TakeDamage(Mathf.RoundToInt(0.2f * maxHP));
            }
            else
            {
                Debug.Log("[엔비] 차지2 실패 → 100% 피해");
                player.TakeDamage(Mathf.RoundToInt(1.0f * maxHP));

                Debug.Log("[엔비] 플레이어가 2배 공격력 버프를 얻음!");
                player.ApplyAttackBuff(2.0f, 1);
            }
        }

        isCharging = false;
        chargeLevel = 0;
        chargeDamageReceived = 0;
        yield return null;
    }

    public override void ApplyDamage(int damage)
    {
        base.ApplyDamage(damage);

        if (isCharging)
        {
            chargeDamageReceived += damage;
            Debug.Log($"[엔비] 차지 중 피해 누적: {chargeDamageReceived}");
        }
    }
}