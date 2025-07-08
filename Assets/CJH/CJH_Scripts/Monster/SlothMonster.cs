using System;
using System.Collections;
using UnityEngine;

public class SlothMonster : EnemyBase
{
    public static SlothMonster Instance { get; private set; }

    // —————————————————————————
    // 1) 기믹 설정 변수
    // —————————————————————————
    [SerializeField] private int requiredDamage = 10;
    [SerializeField] private int clearSuccessThreshold = 3;
    private int successCount = 0;

    // —————————————————————————
    // 2) 순환 행동 상태
    // —————————————————————————
    private int cycleIndex = 0;     // 0~4 순환
    private int damageThisTurn = 0; // 이번 턴 플레이어가 입힌 피해

    public event Action OnClear;

    protected override void Start()
    {
        base.Start();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    /// <summary>
    /// 슬로스는 실제 HP가 깎이지 않으니, 누적용으로만 사용
    /// </summary>
    public override void ApplyDamage(int damage)
    {
        // base.ApplyDamage(damage); // 체력 감소 없음
        damageThisTurn += damage;
        Debug.Log($"[슬로스] 누적 데미지: {damageThisTurn}/{requiredDamage}");
    }

    public override void TakeTurn()
    {
        StartCoroutine(TurnRoutine());
    }

    private IEnumerator TurnRoutine()
    {
        Debug.Log($"[슬로스] 턴 시작 – 요구 데미지 {requiredDamage} 이니라.");

        // 클리어 체크
        if (successCount >= clearSuccessThreshold)
        {
            Debug.Log($"[슬로스] {clearSuccessThreshold}회 성공 → 클리어!");

            // 1-1) 클리어 이벤트 호출
            OnClear?.Invoke();

            // 1-2) 몬스터 제거 (OnDestroy → TurnManager.UnregisterEnemy)
            Destroy(gameObject);

            yield break;
        }

        // 요구 데미지 성공 시
        if (damageThisTurn >= requiredDamage)
        {
            successCount++;
            Debug.Log($"[슬로스] 요구 데미지 달성! ({successCount}/{clearSuccessThreshold})");
            
        }
    //   else
    //   {
    //       // 실패 시 순환 행동
    //       switch (cycleIndex)
    //       {
    //           case 0: yield return AddPlayableSlimeCard(); break;
    //           case 1: yield return AddUnplayableSlimeCard(); break;
    //           case 2: yield return AddPlayableSlimeCard(); break;
    //           case 3: yield return AddUnplayableSlimeCard(); break;
    //           case 4: yield return AddPlayableSlimeCard(); break;
    //       }
    //       cycleIndex = (cycleIndex + 1) % 5;
    //
    //       // 패배 체크: 손패에 사용 불가능 점액질 카드 4장 이상
    //       int badCount = CardManager.Instance.CountCardsInHand(c =>
    //           c.Type == CardEnum.Slime && !c.IsPlayable);
    //       if (badCount >= 4)
    //       {
    //           Debug.Log("[슬로스] 사용 불가능 점액질 카드 4장 이상 → 패배!");
    //           TurnManager.Instance.NotifyPlayerDeath();
    //       }
    //   }

        // 턴 마무리: 누적 피해 초기화
        damageThisTurn = 0;
        yield break;
    }

    // —————————————————————————
    // 3) 사용 가능한 점액질 카드 추가
    // —————————————————————————
    //private IEnumerator AddPlayableSlimeCard()
    //{
    //    Debug.Log("[슬로스] 사용 가능한 점액질 카드 1장 추가!");
    //    var card = CardManager.Instance.CreateCard(CardEnum.Slime);
    //    card.IsPlayable = true;
    //    CardManager.Instance.AddCardToHand(card);
    //    yield return null;
    //}

    // —————————————————————————
    // 4) 사용 불가능한 점액질 카드 추가
    // —————————————————————————
    //private IEnumerator AddUnplayableSlimeCard()
    //{
    //    Debug.Log("[슬로스] 사용 불가능한 점액질 카드 1장 추가!");
    //    var card = CardManager.Instance.CreateCard(CardEnum.Slime);
    //    card.IsPlayable = false;
    //    CardManager.Instance.AddCardToHand(card);
    //    yield return null;
    //}
}