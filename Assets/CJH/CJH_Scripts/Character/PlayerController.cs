using Buffs;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IPlayerActor
{
    [Header("플레이어 스프라이트 렌더러")]
    [SerializeField] private SpriteRenderer[] spriteRenderer;

    [Header("카드·타로 프리팹")]
    [SerializeField] private CardController cardControllerPrefab;
    private CardController _cardController;
    public CardController cardController => _cardController;
    [SerializeField] private TarotDeck tarotDeckPrefab;
    private TarotDeck _tarotDeck;
    public TarotDeck tarotDeck => _tarotDeck;

    [Header("HP 설정")]
    [SerializeField] private int maxHP = 100;
    public int MaxHP => maxHP;
    private int currentHP;


    private int flatAttackBonus = 0;
    private int attackBuffTurns = 0;
    private float additionalDamage;
    private float ratio;
    private float attackMultiplier = 1f;
    public bool IsDead => currentHP <= 0;
    private bool turnEnded;
    private bool isNextTurnSkip = false;
    private bool isTurnSkip = false;
    private bool isInvincible;
    private bool isAdditionalDamage;

    public CardController CardController => cardController;



    public Action OnTurnEnd;
    public Action OnTurnStarted;
    public Action<int> OnPlayerDamaged;
    public Action<int> OnMonsterDamaged;
    public Action<PlayerController> OnPlayerInit;

    [Header("HP UI 연동")]
    [SerializeField] private Slider hpBar; // <- 인스펙터에서 슬라이더 연결
    // public float GetAttackMultiplier() => attackMultiplier;
    public int GetFlatAttackBonus() => ApplyFlatAttackBuff();

    // 버프 관리
    private Queue<Buff> healBonusQueue = new();
    private Queue<Buff> attackBonusQueue = new();

    private SpriteRenderer[] _renderers;


    private void Awake()
    {
        // SpriteRenderer 배열 자동 할당
        if (spriteRenderer == null || spriteRenderer.Length == 0)
            spriteRenderer = GetComponentsInChildren<SpriteRenderer>(true);

        // 기본은 숨김
        SetSpriteVisible(false);
        currentHP = maxHP;
        UpdateHpBar();
    }




    /// <summary>
    /// 전투 시작/종료 시 TurnManager에서 호출하여
    /// 모든 SpriteRenderer를 켜거나 끕니다.
    /// </summary>
    public void SetSpriteVisible(bool visible)
    {
        foreach (var sr in spriteRenderer)
            sr.enabled = visible;
    }


    public IEnumerator StartSetting()
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
        _cardController = Instantiate(cardControllerPrefab, transform);
        _cardController.name = "CardController";
        Debug.Log($"[PC] cardController 할당됨: {cardController.name}");
        _tarotDeck = Instantiate(tarotDeckPrefab, transform);
        _tarotDeck.name = "TarotDeck";
        Debug.Log($"[PC] TarotDeck 할당됨: {_tarotDeck.name}");


        bool ready = false;
        cardController.OnReady += () => ready = true;
        yield return new WaitUntil(() => ready);

        cardController.OnSubmit += OnAttackTriggered;
        Debug.Log("[PC] OnSubmit에 OnAttackTriggered 연결 완료");

        var pc = FindObjectOfType<PlayerController>();
        TurnManager.Instance.RegisterPlayer(pc);
        GameStateManager.Instance.RegisterPlayerController(pc);

        InitializeCard();
    }

    private void InitializeCard()
    {
        cardController.InitializeCC(this);
    }

    private void OnDestroy()
    {
        if (cardController != null)
            cardController.OnSubmit -= OnAttackTriggered;

    }
    private void OnAttackTriggered(List<MinorArcana> cards)
    {
        Debug.Log("[디버그]  OnAttackTriggered 진입");
        if (isTurnSkip)
        {
            Debug.Log("턴 스킵이 활성화되어 있습니다.");
            isTurnSkip = false;
            EndTurn();
        }
        isInvincible = false;
        // 1) 카드 조합 계산
        int comboCardNums = cardController.sumofNums;
        var combo = cardController.cardComb;

        // 3. 타겟 찾기
        var target = TurnManager.Instance
            .GetEnemies()
            .OfType<EnemyBase>()
            .FirstOrDefault(e => !e.IsDead);

        // 4. 공격 수행
        if (target != null)
        {
            int damage = BattleManager.Instance.ExecuteCombinationAttack(combo, comboCardNums, target, this);
            OnMonsterDamaged?.Invoke(damage);
        }
        else
        {
            Debug.LogWarning("[PlayerController] 대상 Boss가 없습니다!");
        }

        if (isNextTurnSkip)
        {
            isNextTurnSkip = false;
            isTurnSkip = true;
        }

        // // 5. 턴 종료
        // EndTurn();
    }

    public void TakeDamage(int dmg)
    {
        if (isInvincible) return;
        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        OnPlayerDamaged?.Invoke(dmg);
        if (isAdditionalDamage)
        {
            currentHP -= (int)additionalDamage;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            dmg = dmg + (int)additionalDamage;
        }

        Debug.Log($"플레이어가 {dmg} 피해를 입음. 남은 체력: {currentHP}");
        UpdateHpBar();

        if (IsDead)
        {
            Debug.Log("플레이어가 사망했습니다.");
            TurnManager.Instance.NotifyPlayerDeath();
        }
    }


    // 일단 사용하지 않아 삭제합니다. 추후 필요하면 복구하면 되겠습니다.
    // public void ApplyAttackBuff(float multiplier, int turns)
    // {
    //     attackMultiplier = multiplier;
    //     attackBuffTurns = turns;
    //     Debug.Log($"[플레이어] 공격력 {multiplier}배 버프 적용, {turns}턴 지속");
    // }

    public int ApplyFlatAttackBuff()
    {
        flatAttackBonus = 0;
        int count = attackBonusQueue.Count;
        for (int i = 0; i < count; i++)
        {
            Buff attackBuff = attackBonusQueue.Dequeue();
            flatAttackBonus += attackBuff.value;
            attackBuff.remainTurn--;
            if (attackBuff.remainTurn > 0)
            {
                attackBonusQueue.Enqueue(attackBuff);
            }
        }
        Debug.Log($"[플레이어] 현재 턴에서 공격력 {(flatAttackBonus >= 0 ? "+" : "")}{flatAttackBonus} 버프 적용");

        return flatAttackBonus;
        // flatAttackBonus = amount;
        // attackBuffTurns = turns;

        // if (attackBuffTurns > 0)
        // {
        //     attackBuffTurns--;
        //     if (attackBuffTurns <= 0)
        //     {
        //         attackMultiplier = 1f;
        //         flatAttackBonus = 0;
        //         Debug.Log("[플레이어] 모든 공격 버프 해제됨");
        //     }
        // }
    }
    /// <summary>
    /// 몬스터가 주는 데미지에 배수를 적용한다.
    /// </summary>
    public void ApplyBonusRatioToMonster(int dmg)
    {
        isAdditionalDamage = true;
        additionalDamage = dmg * (ratio - 1);
    }
    /// <summary>
    /// 몬스터 데미지 배수를 설정한다
    /// </summary>
    public void SetRatio(float _ratio)
    {
        ratio = _ratio;
    }

    /// <summary>
    /// 다음 턴 스킵을 설정한다.
    /// </summary>
    public void SetTurnSkip()
    {
        isNextTurnSkip = true;
    }

    /// <summary>
    /// 플레이어의 최대 체력에 비례하여 설정한다.
    /// </summary>
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
            hpBar.value = currentHP;
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
        cardController.TurnInit();
        Debug.Log("플레이어 턴 시작!");
        turnEnded = false;
    }

    public void EndTurn()
    {
        Debug.Log("플레이어 턴 종료!");
        turnEnded = true;

        // if (attackBuffTurns > 0)
        // {
        //     attackBuffTurns--;
        //     if (attackBuffTurns <= 0)
        //     {
        //         attackMultiplier = 1f;
        //         Debug.Log("[플레이어] 공격력 버프 해제");
        //     }
        // }
        OnTurnEnd?.Invoke();
    }

    public bool IsTurnFinished() => turnEnded;

    public void SetInvincible()
    {
        isInvincible = true;
        Debug.Log(isInvincible);
    }


    //최대 체력 조절
    public void ChangeMaxHp(int amount)
    {
        maxHP = Mathf.Min(maxHP + amount, 0);
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        Debug.Log($"{currentHP}/{maxHP}");
        TurnManager.Instance.NotifyPlayerDeath();
    }


    #region Buff Queue

    /// <summary>
    /// 턴 지속 체력 회복 버프를 큐에 추가합니다. 매 턴마다 지정된 수치만큼 회복됩니다.
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="turns"></param>
    public void AddHealBuff(int amount, int turns)
    {
        healBonusQueue.Enqueue(new Buff(amount, turns));
    }

    /// <summary>
    /// (퍼센트 기반) 턴 지속 체력 회복 버프를 큐에 추가합니다. 매 턴마다 MaxHP 기준 비율만큼 회복됩니다.
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="turns"></param>
    public void AddHealBuff(float amount, int turns)
    {
        healBonusQueue.Enqueue(new Buff(amount, turns));
    }

    /// <summary>
    /// 공격력 버프를 큐에 추가합니다.
    /// 매 턴 동안 지정된 수치만큼 공격력이 증가합니다.
    /// </summary>
    public void AddAttackBuff(int amount, int turns)
    {
        attackBonusQueue.Enqueue(new Buff(amount, turns));
    }

    #endregion

    #region HPControl

    /// <summary>
    /// 힐 버프 큐에 저장된 회복 효과를 적용합니다.
    /// 회복 후 남은 턴 수가 있을 경우 다시 큐에 넣습니다.
    /// </summary>
    public void ApplyHeal()
    {
        int count = healBonusQueue.Count;
        for (int i = 0; i < count; i++)
        {
            Buff healBuff = healBonusQueue.Dequeue();

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
                healBonusQueue.Enqueue(healBuff);
            }
        }
    }

    /// <summary>
    /// 현재 체력에 정수 값만큼 증감합니다.
    /// 체력은 0~MaxHP 사이로 클램프되며, 0 이하가 되면 사망 처리됩니다.
    /// </summary>
    public void ChangeHp(int amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        UpdateHpBar();
        if (IsDead)
        {
            Debug.Log("플레이어가 사망했습니다.");
            TurnManager.Instance.NotifyPlayerDeath();
        }
    }

    /// <summary>
    /// Max HP 기준, 퍼센트 비율만큼 체력을 증감합니다.
    /// 체력은 0~MaxHP 사이로 클램프되며, 0 이하가 되면 사망 처리됩니다.
    /// </summary>
    public void ChangeHpByPercent(float percentValue)
    {
        currentHP = Mathf.Clamp(currentHP + (int)(percentValue * maxHP), 0, maxHP);
        UpdateHpBar();
        if (IsDead)
        {
            Debug.Log("플레이어가 사망했습니다.");
            TurnManager.Instance.NotifyPlayerDeath();
        }
    }

    #endregion

    public void PrintAttackQueue()
    {
        int total = 0;
        foreach (Buff buff in attackBonusQueue)
        {
            Debug.Log($"{buff.value} 증가, {buff.remainTurn} 남음");
            total += buff.value;
        }
        Debug.Log($"total {total} 증가");

    }

    /// <summary>
    /// BattleSceneManager 등 외부에서
    /// HP 바 슬라이더를 할당해 주기 위한 메서드
    /// </summary>
    public void SetHpBar(Slider slider)
    {
        hpBar = slider;
        UpdateHpBar(); // 즉시 현재 HP로 초기화
        Debug.Log("[PlayerController] HP Bar 할당됨: " + slider.name);
    }

    public CardController GetCardController()
    {
        return cardController;
    }

    /// <summary>
    /// 게임 시작 혹은 전투 시작 시
    /// 플레이어의 모든 상태를 기본값으로 되돌립니다.
    /// </summary>
    public void ResetState()
    {
        // 1. 체력 초기화
        currentHP = maxHP;
        UpdateHpBar();

        // 2. 버프·디버프 상태 초기화
        attackMultiplier = 1f;
        flatAttackBonus = 0;
        attackBuffTurns = 0;
        healBonusQueue.Clear();
        attackBonusQueue.Clear();
        isInvincible = false;
        isTurnSkip = false;
        isNextTurnSkip = false;
        isAdditionalDamage = false;
        ratio = 1f;
        additionalDamage = 0f;

         // 3. 카드 컨트롤러(덱·핸드) 초기화
         if (cardController != null)
         {
             cardController.BattleInit();
             Debug.Log("[PlayerController] CardController.BattleInit() 호출됨");
         }
         else
         {
             Debug.LogWarning("[PlayerController] cardController가 할당되지 않음");
         }


        SetSpriteVisible(true);

        // 4. 턴 플래그 초기화
        turnEnded = false;

        Debug.Log("[PlayerController] 상태 초기화 완료");
    }

}