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
    public static bool HasInstance => Instance != null;

    private bool battleStarted = false;
    private bool isGameEnded = false;
    public bool isFinal { get; private set; } = false;

    private void Awake()
    {
        SingletonInit();
        isFinal = false;
        // (몬스터가 미리 있을 경우 InitForBattle 호출)
        foreach (var e in enemies.OfType<EnemyBase>())
            e.InitForBattle();
    }

    public void RegisterPlayer(IPlayerActor p) => player = p;

    public void RegisterEnemy(IEnemyActor e)
    {
        if ((UnityEngine.Object)e == null || enemies.Contains(e))
            return;

        enemies.Add(e);

        if (currentEnemy == null && e is EnemyBase mb)
            currentEnemy = mb;

        if (e is EnemyBase boss && boss.Type == EnemyType.Boss)
        {
            isFinal = true;
            Debug.Log("[TurnManager] 보스 등록 → 최종 전투(isFinal) 설정");
        }
    }

    public void UnregisterEnemy(IEnemyActor e)
    {
        enemies.Remove(e);
    }

    public PlayerController GetPlayerController() => player as PlayerController;
    public List<IEnemyActor> GetEnemies() => enemies;

    public void StartBattle()
    {
        if (battleStarted) return;
        battleStarted = true;

        //  배경 매니저를 확실히 켜고, 다음 배경 보여주기
        var bgm = BackgroundManager.Instance;
        bgm.gameObject.SetActive(true);
        bgm.ShowNextBackground();


        // 플레이어 보이기
        var pc = GetPlayerController();
        if (pc != null)
        {
            pc.SetSpriteVisible(true);
            Debug.Log("[TurnManager] 전투 시작 → 플레이어 스프라이트 켬");
        }

        // 카드 컨트롤러 초기화
        if (pc?.cardController != null)
        {
            pc.cardController.BattleInit();
            Debug.Log("[TurnManager] CardController.BattleInit() 호출");
        }

        StartCoroutine(WaitForRegistrationAndStart());
    }

    private IEnumerator WaitForRegistrationAndStart()
    {
        yield return new WaitUntil(() => player != null && enemies.Count > 0);
        yield return StartCoroutine(BattleLoop());
    }

    private IEnumerator BattleLoop()
    {
        var pc = GetPlayerController();

        while (!isGameEnded)
        {
            // 적 전멸 체크
            if (currentEnemy == null || currentEnemy.IsDead)
            {
                Debug.Log("전투 승리!");
                GameStateManager.Instance.AddWin();

                // 보스/일반 분기
                if (isFinal)
                {
                    GameStateManager.Instance.SetBossDefeated();
                    GameClearUI.Instance.Show();
                    isGameEnded = true;
                }
                else
                {
                    // 일반 전투 종료 → 맵으로
                    ContinueAfterReward();
                }
                yield break;
            }

            // 플레이어 턴
            pc?.ApplyHeal();
            player.StartTurn();
            yield return new WaitUntil(() => player.IsTurnFinished());

            // 적 턴
            foreach (var enemy in enemies.OfType<EnemyBase>().Where(e => !e.IsDead))
            {
                enemy.TakeTurn();
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public void ContinueAfterReward()
    {

        //  전투 배경 숨기기
        BackgroundManager.Instance.gameObject.SetActive(false);
        // 플레이어 숨기기
        var pc = GetPlayerController();
        if (pc != null)
        {
            pc.SetSpriteVisible(false);
            Debug.Log("[TurnManager] 전투 종료 → 플레이어 스프라이트 숨김");
        }

        // 2) 전투 상태 초기화
        ResetBattleState();

        Manager.UI.ShowSelectableMap();
    }

    /// <summary>
    /// 다음 전투를 위해 전투 관련 상태만 초기화 (player는 유지)
    /// </summary>
    private void ResetBattleState()
    {
        battleStarted = false;
        isGameEnded = false;
        isFinal = false;
        currentEnemy = null;
        enemies.Clear();
        Debug.Log("[TurnManager] 전투 상태 초기화 완료");
    }


    public void NotifyPlayerDeath()
    {
        if (isGameEnded) return;
        isGameEnded = true;
        GameOverUI.Instance.Show();
    }

    public void ResetState()
    {
        player = null;
        enemies.Clear();
        currentEnemy = null;
        battleStarted = false;
        isGameEnded = false;
        isFinal = false;
        Debug.Log("[TurnManager] 상태 초기화 완료");
    }
}