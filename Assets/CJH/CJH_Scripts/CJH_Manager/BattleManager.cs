using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CardEnum;


public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ExecuteCombinationAttack(CardCombinationEnum combo, List<int> nums, EnemyController target)
    {
        int basePower = nums.Sum();
        float multiplier = GetMultiplierByCombo(combo);
        int damage = Mathf.FloorToInt(basePower * multiplier);

        // GameStatusUI에 반영되도록 함.
        GameStatusUI.Instance.SetComboInfo(combo.ToString(), multiplier);

        if (combo == CardCombinationEnum.FiveJoker)
        {
            Debug.Log("파이브 조커! 즉사급 피해!");
            damage = 9999;
        }
        else if (combo == CardCombinationEnum.HighCard)
        {
            Debug.Log("족보 없음. 데미지 0.");
            damage = 0;
        }

        target.ApplyDamage(damage);

        // 피해량 UI에 반영
        GameStatusUI.Instance.AddDamage(damage);
        Debug.Log($"[{combo}] → {target.name}에게 {damage}의 피해!");

        if (target.IsDead)
        {
            GameStateManager.Instance.AddWin();
            Debug.Log("승리 카운트 +1 (BattleManager)");
        }
    }

    private float GetMultiplierByCombo(CardCombinationEnum combo)
    {
        return combo switch
        {
            CardCombinationEnum.OnePair => 1.0f,
            CardCombinationEnum.TwoPair => 1.2f,
            CardCombinationEnum.Triple => 1.5f,
            CardCombinationEnum.Straight => 1.7f,
            CardCombinationEnum.Flush => 1.8f,
            CardCombinationEnum.FullHouse => 2.0f,
            CardCombinationEnum.FourCard => 2.5f,
            CardCombinationEnum.StraightFlush => 3.0f,
            CardCombinationEnum.FiveCard => 3.5f,
            CardCombinationEnum.FiveJoker => 10.0f,
            _ => 0.0f
        };
    }
}