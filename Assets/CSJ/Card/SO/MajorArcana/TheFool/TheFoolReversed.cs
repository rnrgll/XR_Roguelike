using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;
using System;

[CreateAssetMenu(fileName = "TheFoolReversed", menuName = "Tarot/Abilities/TheFool/Reversed")]
public class TheFoolReversedAbility : ScriptableObject, IArcanaAbility
{
    private PlayerController playerController;
    private Action<MinorArcana> ToSubscribe;

    public void Excute(ArcanaContext ctx)
    {
        playerController = ctx.player;
        playerController.GetCardController().ApplayDisposableCard(DisposableCardName.Joker);
        OnSubscribe();
    }

    public void OnSubscribe()
    {
        ToSubscribe = _ => OnSubscribe();
        playerController.OnTurnEnd += OnUnSubscribe;
        playerController.GetCardController().OnCardSubmited += ToSubscribe;
    }

    public void OnUnSubscribe()
    {
        playerController.OnTurnEnd -= OnUnSubscribe;
        playerController.GetCardController().OnCardSubmited -= ToSubscribe;
    }

}
