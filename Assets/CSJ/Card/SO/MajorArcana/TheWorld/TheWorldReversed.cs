using CardEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tarot/Abilities/TheWorld/Reversed")]
public class TheWorldReversedAbility : ScriptableObject, IArcanaAbility
{
    [SerializeField] float Ratio = 10f;
    [SerializeField] float TakenRatio = 10f;
    private PlayerController playerController;
    public void Excute(ArcanaContext ctx)
    {
        playerController = ctx.player;

        playerController.GetCardController().SetTurnBonusList(CardBonus.Ratio, BonusType.Bonus, Ratio);

        playerController.OnPlayerDamaged += playerController.ApplyBonusRatioToMonster;
        playerController.OnTurnEnd += OnTurnEnd;
    }

    public void OnTurnEnd()
    {
        playerController.OnPlayerDamaged -= playerController.ApplyBonusRatioToMonster;
        playerController.OnTurnEnd -= OnTurnEnd;
    }
}