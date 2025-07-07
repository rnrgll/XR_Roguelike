using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CardEnum;

[CreateAssetMenu(fileName = "TheFoolUpright", menuName = "Tarot/Abilities/TheFool/Upright")]
public class TheFoolUprightAbility : ScriptableObject, IArcanaAbility
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

