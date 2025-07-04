using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;
using Unity.Burst.Intrinsics;


public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public int ExecuteCombinationAttack(CardCombinationEnum combo, int nums, EnemyBase target)
    {
        int baseDamage = nums;
        float multiplier = TurnManager.Instance.GetPlayerController().GetAttackMultiplier() + TurnManager.Instance.GetPlayerController().GetCardController().GetCardBonus(CardBonus.Mult);
        int bonus = TurnManager.Instance.GetPlayerController().GetFlatAttackBonus() + (int)TurnManager.Instance.GetPlayerController().GetCardController().GetCardBonus(CardBonus.Score);
        float ratio = TurnManager.Instance.GetPlayerController().GetCardController().GetCardBonus(CardBonus.Ratio);
        int finalDamage = Mathf.RoundToInt((baseDamage * multiplier + bonus) * ratio);


        GameStatusUI.Instance.SetComboInfo(combo.ToString(), multiplier);


        // 기본 데미지 적용
        target.ApplyDamage(finalDamage);

        GameStatusUI.Instance.AddDamage(baseDamage);
        Debug.Log($"[{combo}] → {target.name}에게 {baseDamage}의 피해!");

        return finalDamage;
    }

}