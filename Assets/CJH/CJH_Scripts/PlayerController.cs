using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerActor
{
    private bool turnEnded;

    private void Start()
    {
        Managers.Manager.turnManager.RegisterPlayer(this);
        CardManager.Instance.OnMinorArcanaAttack += OnAttackTriggered;
    }

    private void OnAttackTriggered(List<MinorArcana> cards)
    {
        List<int> comboCardNums;
        var combo = CardCombination.CalCombination(cards, out comboCardNums);

        Debug.Log($"족보: {combo}, 카드번호: {string.Join(", ", comboCardNums)}");

        var target = FindAnyObjectByType<Enemy>();
        if (target != null)
        {
            BattleManager.Instance.ExecuteCombinationAttack(combo, comboCardNums, target);
        }
        else
        {
            Debug.LogWarning("공격 대상이 없습니다.");
        }

        EndTurn();
    }

    public void StartTurn()
    {
        Debug.Log("플레이어 턴 시작!");
        turnEnded = false;
    }

    public void EndTurn()
    {
        Debug.Log("플레이어 턴 종료!");
        turnEnded = true;
    }

    public bool IsTurnFinished() => turnEnded;
}