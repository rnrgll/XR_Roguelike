using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LasMonster : EnemyBase
{
    [SerializeField] private List<MonsterAttackPattern> attackPatterns;

    private bool isEnraged => (float)currentHP / maxHP <= 0.5f;
    private int turnCounter = 0;
    private bool isCharging = false;

    public override void TakeTurn()
    {
        turnCounter++;

        if (isCharging)
        {
            // 특수공격 발동
            PerformSpecialAttack();
            isCharging = false;
            return;
        }

        if (isEnraged && ShouldTriggerSpecialAttack())
        {
            PrepareSpecialAttack();
        }
        else
        {
            var pattern = SelectAttackPattern();
            PerformAttack(pattern);
        }
    }

    private MonsterAttackPattern SelectAttackPattern()
    {
        List<MonsterAttackPattern> availablePatterns = new List<MonsterAttackPattern>();

        foreach (var pattern in attackPatterns)
        {
            if (pattern.type == MonsterAttackType.Special) continue; // 특수공격은 따로 처리
            if (pattern.triggerHpRate > 0 && (float)currentHP / maxHP > pattern.triggerHpRate)
                continue;

            availablePatterns.Add(pattern);
        }

        if (availablePatterns.Count == 0)
            return attackPatterns[0]; // fallback

        return availablePatterns[Random.Range(0, availablePatterns.Count)];
    }
    private bool ShouldTriggerSpecialAttack()
    {
        foreach (var pattern in attackPatterns)
        {
            if (pattern.type != MonsterAttackType.Special)
                continue;

            // 분노 상태 + 트리거 턴 조건 확인
            if (isEnraged)
            {
                if (pattern.triggerTurn > 0 && turnCounter == pattern.triggerTurn)
                    return true;

                // 또는 일정 확률로 특수공격 시도 (ex: 30%)
                if (pattern.triggerTurn == 0 && Random.value < 0.3f)
                    return true;
            }
        }

        return false;
    }

    private void PrepareSpecialAttack()
    {
        isCharging = true;
        Debug.Log("[라스] 분노 상태! 특수 공격을 준비한다...");
    }

    private void PerformSpecialAttack()
    {
        var pattern = attackPatterns.Find(p => p.type == MonsterAttackType.Special);
        if (pattern == null)
        {
            Debug.LogWarning("특수 공격 패턴이 정의되어 있지 않다!");
            return;
        }

        // 특수 공격 데미지 계산은 외부에서 처리
        Debug.Log("[라스] 특수 공격 발동! 라스가 받은 피해 3배!");

        // 실제 공격 로직은 게임 전투 시스템에 따라 Player 대상 적용
        // 예시로만 출력
    }

    private void PerformAttack(MonsterAttackPattern pattern)
    {
        float damage = maxHP * pattern.damageRate;
        Debug.Log($"[라스] '{pattern.name}' 공격! 피해율: {pattern.damageRate * 100}% → 피해: {damage}");

        // 피해 적용은 외부 시스템(Player 등)에서 처리
    }
}
