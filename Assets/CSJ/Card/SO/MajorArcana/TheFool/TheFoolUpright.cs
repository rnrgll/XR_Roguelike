using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "TheFoolUpright", menuName = "Tarot/Abilities/TheFool/Upright")]
public class TheFoolUprightAbility : ScriptableObject, IArcanaAbility
{
    [SerializeField] JokerCard joker;
    private PlayerController playerController;
    private Action<MinorArcana> ToSubscribe;
    public void Excute(ArcanaContext ctx)
    {
        playerController = ctx.player;
        joker.AddDisposableCard();
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
        joker.RemoveCard();
        playerController.OnTurnEnd -= OnUnSubscribe;
        playerController.GetCardController().OnCardSubmited -= ToSubscribe;
    }
}

