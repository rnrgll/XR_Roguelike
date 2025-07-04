using Buff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IPlayerActor
{
    [SerializeField] private CardController cardController;
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

    // 버프 관리
    private Queue<HealBuff> recoveryQueue;
    
    
    private IEnumerator Start()
    {
        Debug.Log("[PC] Start 코루틴 진입");
        currentHP = maxHP;
        UpdateHpBar(); // ← 슬라이더 초기화

        yield return new WaitUntil(() => Managers.Manager.turnManager != null);

        Debug.Log("[PC] TurnManager 등록 전");
        TurnManager.Instance.RegisterPlayer(this);
        Debug.Log("[PC] TurnManager 준비 완료");

        yield return new WaitUntil(() => CardManager.Instance != null);
        Debug.Log("[PC] CardManager 대기 전");
        CardManager.Instance.OnMinorArcanaAttack += OnAttackTriggered;
        Debug.Log("[PC] CardManager 준비 완료");

        Debug.Log("[PC] cardController 대기 전");
        yield return new WaitUntil(() => cardController != null);
        Debug.Log($"[PC] cardController 할당됨: {cardController.name}");

        cardController.OnSubmit += OnAttackTriggered;
        Debug.Log("[PC] OnSubmit에 OnAttackTriggered 연결 완료");

    }

    private void OnDestroy()
    {
        if (cardController != null)
            cardController.OnSubmit -= OnAttackTriggered;
    }
    private void OnAttackTriggered(List<MinorArcana> cards)
    {
        Debug.Log("[디버그]  OnAttackTriggered 진입");
        // 1) 카드 조합 계산
        List<int> comboCardNums;
        var combo = CardCombination.CalCombination(cards, out comboCardNums);

        // 2. 공격할 적의 타입 지정 (원한다면 이 부분을 매개변수화 가능)
        var tm = Managers.Manager.turnManager;
        Debug.Log($"[디버그] 사용할 TurnManager = {tm}");

        tm.SetCurrentEnemyByType(EnemyType.Boss);

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
        TurnManager.Instance.NotifyPlayerDeath();
    }

    #region HPControl
    /// <summary>
    /// Heal Buff Queue에 담긴 버프를 하나씩 적용
    /// </summary>
    private void ApplyHeal()
    {
        int count = recoveryQueue.Count;
        for (int i = 0; i < count; i++)
        {
            HealBuff healBuff = recoveryQueue.Dequeue();

            //hp 적용
            if (healBuff.value != 0)
            {
                ChangeHp(healBuff.value);
                Debug.Log($"[플레이어] 체력 : {healBuff.value}만큼 회복, 턴 수 : {healBuff.remainTurn}/{healBuff.turn} ");

            }

            else if (healBuff.percentValue != 0)
            {
                ChangeHpByPercent(healBuff.percentValue);
                Debug.Log($"[플레이어] 체력 : {healBuff.value}% 만큼 회복, 턴 수 : {healBuff.remainTurn}/{healBuff.turn} ");
            }
            else
                Debug.LogError("Recovery value, percentValue 설정 오류");

            healBuff.remainTurn--;
            if (healBuff.remainTurn > 0)
            {
                recoveryQueue.Enqueue(healBuff);
            }
        }
    }

    /// <summary>
    /// HP Change 
    /// </summary>
    /// <param name="amount"></param>
    public void ChangeHp(int amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        if (IsDead)
        {
            Debug.Log("플레이어가 사망했습니다.");
            TurnManager.Instance.NotifyPlayerDeath();
        }
    }

    /// <summary>
    /// max hp의 percent value만큼 증감
    /// </summary>
    /// <param name="percentValue"></param>
    public void ChangeHpByPercent(float percentValue)
    {
        currentHP = Mathf.Clamp(currentHP + (int)(percentValue * maxHP), 0, maxHP);
        if (IsDead)
        {
            Debug.Log("플레이어가 사망했습니다.");
            TurnManager.Instance.NotifyPlayerDeath();
        }
    }
    //매 턴마다 체력 amount 만큼 회복
    
    /// <summary>
    /// 힐 버프를 큐에 추가
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="turns"></param>
    public void AddHealBuff(int amount, int turns)
    {
        recoveryQueue.Enqueue(new HealBuff(amount,turns));
    }
    
    //매 턴마다 체력 amount% 만큼 회복
    public void AddHealBuff(float amount, int turns)
    {
        recoveryQueue.Enqueue(new HealBuff(amount,turns));
    }
    
    #endregion
}