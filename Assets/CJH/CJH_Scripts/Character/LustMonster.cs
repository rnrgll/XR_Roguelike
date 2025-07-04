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
        Debug.Log($"[LustMonster] 스택 {lustStack} → 즉사기 발동!!");
        TurnManager.Instance.GetPlayerController().TakeDamage(maxHP); // 즉사 처리
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

        // 1. 유혹 확률 판단
        if (Random.value < temptChance)
        {
            yield return ExecuteTemptation();
        }
        else
        {
            yield return ExecutePunishment();
        }

        yield return null;
    }

    private IEnumerator ExecuteTemptation()
    {
        Debug.Log("[LustMonster] 유혹 공격: 5% 피해 + Rust 디버프 부여");
        var player = TurnManager.Instance.GetPlayerController();
        player.TakeDamage(Mathf.RoundToInt(maxHP * 0.05f));

        var card = CardManager.Instance.GetRandomHandCard();
        if (card != null)
        {
            // 연결한 이벤트들 작동
            player.GetComponent<CardController>()
                  .ApplyDebuff(card, CardDebuff.Charm);
        }

        yield return null;
    }

    private IEnumerator ExecutePunishment()
    {
        int rustedCount = CardManager.Instance.CountDebuffedCardsInHand(CardDebuff.Charm);

        if (rustedCount >= 3)
        {
            Debug.Log("[LustMonster] 손패에 Charm 카드 3장 이상 → 전부 폐기 + 10% 피해!");

            CardManager.Instance.DiscardAllHandCards();
            TurnManager.Instance.GetPlayerController().TakeDamage(Mathf.RoundToInt(maxHP * 0.1f));
        }
        else
        {
            Debug.Log("[LustMonster] 손패의 Charm 카드 부족 → 아무 일도 일어나지 않음");
        }

        yield return null;
    }
}