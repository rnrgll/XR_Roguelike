using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Managers;

public class BattleSceneManager : MonoBehaviour
{

    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private GameObject lasMonsterPrefab;
    [SerializeField] private GameObject envyMonsterPrefab;
    [SerializeField] private GameObject prideMonsterPrefab;
    [SerializeField] private GameObject CardHandUIPrefab;
    [SerializeField] private GameObject gridMonsterPrefab;
    [SerializeField] private GameObject slothMonsterPrefab;
    [SerializeField] private GameObject lustMonsterPrefab;
    [SerializeField] private GameObject gluttonMonsterPrefab;

    [SerializeField] private Slider hpBarPrefab;

    // —————————————————————
    // 등장 이력 저장용 static 리스트
    // —————————————————————
    private static List<MonsterID> usedMonsters = new List<MonsterID>();

    // 첫 스테이지에는 이 셋만 후보
    private readonly MonsterID[] firstStagePool = {
        MonsterID.Las,
        MonsterID.Grid,
        MonsterID.Pride
    };

    // 전체 몬스터 후보
    private readonly MonsterID[] allMonsters = {
        MonsterID.Las,
        MonsterID.Grid,
        MonsterID.Envy,
        MonsterID.Pride,
        MonsterID.Lust,
        MonsterID.Sloth,
        MonsterID.Glutton

    };


    private IEnumerator Start()
    {

        // 1프레임 기다려 PlayerController가 등장하도록 함
        yield return null;
        var pc = FindObjectOfType<PlayerController>();
        if (pc != null)
        {
            // 1. 적 생성
            SpawnMonstersForStage();


            // 2. 플레이어 등록 및 전투 시작
            TurnManager.Instance.RegisterPlayer(pc);
            Debug.Log("Player 등록 완료");
            yield return null; // 한 프레임
            pc.ResetState();    // 체력·버프·턴 플래그 전부 초기화

            Debug.Log("Card Controller 대기중");
            yield return new WaitUntil(() => pc.GetCardController() != null);
            var cc = pc.GetCardController();


            yield return new WaitUntil(() => cc.IsReady);

            Debug.Log("[BattleManager] CardHandUI init");
            var go = Instantiate(CardHandUIPrefab, transform);
            go.GetComponent<BattleUI>().InitScene(pc);


            Debug.Log("[BattleManager] cardHand 생성");

            TurnManager.Instance.StartBattle();
            Debug.Log(" 전투 시작됨");

            // 3. HP 바 생성 및 연결
            if (hpBarPrefab != null)
            {
                var canvas = FindObjectOfType<Canvas>();
                var hpGo = Instantiate(hpBarPrefab, canvas.transform, false);
                pc.SetHpBar(hpGo);
            }
            else
            {
                Debug.LogWarning("hpBarPrefab이 할당되지 않았습니다.");
            }

            // 4. UI 설정
            GameStatusUI.Instance.SetStage(GameStateManager.Instance.Wins + 1);
        }
    }

    /// <summary>
    /// 스테이지별 몬스터 스폰 로직
    /// 첫 번째 조우(승리 수 0)에서는 Las, Greed, Pride만 스폰
    /// 이후 스테이지는 원하는 대로 변경 가능
    /// </summary>
    private void SpawnMonstersForStage()
    {
        int wins = GameStateManager.Instance.Wins;
        MonsterID toSpawn;


        List<MonsterID> pool;
        if (wins == 0)
        {
            // 첫 스테이지: 라스, 그리드, 프라이드 중 랜덤 1종
            pool = new List<MonsterID>(firstStagePool);
        }
        else
        {
            // 이후 스테이지: 전체 몬스터 중
            pool = new List<MonsterID>(allMonsters);
        }
        //  이미 나온 몬스터는 후보에서 제거
        pool.RemoveAll(id => usedMonsters.Contains(id));


        // 3) 만약 후보가 비어 있으면(모두 나왔으면) 이력 초기화 후 전부 다시 후보로
        if (pool.Count == 0)
        {
            Debug.Log("[BattleSceneManager] 모든 몬스터 등장 → 이력 초기화 후 재추첨");
            usedMonsters.Clear();
            pool = new List<MonsterID>(wins == 0 ? firstStagePool : allMonsters);
        }

        // 4) 무작위 1마리 선택
        toSpawn = pool[Random.Range(0, pool.Count)];
        usedMonsters.Add(toSpawn);

        // 5) 실제 스폰
        SpawnSingleMonster(toSpawn);
    }

    private void SpawnSingleMonster(MonsterID id)
    {
        var prefab = GetMonsterPrefab(id);
        var go = Instantiate(prefab);
        var enemy = go.GetComponent<EnemyBase>();

        enemy.InitForBattle();
        if (enemy != null)
        {
            TurnManager.Instance.RegisterEnemy(enemy);
            Debug.Log($"[{id}] 몬스터 스폰 및 등록 완료");

            StartCoroutine(InitializeMonsterUI(enemy));
        }
        if (GameStatusUI.Instance != null)
        {
            GameStatusUI.Instance.SetTarget(enemy);
        }
        else
        {
            Debug.LogError($"[{id}] 프리팹에 EnemyBase가 없습니다!");
        }

        // 3) 슬로스의 OnClear 이벤트 구독 (클리어 시 전투 종료)
        if (enemy is SlothMonster sloth)
        {
            sloth.OnClear += () =>
            {
                Debug.Log("[BattleSceneManager] Sloth 클리어됨 → 전투 종료 처리");
                TurnManager.Instance.ContinueAfterReward();
            };
        }
        GameStatusUI.Instance.SetTarget(enemy);


    }

    private IEnumerator InitializeMonsterUI(EnemyBase enemy)
    {
        yield return null; // Start()가 실행될 때까지 한 프레임 대기

        if (GameStatusUI.Instance != null)
            GameStatusUI.Instance.Init(enemy);
    }

    private GameObject GetMonsterPrefab(MonsterID id)
    {
        return id switch
        {
            MonsterID.Las => lasMonsterPrefab,
            MonsterID.Grid => gridMonsterPrefab,
            MonsterID.Envy => envyMonsterPrefab,
            MonsterID.Pride => prideMonsterPrefab,
            MonsterID.Sloth => slothMonsterPrefab,
            MonsterID.Lust => lustMonsterPrefab,
            MonsterID.Glutton => gluttonMonsterPrefab,
            _ => null
        };
    }
}