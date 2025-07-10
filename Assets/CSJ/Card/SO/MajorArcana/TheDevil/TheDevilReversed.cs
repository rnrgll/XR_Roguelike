using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(fileName = "TheDevilReversed", menuName = "Tarot/Abilities/TheDevil/Reversed")]
public class TheDevilReversedAbility : ScriptableObject, IArcanaAbility
{
    private PlayerController playerController;
    private DisposableCardSO contractSO;
    public void Excute(ArcanaContext ctx)
    {
        playerController = ctx.player;
        if (!TurnManager.Instance.isFinal)
        {
            playerController.OnMonsterDamaged += GetGold;
            playerController.OnTurnEnd += OnTurnEnd;
        }
        else
        {
            int nowGold = GameStateManager.Instance.Gold;
            GameStateManager.Instance.AddGold(-100);
            if (nowGold < 100)
            {
                playerController.ChangeMaxHp(-(nowGold / 10 + 1));
            }
            MinorArcana contractCard = new MinorArcana("Contract", MinorSuit.Special, 0);
            playerController.GetCardController().ApplayDisposableCard(DisposableCardName.Contract);
        }
        // TODO : 추후 제대로 구현
    }

    private void GetGold(int Amount)
    {
        GameStateManager.Instance.AddGold(Amount * 2);
    }

    public void OnTurnEnd()
    {
        playerController.OnPlayerDamaged -= GetGold;
        playerController.OnTurnEnd -= OnTurnEnd;
    }
}