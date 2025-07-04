using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvyMonster : EnemyBase
{
    private bool isCharging = false;
    private int chargeLevel = 0;
    private int chargeDamageReceived = 0;

    private int patternIndex = 0;
    private enum EnvyPattern
    {
        Basic,
        Charge1,
        Charge2
    }

    private EnvyPattern[] patternCycle = new EnvyPattern[]
    {
        EnvyPattern.Basic,
        EnvyPattern.Charge1,
        EnvyPattern.Charge2
    };

    public override void TakeTurn()
    {
        StartCoroutine(TurnRoutine());
    }

    private IEnumerator TurnRoutine()
    {
        var player = TurnManager.Instance.GetPlayerController();
        var pattern = patternCycle[patternIndex % patternCycle.Length];

        if (isCharging)
        {
            yield return ExecuteCharge();
            isCharging = false;
            patternIndex++;
            yield break;
        }

        switch (pattern)
        {
            case EnvyPattern.Basic:
                Debug.Log("[엔비] 기본 공격!");
                player.TakeDamage(Mathf.RoundToInt(0.03f * maxHP));
                break;

            case EnvyPattern.Charge1:
                Debug.Log("[엔비] 차지 공격 준비 (60%)!");
                isCharging = true;
                chargeLevel = 1;
                chargeDamageReceived = 0;
                break;

            case EnvyPattern.Charge2:
                Debug.Log("[엔비] 차지 공격2 준비 (100%)!");
                isCharging = true;
                chargeLevel = 2;
                chargeDamageReceived = 0;
                break;
        }

        if (!isCharging)
            patternIndex++;

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
                player.TakeDamage(Mathf.RoundToInt(0.015f * maxHP));
            }
            else
            {
                Debug.Log("[엔비] 차지1 실패 → 60% 피해 + 부상카드 1장!");
                player.TakeDamage(Mathf.RoundToInt(0.06f * maxHP));
                //CardManager.Instance.AddInjuryCardToHand(1);
            }
        }
        else if (chargeLevel == 2)
        {
            if (chargeDamageReceived >= 150)
            {
                Debug.Log("[엔비] 차지2 저지 성공 → 20% 피해");
                player.TakeDamage(Mathf.RoundToInt(0.02f * maxHP));
            }
            else
            {
                Debug.Log("[엔비] 차지2 실패 → 100% 피해 + 부상카드 3장 + 플레이어 버프!");
                player.TakeDamage(Mathf.RoundToInt(1.0f * maxHP));
                //CardManager.Instance.AddInjuryCardToHand(3);
                player.ApplyAttackBuff(2.0f, 1);
            }
        }

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