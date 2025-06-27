using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerActor
{
    [SerializeField] private int maxHP = 100;
    private int currentHP;
    public bool IsDead => currentHP <= 0;
    private bool turnEnded;

    private void Start()
    {
        currentHP = maxHP;


        Managers.Manager.turnManager.RegisterPlayer(this);
        //CardManager.Instance.OnMinorArcanaAttack += OnAttackTriggered;
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

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        Debug.Log($"플레이어 피해: {dmg}, 남은 체력: {currentHP}");

        if (currentHP <= 0)
        {
            Debug.Log("플레이어 사망!");
            // TODO: 게임 오버 처리
        }
    }

    public void RestoreHP()
    {
        currentHP = maxHP;
        Debug.Log("플레이어 HP 완전 회복!");
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