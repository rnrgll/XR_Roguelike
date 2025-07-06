using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrideLuciferMonster : EnemyBase
{
    private int turn = 1;
    private List<int> lastThreeTurnDamages = new();

    private bool isAwaitingSpecialCheck = false;
    private bool forceNextStrong = false;

    private enum SpecialState { None, ExpectLowDamage, ExpectHighDamage }
    private SpecialState pendingCheck = SpecialState.None;

    public override void TakeTurn()
    {
        StartCoroutine(TurnLogic());
    }

    private IEnumerator TurnLogic()
    {
        var player = TurnManager.Instance.GetPlayerController();
        int playerMax = player.MaxHP;

        // ▶ 3턴째 → 반드시 특수공격
        if (turn == 3)
        {
            int recentDamage = lastThreeTurnDamages.Sum();

            if (recentDamage < 400)  // 400미만 → 특수1
            {
                Debug.Log("[프라이드] 특수공격1 발동! 공격력 +40 버프!");
                player.AddAttackBuff(40, 1);
                pendingCheck = SpecialState.ExpectLowDamage;
            }
            else                       // 400이상 → 특수2
            {
                Debug.Log("[프라이드] 특수공격2 발동! 공격력 -50 디버프!");
                player.AddAttackBuff(-50, 1);
                pendingCheck = SpecialState.ExpectHighDamage;
            }

            isAwaitingSpecialCheck = true;
            turn++;
            yield break;
        }

        // ▶ 특수 직후 강공격 보장
        if (forceNextStrong)
        {
            forceNextStrong = false;
            int dmg = Mathf.RoundToInt(playerMax * 0.3f);
            Debug.Log("[프라이드] 강공격! 플레이어 최대체력 30% → " + dmg);
            player.TakeDamage(dmg);

            turn++;
            yield break;
        }

        // ▶ 그 외엔 언제나 기본공격
        {
            int dmg = Mathf.RoundToInt(playerMax * 0.1f);
            Debug.Log("[프라이드] 기본 공격! 플레이어 최대체력 10% → " + dmg);
            player.TakeDamage(dmg);

            turn++;
            yield return null;
        }
    }

    public override void ApplyDamage(int damage)
    {
        base.ApplyDamage(damage);

        // 지난 3턴 피해 누적
        lastThreeTurnDamages.Add(damage);
        if (lastThreeTurnDamages.Count > 3)
            lastThreeTurnDamages.RemoveAt(0);

        // 특수공격 성공/실패 체크
        if (isAwaitingSpecialCheck)
        {
            var player = TurnManager.Instance.GetPlayerController();
            bool success = false;

            if (pendingCheck == SpecialState.ExpectLowDamage)
                success = (damage <= 100);      // 100이하면 성공
            else if (pendingCheck == SpecialState.ExpectHighDamage)
                success = (damage >= 150);     // 150이상이면 성공

            if (success)
            {
                Debug.Log("[프라이드] 특수공격 성공! 프라이드 체력 30%로 감소");
                currentHP = Mathf.RoundToInt(maxHP * 0.3f);
            }
            else
            {
                Debug.Log("[프라이드] 특수공격 실패! 플레이어 체력 35%로 감소");
                player.ForceSetHpToRate(0.35f);
            }

            // 체크 종료, 다음 턴에 강공격 예약
            isAwaitingSpecialCheck = false;
            pendingCheck = SpecialState.None;
            forceNextStrong = true;
        }
    }
}