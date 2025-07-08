using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStatusUI : MonoBehaviour
{
    public static GameStatusUI Instance;

    [Header("몬스터 정보")]
    public TextMeshProUGUI monsterHPText;

    [SerializeField] private Slider monsterHpSlider;


    private int currentStage = 1;

    private EnemyBase target;




    private void Awake()
    {
        // null일 경우 인스턴스 호출
        if (Instance == null) Instance = this;
        // null이 아닐경우 오브젝트를 제거
        else Destroy(gameObject);


        /// <summary>
        /// null이면 자동 참조
        /// </summary>
        if (monsterHPText == null)
        {
            monsterHPText = GetComponentInChildren<TextMeshProUGUI>(true);
            if (monsterHPText == null)
                Debug.LogWarning("GameStatusUI: monsterHPText를 찾지 못했습니다!");
        }

        if (monsterHpSlider == null)
        {
            monsterHpSlider = GetComponentInChildren<Slider>(true);
            if (monsterHpSlider == null)
                Debug.LogWarning("GameStatusUI: monsterHpSlider를 찾지 못했습니다!");
        }


    }

    /// <summary>
    /// 타겟 몬스터와 최대 체력까지 같이 초기화
    /// </summary>
    public void Init(EnemyBase monster)
    {
        target = monster;
        // 슬라이더가 있으면 최대값 세팅
        if (monsterHpSlider != null)
        {
            monsterHpSlider.maxValue = monster.maxHP;
            monsterHpSlider.value = monster.maxHP;
        }
        UpdateMonsterStatus();
    }

    /// <summary>
    /// 전투 개시 직후(또는 몬스터 스폰 직후) 호출해서
    /// 최대 HP를 알려주기 위한 메서드
    /// </summary>

    public void SetTarget(EnemyBase monster)
    {
        target = monster;

        if (monsterHpSlider != null)
        {
            monsterHpSlider.maxValue = monster.maxHP;
            monsterHpSlider.value = monster.currentHP;
            Debug.Log($"[GameStatusUI][SetTarget] maxValue={monsterHpSlider.maxValue}, value={monsterHpSlider.value}");
        }

        UpdateMonsterStatus();
    }

    /// <summary>
    /// 데미지가 들어올 때마다 호출
    /// </summary>
    public void AddDamage(int damage)
    {
        if (target == null) return;

        // UI 갱신
        UpdateMonsterStatus();
    }




    private void UpdateMonsterStatus()
    {
        if (target == null) return;

        monsterHPText.text = $"HP: {target.currentHP}/{target.maxHP}";
        if (monsterHpSlider != null)
        {
            monsterHpSlider.value = target.currentHP;
            Debug.Log($"[Debug][UpdateMonsterStatus] Slider.value = {monsterHpSlider.value}");
        }
    }


    public void SetStage(int stage)
    {
        currentStage = stage;
    }
}