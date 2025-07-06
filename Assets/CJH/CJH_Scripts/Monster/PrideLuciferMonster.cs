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

    private enum SpecialState
    {
        None,
        ExpectLowDamage,   // 특수공격1: 100 이하
        ExpectHighDamage   // 특수공격2: 150 이상
    }

    private SpecialState pendingCheck = SpecialState.None;

    public override void TakeTurn()
    {
        StartCoroutine(TurnLogic());
    }

    private IEnumerator TurnLogic()
    {
        var player = TurnManager.Instance.GetPlayerController();
        int playerMax = player.MaxHP;

        // 3턴째라면 특수 체크 패턴
        if (turn == 3)
        {
            int recentDamage = lastThreeTurnDamages.Sum();

            if (recentDamage <= 400)
            {
                // 특수공격2
                Debug.Log("[프라이드] 특수공격2 발동! 플레이어 공격력 -50 디버프 적용!");
                player.AddAttackBuff(-50, 1); // 버프를 큐로 관리로 변경함에 따라 함수 수정
                pendingCheck = SpecialState.ExpectHighDamage;
                isAwaitingSpecialCheck = true;
            }
            else
            {
                // 특수공격1
                Debug.Log("[프라이드] 특수공격1 발동! 플레이어 공격력 +40 버프 적용!");
                player.AddAttackBuff(40, 1); // 버프를 큐로 관리로 변경함에 따라 함수 수정
                pendingCheck = SpecialState.ExpectLowDamage;
                isAwaitingSpecialCheck = true;
            }

            turn++;
            yield break;
        }

        // 예고된 강공격
        if (forceNextStrong)
        {
            forceNextStrong = false;
            int dmg = Mathf.RoundToInt(playerMax * 0.3f);  // 플레이어 최대체력의 30%
            Debug.Log("[프라이드] 예고된 강공격 시전! 플레이어 최대체력 30% → " + dmg);
            player.TakeDamage(dmg);
            turn++;
            yield break;
        }

        // 기본 공격 (3번째 턴이 아닐 때 스킵)
        if (turn % 3 != 0)
        {
            int dmg = Mathf.RoundToInt(playerMax * 0.1f);  // 플레이어 최대체력의 10%
            Debug.Log("[프라이드] 기본 공격! 플레이어 최대체력 10% → " + dmg);
            player.TakeDamage(dmg);
        }
        else
        {
            // **기본 공격 외에는 아무 행동 없음**
            yield break;
        }

        turn++;
        yield return null;
    }

    public override void ApplyDamage(int damage)
    {
        base.ApplyDamage(damage);

        // 지난 3턴 동안 받은 피해량 기록
        lastThreeTurnDamages.Add(damage);
        if (lastThreeTurnDamages.Count > 3)
            lastThreeTurnDamages.RemoveAt(0);

        // 특수공격 체크 중이라면
        if (isAwaitingSpecialCheck)
        {
            var player = TurnManager.Instance.GetPlayerController();

            switch (pendingCheck)
            {
                case SpecialState.ExpectLowDamage:
                    if (damage > 100)
                    {
                        Debug.Log("[프라이드] 특수공격1 성공! 프라이드 체력 30%로 감소");
                        currentHP = Mathf.RoundToInt(maxHP * 0.3f);
                    }
                    else
                    {
                        Debug.Log("[프라이드] 특수공격1 실패! 플레이어 체력 35%로 감소");
                        player.ForceSetHpToRate(0.35f);
                    }
                    break;

                case SpecialState.ExpectHighDamage:
                    if (damage >= 150)
                    {
                        Debug.Log("[프라이드] 특수공격2 성공! 프라이드 체력 30%로 감소");
                        currentHP = Mathf.RoundToInt(maxHP * 0.3f);
                    }
                    else
                    {
                        Debug.Log("[프라이드] 특수공격2 실패! 플레이어 체력 35%로 감소");
                        player.ForceSetHpToRate(0.35f);
                    }
                    break;
            }

            // 체크 종료 및 강공격 예약
            isAwaitingSpecialCheck = false;
            pendingCheck = SpecialState.None;
            forceNextStrong = true;
        }
    }
}