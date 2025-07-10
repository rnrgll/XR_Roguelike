using CardEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Injury", menuName = "Cards/Debuff/Injury")]
public class InjurySO : CardDebuffSO
{


    public override void OnTurnEnd(MinorArcana card)
    {
        controller.RemoveStatusEffectCard(card);
    }
}
