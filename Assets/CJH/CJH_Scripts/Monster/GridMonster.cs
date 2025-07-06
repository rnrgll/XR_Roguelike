using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMonster : EnemyBase
{
    public static GridMonster Instance { get; private set; }

    // ——————————————————
    // 1) 공격 배율 상수
    // ——————————————————
    private const float BasicRate = 0.20f;    // 기본 공격: 플레이어 최대체력 20%
    private const float HeavyRate = 0.35f;    // 강공격: 플레이어 최대체력 35%
    private const float SpecialRate = 0.15f;  // 특수 실패 시: 플레이어 최대체력 15%

    // ——————————————————
    // 2) 특수패턴 상태 변수
    // ——————————————————
    private int mainArcanaCount = 0;          // 플레이어가 사용한 메인(메이저) 아르카나 누적
    private bool isPreparingSpecial = false;  // 특수 준비 중인가?
    private int damageDuringPrepare = 0;      // 준비 턴 동안 받은 피해 누적
    private bool isGroggy = false;            // 저지되어 그로그기 상태인가?
    private bool nextIsHeavy = false;         // 다음 턴에 강공격을 강제할 것인가?

    // ——————————————————
    // 3) 진입
    // ——————————————————
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // 플레이어가 메인 아르카나를 사용할 때 외부에서 호출해 주거라
    public void OnPlayerUsedMajorArcana()
    {
        mainArcanaCount++;
        Debug.Log($"[그리드] 플레이어 메이저 아르카나 사용 누적 → {mainArcanaCount}회");
    }

    // ——————————————————
    // 4) 턴 실행
    // ——————————————————
    public override void TakeTurn()
    {
        StartCoroutine(TurnRoutine());
    }

    private IEnumerator TurnRoutine()
    {
        var player = TurnManager.Instance.GetPlayerController();
        int playerMax = player.MaxHP;

        // 4-1) 특수 준비 중 → 해제 로직
        if (isPreparingSpecial)
        {
            yield return ResolveSpecial(player, playerMax);
            yield break;
        }

        // 4-2) 그로그기 상태 → 행동 없이 해제, 다음은 강공격
        if (isGroggy)
        {
            Debug.Log("[그리드] 그로그기 상태라 한 턴 쉰다…");
            isGroggy = false;
            nextIsHeavy = true;
            yield break;
        }

        // 4-3) 강공격 강제
        if (nextIsHeavy)
        {
            yield return HeavyAttack(player, playerMax);
            nextIsHeavy = false;
            yield break;
        }

        // 4-4) 메이저 아르카나 2회 사용시 → 특수 준비
        if (mainArcanaCount >= 2)
        {
            yield return PrepareSpecial();
            yield break;
        }

        // 4-5) 기본 공격
        yield return BasicAttack(player, playerMax);
    }

    // ——————————————————
    // 5) 기본 공격
    // ——————————————————
    private IEnumerator BasicAttack(PlayerController player, int playerMax)
    {
        int dmg = Mathf.RoundToInt(playerMax * BasicRate);
        Debug.Log($"[그리드] 기본 공격 → 플레이어 최대체력의 20%: {dmg} 데미지");
        player.TakeDamage(dmg);
        yield return null;
    }

    // ——————————————————
    // 6) 강공격
    // ——————————————————
    private IEnumerator HeavyAttack(PlayerController player, int playerMax)
    {
        int dmg = Mathf.RoundToInt(playerMax * HeavyRate);
        Debug.Log($"[그리드] 강공격 발동! 플레이어 최대체력의 35%: {dmg} 데미지");
        player.TakeDamage(dmg);
        yield return null;
    }

    // ——————————————————
    // 7) 특수 준비
    // ——————————————————
    private IEnumerator PrepareSpecial()
    {
        isPreparingSpecial = true;
        damageDuringPrepare = 0;
        Debug.Log("[그리드] 특수패턴 준비 중… 1턴간 공격을 저지하세요!");
        yield return null;
    }

    // ——————————————————
    // 8) 특수 해제(다음 턴 실행)
    // ——————————————————
    private IEnumerator ResolveSpecial(PlayerController player, int playerMax)
    {
        // 기준 데미지
        const int requiredDamage = 100;

        if (damageDuringPrepare >= requiredDamage)
        {
            // 저지 성공
            Debug.Log("[그리드] 특수 저지 성공! 그리드를 제압했어요! → 한 턴 그로그기 상태가 된다");
            isGroggy = true;
        }
        else
        {
            // 저지 실패
            int dmg = Mathf.RoundToInt(playerMax * SpecialRate);
            Debug.Log($"[그리드] 특수 저지 실패… 플레이어 최대체력의 15%: {dmg} 데미지 + 메이저 아르카나 탈취");
            player.TakeDamage(dmg);

            // 메이저 아르카나 탈취 (플레이어 컨트롤러에 구현된 훔치기 메서드 호출 예시)
            //player.StealRandomMajorArcana();
        }

        // 초기화
        mainArcanaCount = 0;
        isPreparingSpecial = false;
        damageDuringPrepare = 0;

        yield return null;
    }

    // ——————————————————
    // 9) 피해 적용 시 누적 (특수 준비 중)
    // ——————————————————
    public override void ApplyDamage(int damage)
    {
        base.ApplyDamage(damage);

        if (isPreparingSpecial)
        {
            damageDuringPrepare += damage;
            Debug.Log($"[그리드] 준비 중 받은 피해 누적: {damageDuringPrepare}");
        }
    }
}