using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnButton : MonoBehaviour
{
    [Header("옵션 씬 이름")]
    [SerializeField] private string optionSceneName = "OptionScene"; // 인스펙터에서 설정 가능

    public void OnClickEndTurn()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        player?.EndTurn();
    }

    public void OnClickAttack()
    {
        PatternMonster enemy = FindAnyObjectByType<PatternMonster>();
        if (enemy != null && !enemy.IsDead)
        {
            enemy.ApplyDamage(100);
        }
        else
        {
            Debug.Log("이미 죽은 적입니다.");
        }
    }

    public void OnClickOptionSceneLoad()
    {
        if (string.IsNullOrEmpty(optionSceneName))
        {
            Debug.LogWarning("옵션 씬 이름이 아닙니다!");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(optionSceneName))
        {
            Debug.Log($"[옵션] 옵션 씬 '{optionSceneName}' 로 전환 중...");
            SceneManager.LoadScene(optionSceneName);
        }
        else
        {
            Debug.LogError($"[옵션] 씬 '{optionSceneName}' 를 찾을 수 없습니다. Build Settings에 등록되었는지 확인하세요!");
        }
    }
}