using CardEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new StatusEffect", menuName = "Cards/StatusEffectCards")]
public class StatusEffectCardSO : ScriptableObject
{
    [Header("카드 정보")]
    public string cardName;
    public Sprite sprite;

    [Header("MinorArcana 정보")]
    public MinorSuit suit = MinorSuit.statusEffect;
    public int cardNum = 0;

    [Header("사용 가능 여부")]
    public bool IsUsable = true;

    [Header("부가 효과 (디버프로 생성)")]
    public CardDebuffSO debuff;

    public MinorArcana CreateCard()
    {
        return new MinorArcana(cardName, suit, cardNum);
    }

    public void AddStatusEffect(CardController cardController)
    {
        cardController.AddStatusEffectCard(this, CreateCard());
    }

}
