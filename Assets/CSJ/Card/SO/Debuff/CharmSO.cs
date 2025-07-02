using CardEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Charm", menuName = "Cards/Debuff/Charm")]
public class CharmSO : CardDebuffSO
{
    public int charmAmount = 1;
    public Action<int> OnCharmCardUsed;

    public override void OnSubscribe(MinorArcana card, CardController controller)
    {
        controller.OnCardSubmited += c =>
        {
            if (c == card)
                OnCardPlayed(c, controller);
        };
    }

    public override void OnUnSubscribe(MinorArcana card, CardController controller)
    {
        controller.OnCardSubmited -= c =>
        {
            if (c == card)
                OnCardPlayed(c, controller);
        };
    }

    public override void OnCardPlayed(MinorArcana card, CardController controller)
    {
        OnCharmCardUsed?.Invoke(charmAmount);
    }
}
