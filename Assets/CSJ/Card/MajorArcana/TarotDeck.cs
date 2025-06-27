using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarotDeck : MonoBehaviour
{
    [SerializeField] List<MajorArcanaSO> majorCards;
    MajorArcanaSO curCard;
    //
    //
    // public void Draw()
    // {
    //     curCard = majorCards[manager.randomManager.randInt(0, majorCards.Count)];
    // }
    // public void Activate(GameObject player)
    // {
    //     player.CardRotation();
    //     var _positiong = player.cardPos;
    //     card.Activate(player, ori);
    // }
}
