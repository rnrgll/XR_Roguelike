using DesignPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;


public class TurnManager : Singleton<TurnManager>
{
    private IPlayerActor player;
    private List<IEnemyActor> enemies = new();
    private EnemyBase currentEnemy;

    private bool battleStarted = false;
    private bool isGameEnded = false;

    private void Awake() => SingletonInit();

    public void RegisterPlayer(IPlayerActor p) => player = p;

    public void RegisterEnemy(IEnemyActor e)
    {
        if (!enemies.Contains(e))
        {
            enemies.Add(e);
            Debug.Log($"[디버그] 등록된 적들: {string.Join(", ", enemies.Select(en => ((MonoBehaviour)en).name))}");

            // 첫 등록된 EnemyBase를 기본 타겟으로 설정
            if (currentEnemy == null && e is EnemyBase monster)
            {
                currentEnemy = monster;
            }
        }
    }

    public List<IEnemyActor> GetEnemies() => enemies;

    public PlayerController GetPlayerController() => player as PlayerController;

    public void SetCurrentEnemyByType(EnemyType type)
    {
        var match = enemies
            .OfType<EnemyBase>()
            .FirstOrDefault(e => e.Type == type && !e.IsDead);
        Debug.Log($"[디버그] 공격 대상: {match} ({match?.name})");

        if (match != null)
        {
            currentEnemy = match;
            Debug.Log($"[타겟] {type} 타입의 적이 타겟으로 설정됨");
        }
        else
        {
            Debug.LogWarning($"[타겟] {type} 타입의 살아 있는 적이 없음");
        }
    }

    public void StartBattle()
    {
        if (battleStarted) return;
        battleStarted = true;
        StartCoroutine(WaitForRegistrationAndStart());
    }

    private IEnumerator WaitForRegistrationAndStart()
    {
        yield return new WaitUntil(() => player != null && enemies.Count > 0);
        yield return StartCoroutine(BattleLoop());
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
                if (enemy is EnemyBase ec && !ec.IsDead)
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


}