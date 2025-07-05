using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Contract", menuName = "Cards/Disposable/Contract")]
public class ContractCard : DisposableCardSO
{
    public override void OnCardPlayed(MinorArcana card)
    {
        playerController.SetInvincible();
    }
}

