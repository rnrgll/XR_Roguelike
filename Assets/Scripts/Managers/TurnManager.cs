using DesignPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;


public class TurnManager : Singleton<TurnManager>
{
    private IPlayerActor player;
    private List<IEnemyActor> enemies = new List<IEnemyActor>();
    private EnemyController currentEnemy;
    private bool battleStarted = false;
    private bool isGameEnded = false;

    private void Awake() => SingletonInit();

    public void RegisterPlayer(IPlayerActor p) => player = p;

    public void RegisterEnemy(IEnemyActor e)
    {
        enemies.Add(e);

        // 기본 타겟은 가장 먼저 등록된 EnemyController (또는 Slime 등)
        if (currentEnemy == null && e is EnemyController ec)
        {
            currentEnemy = ec;
        }
    }

    public void StartBattle()
    {
        if (battleStarted) return;
        battleStarted = true;
        StartCoroutine(BattleLoop());
    }

    public void NotifyPlayerDeath()
    {
        if (isGameEnded) return;

        Debug.Log("게임 오버 호출: TurnManager에서 처리함");
        isGameEnded = true;
        GameOverUI.Instance.Show();
    }
    private IEnumerator BattleLoop()
    {
        PlayerController pc = player as PlayerController;
        while (true)
        {
            // 플레이어 사망 및 게임 종료
            if (isGameEnded) yield break;

            // 1. 몬스터 사망
            if (currentEnemy == null || currentEnemy.IsDead)
            {
                Debug.Log("전투 승리!");
                GameStateManager.Instance.AddWin();

                if (currentEnemy != null && currentEnemy.Type == EnemyType.Boss)
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

                yield break;
            }

            // 2. 플레이어 턴
            player.StartTurn();
            yield return new WaitUntil(() => player.IsTurnFinished());

            // 3. 적 턴
            foreach (var enemy in enemies)
            {
                if (enemy is EnemyController ec && !ec.IsDead)
                {
                    ec.TakeTurn();
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }

    public void ContinueAfterReward()
    {
        Debug.Log("보상 선택 완료 → 다음 노드로 이동 or 씬 전환");
        SceneManager.LoadScene("MapScene");
    }

    // 특정 타입의 적을 현재 타겟으로 설정
    public void SetCurrentEnemyByType(EnemyType type)
    {
        var match = enemies.OfType<EnemyController>().FirstOrDefault(e => e.Type == type && !e.IsDead);
        if (match != null)
        {
            currentEnemy = match;
            Debug.Log($"타겟 변경: {type} 선택됨");
        }
        else
        {
            Debug.LogWarning($"타입 {type}에 해당하는 살아있는 적이 없습니다.");
        }
    }

    public List<IEnemyActor> GetEnemies() => enemies;
}