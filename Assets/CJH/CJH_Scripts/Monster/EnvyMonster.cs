using CardEnum;
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
        // 1) 플레이어와 최대체력 가져오기
        var player = TurnManager.Instance.GetPlayerController();
        int playerMax = player.MaxHP;

        // 2) 이번 턴 패턴 결정
        var pattern = patternCycle[patternIndex % patternCycle.Length];

        // 3) 충전 중이면 즉시 실행
        if (isCharging)
        {
            yield return ExecuteCharge(player, playerMax);
            isCharging = false;
            patternIndex++;
            yield break;
        }

        // 4) 패턴별 일반 처리
        switch (pattern)
        {
            case EnvyPattern.Basic:
                {
                    // 3% of playerMax
                    int dmg = Mathf.RoundToInt(playerMax * 0.03f);
                    Debug.Log($"[엔비] 기본 공격! 플레이어 최대체력 30% → {dmg} 데미지");
                    player.TakeDamage(dmg);
                }
                break;

            case EnvyPattern.Charge1:
                Debug.Log("[엔비] 차지1 준비 (60% 저지 요구)!");
                isCharging = true;
                chargeLevel = 1;
                chargeDamageReceived = 0;
                break;

            case EnvyPattern.Charge2:
                Debug.Log("[엔비] 차지2 준비 (100% 저지 요구)!");
                isCharging = true;
                chargeLevel = 2;
                chargeDamageReceived = 0;
                break;
        }

        // 5) 충전이 아니면 패턴 인덱스 증가
        if (!isCharging)
            patternIndex++;

        yield return null;
    }

    private IEnumerator ExecuteCharge(PlayerController player, int playerMax)
    {
        // 수행될 데미지와 로그 메시지를 계산
        if (chargeLevel == 1)
        {
            if (chargeDamageReceived >= 100)
            {
                int dmg = Mathf.RoundToInt(playerMax * 0.15f);  // 15%
                Debug.Log($"[엔비] 차지1 저지 성공 → 플레이어 최대체력 15% → {dmg} 데미지");
                player.TakeDamage(dmg);
            }
            else
            {
                int dmg = Mathf.RoundToInt(playerMax * 0.6f);   // 60%
                Debug.Log($"[엔비] 차지1 실패 → 플레이어 최대체력 60% → {dmg} 데미지 + 부상카드 1장");
                player.TakeDamage(dmg);
                var card = CardManager.Instance.GetRandomHandCard();
                if (card != null)
                {
                    player.GetComponent<CardController>()
                          .ApplyDebuff(card, CardDebuff.Injury);
                }

                yield return null;
            }
        }
        else // chargeLevel == 2
        {
            if (chargeDamageReceived >= 150)
            {
                int dmg = Mathf.RoundToInt(playerMax * 0.2f);   // 20%
                Debug.Log($"[엔비] 차지2 저지 성공 → 플레이어 최대체력 20% → {dmg} 데미지");
                player.TakeDamage(dmg);
            }
            else
            {
                int dmg = Mathf.RoundToInt(playerMax * 1.0f);    // 100%
                Debug.Log($"[엔비] 차지2 실패 → 플레이어 최대체력 100% → {dmg} 데미지 + 부상카드 3장 + 버프");
                player.TakeDamage(dmg);
                player.ApplyAttackBuff(2.0f, 1);
                var card = CardManager.Instance.GetRandomHandCard();
                if (card != null)
                {
                    player.GetComponent<CardController>()
                          .ApplyDebuff(card, CardDebuff.Injury);
                    player.GetComponent<CardController>()
                          .ApplyDebuff(card, CardDebuff.Injury);
                    player.GetComponent<CardController>()
                          .ApplyDebuff(card, CardDebuff.Injury);
                }

                yield return null;
            }
        }

        // 충전 상태 초기화
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