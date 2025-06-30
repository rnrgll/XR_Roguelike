using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void ExecuteCombinationAttack(CardCombinationEnum combo, List<int> nums, PatternMonster target)
    {
        int basePower = nums.Sum();
        float multiplier = GetMultiplierByCombo(combo);
        int damage = Mathf.FloorToInt(basePower * multiplier);

        GameStatusUI.Instance.SetComboInfo(combo.ToString(), multiplier);

        //  특수판정 처리
        if (target.IsAwaitingPrideJudgement)
        {
            if (damage <= 100)
            {
                Debug.Log("[프라이드 판단] 성공 → 몬스터 체력 30%로 감소");
                target.ForceSetHpToRate(0.3f);
            }
            else
            {
                Debug.Log("[프라이드 판단] 실패 → 플레이어 체력 35%로 감소");
                var player = FindAnyObjectByType<PlayerController>();
                if (player != null)
                    player.ForceSetHpToRate(0.35f);
            }

            target.EndPrideJudgement();
        }

        // 기본 데미지 적용
        target.ApplyDamage(damage);

        GameStatusUI.Instance.AddDamage(damage);
        Debug.Log($"[{combo}] → {target.name}에게 {damage}의 피해!");
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

  //public float GetBonusMultiplierByMajorArcana(List<MajorArcana> majors, PlayerController playerController, CardCombinationEnum combo)
  //{
  //    float multiplier = 1f;
  //
  //    foreach (var major in majors)
  //    {
  //        switch (major.ArcanaType)
  //        {
  //            case MajorArcanaType.TheDevil:
  //                
  //                break;
  //
  //            case MajorArcanaType.TheWorld:
  //                if (major.IsReversed)
  //                {
  //                    if(combo == CardCombinationEnum.FiveCard)
  //                    {
  //                        multiplier *= 10f;
  //                        playerController.EndTurn();
  //                    }
  //                }
  //                else
  //                {
  //                    multiplier *= 10f; // 세계의 힘, 데미지 10배
  //                    //Todo 받는 데미지 10배 구현해야함.
  //                }
  //                break;
  //
  //            default:
  //                break;
  //        }
  //    }
  //
  //    return multiplier;
  //}
}