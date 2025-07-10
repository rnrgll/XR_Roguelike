using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(fileName = "TheWorldUpright", menuName = "Tarot/Abilities/TheWorld/Upright")]
public class TheWorldUprightAbility : ScriptableObject, IArcanaAbility
{
    [SerializeField] float Ratio = 10f;
    private PlayerController playerController;
    private Action<MinorArcana> CheckTheHands;
    public void Excute(ArcanaContext ctx)
    {
        playerController = ctx.player;
        CheckTheHands = _ => CheckFiveCard();

        playerController.GetCardController().OnCardSubmited += CheckTheHands;
        playerController.OnTurnEnd += OnUnSubscribe;
    }

    public void CheckFiveCard()
    {
        if (playerController.GetCardController().cardComb == CardCombinationEnum.FiveCard)
        {
            playerController.GetCardController().SetTurnBonusList(CardBonus.Ratio, BonusType.Bonus, Ratio);
            playerController.SetTurnSkip();
        }
    }

    public void OnUnSubscribe()
    {
        playerController.GetCardController().OnCardSubmited -= CheckTheHands;
        playerController.OnTurnEnd -= OnUnSubscribe;
    }
}