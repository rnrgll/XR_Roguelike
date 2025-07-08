using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LasMonster : EnemyBase
{
    // ——————————————————
    // 1) 공격 배율 상수 선언
    // ——————————————————
    private const float BasicRate = 0.25f;  // 기본 25%
    private const float StrongRate = 0.40f;  // 강공격 40%

    // ——————————————————————————
    // 2) 분노 및 특수공격 상태 변수
    // ——————————————————————————
    private bool isCharging;                   // 특수공격 준비 중
    private int damageReceivedInCharge;       // 충전 중 받은 누적 피해량
    private int enragePatternIndex;           // 분노 상태에서 순번(0:충전,1:강,2:강)
    private bool hasEnteredEnrage;             // 분노 진입 알림용 플래그

    // ——————————————————————————
    // 3) 편의 프로퍼티
    // ——————————————————————————
    private bool IsEnraged => (float)currentHP / maxHP <= 0.5f;

    // ——————————————————————————
    // 4) 매 턴 실행 로직
    // ——————————————————————————

    protected override void Start()
    {
        base.Start();
    }

    public override void TakeTurn()
    {
        // 4-1) 분노 첫 진입
        if (!hasEnteredEnrage && IsEnraged)
        {
            hasEnteredEnrage = true;
            enragePatternIndex = 0;
            Debug.Log("[라스] 체력 50% 이하! 분노 상태 돌입!");
        }

        // 4-2) 특수공격 발동 여부 먼저 검사
        if (isCharging)
        {
            PerformSpecialAttack();
            isCharging = false;
            // 분노 순번만 다음으로 밀기
            enragePatternIndex = (enragePatternIndex + 1) % 3;
            return;
        }

        // 4-3) 분노인지 아닌지에 따라 분기
        if (IsEnraged)
            PerformEnrageTurn();
        else
            PerformBasicAttack();
    }

    // ——————————————————————————
    // 5) 일반 상태 기본공격
    // ——————————————————————————
    private void PerformBasicAttack()
    {
        var player = TurnManager.Instance.GetPlayerController();
        // 플레이어 MaxHP 기준 25%
        int dmg = Mathf.RoundToInt(player.MaxHP * BasicRate);
        Debug.Log($"[라스] 기본공격 → 플레이어 HP의 25%: {dmg} 데미지");
        player.TakeDamage(dmg);
    }

    // ——————————————————————————
    // 6) 분노 상태 턴 처리
    //    0 → 특수충전, 1,2 → 강공격
    // ——————————————————————————
    private void PerformEnrageTurn()
    {
        if (enragePatternIndex == 0)
        {
            PrepareSpecialAttack();
        }
        else
        {
            PerformStrongAttack();
        }
        // 다음 순번으로
        enragePatternIndex = (enragePatternIndex + 1) % 3;
    }

    // ——————————————————————————
    // 7) 강공격
    // ——————————————————————————
    private void PerformStrongAttack()
    {
        var player = TurnManager.Instance.GetPlayerController();
        int dmg = Mathf.RoundToInt(maxHP * StrongRate);
        Debug.Log($"[라스] 강공격 → {dmg} 데미지");
        player.TakeDamage(dmg);
    }

    // ——————————————————————————
    // 8) 특수공격 준비
    // ——————————————————————————
    private void PrepareSpecialAttack()
    {
        isCharging = true;
        damageReceivedInCharge = 0;
        Debug.Log("[라스] 특수공격 준비… 다음 턴까지 공격을 받아라!");
    }

    // ——————————————————————————
    // 9) 특수공격 발동
    // ——————————————————————————
    private void PerformSpecialAttack()
    {
        if (damageReceivedInCharge <= 0)
        {
            Debug.Log("[라스] 공격 미발생 → 특수공격 무산");
            return;
        }

        int dmg = damageReceivedInCharge * 3;
        Debug.Log($"[라스] 특수공격 발동! 누적 피해 {damageReceivedInCharge} × 3 = {dmg}");
        TurnManager.Instance.GetPlayerController().TakeDamage(dmg);
    }

    // ——————————————————————————
    // 10) 데미지 적용 시 특수충전 누적
    // ——————————————————————————
    public override void ApplyDamage(int damage)
    {
        base.ApplyDamage(damage);

        if (isCharging)
        {
            damageReceivedInCharge += damage;
            Debug.Log($"[라스] 충전 중 받은 피해 누적: {damageReceivedInCharge}");
        }
    }
}