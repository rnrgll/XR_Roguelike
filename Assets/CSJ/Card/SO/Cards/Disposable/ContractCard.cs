using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Contract", menuName = "Cards/Disposable/Contract")]
public class ContractCard : DisposableCardSO
{
    [SerializeField] private int UseMoreSpace;
    public override void OnCardPlayed(MinorArcana card)
    {
        playerController.SetInvincible();
    }

    public override void OnSubscribe(MinorArcana card)
    {
        base.OnSubscribe(card);
        controller.MultiPleCardDic[card] = UseMoreSpace;
    }

    public override void OnUnSubscribe(MinorArcana card)
    {
        base.OnUnSubscribe(card);
        if (controller.MultiPleCardDic.TryGetValue(card, out int _))
        {
            controller.MultiPleCardDic.Remove(card);
        }
    }
}

