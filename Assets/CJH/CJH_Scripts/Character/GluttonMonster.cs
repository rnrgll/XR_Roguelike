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
        // 조건부 행동: 손패에 부식 카드 4장 이상 → 즉사
        int rustCount = CardManager.Instance.CountDebuffedCardsInHand(CardDebuff.Rust);
        if (rustCount >= 4)
        {
            Debug.Log("[GluttonMonster] 부식 카드 4장 이상 → 패배 조건 발동");
            TurnManager.Instance.GetPlayerController().TakeDamage(maxHP);
            yield break;
        }

        float roll = Random.value;
        if (roll < basicChance)
        {
            // 일반 패턴: 기본 or 강공격
            if (consecutiveBasic < 2)
                yield return BasicAttack();
            else
                yield return HeavyAttack();
        }
        else
        {
            // 특수 패턴 (특수공격 1/3, 특수행동 2/3)
            if (Random.value < 0.33f)
                yield return SpecialAttack();
            else
                yield return SpecialAction();
        }

        yield return null;
    }

    private IEnumerator BasicAttack()
    {
        Debug.Log("[GluttonMonster] 기본 공격: 5% 피해 + 부패 1장");
        var player = TurnManager.Instance.GetPlayerController();
        player.TakeDamage(Mathf.RoundToInt(maxHP * basicRatio));

        var card = CardManager.Instance.GetRandomHandCard();
        // if (card != null)
        //     player.GetComponent<CardController>().ApplyDebuff(card, CardDebuff.Corruption);

        consecutiveBasic++;
        yield return null;
    }

    private IEnumerator HeavyAttack()
    {
        Debug.Log("[GluttonMonster] 강공격: 15% 피해 + 부식 1장");
        var player = TurnManager.Instance.GetPlayerController();
        player.TakeDamage(Mathf.RoundToInt(maxHP * heavyRatio));

        var card = CardManager.Instance.GetRandomHandCard();
        // if (card != null)
        //     player.GetComponent<CardController>().ApplyDebuff(card, CardDebuff.Rust);

        consecutiveBasic = 0;
        yield return null;
    }

    private IEnumerator SpecialAttack()
    {
        Debug.Log("[GluttonMonster] 특수공격: 10% 피해 + 부패 2장");
        var player = TurnManager.Instance.GetPlayerController();
        player.TakeDamage(Mathf.RoundToInt(maxHP * specialRatio));

        for (int i = 0; i < 2; i++)
        {
            var card = CardManager.Instance.GetRandomHandCard();
            // if (card != null)
            //     player.GetComponent<CardController>().ApplyDebuff(card, CardDebuff.Corruption);
        }
        yield return null;
    }

    private IEnumerator SpecialAction()
    {
        Debug.Log("[GluttonMonster] 특수 행동: 피해 없음 + 부식1장, 부패1장");
        var player = TurnManager.Instance.GetPlayerController();

        var rustCard = CardManager.Instance.GetRandomHandCard();
        // if (rustCard != null)
        //     player.GetComponent<CardController>().ApplyDebuff(rustCard, CardDebuff.Rust);

        var corCard = CardManager.Instance.GetRandomHandCard();
        // if (corCard != null)
        //     player.GetComponent<CardController>().ApplyDebuff(corCard, CardDebuff.Corruption);

        yield return null;
    }

}