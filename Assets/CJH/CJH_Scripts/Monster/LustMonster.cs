using CardEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LustMonster : EnemyBase
{
    public static LustMonster Instance { get; private set; }

    [SerializeField] private int fatalStackThreshold = 5;
    [SerializeField] private float temptChance = 0.6f;

    private int lustStack = 0;
    private const int stackMax = 10;

    protected override void Start()
    {
        base.Start();
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public int LustStack => lustStack;

    public void IncreaseLustStack(int amount)
    {
        lustStack = Mathf.Min(lustStack + amount, stackMax);
        Debug.Log($"[LustMonster] 러스트 스택 증가 → 현재: {lustStack}");

        if (lustStack >= fatalStackThreshold)
        {
            StartCoroutine(TriggerFatalAttack());
        }
    }

    private IEnumerator TriggerFatalAttack()
    {
        var player = TurnManager.Instance.GetPlayerController();
        int playerMax = player.MaxHP;

        Debug.Log($"[LustMonster] 스택 {lustStack} → 즉사기 발동!!");
        // 플레이어 최대체력 전부만큼 대미지 입혀 즉사 처리
        player.TakeDamage(playerMax);
        lustStack = 0;
        yield return null;
    }

    public override void TakeTurn()
    {
        StartCoroutine(TurnRoutine());
    }

    private IEnumerator TurnRoutine()
    {
        Debug.Log("[LustMonster] 턴 시작");

        var player = TurnManager.Instance.GetPlayerController();
        int playerMax = player.MaxHP;

        // 1. 유혹 확률 판단
        if (Random.value < temptChance)
        {
            yield return ExecuteTemptation(player, playerMax);
        }
        else
        {
            yield return ExecutePunishment(player, playerMax);
        }

        yield return null;
    }

    private IEnumerator ExecuteTemptation(PlayerController player, int playerMax)
    {
        // 5% of player's max HP
        int dmg = Mathf.RoundToInt(playerMax * 0.05f);
        Debug.Log($"[LustMonster] 유혹 공격: 플레이어 최대체력 5% → {dmg} 피해 + Rust 디버프");
        player.TakeDamage(dmg);

        var card = CardManager.Instance.GetRandomHandCard();
        if (card != null)
        {
            player.GetComponent<CardController>()
                  .ApplyDebuff(card, CardDebuff.Charm);
        }

        yield return null;
    }

    private IEnumerator ExecutePunishment(PlayerController player, int playerMax)
    {
        int rustedCount = CardManager.Instance.CountDebuffedCardsInHand(CardDebuff.Charm);

        if (rustedCount >= 3)
        {
            // 10% of player's max HP
            int dmg = Mathf.RoundToInt(playerMax * 0.10f);
            Debug.Log($"[LustMonster] Charm 카드 3장 이상 → 전부 폐기 + 플레이어 최대체력 10% → {dmg} 피해");
            CardManager.Instance.DiscardAllHandCards();
            player.TakeDamage(dmg);
        }
        else
        {
            Debug.Log("[LustMonster] Charm 카드 부족 → 아무 일도 일어나지 않음");
        }

        yield return null;
    }

    public override void ApplyDamage(int damage)
    {
        base.ApplyDamage(damage);

        // 러스트 차지 중에는 stack 증감만
        if (lustStack > 0) { /* 아무 추가 효과 없음 */ }
    }
}