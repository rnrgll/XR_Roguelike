using CardEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GluttonMonster : EnemyBase
{
    public static GluttonMonster Instance { get; private set; }

    [SerializeField] private float basicRatio = 0.05f;
    [SerializeField] private float heavyRatio = 0.15f;
    [SerializeField] private float specialRatio = 0.10f;
    [SerializeField] private float basicChance = 0.70f;

    private int consecutiveBasic = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public override void TakeTurn()
    {
        StartCoroutine(TurnRoutine());
    }

    private IEnumerator TurnRoutine()
    {
        // 1) 플레이어와 최대체력 참조
        var player = TurnManager.Instance.GetPlayerController();
        int playerMax = player.MaxHP;

        // 2) 부식 카드 4장 이상이면 즉사
        int rustCount = CardManager.Instance.CountDebuffedCardsInHand(CardDebuff.Rust);
        if (rustCount >= 4)
        {
            Debug.Log("[GluttonMonster] 부식 카드 4장 이상 → 즉사!");
            player.TakeDamage(playerMax);
            yield break;
        }

        // 3) 기본•강•특수 패턴 분기
        float roll = Random.value;
        if (roll < basicChance)
        {
            if (consecutiveBasic < 2)
                yield return BasicAttack(player, playerMax);
            else
                yield return HeavyAttack(player, playerMax);
        }
        else
        {
            if (Random.value < 0.33f)
                yield return SpecialAttack(player, playerMax);
            else
                yield return SpecialAction(player);
        }

        yield return null;
    }

    private IEnumerator BasicAttack(PlayerController player, int playerMax)
    {
        int dmg = Mathf.RoundToInt(playerMax * basicRatio);
        Debug.Log($"[GluttonMonster] 기본 공격: 플레이어 최대체력 5% → {dmg} 피해 + 부패 1장");
        player.TakeDamage(dmg);

        var card = CardManager.Instance.GetRandomHandCard();
        if (card != null)
            player.GetComponent<CardController>()
                  .ApplyDebuff(card, CardDebuff.Corruption);

        consecutiveBasic++;
        yield return null;
    }

    private IEnumerator HeavyAttack(PlayerController player, int playerMax)
    {
        int dmg = Mathf.RoundToInt(playerMax * heavyRatio);
        Debug.Log($"[GluttonMonster] 강공격: 플레이어 최대체력 15% → {dmg} 피해 + 부식 1장");
        player.TakeDamage(dmg);

        var card = CardManager.Instance.GetRandomHandCard();
        if (card != null)
            player.GetComponent<CardController>()
                  .ApplyDebuff(card, CardDebuff.Rust);

        consecutiveBasic = 0;
        yield return null;
    }

    private IEnumerator SpecialAttack(PlayerController player, int playerMax)
    {
        int dmg = Mathf.RoundToInt(playerMax * specialRatio);
        Debug.Log($"[GluttonMonster] 특수공격: 플레이어 최대체력 10% → {dmg} 피해 + 부패 2장");
        player.TakeDamage(dmg);

        for (int i = 0; i < 2; i++)
        {
            var card = CardManager.Instance.GetRandomHandCard();
            if (card != null)
                player.GetComponent<CardController>()
                      .ApplyDebuff(card, CardDebuff.Corruption);
        }
        yield return null;
    }

    private IEnumerator SpecialAction(PlayerController player)
    {
        Debug.Log("[GluttonMonster] 특수 행동: 피해 없음 + 부식1장 + 부패1장");
        var rustCard = CardManager.Instance.GetRandomHandCard();
        if (rustCard != null)
            player.GetComponent<CardController>()
                  .ApplyDebuff(rustCard, CardDebuff.Rust);

        var corCard = CardManager.Instance.GetRandomHandCard();
        if (corCard != null)
            player.GetComponent<CardController>()
                  .ApplyDebuff(corCard, CardDebuff.Corruption);

        yield return null;
    }
}