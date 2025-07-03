using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GoldCard", menuName = "Cards/Enchant/Gold")]
public class GoldCard : CardEnchantSO
{
    [SerializeField] int Money = 100;
    [SerializeField] int Damage = 100;

    public override void OnCardPlayed(MinorArcana card, CardController controller)
    {
        TurnManager.Instance.GetPlayerController().ApplyAttackBuff(100, 1);
    }
    public override void OnTurnEnd(MinorArcana card, CardController controller)
    {
        GameStateManager.Instance.AddGold(Money);
    }
}
