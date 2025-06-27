using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
       CardManager.Instance.OnMinorArcanaAttack += OnAttackTriggered;
    }

    private void OnAttackTriggered(List<MinorArcana> cards)
    {
        // 1. 카드 조합 계산
        List<int> comboCardNums;
        var combo = CardCombination.CalCombination(cards, out comboCardNums);

        Debug.Log($"족보: {combo}, 카드번호: {string.Join(", ", comboCardNums)}");

        // 2. 공격할 적의 타입 지정 (원한다면 이 부분을 매개변수화 가능)
        TurnManager.Instance.SetCurrentEnemyByType(EnemyType.Boss);

        // 3. 타겟 찾기
        var target = TurnManager.Instance
            .GetEnemies()
            .OfType<EnemyController>()
            .FirstOrDefault(e => e.Type == EnemyType.Boss && !e.IsDead);

        // 4. 공격 수행
        if (target != null)
        {
            BattleManager.Instance.ExecuteCombinationAttack(combo, comboCardNums, target);
        }
        else
        {
            Debug.LogWarning("[PlayerController] 대상 Orc가 없습니다!");
        }

        // 5. 턴 종료
        EndTurn();
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        Debug.Log($"플레이어가 {dmg} 피해를 입음. 남은 체력: {currentHP}");

        if (IsDead)
        {
            Debug.Log("플레이어가 사망했습니다.");
            // 여기선 GameOver 직접 처리는 하지 않음
            TurnManager.Instance.NotifyPlayerDeath(); // 알리기만 함
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