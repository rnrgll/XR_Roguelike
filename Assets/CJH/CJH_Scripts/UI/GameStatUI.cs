using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStatusUI : MonoBehaviour
{
    public static GameStatusUI Instance;

    [Header("몬스터 정보")]
    public TextMeshProUGUI monsterHPText;
    public TextMeshProUGUI accumulatedDamageText;

    private int accumulatedDamage = 0;
    private int maxMonsterHP = 1000;

    private int currentStage = 1;

    private EnemyBase target;



    private void Awake()
    {
        // null일 경우 인스턴스 호출
        if (Instance == null) Instance = this;
        // null이 아닐경우 오브젝트를 제거
        else Destroy(gameObject);
    }

    public void SetTarget(EnemyBase monster)
    {
        target = monster;
        UpdateMonsterStatus();
    }

    public void AddDamage(int damage)
    {
        UpdateMonsterStatus();
    }

    private void UpdateMonsterStatus()
    {
        if (target == null) return;

        monsterHPText.text = $"HP: {target.currentHP} / {target.maxHP}";
        accumulatedDamageText.text = $"누적 피해: {target.maxHP - target.currentHP}";
    }


    public void SetStage(int stage)
    {
        currentStage = stage;
    }
}