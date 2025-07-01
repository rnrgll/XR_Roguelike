using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; // ← 유니티 Inspector에 프리팹 연결
    [SerializeField] private PlayerController player;

    private IEnumerator Start()
    {
        // 1프레임 기다려 PlayerController가 등장하도록 함
        yield return null;
        var pc = FindObjectOfType<PlayerController>();
        if (pc != null)
        {
            // 1. 적 생성
            GameObject enemyObj = Instantiate(enemyPrefab);
            PatternMonster enemy = enemyObj.GetComponent<PatternMonster>();
            TurnManager.Instance.RegisterEnemy(enemy);

            // 2. 플레이어 등록 및 전투 시작
            TurnManager.Instance.RegisterPlayer(pc);
            Debug.Log(" Player 등록 완료");
            yield return null; // 한 프레임

            TurnManager.Instance.StartBattle();
            Debug.Log("⚔ 전투 시작됨");

            // 3. UI 설정
            GameStatusUI.Instance.SetStage(GameStateManager.Instance.Wins + 1);
        }
        else
        {
            Debug.LogError("PlayerController가 null입니다");
        }
    }
}
