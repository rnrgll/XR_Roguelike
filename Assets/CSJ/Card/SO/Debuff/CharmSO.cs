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

    // 델리게이트 참조를 저장할 필드
    private Action<MinorArcana> playHandler;

    public override void OnSubscribe(MinorArcana card, CardController controller)
    {
        playHandler = c =>
        {
            if (c == card)
                OnCardPlayed(c, controller);
        };
        controller.OnCardSubmited += playHandler;
    }

    public override void OnUnSubscribe(MinorArcana card, CardController controller)
    {
        if (playHandler != null)
            controller.OnCardSubmited -= playHandler;
    }

    public override void OnCardPlayed(MinorArcana card, CardController controller)
    {
        OnCharmCardUsed?.Invoke(charmAmount);
    }
}