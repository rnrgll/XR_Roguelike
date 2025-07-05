using DesignPattern;
using Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : Singleton<TurnManager>
{
    private IPlayerActor player;
    private List<IEnemyActor> enemies = new();
    private EnemyBase currentEnemy;

    private bool battleStarted = false;
    private bool isGameEnded = false;

    // 보스 전투(최종 전투) 여부
    public bool isFinal { get; private set; } = false;

    private void Awake()
    {
        SingletonInit();
        isFinal = false;  // 초기화
    }

    public void RegisterPlayer(IPlayerActor p) => player = p;

    public void RegisterEnemy(IEnemyActor e)
    {
        if (!enemies.Contains(e))
        {
            enemies.Add(e);
            Debug.Log($"[디버그] 등록된 적들: {string.Join(", ", enemies.Select(en => ((MonoBehaviour)en).name))}");

            // 첫 등록된 EnemyBase를 기본 타겟으로 설정
            if (currentEnemy == null && e is EnemyBase monster)
                currentEnemy = monster;

            // 보스 몬스터가 등록되면 최종 전투 플래그 켜기
            if (e is EnemyBase boss && boss.Type == EnemyType.Boss)
            {
                isFinal = true;
                Debug.Log("[디버그] 보스 몬스터 등록 → 최종 전투(isFinal)로 설정");
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
            // ▶ 배틀 시작 직전에 카드 컨트롤러 초기화
        var pc = player as PlayerController;
            if (pc != null)
                {
                    // PlayerController 에 CardController 컴포넌트가 붙어 있다고 가정
            var cardCtrl = pc.GetComponent<CardController>();
                    if (cardCtrl != null)
                        {
                cardCtrl.BattleInit();
                Debug.Log("[TurnManager] CardController.BattleInit() 호출됨");
                        }
                   else
                        {
                Debug.LogWarning("[TurnManager] CardController 컴포넌트를 찾을 수 없음");
                        }
                }
        StartCoroutine(WaitForRegistrationAndStart());
    }

    private IEnumerator WaitForRegistrationAndStart()
    {
        // 플레이어와 적이 모두 등록될 때까지 대기
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
        var pc = player as PlayerController;

        while (true)
        {
            // 0. 게임 종료 상태 검사
            if (isGameEnded)
                yield break;

            // 1. 몬스터 사망 및 전투 종료 처리
            if (currentEnemy == null || currentEnemy.IsDead)
            {
                Debug.Log("전투 승리!");
                GameStateManager.Instance.AddWin();

                if (isFinal)
                {
                    // 보스(최종 전투) 승리 시
                    GameStateManager.Instance.SetBossDefeated();
                    Debug.Log("[디버그] 최종 전투 종료 → GameClearUI 표시");
                    GameClearUI.Instance.Show();
                    isGameEnded = true;
                }
                else
                {
                    // 일반 몬스터 처치 시 → 즉시 맵으로 복귀
                    Debug.Log("[디버그] 일반 전투 종료 → 맵 씬으로 복귀");
                    Manager.Map.ShowMap();
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

    // (보상 후 맵으로 돌아갈 때 호출되는 메서드가 필요하다면 사용)
    public void ContinueAfterReward()
    {
        Debug.Log("보상 선택 완료 → 맵 씬으로 복귀");
        Manager.Map.ShowMap();
    }
}