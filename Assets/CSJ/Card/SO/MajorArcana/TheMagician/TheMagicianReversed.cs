using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheMagicianReversed : MonoBehaviour
{
    public MinorArcana DeckCard;
    public MinorArcana HandCard;
    public void Excute(ArcanaContext ctx)
    {
        var controller = ctx.player.GetCardController();
        var tempCards = new List<MinorArcana>();
        for (int i = 0; i < 3; i++)
        {
            PickCards(controller);
            if (!tempCards.Contains(DeckCard))
            {
                tempCards.Add(DeckCard);
            }
            if (tempCards.Contains(HandCard))
            {
                tempCards.Remove(HandCard);
            }
            controller.SwapCard(DeckCard, HandCard);
        }
        var discardList = new List<MinorArcana>();
        foreach (MinorArcana _card in controller.GetHand())
        {
            if (!tempCards.Contains(_card))
            {
                discardList.Add(_card);
            }
        }
        controller.exchangeHand(discardList);
    }

    // TODO: UI 연계 카드 뽑기
    private void PickCards(CardController con)
    {
        DeckCard = con.BattleDeck.GetCard(0);
        HandCard = con.Hand.GetCard(0);
    }
}
