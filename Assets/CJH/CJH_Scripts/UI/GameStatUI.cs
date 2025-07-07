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
        UpdateMonsterStatus();
    }

    /// <summary>
    /// 데미지가 들어올 때마다 호출
    /// </summary>
    public void AddDamage(int damage)
    {
        UpdateMonsterStatus();
    }


    private void UpdateMonsterStatus()
    {
        if (target == null) return;

        // 텍스트
        monsterHPText.text = $"HP: {target.currentHP} / {target.maxHP}";

        // 슬라이더
        if (monsterHpSlider != null)
            monsterHpSlider.value = target.currentHP;
    }


    public void SetStage(int stage)
    {
        currentStage = stage;
    }
}