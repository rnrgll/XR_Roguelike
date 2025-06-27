using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; // ← 유니티 Inspector에 프리팹 연결

    private void Start()
    {
        var pc = FindObjectOfType<PlayerController>();
        if (pc != null)
        {
            // 1. 적 생성
            GameObject enemyObj = Instantiate(enemyPrefab);
            EnemyController enemy = enemyObj.GetComponent<EnemyController>();

            TurnManager.Instance.RegisterEnemy(enemy);

            // 2. 플레이어 등록 및 전투 시작
            TurnManager.Instance.RegisterPlayer(pc);
            TurnManager.Instance.StartBattle();

            // 3. UI 설정
            GameStatusUI.Instance.SetStage(GameStateManager.Instance.Wins + 1);
        }
        else
        {
            Debug.LogError("PlayerController가 null입니다");
        }
    }
}