using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LasMonster : EnemyBase
{
    [SerializeField] private List<MonsterAttackPattern> attackPatterns;

    private int turnCounter = 0;

    private bool isCharging = false;
    private bool receivedDamageDuringCharge = false;
    private int damageReceivedInCharge = 0;
    private int enragePatternIndex = 0; // 분노 패턴 순서 추적
    private bool hasEnteredEnrage = false;
    private bool isEnraged => (float)currentHP / maxHP <= 0.5f;

    public override void TakeTurn()
    {
        turnCounter++;

        // 분노 진입 알림
        if (!hasEnteredEnrage && isEnraged)
        {
            hasEnteredEnrage = true;
            enragePatternIndex = 0;
            Debug.Log("[라스] 체력이 절반 이하로 떨어졌다! 분노 상태에 돌입한다!");
        }

        if (isCharging)
        {
            PerformSpecialAttack();
            isCharging = false;
            enragePatternIndex = (enragePatternIndex + 1) % 3; // 다음 분노 패턴으로
            return;
        }

        if (isEnraged)
        {
            PerformEnragePattern(); 
        }
        else
        {
            var pattern = SelectAttackPattern();
            PerformAttack(pattern);
        }
    }

    private void PerformEnragePattern()
    {
        switch (enragePatternIndex)
        {
            case 0: // 특수공격 준비
                PrepareSpecialAttack();
                break;

            case 1: // 강공격
            case 2:
                var pattern = attackPatterns.Find(p => p.name.Contains("강공격"));
                if (pattern != null)
                {
                    PerformAttack(pattern);
                }
                else
                {
                    Debug.LogWarning("[라스] 강공격 패턴이 정의되어 있지 않다!");
                }
                enragePatternIndex = (enragePatternIndex + 1) % 3;
                break;
        }
    }

    private MonsterAttackPattern SelectAttackPattern()
    {
        List<MonsterAttackPattern> availablePatterns = new List<MonsterAttackPattern>();

        foreach (var pattern in attackPatterns)
        {
            if (pattern.type == MonsterAttackType.Special)
                continue;

            if (isEnraged)
            {
                // 분노 상태에서는 강공격만 허용
                if (pattern.name.Contains("강공격"))
                {
                    availablePatterns.Add(pattern);
                }
            }
            else
            {
                // 일반 상태에서는 기본공격만 허용
                if (pattern.name.Contains("기본공격"))
                {
                    availablePatterns.Add(pattern);
                }
            }
        }

        if (availablePatterns.Count == 0)
        {
            Debug.LogWarning("[라스] 사용 가능한 공격 패턴이 없어 fallback 한다.");
            return attackPatterns[0];
        }

        return availablePatterns[Random.Range(0, availablePatterns.Count)];
    }

    private bool ShouldTriggerSpecialAttack()
    {
        foreach (var pattern in attackPatterns)
        {
            if (pattern.type != MonsterAttackType.Special) continue;

            if (pattern.triggerTurn > 0 && turnCounter == pattern.triggerTurn)
                return true;

            if (pattern.triggerTurn == 0 && Random.value < 0.3f)
                return true;
        }

        return false;
    }

    private void PrepareSpecialAttack()
    {
        isCharging = true;
        damageReceivedInCharge = 0;
        receivedDamageDuringCharge = false;

        Debug.Log("[라스] 분노 상태! 특수공격을 준비한다... 다음 턴까지 공격해보아라!");
    }

    private void PerformSpecialAttack()
    {
        if (!receivedDamageDuringCharge)
        {
            Debug.Log("[라스] 플레이어가 공격하지 않아 특수공격이 무산되었다...");
            return;
        }

        int damage = damageReceivedInCharge * 3;
        Debug.Log($"[라스] 특수공격 발동! 받은 피해의 3배({damage})를 플레이어에게 되갚는다!");
        TurnManager.Instance.GetPlayerController().TakeDamage(damage);

        // 초기화
        damageReceivedInCharge = 0;
        receivedDamageDuringCharge = false;
    }

    public override void ApplyDamage(int damage)
    {
        base.ApplyDamage(damage);

        if (isCharging)
        {
            damageReceivedInCharge += damage;
            receivedDamageDuringCharge = true;
            Debug.Log($"[라스] 특수공격 준비 중 피해 누적: {damageReceivedInCharge}");
        }
    }

    private void PerformAttack(MonsterAttackPattern pattern)
    {
        float damage = maxHP * pattern.damageRate;
        Debug.Log($"[라스] '{pattern.name}' 공격! 피해율: {pattern.damageRate * 100}% → 피해: {damage}");

        TurnManager.Instance.GetPlayerController().TakeDamage(Mathf.RoundToInt(damage));
    }
}
