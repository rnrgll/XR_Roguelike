using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheMagicianReversed : MonoBehaviour
{
    public MinorArcana DeckCard;
    public MinorArcana HandCard;
    public void Excute(ArcanaContext ctx)
    {
        var controller = ctx.Owner.GetComponent<CardController>();
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
            controller.SwapCards(DeckCard, HandCard);
        }
        var discardList = new List<MinorArcana>();
        foreach (MinorArcana _card in controller.CardListDic[CardEnum.CardStatus.Hand])
        {
            if (!tempCards.Contains(_card))
            {
                discardList.Add(_card);
            }
        }
        int _num = controller.Discard(discardList);
        controller.Draw(_num);
    }

    // TODO: UI 연계 카드 뽑기
    private void PickCards(CardController con)
    {
        DeckCard = con.CardListDic[CardEnum.CardStatus.BattleDeck][0];
        HandCard = con.CardListDic[CardEnum.CardStatus.Hand][0];
    }
}
