using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IPlayerActor
{
    [SerializeField] private Text hpText;
    [SerializeField] private int maxHP = 100;
    private int currentHP;
    public bool IsDead => currentHP <= 0;
    private bool turnEnded;

    private float attackMultiplier = 1f;
    private int flatAttackBonus = 0;
    private int attackBuffTurns = 0;
    public Action OnTurnEnd;
    public Action OnTurnStarted;

    [Header("HP UI 연동")]
    [SerializeField] private Slider hpBar; // <- 인스펙터에서 슬라이더 연결
    public float GetAttackMultiplier() => attackMultiplier;
    public int GetFlatAttackBonus() => flatAttackBonus;

    private IEnumerator Start()
    {
        currentHP = maxHP;
        UpdateHpBar(); // ← 슬라이더 초기화

        yield return new WaitUntil(() => Managers.Manager.turnManager != null);
        Managers.Manager.turnManager.RegisterPlayer(this);

        yield return new WaitUntil(() => CardManager.Instance != null);
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
            .OfType<EnemyBase>()
            .FirstOrDefault(e => e.Type == EnemyType.Boss && !e.IsDead);

        // 4. 공격 수행
        if (target != null)
        {
            BattleManager.Instance.ExecuteCombinationAttack(combo, comboCardNums, target);
        }
        else
        {
            Debug.LogWarning("[PlayerController] 대상 Boss가 없습니다!");
        }

        // 5. 턴 종료
        EndTurn();
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        Debug.Log($"플레이어가 {dmg} 피해를 입음. 남은 체력: {currentHP}");
        UpdateHpBar();

        if (IsDead)
        {
            Debug.Log("플레이어가 사망했습니다.");
            TurnManager.Instance.NotifyPlayerDeath();
        }
    }


    public void ApplyAttackBuff(float multiplier, int turns)
    {
        attackMultiplier = multiplier;
        attackBuffTurns = turns;
        Debug.Log($"[플레이어] 공격력 {multiplier}배 버프 적용, {turns}턴 지속");
    }

    public void ApplyFlatAttackBuff(int amount, int turns)
    {
        flatAttackBonus = amount;
        attackBuffTurns = turns;

        Debug.Log($"[플레이어] 공격력 {(amount >= 0 ? "+" : "")}{amount} 고정 버프 적용, {turns}턴 지속");

        if (attackBuffTurns > 0)
        {
            attackBuffTurns--;
            if (attackBuffTurns <= 0)
            {
                attackMultiplier = 1f;
                flatAttackBonus = 0;
                Debug.Log("[플레이어] 모든 공격 버프 해제됨");
            }
        }
    }

    public void ForceSetHpToRate(float rate)
    {
        currentHP = Mathf.RoundToInt(maxHP * rate);
        UpdateHpBar();
        Debug.Log($"[플레이어] 체력이 {rate * 100}%로 감소");
    }

    private void UpdateHpBar()
    {
        if (hpBar != null)
        {
            hpBar.value = (float)currentHP / maxHP;
        }

        if (hpText != null)
        {
            hpText.text = $"{currentHP} / {maxHP}";
        }
    }

    public void RestoreHP()
    {
        currentHP = maxHP;
        UpdateHpBar();
        Debug.Log("플레이어 HP 완전 회복!");
    }

    public void StartTurn()
    {
        OnTurnStarted?.Invoke();
        Debug.Log("플레이어 턴 시작!");
        turnEnded = false;
    }

    public void EndTurn()
    {
        Debug.Log("플레이어 턴 종료!");
        turnEnded = true;

        if (attackBuffTurns > 0)
        {
            attackBuffTurns--;
            if (attackBuffTurns <= 0)
            {
                attackMultiplier = 1f;
                Debug.Log("[플레이어] 공격력 버프 해제");
            }
        }
        OnTurnEnd?.Invoke();
    }
    
    public bool IsTurnFinished() => turnEnded;
    
    
    //최대 체력 조절
    public void ChangeMaxHp(int amount)
    {
        maxHP += Mathf.Clamp(maxHP + amount, 0, 100);
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        
        Debug.Log($"{currentHP}/{maxHP}");
        //플레이어 사망..? 처리를 어떻게 할지... 
        //이벤트 씬에서 ChangeMaxHp를 호출, hp가 0이 되면, 사망처리필요. 
        //턴매니저를 계속 인게임 내에서 계속 살려둔다면, turnmanager로 게임 종료 호출 처리..?
    }
}