using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SteelCard", menuName = "Cards/Enchant/Steel")]
public class SteelCard : CardEnchantSO
{
    [SerializeField] private int MultPlayed = 3;
    [SerializeField] private float MultInHand = 1.5f;
    private int MultCount = 0;

    public override void OnCardPlayed(MinorArcana card, CardController controller)
    {
        TurnManager.Instance.GetPlayerController().ApplyAttackBuff(3, 1);
    }
    public override void OnTurnEnd(MinorArcana card, CardController controller)
    {
        TurnManager.Instance.GetPlayerController().ApplyAttackBuff(1.5f, 999);
        MultCount++;
    }

    //TODO : 추후 구현
    public override void OnBattleEnd(MinorArcana card, CardController controller)
    {
        //TurnManager.Instance.GetPlayerController().ApplyAttackBuffClear(MultCount);
        MultCount = 0;
    }
}
