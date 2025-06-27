using DesignPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TurnManager : Singleton<TurnManager>
{
    private IPlayerActor player;
    private List<IEnemyActor> enemies = new List<IEnemyActor>();
    private Enemy currentEnemy;
    private bool battleStarted = false;
    private bool isGameEnded = false;

    private void Awake() => SingletonInit();

    public void RegisterPlayer(IPlayerActor p) => player = p;

    public void RegisterEnemy(IEnemyActor e)
    {
        enemies.Add(e);
        currentEnemy = e as Enemy;
    }



    public void StartBattle()
    {

        if (battleStarted) return;
        battleStarted = true;
        StartCoroutine(BattleLoop());
    }

    private IEnumerator BattleLoop()
    {
        PlayerController pc = player as PlayerController;
        while (true)
        {
            if (isGameEnded) yield break;

            // 1. 플레이어 사망 시
            if (player != null && pc.IsDead)
            {
                Debug.Log("게임 오버!");
                GameOverUI.Instance.Show();
                isGameEnded = true;
                yield break;
            }

            // 2. 몬스터 사망 시 → 보상 UI 또는 게임 클리어
            if (currentEnemy == null || currentEnemy.IsDead)
            {
                Debug.Log("전투 승리!");
                GameStateManager.Instance.AddWin();

                if (currentEnemy != null && currentEnemy.IsBoss)
                {
                    GameStateManager.Instance.SetBossDefeated();
                    GameClearUI.Instance.Show();
                    isGameEnded = true;
                }
                else
                {
                    pc.RestoreHP();
                    RewardPopupUI.Instance.Show();

                }

                yield break; // 보상 이후 진행은 ContinueAfterReward()에서
            }

            // 플레이어 턴
            player.StartTurn();
            yield return new WaitUntil(() => player.IsTurnFinished());

            // 적 턴
            foreach (var enemy in enemies)
            {
                if (enemy is MonoBehaviour mb && mb != null && (enemy as Enemy)?.IsDead == false)
                {
                    enemy.TakeTurn();
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }

    //  보상 선택 완료 후 호출
    public void ContinueAfterReward()
    {
        Debug.Log("보상 선택 완료 → 다음 노드로 이동 or 씬 전환");

        // 맵 씬으로 전환
        SceneManager.LoadScene("MapScene");

        // 또는 MapManager에서 다음 노드로 이동하도록 처리할 수도 있음:
        // Manager.Map.GoToNextNode();
    }
}
