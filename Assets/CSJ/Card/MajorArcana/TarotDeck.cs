using CardEnum;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarotDeck : MonoBehaviour
{
    [SerializeField] List<MajorArcanaSO> majorCardCandidates;
    [SerializeField] GameObject cardCon;
    List<MajorArcanaSO> deckMajorCards = new();
    MajorArcanaSO curCard;
    private int prevIndex;

    public List<MajorArcanaSO> GetMajorCardCandidates()
    {
        return majorCardCandidates;
    }

    public void AddMajorCards(MajorArcanaSO majorCard)
    {
        deckMajorCards.Add(majorCard);
        majorCardCandidates.Remove(majorCard);
    }

    public List<MajorArcanaSO> GetMajorCards()
    {
        return deckMajorCards;
    }

    public MajorArcanaSO Draw()
    {
        prevIndex = deckMajorCards.IndexOf(curCard);
        int CardNum;

        do
        {
            CardNum = Manager.randomManager.RandInt(0, deckMajorCards.Count);
        } while (deckMajorCards.Count > 1 && CardNum == prevIndex);

        curCard = deckMajorCards[CardNum];
        bool IsChanged = curCard.CardRotation();
        if (deckMajorCards.Count <= 1 && !IsChanged)
        {
            curCard.Rotate();
        }
        return curCard;
    }

    public void Activate()
    {
        curCard.Activate(cardCon);
    }
}
