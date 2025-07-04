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

        if (turn == 3)
        {
            int recentDamage = lastThreeTurnDamages.Sum();

            if (recentDamage <= 400)
            {
                // 특수공격2 조건 발동
                Debug.Log("[프라이드] 특수공격2 발동! 플레이어 공격력 -50 디버프 적용!");
                player.AddAttackBuff(-50, 1); // 버프를 큐로 관리로 변경함에 따라 함수 수정
                pendingCheck = SpecialState.ExpectHighDamage;
                isAwaitingSpecialCheck = true;
            }
            else if (recentDamage > 400)
            {
                // 특수공격1 조건 발동
                Debug.Log("[프라이드] 특수공격1 발동! 플레이어 공격력 +40 버프 적용!");
                player.AddAttackBuff(40, 1); // 버프를 큐로 관리로 변경함에 따라 함수 수정
                pendingCheck = SpecialState.ExpectLowDamage;
                isAwaitingSpecialCheck = true;
            }
            else
            {
                yield break;
            }

            turn++;
            yield break;
        }

        if (forceNextStrong)
        {
            forceNextStrong = false;
            Debug.Log("[프라이드] 예고된 강공격 시전!");
            player.TakeDamage(Mathf.RoundToInt(0.3f * maxHP));
            turn++;
            yield break;
        }

        // 기본 패턴
        if (turn % 3 == 0)
        {
            Debug.Log("[프라이드] 기본 공격!");
            player.TakeDamage(Mathf.RoundToInt(0.1f * maxHP));
        }
        else
            yield break;

            turn++;
        yield return null;
    }

    public override void ApplyDamage(int damage)
    {
        base.ApplyDamage(damage);

        lastThreeTurnDamages.Add(damage);
        if (lastThreeTurnDamages.Count > 3)
            lastThreeTurnDamages.RemoveAt(0);

        // 특수공격1 또는 2의 데미지 체크 처리
        if (isAwaitingSpecialCheck)
        {
            switch (pendingCheck)
            {
                case SpecialState.ExpectLowDamage:
                    if (damage > 100)
                    {
                        Debug.Log("[프라이드] 특수공격1 성공! 프라이드 체력을 30%로 감소시킴!");
                        this.currentHP = Mathf.RoundToInt(maxHP * 0.3f);
                    }
                    else
                    {
                        Debug.Log("[프라이드] 특수공격1 조건 실패! 플레이어 체력을 35%로 감소시킴!");
                        TurnManager.Instance.GetPlayerController().ForceSetHpToRate(0.35f);
                    }
                    break;

                case SpecialState.ExpectHighDamage:
                    if (damage >= 150)
                    {
                        Debug.Log("[프라이드] 특수공격2 조건 성공! 프라이드 체력을 30%로 감소시킴!");
                        this.currentHP = Mathf.RoundToInt(maxHP * 0.3f);
                    }
                    else
                    {
                        Debug.Log("[프라이드] 특수공격2 조건 실패! 플레이어 체력을 35%로 감소시킴!");
                        TurnManager.Instance.GetPlayerController().ForceSetHpToRate(0.35f);
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