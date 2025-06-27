using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tarot/Abilities/TheMagician/Upright")]
public class TheMagicianUpright : ScriptableObject, IArcanaAbility
{
    public MinorArcana DeckCard;
    public MinorArcana HandCard;
    public void Excute(ArcanaContext ctx)
    {
        var controller = ctx.Owner.GetComponent<CardController>();
        PickCards(controller);

        controller.SwapCards(DeckCard, HandCard);
    }

    // TODO: UI 연계 카드 뽑기
    private void PickCards(CardController con)
    {
        DeckCard = con.CardListDic[CardEnum.CardStatus.BattleDeck][0];
        HandCard = con.CardListDic[CardEnum.CardStatus.Hand][0];
    }
}
