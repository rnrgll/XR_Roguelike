using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStatusUI : MonoBehaviour
{
    public static GameStatusUI Instance;

    [Header("몬스터 정보")]
    public TextMeshProUGUI monsterHPText;
    public TextMeshProUGUI accumulatedDamageText;

    [Header("공격 정보")]
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI multiplierText;

    private int accumulatedDamage = 0;
    private int maxMonsterHP = 1000;

    [SerializeField] private TextMeshProUGUI stageLabel;
    private int currentStage = 1;

    private void Awake()
    {
        // null일 경우 인스턴스 호출
        if (Instance == null) Instance = this;
        // null이 아닐경우 오브젝트를 제거
        else Destroy(gameObject);
    }

    public void Init(int monsterMaxHP)
    {
        maxMonsterHP = monsterMaxHP;
        accumulatedDamage = 0;
        UpdateMonsterStatus();
    }

    public void AddDamage(int damage)
    {
        accumulatedDamage += damage;
        UpdateMonsterStatus();
    }

    private void UpdateMonsterStatus()
    {
        monsterHPText.text = $"HP: {accumulatedDamage} / {maxMonsterHP}";
        accumulatedDamageText.text = $"누적 피해: {accumulatedDamage}";
    }

    public void SetComboInfo(string comboName, float multiplier)
    {
        comboText.text = $"{comboName}";
        multiplierText.text = $"x{multiplier}";
    }

    public void SetStage(int stage)
    {
        currentStage = stage;
        stageLabel.text = $"Stage {currentStage}";
    }
}