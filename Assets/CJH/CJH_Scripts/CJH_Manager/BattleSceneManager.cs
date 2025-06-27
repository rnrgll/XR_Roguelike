using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneManager : MonoBehaviour
{
    private void Start()
    {
        var pc = FindObjectOfType<PlayerController>();
        if (pc != null)
        {
            // 전투 시작
            TurnManager.Instance.RegisterPlayer(pc);
            TurnManager.Instance.StartBattle();

            GameStatusUI.Instance.SetStage(GameStateManager.Instance.Wins + 1);
            // 추가로: 플레이어 / 적 스폰도 여기서 담당 가능
        }
        else
        {
            Debug.LogError("PlayerController가 null입니다");
        }
    }
}