using CardEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    private List<MinorArcana> selectedCards = new List<MinorArcana>();

    public event Action<List<MinorArcana>> OnMinorArcanaAttack;

    [SerializeField] private StatusEffectCardSO[] StatusEffectArr;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShootCard(MinorArcana card)
    {
        if (selectedCards.Count >= 5) return;

        selectedCards.Add(card);

        //  디버프 적용
        var debuffType = card.debuff.debuffinfo;


        if (selectedCards.Count == 5)
        {
            OnMinorArcanaAttack?.Invoke(new List<MinorArcana>(selectedCards));
            selectedCards.Clear();
        }
    }


    private List<MinorArcana> handCards = new();

    public int GetAvailableCardCount()
    {
        return handCards.Count;
    }

    public void AddCard(MinorArcana card)
    {
        handCards.Add(card);
    }

    public void RemoveCard(MinorArcana card)
    {
        handCards.Remove(card);
    }

    public void Reset() => selectedCards.Clear();

    public MinorArcana GetRandomHandCard()
    {
        if (handCards.Count == 0) return default;
        return handCards[UnityEngine.Random.Range(0, handCards.Count)];
    }

    public int CountDebuffedCardsInHand(CardDebuff debuffType)
    {
        int count = 0;
        foreach (var card in handCards)
        {
            if (card.debuff.debuffinfo == debuffType)
                count++;
        }
        return count;
    }

    public void DiscardAllHandCards()
    {
        foreach (var card in new List<MinorArcana>(handCards))
        {
            RemoveCard(card);
            Debug.Log($"[CardManager] 카드 폐기됨: {card.CardName}");
        }
    }


}