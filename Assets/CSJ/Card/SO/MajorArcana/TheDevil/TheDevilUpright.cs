using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(fileName = "TheDevilUpright", menuName = "Tarot/Abilities/TheDevil/Upright")]
public class TheDevilUprightAbility : ScriptableObject, IArcanaAbility
{
    [SerializeField] private int BonusMult = 1;
    private PlayerController playerController;
    public void Excute(ArcanaContext ctx)
    {
        playerController = ctx.player;
        // TODO : 추후 제대로 구현
        if (!TurnManager.Instance.isFinal)
        {
            playerController.OnPlayerDamaged += GetGold;
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
            playerController.GetCardController().SetBattleBonusList(CardBonus.Mult, BonusType.Bonus, BonusMult);
        }
    }

    private void GetGold(int Amount)
    {
        GameStateManager.Instance.AddGold(Amount);
    }

    public void OnTurnEnd()
    {
        playerController.OnPlayerDamaged -= GetGold;
        playerController.OnTurnEnd -= OnTurnEnd;
    }
}