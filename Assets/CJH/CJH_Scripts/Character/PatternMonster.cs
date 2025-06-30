using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatternMonster : MonoBehaviour, IEnemyActor
{
    [Header("몬스터 체력")]
    [SerializeField] private int maxHP = 1000;
    private int currentHP;

    [Header("공격 패턴 목록")]
    [SerializeField] private List<MonsterAttackPattern> patterns = new();

    [SerializeField] private EnemyType type;
    public EnemyType Type => type;

    private int currentTurn = 1;

    // 누적 피해 추적
    private List<int> lastThreeTurnDamages = new List<int>();

    // 프라이드 판단 관련 상태
    private bool forceStrongNextTurn = false;
    private bool awaitingSpecialJudgement = false;

    public bool IsDead => currentHP <= 0;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeTurn()
    {
        StartCoroutine(ExecuteTurn());
    }

    private IEnumerator ExecuteTurn()
    {
        float hpRate = (float)currentHP / maxHP;

        // 1. 프라이드 판단: 3턴일 때 최근 피해가 부족하면 특수 판단 발동
        if (currentTurn == 3)
        {
            int recentDamage = lastThreeTurnDamages.Sum();
            if (recentDamage < 400)
            {
                yield return ExecuteSpecialPrideJudgement(); // 플레이어 강화 + 다음 턴 강공격 예약
                currentTurn++;
                yield break;
            }
        }

        // 2. 강공격 예약 상태: 무조건 Basic 스킬을 강공격처럼 시전
        if (forceStrongNextTurn)
        {
            var basic = patterns.Find(p => p.type == MonsterAttackType.Basic);
            if (basic != null)
            {
                Debug.Log("<color=red>[강공격]</color> 특수기 이후 분노의 강공격!");
                forceStrongNextTurn = false;
                yield return ApplyPattern(basic); // 같은 Basic 패턴이라도 강공격처럼 처리
            }
            else
            {
                Debug.LogWarning("[강공격] Basic 패턴이 없어 시전 불가!");
            }

            currentTurn++;
            yield break;
        }

        // 3. 턴 1~2는 무조건 Basic (일반공격)
        if (currentTurn == 1 || currentTurn == 2)
        {
            var basic = patterns.Find(p => p.type == MonsterAttackType.Basic);
            if (basic != null)
            {
                yield return ApplyPattern(basic);
            }
            else
            {
                Debug.LogWarning("[기본공격] Basic 패턴이 없어 시전 불가!");
            }

            currentTurn++;
            yield break;
        }

        // 4. 턴 3은 무조건 Special
        if (currentTurn == 3)
        {
            var special = patterns.Find(p => p.type == MonsterAttackType.Special);
            if (special != null)
            {
                Debug.Log("[턴 3] 특수기 강제 발동!");
                yield return ApplyPattern(special);

                forceStrongNextTurn = true; // 다음 턴 강공격 예약
            }
            else
            {
                Debug.LogWarning("[특수기] Special 패턴이 없어 시전 불가!");
            }

            currentTurn++;
            yield break;
        }

        // 5. 이후 턴부터는 궁극기 > 특수기 > 기본기 순
        var ultimate = patterns.Find(p =>
            p.type == MonsterAttackType.Ultimate &&
            (p.triggerTurn == currentTurn || (p.triggerHpRate > 0 && hpRate <= p.triggerHpRate)));

        if (ultimate != null)
        {
            Debug.Log("[패턴] 궁극기 조건 충족");
            yield return ApplyPattern(ultimate);
            currentTurn++;
            yield break;
        }

        var specialGeneral = patterns.Find(p =>
            p.type == MonsterAttackType.Special &&
            (p.triggerTurn == currentTurn || (p.triggerHpRate > 0 && hpRate <= p.triggerHpRate)));

        if (specialGeneral != null)
        {
            Debug.Log("[패턴] 특수기 조건 충족");
            yield return ApplyPattern(specialGeneral);

            // 후속 공격 - 같은 Basic 재사용
            if (CanActThisTurn())
            {
                var basic = patterns.Find(p => p.type == MonsterAttackType.Basic);
                if (basic != null)
                {
                    Debug.Log("[후속기] 기본 공격으로 후속 시전!");
                    yield return ApplyPattern(basic);
                }
            }

            currentTurn++;
            yield break;
        }

        // 기본 공격 fallback
        var fallback = patterns.Find(p => p.type == MonsterAttackType.Basic);
        if (fallback != null)
        {
            yield return ApplyPattern(fallback);
        }
        else
        {
            Debug.LogWarning("[패턴 없음] Basic 패턴이 없어 아무 행동도 하지 않음!");
        }

        currentTurn++;
    }

    private IEnumerator ExecuteSpecialPrideJudgement()
    {
        Debug.Log("[특수] 프라이드 판단 패턴 발동 - 최근 피해 부족");

        var player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            player.ApplyBuff_DamageMultiplier(1.4f, 1);
            Debug.Log("[프라이드] 플레이어에게 데미지 +40% 버프 부여");
        }

        awaitingSpecialJudgement = true;
        forceStrongNextTurn = true;
        yield return null;
    }

    public bool IsAwaitingPrideJudgement => awaitingSpecialJudgement;

    public void EndPrideJudgement()
    {
        awaitingSpecialJudgement = false;
    }

    public void ForceSetHpToRate(float rate)
    {
        currentHP = Mathf.RoundToInt(maxHP * rate);
        Debug.Log($"[프라이드] 체력을 {rate * 100}%로 조정");
    }

    private bool CanActThisTurn()
    {
        return CardManager.Instance != null && CardManager.Instance.GetAvailableCardCount() > 0;
    }


    private IEnumerator ApplyPattern(MonsterAttackPattern pattern)
    {
        if (pattern == null)
        {
            Debug.LogWarning("[패턴 오류] 전달받은 패턴이 null! 적용 중단.");
            yield break;
        }

        Debug.Log($"[몬스터] 턴 {currentTurn} - {pattern.type} 발동: {pattern.name} - {pattern.description}");

        var player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            int dmg = Mathf.RoundToInt(pattern.damageRate * maxHP);
            player.TakeDamage(dmg);
        }

        yield return new WaitForSeconds(0.5f);
    }

    public void ApplyDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);
        Debug.Log($"[몬스터] 피해 {damage}, 남은 체력: {currentHP}");

        lastThreeTurnDamages.Add(damage);
        if (lastThreeTurnDamages.Count > 3)
            lastThreeTurnDamages.RemoveAt(0);

        if (IsDead)
        {
            Debug.Log("[몬스터] 사망 처리됨");
        }
    }
}